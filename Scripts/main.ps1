#This section of the script sets some global variables.
$rulesRepo = "\\10.0.20.57\freeedr\Rules"
$toolsRepo = "\\10.0.20.57\freeedr\Tools"
$sigmaRepo = "\\10.0.20.57\freeedr\sigma"
$clientFolder = "C:\Windows\freeedr\"
$vtApi = "276baa96547e2a405c3751eeb24a33b0bc63a3b965e9cbe6d2c8ddefba54168f" #hardcoded api value, change as necessary
$API_hybrid = 'e2mnl2wcd7b0dfbbmkbx3qumaa6ddfeceptxuy5uab870ed5hp7jjzzo455675fc' #static HA api key, limited to 200 reqs and hr


<#
    This function installs Python 3.7 which is a requirement to run the SigmaC tool to convert a Sigma rule into a Get-WinEvent query.
#>
function Install-Python {
    New-Item -Path "C:\Windows" -Name "freeedr" -ItemType "directory" -Force
    Copy-Item $toolsRepo $clientFolder -Force -Recurse
    C:\Windows\freeedr\Tools\python-3.7.0.exe /quiet InstallAllUsers=0 PrependPath=1 Include_test=0
}

# Pip is required to install other prerequisites for the SimgmaC python script.
function Upgrade-Pip {
    python -m pip install --upgrade pip
}

# PyYAML is required to get the YAML package for the SigmaC script to run. 
function Install-Requirements {
    pip install pyyaml
}

# Sigma is required to translate the rules into Windows Event queries.
function Get-Sigma {
    Copy-Item $sigmaRepo $clientFolder -Force -Recurse
}

<# 
    This portion of the sciprt creates a variable for the rules repository server and the destination folder for each endpoint.
    The rules are then copied from the reposiotry server to the destination on the endpoint.
    The "-Recurse" copies all folders and files under the \rules folder
    The "-Force" will create the folder on the endpoint if it does not already exist
#>

<#
    Compare using overall file length between the rules repo and the local rules directory. This will run periodically.
#>
function Update-Rules {
    $LocalFolder = "C:\Windows\"
    Compare-Object $LocalFolder $rulesRepo -Property Name, Length  | Where-Object {$_.SideIndicator -eq "<="} | ForEach-Object {
        Copy-Item "\\10.0.20.57\freeedr\Rules\$($_.name)" -Destination $LocalFolder -Force -Recurse
    }
}
<#---------------------------------------------ALL PREREQUIREMENTS FULLFILLED  --------------------------------------------------#>

<#
    Calls sigmac to convert rules in the rules repository and store them in a variable.
    The variable is piped into a foreach-object loop and each rule is written to its own file.
    The file name is numerically increased.
#>
function Translate-Rules {
    cd C:\Windows\freeedr\sigma\tools
    $translatedRules = python sigmac -t powershell -r C:\Windows\Rules\Demo
    $translatedRules | ForEach-Object {$i = 1}{
        $rule = $_
        $rule | Out-File C:\Windows\sigmaTranslation\rule$i.txt
        $i++
    }
}

<#
    This section of the script will read the translated rules from the files into a powershell foreach loop
#>

