#This section of the script sets some global variables.
$rulesRepo = "\\10.0.20.57\freeedr\Rules"
$toolsRepo = "\\10.0.20.57\freeedr\Tools"
$clientFolder = "C:\Windows\freeedr\"
$vtApi = "276baa96547e2a405c3751eeb24a33b0bc63a3b965e9cbe6d2c8ddefba54168f" #hardcoded api value, change as necessary


<#
    This function installs Python 3.7 which is a requirement to run the SigmaC tool to convert a Sigma rule into a Get-WinEvent query.
#>
function installPython {
    #$pythonDestination = $toolsRepo + "\python-3.7.0.exe"
    Copy-Item $toolsRepo $clientFolder -Force
    #  Invoke-WebRequest -Uri "https://www.python.org/ftp/python/3.7.0/python-3.7.0.exe" -OutFile "C:/Windows/python-3.7.0.exe"
    C:\Windows\freeedr\python-3.7.0.exe /quiet InstallAllUsers=0 PrependPath=1 Include_test=0
}

# Pip is required to install other prerequisites for the SimgmaC python script.
function upgradePip {
    python -m pip install --upgrade pip
}

# PyYAML is required to get the YAML package for the SigmaC script to run. 
fucntion installRequirements {
    pip install pyyaml
}

# Sigma is required to translate the rules into Windows Event queries.
function getSigma {
    Copy-Item $toolsRepo $clientFolder
}

<# 
    This portion of the sciprt creates a variable for the rules repository server and the destination folder for each endpoint.
    The rules are then copied from the reposiotry server to the destination on the endpoint.
    The "-Recurse" copies all folders and files under the \rules folder
    The "-Force" will create the folder on the endpoint if it does not already exist
#>
$ruleRepository = "\\10.0.20.57\rules\Rules"
$destinationFolder = "C:\Windows"
#Copy-Item $ruleRepository $destinationFolder -Recurse -Force
# The above function has been moved to updateRules

try {python --version}
catch {installPython}
upgradePip

<#
    Compare using overall file length between the rules repo and the local rules directory. This will run periodically.
#>
function updateRules {
    $LocalFolder = "C:\Windows\rules"
    Compare-Object $LocalFolder $ruleRepository -Property Name, Length  | Where-Object {$_.SideIndicator -eq "<="} | ForEach-Object {
        Copy-Item "\\10.0.20.57\rules\$($_.name)" -Destination $LocalFolder -Force
    }
}

<#
    This is where we will pull the the Sigma script from the rules reposiotry.
#>


# Change to sigma directory and translate the rules.
cd C:\Windows\freeedr\sigma\tools

<#
    Calls sigmac to convert rules in the rules repository and store them in a variable.
    The variable is piped into a foreach-object loop and each rule is written to its own file.
    The file name is numerically increased.
#>
$rules = python sigmac -t powershell -r C:\Windows\rules\Discovery
$rules | ForEach-Object {$i = 1}{
    $_ | Out-File C:\Windows\sigmaTranslation\rule$i.txt
    $i++}

<#
    This section of the script will read the translated rules from the files into a powershell foreach loop
#>

$translatedRules = "C:\Windows\sigmaTranslation"
$translatedRules | ForEach-Object {
    $powershellQuery = Get-Content $_
    Invoke-Expression $powershellQuery

    #this following section will pull relevant informaiton from the alerted logs into variables and combine the information with data gathered from APIs

}

<#
	This next section is solely for adapted VirusTotal Code
	This code is entirely adapted from: https://gallery.technet.microsoft.com/Get-VirusTotalReport-90065fad
	This code will not upload any file to VirusTotal, but convert a file to hash, and then query for a report based on the hash. 
	If there is no corresponding hash in VT, then the script will return that the hash is not part of an existing or pending scan.
	/////
	Uploading another script for posterity and further adaptation. Curl command below for file upload
	curl --request POST --url 'https://www.virustotal.com/vtapi/v2/file/scan' --form 'apikey=<apikey>' --form 'file=@/path/to/file'
#>
Add-Type -assembly System.Security

