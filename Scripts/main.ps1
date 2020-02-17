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