function Search-WinEvent {
    $translatedRuleDir = "C:\Windows\sigmaTranslation"
    Get-ChildItem $translatedRuleDir -Recurse | ForEach-Object {
        $path = $_.FullName
        $powershellQuery = Get-Content $path
        Invoke-Expression $powershellQuery
    }
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

    $body = @{ resource = $hash; apikey = "276baa96547e2a405c3751eeb24a33b0bc63a3b965e9cbe6d2c8ddefba54168f" } #hardcoded api value, change as necessary
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

function Query-VirusTotalNet {

    param([string]$uri)

    $body = @{ resource = $uri; apikey = "276baa96547e2a405c3751eeb24a33b0bc63a3b965e9cbe6d2c8ddefba54168f" } #hardcoded api value, change as necessary
    $VTReport = Invoke-RestMethod -Method 'POST' -Uri 'https://www.virustotal.com/vtapi/v2/url/report' -Body $body
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

#sample usage of the functions in this section
#Get-VirusTotalReport -FilePath C:\Users\Public\virustotal.ps1
function Get-VirusTotalReport {
    Param (
    [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true, ParameterSetName='byHash')]
    [String[]] $Hash,

    [Parameter(Mandatory=$true, Position=0, ValueFromPipelineByPropertyName=$true, ParameterSetName='byPath')]
    [Alias('Path', 'FullName')]
    [String[]] $FilePath,
	
    [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true, ValueFromPipelineByPropertyName=$true, ParameterSetName="byUri")]
	[String[]] $uri,
	
    [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true, ValueFromPipelineByPropertyName=$true, ParameterSetName="byIP")]
	[String[]] $ip,
	
    [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true, ValueFromPipelineByPropertyName=$true, ParameterSetName="byDomain")]
	[String[]] $domain
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
			
			'byUri' {
                $uri | ForEach-Object {
                    Query-VirusTotalNet -uri $_
					#Get-VTReport -uri $_
                }
            }
			
			'byIP' {
                $ip | ForEach-Object {
                    Query-VirusTotalNet -uri $_
					Get-VTReport -ip $_
                }
            }
			
			'byDomain' {
                $domain | ForEach-Object {
                    Query-VirusTotalNet -uri $_
					#Get-VTReport -domain $_
                }
            }
        }
    }
}

<#
End of adapted VT code
This next portion of the script focuses on reading information from the sysmon event log to include in the custom windows event
#>

function Invoke-HybridAnalysis {
    param (
    [Parameter(Mandatory=$true, Position=0)]
    [string]$filePath 
    )
    $verdict = 'Clean'
    $h = Get-Hash -FilePath $filePath
    $a =  Invoke-RestMethod "https://www.hybrid-analysis.com/api/v2/search/hash?_timestamp=1570613873480" -Headers @{'accept' = 'application/json'; 'user-agent' = 'Falcon Sandbox' ; 'Content-Type' = 'application/x-www-form-urlencoded' ; 'api-key' = "$API_hybrid"} -Method Post -Body @{'hash' = "$h" }
    #In the case that the number of returned results is 0, in which case the hash is unknown
    if($a.count -eq 0){
        return 'Unknown File Hash'
    }
    foreach($b in $a){ 
        #For our purposes, if HA sees a single malicious analysis based on a static file hash it is considered malicious
        if ($b.verdict -eq 'malicious'){ 
            $verdict = 'Malicious'
        }
    }
    return $verdict
}

#example usage: Invoke-HybridAnalysis -filePath 'C:\Users\tester\Desktop\mimikatz.exe'. Returns either Unknown File Hash, malicious, or clean