function Get-Hash() {

    param([string] $FilePath)

    $fileStream = [System.IO.File]::OpenRead($FilePath)
    $hash = ([System.Security.Cryptography.HashAlgorithm]::Create('SHA256')).ComputeHash($fileStream)
    $fileStream.Close()
    $fileStream.Dispose()
    [System.Bitconverter]::tostring($hash).replace('-','')
}


function Query-VirusTotal {

    param([string]$Hash)

    $body = @{ resource = $hash; apikey = $VTApiKey }
    $VTReport = Invoke-RestMethod -Method 'POST' -Uri 'https://www.virustotal.com/vtapi/v2/file/report' -Body $body
    $AVScanFound = @()

    if ($VTReport.positives -gt 0) {
        foreach($scan in ($VTReport.scans | Get-Member -type NoteProperty)) {
            if($scan.Definition -match "detected=(?<detected>.*?); version=(?<version>.*?); result=(?<result>.*?); update=(?<update>.*?})") {
                if($Matches.detected -eq "True") {
                    $AVScanFound += "{0}({1}) - {2}" -f $scan.Name, $Matches.version, $Matches.result
                }
            }
        }
    }

 

    New-Object –TypeName PSObject -Property ([ordered]@{
    MD5 = $VTReport.MD5
    SHA1 = $VTReport.SHA1
    SHA256 = $VTReport.SHA256
    VTLink = $VTReport.permalink
    VTReport = "$($VTReport.positives)/$($VTReport.total)"
    VTMessage = $VTReport.verbose_msg
    Engines = $AVScanFound
    })
}


function Get-VirusTotalReport {
    Param (
    [Parameter(Mandatory=$true, Position=0)]
    [String]$VTApiKey,

    [Parameter(Mandatory=$true, Position=1, ValueFromPipeline=$true, ParameterSetName='byHash')]
    [String[]] $Hash,

    [Parameter(Mandatory=$true, Position=1, ValueFromPipelineByPropertyName=$true, ParameterSetName='byPath')]
    [Alias('Path', 'FullName')]
    [String[]] $FilePath
    )

    Process {

        switch ($PsCmdlet.ParameterSetName) {
            'byHash' {
                $Hash | ForEach-Object {
                    Query-VirusTotal -Hash $_
                }
            }

            'byPath' {
                $FilePath | ForEach-Object {
                    Query-VirusTotal -Hash (Get-Hash -FilePath $_) |
                    Add-Member -MemberType NoteProperty -Name FilePath -Value $_ -PassThru
                }
             }
         }
     }
}
#sample usage of the functions in this section
#Get-VirusTotalReport -VTApiKey $vtApi -FilePath C:\Users\Public\virustotal.ps1
#This section of the script sets some global variables.
$rulesRepo = "\\10.0.20.57\freeedr\Rules"
$toolsRepo = "\\10.0.20.57\freeedr\Tools"
$clientFolder = "C:\Windows\freeedr\"
$vtApi = "276baa96547e2a405c3751eeb24a33b0bc63a3b965e9cbe6d2c8ddefba54168f" #hardcoded api value, change as necessary


<#
    This function installs Python 3.7 which is a requirement to run the SigmaC tool to convert a Sigma rule into a Get-WinEvent query.
#>
function installPython {
    #$pythonDestination = $toolsRepo + "\python-3.7.0.exe"
    Copy-Item $toolsRepo $clientFolder -Force
    #  Invoke-WebRequest -Uri "https://www.python.org/ftp/python/3.7.0/python-3.7.0.exe" -OutFile "C:/Windows/python-3.7.0.exe"
    C:\Windows\freeedr\python-3.7.0.exe /quiet InstallAllUsers=0 PrependPath=1 Include_test=0
}

# Pip is required to install other prerequisites for the SimgmaC python script.
function upgradePip {
    python -m pip install --upgrade pip
}

# PyYAML is required to get the YAML package for the SigmaC script to run. 
fucntion installRequirements {
    pip install pyyaml
}

