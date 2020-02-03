<#
    This function installs Python 3.7 which is a requirement to run the SigmaC tool to convert a Sigma rule into a Get-WinEvent query.
#>
function installPython {
    Invoke-WebRequest -Uri "https://www.python.org/ftp/python/3.7.0/python-3.7.0.exe" -OutFile "C:/Windows/python-3.7.0.exe"
    C:/Windows/python-3.7.0.exe /quiet InstallAllUsers=0 PrependPath=1 Include_test=0
}

# Pip is required to install other prerequisites for the SimgmaC python script.
function upgradePip {
    python -m pip install --upgrade pip
}

# PyYAML is required to get the YAML package for the SigmaC script to run. 
fucntion installRequirements {
    pip install pyyaml
}

<# 
    This portion of the sciprt creates a variable for the rules repository server and the destination folder for each endpoint.
    The rules are then copied from the reposiotry server to the destination on the endpoint.
    The "-Recurse" copies all folders and files under the \rules folder
    The "-Force" will create the folder on the endpoint if it does not already exist
#>
$ruleRepository = "\\10.0.20.57\rules"
$destinationFolder = "C:\Windows"
Copy-Item $ruleRepository $destinationFolder -Recurse -Force

try {python --version}
catch {installPython}
upgradePip

<#
    This is where we will pull the the Sigma script from the rules reposiotry.
#>

# Change to sigma directory and translate the rules.
cd C:\Windows\sigma\tools

<#
    Calls sigmac to convert rules in the rules repository and store them in a variable.
    The variable is piped into a foreach-object loop and each rule is written to its own file.
    The file name is numerically increased.
#>
$rules = python sigmac -t powershell -r C:\Windows\rules\Discovery
$rules | ForEach-Object {$i = 1}{
    $_ | Out-File C:\Windows\sigmaTranslation\rule$i.txt
    $i++}