function Read-WindowsEvent {
    Param (
    [Parameter(Mandatory=$true, Position=0)]
    $windowsEvent
    )
    Process {
     
        $Global:event = $windowsEvent
        $Global:severity = $event.LevelDisplayName
        $Global:time = $event.TimeCreated
        $Global:message = $event.Message

        if ($event.Id -eq 1) {
            
            $event.Message -match "Process Create:\s?([^\n]+)"
            $processCreate = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "RuleName:\s?([^\n]+)"
            $ruleName = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "UtcTime:\s?([^\n]+)"
            $utcTime = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "ProcessGuid:\s?([^\n]+)"
            $processGuid = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ProcessId:\s?([^\n]+)"
            $processId = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "Image:\s?([^\n]+)"
            $imagesValue = $Matches.Values | Select-Object -First 1
            $imagesValue = $imagesValue.Trim()
            $images = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "FileVersion:\s?([^\n]+)"
            $fileVersion = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "Description:\s?([^\n]+)"
            $description = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "Product:\s?([^\n]+)"
            $product = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "Company:\s?([^\n]+)"
            $company = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "OriginalFileName:\s?([^\n]+)"
            $originalFileNameValue = $Matches.Values | Select-Object -First 1
            $originalFileName = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "CommandLine:\s?([^\n]+)"
            $commandLine = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "CurrentDirectory:\s?([^\n]+)"
            $currentDirectory = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "User:\s?([^\n]+)"
            $user = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "LogonGuid:\s?([^\n]+)"
            $logonGuid = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "LogonId:\s?([^\n]+)"
            $logonId = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "TerminalSessionId:\s?([^\n]+)"
            $terminalSessionId = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "IntegrityLevel:\s?([^\n]+)"
            $integrityLevel = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "Hashes:\s?([^\n]+)"
            $hashes = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "MD5=([^,]+)"
            $md5 = $Matches.Values | Select-Object -First 1

            $event.Message -match "ParentProcessId:\s?([^\n]+)"
            $parentProcessId = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "ParentImage:\s?([^\n]+)"
            $parentImage = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "ParentCommandLine:\s?([^\n]+)"
            $parentCommandLine = $Matches.Values | Select-Object -Skip 1

            $Global:VirusTotalResults = Get-VirusTotalReport -FilePath $imagesValue
            $Global:HybridAnalyResults = Invoke-HybridAnalysis -filePath $imagesValue
            $Global:HybridAnalyResults = "Hybrid Analysis Results: " + $Global:HybridAnalyResults
        }

        elseif ($event.Id -eq 2) {
            
            $event.Message -match "File creation time changed: ?([^\n]+)"
            $fileCreationTimeChanged = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "RuleName: ?([^\n]+)"
            $ruleName = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "UtcTime: ?([^\n]+)"
            $utcTime = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ProcessGuid: ?([^\n]+)"
            $processGuid = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ProcessId: ?([^\n]+)"
            $processId = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "Image: ?([^\n]+)"
            $imagesValue = $Matches.Values | Select-Object -First 1
            $imagesValue = $imagesValue.Trim()
            $image = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "TargetFilename: ?([^\n]+)"
            $targetFilename = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "CreationUtcTime: ?([^\n]+)"
            $creationUtcTime = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "PreviousCreationUtcTime: ?([^\n]+)"
            $previousCreationUtcTime = $Matches.Values | Select-Object -Skip 1

            $Global:VirusTotalResults = Get-VirusTotalReport -FilePath $imagesValue
            $Global:HybridAnalyResults = Invoke-HybridAnalysis -filePath $imagesValue
            $Global:HybridAnalyResults = "Hybrid Analysis Results: " + $Global:HybridAnalyResults
            
        }

        elseif ($event.Id -eq 3) {
            
            $event.Message -match "Network connection detected: ?([^\n]+)"
            $networkConnectionDetected = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "RuleName: ?([^\n]+)"
            $ruleName = $Matches.Values | Select-Object -Skip 1           
            
            $event.Message -match "UtcTime: ?([^\n]+)"
            $utcTime = $Matches.Values | Select-Object -Skip 1            
            
            $event.Message -match "ProcessGuid: ?([^\n]+)"
            $processGuid = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ProcessId: ?([^\n]+)"
            $processId = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "User: ?([^\n]+)"
            $user = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "Protocol: ?([^\n]+)"
            $protocol = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "Initiated: ?([^\n]+)"
            $initiated = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "SourceIsIpv6: ?([^\n]+)"
            $sourceIsIpv6 = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "SourceIp: ?([^\n]+)"
            $sourceIpValue = $Matches.Values | Select-Object -First 1
            $sourceIpValue = $sourceIpValue.Trim()
            $sourceIp = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "SourceHostname: ?([^\n]+)"
            $sourceHostname = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "SourcePort: ?([^\n]+)"
            $sourcePort = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "SourcePortName: ?([^\n]+)"
            $sourcePortName = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "DestinationIsIpv6: ?([^\n]+)"
            $destinationIsIpv6 = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "DestinationIp: ?([^\n]+)"
            $destinationIp = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "DestinationHostname: ?([^\n]+)"
            $destinationHostname = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "DestinationPort: ?([^\n]+)"
            $destinationPort = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "DestinationPortName: ?([^\n]+)"
            $destinationPortName = $Matches.Values | Select-Object -Skip 1

            $Global:VirusTotalResults = Get-VirusTotalReport -ip $sourceIpValue
        
        }

        elseif ($event.Id -eq 4) {
            
            $event.Message -match "Sysmon service state changed: ?([^\n]+)"
            $serviceStateChanged = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "UtcTime: ?([^\n]+)"
            $utcTime = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "State: ?([^\n]+)"
            $state = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "Version: ?([^\n]+)"
            $version = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "SchemaVersion: ?([^\n]+)"
            $schemaVersion = $Matches.Values | Select-Object -Skip 1

        }

        elseif ($event.Id -eq 5) {
            
            $event.Message -match "Process terminated: ?([^\n]+)"
            $processTerminated = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "RuleName: ?([^\n]+)"
            $ruleName = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "UtcTime: ?([^\n]+)"
            $utcTime = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ProcessGuid: ?([^\n]+)"
            $processGuid = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ProcessId: ?([^\n]+)"
            $processId = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "Image: ?([^\n]+)"
            $imagesValue = $Matches.Values | Select-Object -First 1
            $imagesValue = $imagesValue.Trim()
            $image = $Matches.Values | Select-Object -Skip 1

            $Global:VirusTotalResults = Get-VirusTotalReport -FilePath $imagesValue
            $Global:HybridAnalyResults = Invoke-HybridAnalysis -filePath $imagesValue
            $Global:HybridAnalyResults = "Hybrid Analysis Results: " + $Global:HybridAnalyResults

        }

        elseif ($event.Id -eq 6) {
        
            $event.Message -match "Driver loaded: ?([^\n]+)"
            $driverLoaded = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "RuleName: ?([^\n]+)"
            $ruleName = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "UtcTime: ?([^\n]+)"
            $utcTime = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ImageLoaded: ?([^\n]+)"
            $imageLoadedValue = $Matches.Values | Select-Object -First 1
            $imageLoadedValue = $imageLoadedValue.Trim()
            $imageLoaded = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "Hashes: ?([^\n]+)"
            $hashes = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "Signed: ?([^\n]+)"
            $signed = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "Signature: ?([^\n]+)"
            $signature = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "SignatureStatus: ?([^\n]+)"
            $signatureStatus = $Matches.Values | Select-Object -Skip 1
            
            $Global:VirusTotalResults = Get-VirusTotalReport -FilePath $imageLoadedValue
            $Global:HybridAnalyResults = Invoke-HybridAnalysis -filePath $imageLoadedValue
            $Global:HybridAnalyResults = "Hybrid Analysis Results: " + $Global:HybridAnalyResults
        }

        elseif ($event.Id -eq 7) {
        # Parse out relevent event fields for Image loaded
        }

        elseif ($event.Id -eq 8) {
            
            $event.Message -match "RuleName: ?([^\n]+)"
            $ruleName = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "UtcTime: ?([^\n]+)"
            $utcTime = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "SourceProcessGuid: ?([^\n]+)"
            $sourceProcessGuid = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "SourceProcessId: ?([^\n]+)"
            $sourceProcessId = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "SourceImage: ?([^\n]+)"
            $sourceImageValue = $Matches.Values | Select-Object -First 1
            $sourceImageValue = $sourceImageValue.Trim()
            $sourceImage = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "TargetProcessGuid: ?([^\n]+)"
            $targetProcessGuid = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "TargetProcessId: ?([^\n]+)"
            $targetProcessId = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "TargetImage: ?([^\n]+)"
            $targetImage = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "NewThreadId: ?([^\n]+)"
            $newThreadId = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "StartAddress: ?([^\n]+)"
            $startAddress = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "StartModule: ?([^\n]+)"
            $startModule = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "StartFunction: ?([^\n]+)"
            $startFunction = $Matches.Values | Select-Object -Skip 1

            $Global:VirusTotalResults = Get-VirusTotalReport -FilePath $sourceImageValue
            $Global:HybridAnalyResults = Invoke-HybridAnalysis -filePath $sourceImageValue
            $Global:HybridAnalyResults = "Hybrid Analysis Results: " + $Global:HybridAnalyResults
        
        }
        elseif ($event.Id -eq 9) {
        # Parse out relevent event fields for RawAccessRead
        }

        elseif ($event.Id -eq 10) {
        # Parse out relevent event fields for ProcessAccess
        }

        elseif ($event.Id -eq 11) {
            
            $event.Message -match "File created: ?([^\n]+)"
            $fileCreated = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "RuleName: ?([^\n]+)"
            $ruleName = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "UtcTime: ?([^\n]+)"
            $utcTime = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ProcessGuid: ?([^\n]+)"
            $processGuid = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ProcessId: ?([^\n]+)"
            $processId = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "TargetFilename: ?([^\n]+)"
            $targetFilenameValue = $Matches.Values | Select-Object -First 1
            $targetFilenameValue = $targetFilenameValue.Trim()
            $targetFilename = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "CreationUtcTime: ?([^\n]+)"
            $creationUtcTime = $Matches.Values | Select-Object -Skip 1

            $Global:VirusTotalResults = Get-VirusTotalReport -FilePath $targetFilenameValue
            $Global:HybridAnalyResults = Invoke-HybridAnalysis -filePath $targetFilenameValue
            $Global:HybridAnalyResults = "Hybrid Analysis Results: " + $Global:HybridAnalyResults

        }

        elseif ($event.Id -eq 12) {
            
            $event.Message -match "Registry object added or deleted: ?([^\n]+)"
            $objectAddedOrDeleted = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "RuleName: ?([^\n]+)"
            $ruleName = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "EventType: ?([^\n]+)"
            $eventType = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "UtcTime: ?([^\n]+)"
            $utcTime = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ProcessGuid: ?([^\n]+)"
            $processGuid = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ProcessId: ?([^\n]+)"
            $processId = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "Image: ?([^\n]+)"
            $imagesValue = $Matches.Values | Select-Object -First 1
            $imagesValue = $imagesValue.Trim()
            $image = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "TargetObject: ?([^\n]+)"
            $targetObject = $Matches.Values | Select-Object -Skip 1

            $Global:VirusTotalResults = Get-VirusTotalReport -FilePath $imagesValue
            $Global:HybridAnalyResults = Invoke-HybridAnalysis -filePath $imagesValue
            $Global:HybridAnalyResults = "Hybrid Analysis Results: " + $Global:HybridAnalyResults

        }

        elseif ($event.Id -eq 13) {
            
            $event.Message -match "Registry value set: ?([^\n]+)"
            $registryValueSet = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "RuleName: ?([^\n]+)"
            $ruleName = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "EventType: ?([^\n]+)"
            $eventType = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "UtcTime: ?([^\n]+)"
            $utcTime = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ProcessGuid: ?([^\n]+)"
            $processGuid = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ProcessId: ?([^\n]+)"
            $processId = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "Image: ?([^\n]+)"
            $imagesValue = $Matches.Values | Select-Object -First 1
            $imagesValue = $imagesValue.Trim()
            $image = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "TargetObject: ?([^\n]+)"
            $targetObject = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "Details: ?([^\n]+)"
            $details = $Matches.Values | Select-Object -Skip 1

            $Global:VirusTotalResults = Get-VirusTotalReport -FilePath $imagesValue
            $Global:HybridAnalyResults = Invoke-HybridAnalysis -filePath $imagesValue
            $Global:HybridAnalyResults = "Hybrid Analysis Results: " + $Global:HybridAnalyResults

        }

        elseif ($event.Id -eq 14) {
        # Parse out relevent event fields for RegistryEvent (Key and Value Rename)
        }

        elseif ($event.Id -eq 15) {
            
            $event.Message -match "File stream created: ?([^\n]+)"
            $fileStreamCreated = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "RuleName: ?([^\n]+)"
            $ruleName = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "UtcTime: ?([^\n]+)"
            $utcTime = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ProcessGuid: ?([^\n]+)"
            $processGuid = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ProcessId: ?([^\n]+)"
            $processId = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "Image: ?([^\n]+)"
            $imagesValue = $Matches.Values | Select-Object -First 1
            $imagesValue = $imagesValue.Trim()
            $image = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "TargetFilename: ?([^\n]+)"
            $targetFilename = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "CreationUtcTime: ?([^\n]+)"
            $creationUtcTime = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "Hash: ?([^\n]+)"
            $hash = $Matches.Values | Select-Object -Skip 1

            $Global:VirusTotalResults = Get-VirusTotalReport -FilePath $imagesValue
            $Global:HybridAnalyResults = Invoke-HybridAnalysis -filePath $imagesValue
            $Global:HybridAnalyResults = "Hybrid Analysis Results: " + $Global:HybridAnalyResults
        
        }

        elseif ($event.Id -eq 17) {
        # Parse out relevent event fields for PipeEvent (Pipe Created)
        }

        elseif ($event.Id -eq 18) {
        # Parse out relevent event fields for PipeEvent (Pipe Connected)
        }

        elseif ($event.Id -eq 19) {
        # Parse out relevent event fields for WmiEvent (WmiEventFilter activity detected)
        }

        elseif ($event.Id -eq 20) {
        # Parse out relevent event fields for WmiEvent (WmiEventConsumer activity detected)
        }

        elseif ($event.Id -eq 21) {
        # Parse out relevent event fields for WmiEvent (WmiEventConsumerToFilter activity detected)
        }

        elseif ($event.Id -eq 22) {
            
            $event.Message -match "Dns query: ?([^\n]+)"
            $dnsQuery = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "RuleName: ?([^\n]+)"
            $ruleName = $Matches.Values | Select-Object -Skip 1

            $event.Message -match "UtcTime: ?([^\n]+)"
            $utcTime = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ProcessGuid: ?([^\n]+)"
            $event.Message -match "ProcessId: ?([^\n]+)"
            $processId = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "QueryName: ?([^\n]+)"
            $queryName = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "QueryStatus: ?([^\n]+)"
            $queryStatus = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "QueryResults: ?([^\n]+)"
            $queryResults = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "Image: ?([^\n]+)"
            $imagesValue = $Matches.Values | Select-Object -First 1
            $imagesValue = $imagesValue.Trim()
            $image = $Matches.Values | Select-Object -Skip 1

            $Global:VirusTotalResults = Get-VirusTotalReport -FilePath $imagesValue
            $Global:HybridAnalyResults = Invoke-HybridAnalysis -filePath $imagesValue
            $Global:HybridAnalyResults = "Hybrid Analysis Results: " + $Global:HybridAnalyResults
        
        }

        elseif ($event.Id -eq 255) {
            
            $event.Message -match "Error report: ?([^\n]+)"
            $errorReport = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "UtcTime: ?([^\n]+)"
            $utcTime = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "ID: ?([^\n]+)"
            $id = $Matches.Values | Select-Object -Skip 1
            
            $event.Message -match "Description: ?([^\n]+)"
            $description = $Matches.Values | Select-Object -Skip 1
        
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
        $logName = "Application"
        $source = "SysmonLogs"
        
        # To specify Sysmon as an event source, we need to create a new event source in the Security event log with the name Sysmon.
        Write-EventLog -ComputerName $computerName -EntryType $entryType -EventId $eventID -Message $message -LogName $logName -Source $source
    
    }
}

function main {
    <#-------------------------Install Pre-Requirements-------------------------#>

    try {python --version} # Installs Python and checks version
    catch {Install-Python}
    Upgrade-Pip # Upgrades Pip
    Install-Requirements # Installs PyYAML
    Get-Sigma # Copies the Sigma repository from server
    Update-Rules # Copies rules from server down to C:\Windows\Rules folder

    # Change to sigma directory and translate the rules.
    cd C:\Windows\freeedr\sigma\tools
    Translate-Rules # Translates rules and places them in rules translation folder.
    Search-WinEvent | ForEach-Object {
        Read-WindowsEvent -windowsEvent $_
        $newMessage = $message + "`n" + $VirusTotalResults + "`n" + $HybridAnalyResults
        Write-CustomEvent -message $newMessage
    }
     


}

main