# Sigma is required to translate the rules into Windows Event queries.
function getSigma {
    Copy-Item $toolsRepo $clientFolder
}

<# 
    This portion of the sciprt creates a variable for the rules repository server and the destination folder for each endpoint.
    The rules are then copied from the reposiotry server to the destination on the endpoint.
    The "-Recurse" copies all folders and files under the \rules folder
    The "-Force" will create the folder on the endpoint if it does not already exist
#>
$ruleRepository = "\\10.0.20.57\rules\Rules"
$destinationFolder = "C:\Windows"
#Copy-Item $ruleRepository $destinationFolder -Recurse -Force
# The above function has been moved to updateRules

try {python --version}
catch {installPython}
upgradePip

<#
    Compare using overall file length between the rules repo and the local rules directory. This will run periodically.
#>
function updateRules {
    $LocalFolder = "C:\Windows\rules"
    Compare-Object $LocalFolder $ruleRepository -Property Name, Length  | Where-Object {$_.SideIndicator -eq "<="} | ForEach-Object {
        Copy-Item "\\10.0.20.57\rules\$($_.name)" -Destination $LocalFolder -Force
    }
}

<#
    This is where we will pull the the Sigma script from the rules reposiotry.
#>


# Change to sigma directory and translate the rules.
cd C:\Windows\freeedr\sigma\tools

<#
    Calls sigmac to convert rules in the rules repository and store them in a variable.
    The variable is piped into a foreach-object loop and each rule is written to its own file.
    The file name is numerically increased.
#>
$rules = python sigmac -t powershell -r C:\Windows\rules\Discovery
$rules | ForEach-Object {$i = 1}{
    $_ | Out-File C:\Windows\sigmaTranslation\rule$i.txt
    $i++}

<#
    This section of the script will read the translated rules from the files into a powershell foreach loop
#>

$translatedRules = "C:\Windows\sigmaTranslation"
$translatedRules | ForEach-Object {
    $powershellQuery = Get-Content $_
    Invoke-Expression $powershellQuery

    #this following section will pull relevant informaiton from the alerted logs into variables and combine the information with data gathered from APIs

}

<#
	This next section is solely for adapted VirusTotal Code
	This code is entirely adapted from: https://gallery.technet.microsoft.com/Get-VirusTotalReport-90065fad
	This code will not upload any file to VirusTotal, but convert a file to hash, and then query for a report based on the hash. 
	If there is no corresponding hash in VT, then the script will return that the hash is not part of an existing or pending scan.
	/////
	Uploading another script for posterity and further adaptation. Curl command below for file upload
	curl --request POST --url 'https://www.virustotal.com/vtapi/v2/file/scan' --form 'apikey=<apikey>' --form 'file=@/path/to/file'
#>
Add-Type -assembly System.Security

function Get-Hash() {

    param([string] $FilePath)

    $fileStream = [System.IO.File]::OpenRead($FilePath)
    $hash = ([System.Security.Cryptography.HashAlgorithm]::Create('SHA256')).ComputeHash($fileStream)
    $fileStream.Close()
    $fileStream.Dispose()
    [System.Bitconverter]::tostring($hash).replace('-','')
}


function Query-VirusTotal {

    param([string]$Hash)

    $body = @{ resource = $hash; apikey = $VTApiKey }
    $VTReport = Invoke-RestMethod -Method 'POST' -Uri 'https://www.virustotal.com/vtapi/v2/file/report' -Body $body
    $AVScanFound = @()

    if ($VTReport.positives -gt 0) {
        foreach($scan in ($VTReport.scans | Get-Member -type NoteProperty)) {
            if($scan.Definition -match "detected=(?<detected>.*?); version=(?<version>.*?); result=(?<result>.*?); update=(?<update>.*?})") {
                if($Matches.detected -eq "True") {
                    $AVScanFound += "{0}({1}) - {2}" -f $scan.Name, $Matches.version, $Matches.result
                }
            }
        }
    }

 

    New-Object –TypeName PSObject -Property ([ordered]@{
    MD5 = $VTReport.MD5
    SHA1 = $VTReport.SHA1
    SHA256 = $VTReport.SHA256
    VTLink = $VTReport.permalink
    VTReport = "$($VTReport.positives)/$($VTReport.total)"
    VTMessage = $VTReport.verbose_msg
    Engines = $AVScanFound
    })
}


