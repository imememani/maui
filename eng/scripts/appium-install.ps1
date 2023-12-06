<#
.SYNOPSIS

Install dependencies for Appium UITests

.DESCRIPTION

This will install or update npm, appium and the following drivers: appium-windows-driver, uiautomator2, xcuitest and mac2

.PARAMETER appiumVersion

The Appium version to install

.PARAMETER windowsDriverVersion

The windows driver version to update or install

.PARAMETER androidDriverVersion

The uiautomator2 driver version to update or install

.PARAMETER iOSDriverVersion

The xcuitest driver version to update or install

.PARAMETER macDriverVersion

The mac2 driver version to update or install


.EXAMPLE

PS> .\appium-install.ps1 '2.1.1' 2.7.2 2.25.1 4.30.2 1.6.1

This would install or update Appium version 2.1.1, the windows driver 2.7.2, the uiautomator2 driver with 2.25.1, the xcuitest driver with 4.30.2 and mac2 driver with 1.6.1
#>


param
(
    [string] $appiumVersion = '2.1.1',
    [string] $windowsDriverVersion = '2.10.1',
    [string] $androidDriverVersion = '2.29.4',
    [string] $iOSDriverVersion = '5.2.0',
    [string] $macDriverVersion = '1.7.2'
)

Write-Output  "Welcome to the Appium installer"

Write-Output  "Node version"
node -v

# # Capture global NPM modules folder in a variable
# $nodeModulesRoot=npm root -g

# # Get all files in the subfolder matching the pattern for either appium or .appium-????????
# $appiumPattern = ".appium(-[0-9a-zA-Z]{8})|appium$"

# # Get all subfolders in the specified path matching the pattern, including hidden folders
# $subfolders = Get-ChildItem -Path $nodeModulesRoot -Directory -Force | Where-Object { $_.Name -match $appiumPattern }

# # One or more subfolders found, let's remove them to be sure
# if ($subfolders.Count -gt 0) {
#     Write-Host "$nodeModulesRoot has subfolders matching '$appiumPattern', cleaning up existing appium modules..."

#     #Get-ChildItem -Path $nodeModulesRoot -Directory -Filter ".appium-????????" -Force | ForEach-Object { Remove-Item -Recurse $_.FullName -Force }
#     #Remove-Item -Recurse $nodeModulesRoot/appium
#     npm rm appium
# }

Write-Output  "Installing appium $appiumVersion"
npm install appium@$appiumVersion

write-Output  "Installed appium"
appium -v

# # Get all files in the subfolder matching the pattern for either appium-doctor or .appium-doctor-????????
# $appiumDoctorPattern = ".appium-doctor(-[0-9a-zA-Z]{8})|appium-doctor$"

# # Get all subfolders in the specified path matching the pattern, including hidden folders
# $subfolders = Get-ChildItem -Path $nodeModulesRoot -Directory -Force | Where-Object { $_.Name -match $appiumDoctorPattern }

# # One or more subfolders found, let's remove them to be sure
# if ($subfolders.Count -gt 0) {
#     Write-Host "$nodeModulesRoot has subfolders matching '$appiumDoctorPattern', cleaning up existing appium-doctor modules..."

#     #Get-ChildItem -Path $nodeModulesRoot -Directory -Filter ".appium-doctor-????????" -Force | ForEach-Object { Remove-Item -Recurse $_.FullName -Force }
#     #Remove-Item -Recurse $nodeModulesRoot/appium-doctor
#     npm rm appium-doctor
# }

Write-Output  "Installing appium doctor"
npm install appium-doctor

Write-Output  "Installed appium doctor"

$existingDrivers = appium driver list --installed --json  | ConvertFrom-Json
Write-Output "List of installed drivers $existingDrivers"
if ($existingDrivers.windows) {
    Write-Output  "Uninstalling appium driver windows"
    appium driver uninstall windows
    Write-Output  "Unistalled appium driver windows"
}

if ($existingDrivers.uiautomator2) {
    Write-Output  "Uninstalling appium driver uiautomator2"
    appium driver uninstall uiautomator2
    Write-Output  "Unistalled appium driver uiautomator2"
}

if ($existingDrivers.xcuitest) {
    Write-Output  "Uninstalling appium driver xcuitest"
    appium driver uninstall xcuitest
    Write-Output  "Unistalled appium driver xcuitest"
}

if ($existingDrivers.mac2) {
    Write-Output  "Uninstalling appium driver mac2"
    appium driver uninstall mac2
    Write-Output  "Unistalled appium driver mac2"
}

$drivers = appium driver list --installed --json  | ConvertFrom-Json
Write-Output "List of installed drivers after cleaup $drivers"

Write-Output  "We will now install the appium drivers windows $windowsDriverVersion, uiautomator2 $androidDriverVersion, xcuitest $iOSDriverVersion and mac2 $macDriverVersion"

Write-Output  "Installing appium driver windows $windowsDriverVersion"
appium driver install --source=npm appium-windows-driver@$windowsDriverVersion
Write-Output  "Installed appium driver windows"

Write-Output  "Installing appium driver uiautomator2 $androidDriverVersion"
appium driver install uiautomator2@$androidDriverVersion
Write-Output  "Installed appium driver uiautomator2"

Write-Output  "Installing appium driver xcuitest $iOSDriverVersion"
appium driver install xcuitest@$iOSDriverVersion
Write-Output  "Installed appium driver xcuitest"

Write-Output  "Installing appium driver mac2 $macDriverVersion"
appium driver install mac2@$macDriverVersion
Write-Output  "Installed appium driver mac2"

Write-Output  "Check everything is installed correctly with appium doctor"
appium-doctor

Write-Output  "Done, thanks!"
