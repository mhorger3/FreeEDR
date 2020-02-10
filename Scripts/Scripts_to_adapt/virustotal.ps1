<#

*********** This is Powershell PS1 script so ensure you have the right libraries loaded first ************

Syntax #1 : Get-VirusTotalReport -VTApiKey <your API key without brackets> -Hash <sha256 of file required>
Syntax #2 : Get-VirusTotalReport -VTApiKey <your API key without brackets> -FilePath C:\temp\kamran.exe


Get the public API for free by signing up on https://www.virustotal.com/en/documentation/public-api (its free !!!!)

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