function Get-VirusTotalReport {
    Param (
    [Parameter(Mandatory=$true, Position=0)]
    [String]$VTApiKey,

    [Parameter(Mandatory=$true, Position=1, ValueFromPipeline=$true, ParameterSetName='byHash')]
    [String[]] $Hash,

    [Parameter(Mandatory=$true, Position=1, ValueFromPipelineByPropertyName=$true, ParameterSetName='byPath')]
    [Alias('Path', 'FullName')]
    [String[]] $FilePath
    )

    Process {

        switch ($PsCmdlet.ParameterSetName) {
            'byHash' {
                $Hash | ForEach-Object {
                    Query-VirusTotal -Hash $_
                }
            }

            'byPath' {
                $FilePath | ForEach-Object {
                    Query-VirusTotal -Hash (Get-Hash -FilePath $_) |
                    Add-Member -MemberType NoteProperty -Name FilePath -Value $_ -PassThru
                }
             }
         }
     }
}
#sample usage of the functions in this section
#Get-VirusTotalReport -VTApiKey $vtApi -FilePath C:\Users\Public\virustotal.ps1

<#
	This next section is solely for adapted VirusTotal Code. This is a Virus Total Powershell Module by David B Heise
	This code is entirely adapted from: https://archive.codeplex.com/?p=psvirustotal
	This code will retrieve VirusTotal reports for files, hashes, IPs, URLs, and domains
	This code will not upload the file, but will get the SHA-256 File Hash and use the hash to retrieve the VT report
#>
function Get-VTReport {
    [CmdletBinding()]
    Param( 
    [String] $VTApiKey = "276baa96547e2a405c3751eeb24a33b0bc63a3b965e9cbe6d2c8ddefba54168f", #Hardcoded API Key Value
    [Parameter(ParameterSetName="hash", ValueFromPipeline=$true,ValueFromPipelineByPropertyName=$true)][String] $hash,
    [Parameter(ParameterSetName="file", ValueFromPipeline=$true,ValueFromPipelineByPropertyName=$true)][System.IO.FileInfo] $file,
    [Parameter(ParameterSetName="uri", ValueFromPipeline=$true,ValueFromPipelineByPropertyName=$true)][Uri] $uri,
    [Parameter(ParameterSetName="ipaddress", ValueFromPipeline=$true,ValueFromPipelineByPropertyName=$true)][String] $ip,
    [Parameter(ParameterSetName="domain", ValueFromPipeline=$true,ValueFromPipelineByPropertyName=$true)][String] $domain
    )
    Begin {
        $fileUri = 'https://www.virustotal.com/vtapi/v2/file/report'
        $UriUri = 'https://www.virustotal.com/vtapi/v2/url/report'
        $IPUri = 'http://www.virustotal.com/vtapi/v2/ip-address/report'
        $DomainUri = 'http://www.virustotal.com/vtapi/v2/domain/report'
       
        function Get-Hash(
            [System.IO.FileInfo] $file = $(Throw 'Usage: Get-Hash [System.IO.FileInfo]'), 
            [String] $hashType = 'sha256')
        {
          $stream = $null;  
          [string] $result = $null;
          $hashAlgorithm = [System.Security.Cryptography.HashAlgorithm]::Create($hashType )
          $stream = $file.OpenRead();
          $hashByteArray = $hashAlgorithm.ComputeHash($stream);
          $stream.Close();

          trap
          {
            if ($stream -ne $null) { $stream.Close(); }
            break;
          }

          # Convert the hash to Hex
          $hashByteArray | foreach { $result += $_.ToString("X2") }
          return $result
        }
    }
    Process {
        [String] $h = $null
        [String] $u = $null
        [String] $method = $null
        $body = @{}

        switch ($PSCmdlet.ParameterSetName) {
        "file" { 
            $h = Get-Hash -file $file
            Write-Verbose -Message ("FileHash:" + $h)
            $u = $fileUri
            $method = 'POST'
            $body = @{ resource = $h; apikey = $VTApiKey}
            }
        "hash" {            
            $u = $fileUri
            $method = 'POST'
            $body = @{ resource = $hash; apikey = $VTApiKey}
            }
        "uri" {
            $u = $UriUri
            $method = 'POST'
            $body = @{ resource = $uri; apikey = $VTApiKey}
            }
        "ipaddress" {
            $u = $IPUri
            $method = 'GET'
            $body = @{ ip = $ip; apikey = $VTApiKey}
        }
        "domain" {            
            $u = $DomainUri
            $method = 'GET'
            $body = @{ domain = $domain; apikey = $VTApiKey}}
        }        

        return Invoke-RestMethod -Method $method -Uri $u -Body $body
    }    
}

function Invoke-VTScan {
    [CmdletBinding()]
    Param( 
    [String] $VTApiKey = "276baa96547e2a405c3751eeb24a33b0bc63a3b965e9cbe6d2c8ddefba54168f", #Hardcoded API Key Value
    [Parameter(ParameterSetName="file", ValueFromPipeline=$true,ValueFromPipelineByPropertyName=$true)]
        [System.IO.FileInfo] $file,
    [Parameter(ParameterSetName="uri", ValueFromPipeline=$true,ValueFromPipelineByPropertyName=$true)]
        [Uri] $uri
    )
    Begin {
        $fileUri = 'https://www.virustotal.com/vtapi/v2/file/scan'
        $UriUri = 'https://www.virustotal.com/vtapi/v2/url/scan'
        [byte[]]$CRLF = 13, 10

        function Get-AsciiBytes([String] $str) {
            return [System.Text.Encoding]::ASCII.GetBytes($str)            
        }
    }
    Process {
        [String] $h = $null
        [String] $u = $null
        [String] $method = $null
        $body = New-Object System.IO.MemoryStream

        switch ($PSCmdlet.ParameterSetName) {
        "file" { 
            $u = $fileUri
            $method = 'POST'
            $boundary = [Guid]::NewGuid().ToString().Replace('-','')
            $ContentType = 'multipart/form-data; boundary=' + $boundary
            $b2 = Get-AsciiBytes ('--' + $boundary)
            $body.Write($b2, 0, $b2.Length)
            $body.Write($CRLF, 0, $CRLF.Length)
            
            $b = (Get-AsciiBytes ('Content-Disposition: form-data; name="apikey"'))
            $body.Write($b, 0, $b.Length)

            $body.Write($CRLF, 0, $CRLF.Length)
            $body.Write($CRLF, 0, $CRLF.Length)
            
            $b = (Get-AsciiBytes $VTApiKey)
            $body.Write($b, 0, $b.Length)

            $body.Write($CRLF, 0, $CRLF.Length)
            $body.Write($b2, 0, $b2.Length)
            $body.Write($CRLF, 0, $CRLF.Length)
            
            $b = (Get-AsciiBytes ('Content-Disposition: form-data; name="file"; filename="' + $file.Name + '";'))
            $body.Write($b, 0, $b.Length)
            $body.Write($CRLF, 0, $CRLF.Length)            
            $b = (GgetAsciiBytes 'Content-Type:application/octet-stream')
            $body.Write($b, 0, $b.Length)
            
            $body.Write($CRLF, 0, $CRLF.Length)
            $body.Write($CRLF, 0, $CRLF.Length)
            
            $b = [System.IO.File]::ReadAllBytes($file.FullName)
            $body.Write($b, 0, $b.Length)

            $body.Write($CRLF, 0, $CRLF.Length)
            $body.Write($b2, 0, $b2.Length)
            
            $b = (Get-AsciiBytes '--')
            $body.Write($b, 0, $b.Length)
            
            $body.Write($CRLF, 0, $CRLF.Length)
            
                
            Invoke-RestMethod -Method $method -Uri $u -ContentType $ContentType -Body $body.ToArray()
            }
        "uri" {
            $h = $uri
            $u = $UriUri
            $method = 'POST'
            $body = @{ url = $uri; apikey = $VTApiKey}
            Invoke-RestMethod -Method $method -Uri $u -Body $body
            }            
        }                        
    }    
}
<#
End of adapted VT code
#>
}
<# 
    This portion of the scrip focuses on reading information from the sysmon event log to include in the custom windows event
#>

function Read-WindowsEvent {
    Param (
    [Parameter(Mandatory=$true, Position=0)]
    [string]$windowsEventQuery 
    )
     Process {
     
     $windowsEventQuery | ForEach-Object {
     
        $event = $_
        $severity = $event.LevelDisplayName
        $time = $event.TimeCreated
        $message = $event.Message

        if ($event.Id -eq 1) {
        # Parse out relevent event fields for Process creation
        }
        if ($event.Id -eq 2) {
        # Parse out relevent event fields for A process changed a file creation time
        }
        if ($event.Id -eq 3) {
        # Parse out relevent event fields for Network connection
        }
        if ($event.Id -eq 4) {
        # Parse out relevent event fields for Sysmon service state changed
        }
        if ($event.Id -eq 5) {
        # Parse out relevent event fields for Process terminated
        }
        if ($event.Id -eq 6) {
        # Parse out relevent event fields for Driver loaded
        }
        if ($event.Id -eq 7) {
        # Parse out relevent event fields for Image loaded
        }
        if ($event.Id -eq 8) {
        # Parse out relevent event fields for CreateRemoteThread
        }
        if ($event.Id -eq 9) {
        # Parse out relevent event fields for RawAccessRead
        }
        if ($event.Id -eq 10) {
        # Parse out relevent event fields for ProcessAccess
        }
        if ($event.Id -eq 11) {
        # Parse out relevent event fields for FileCreate
        }
        if ($event.Id -eq 12) {
        # Parse out relevent event fields for RegistryEvent (Object create and delete)
        }
        if ($event.Id -eq 13) {
        # Parse out relevent event fields for RegistryEvent (Value Set)
        }
        if ($event.Id -eq 14) {
        # Parse out relevent event fields for RegistryEvent (Key and Value Rename)
        }
        if ($event.Id -eq 15) {
        # Parse out relevent event fields for FileCreateStreamHash
        }
        if ($event.Id -eq 17) {
        # Parse out relevent event fields for PipeEvent (Pipe Created)
        }
        if ($event.Id -eq 18) {
        # Parse out relevent event fields for PipeEvent (Pipe Connected)
        }
        if ($event.Id -eq 19) {
        # Parse out relevent event fields for WmiEvent (WmiEventFilter activity detected)
        }
        if ($event.Id -eq 20) {
        # Parse out relevent event fields for WmiEvent (WmiEventConsumer activity detected)
        }
        if ($event.Id -eq 21) {
        # Parse out relevent event fields for WmiEvent (WmiEventConsumerToFilter activity detected)
        }
        if ($event.Id -eq 22) {
        # Parse out relevent event fields for DNSEvent (DNS query)
        }
        if ($event.Id -eq 255) {
        # Parse out relevent event fields for Error
        }
        elseif {
        # Parse out general information from all other Windows Events
        }
     
     }
     
     }
    
}
<#
    This portion of the script will take the information gathered above and write a custom Windows event to the Event Viewer.
#>

function Write-CustomEvent {
    Param (
    [Parameter(Mandatory=$true, Position=0)]
    [string]$message 
    )

    Process {
        
        $computerName = $env:COMPUTERNAME
        $entryType = "Warning"
        $eventID = 3604
        $messageInfo
        $logName = "Security"
        
        # To specify Sysmon as an event source, we need to create a new event source in the Security event log with the name Sysmon.
        [system.diagnostics.EventLog]::CreateEventSource(“Sysmon”, “Security”)

        Write-EventLog -ComputerName $computerName -EntryType $entryType -EventId $eventID -Message "This is a test custom event" -LogName $logName
    
    }
}