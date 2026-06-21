param(
    [string]$InstallPath = "$env:LOCALAPPDATA\PulseBrowser",
    [switch]$Silent,
    [switch]$Uninstall
)

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$exeName = "PulseBrowser.exe"
$iconName = "Pulse.ico"
$appName = "Pulse Browser"
$companyName = "Pulse"
$uninstallKey = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\PulseBrowser"

function Write-Info { if (!$Silent) { Write-Host @args } }

if ($Uninstall) {
    Write-Info "Uninstalling $appName..." -ForegroundColor Yellow

    $shortcutDirs = @(
        [Environment]::GetFolderPath("Desktop"),
        [Environment]::GetFolderPath("StartMenu") + "\Programs\$appName.lnk",
        [Environment]::GetFolderPath("StartMenu") + "\Programs\Accessories\$appName.lnk"
    )

    $desktopLnk = Join-Path ([Environment]::GetFolderPath("Desktop")) "$appName.lnk"
    if (Test-Path $desktopLnk) { Remove-Item $desktopLnk -Force; Write-Info "  Removed desktop shortcut" }
    $startMenuLnk = Join-Path ([Environment]::GetFolderPath("StartMenu")) "Programs\$appName.lnk"
    if (Test-Path $startMenuLnk) { Remove-Item $startMenuLnk -Force; Write-Info "  Removed Start menu shortcut" }

    if (Test-Path $InstallPath) {
        Remove-Item $InstallPath -Recurse -Force
        Write-Info "  Removed installation directory"
    }

    if (Test-Path $uninstallKey) { Remove-Item $uninstallKey -Recurse -Force }

    Write-Info "$appName uninstalled successfully." -ForegroundColor Green
    return
}

Write-Info "========================================" -ForegroundColor Cyan
Write-Info "  $appName Installer" -ForegroundColor Cyan
Write-Info "========================================" -ForegroundColor Cyan
Write-Info ""

if (!(Test-Path $InstallPath)) {
    New-Item -ItemType Directory -Path $InstallPath -Force | Out-Null
}

Write-Info "Copying files to $InstallPath..."
Copy-Item (Join-Path $scriptDir "bin\Release\$exeName") (Join-Path $InstallPath $exeName) -Force
Copy-Item (Join-Path $scriptDir $iconName) (Join-Path $InstallPath $iconName) -Force

$icoPath = Join-Path $InstallPath $iconName

$desktopShortcut = Join-Path ([Environment]::GetFolderPath("Desktop")) "$appName.lnk"
$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut($desktopShortcut)
$shortcut.TargetPath = Join-Path $InstallPath $exeName
$shortcut.WorkingDirectory = $InstallPath
$shortcut.Description = "$appName - Fast web browser"
$shortcut.IconLocation = "$icoPath, 0"
$shortcut.Save()
Write-Info "  Created desktop shortcut"

$startMenuDir = Join-Path ([Environment]::GetFolderPath("StartMenu")) "Programs"
if (!(Test-Path $startMenuDir)) { New-Item -ItemType Directory -Path $startMenuDir -Force | Out-Null }
$startMenuShortcut = Join-Path $startMenuDir "$appName.lnk"
$shortcut2 = $shell.CreateShortcut($startMenuShortcut)
$shortcut2.TargetPath = Join-Path $InstallPath $exeName
$shortcut2.WorkingDirectory = $InstallPath
$shortcut2.Description = "$appName - Fast web browser"
$shortcut2.IconLocation = "$icoPath, 0"
$shortcut2.Save()
Write-Info "  Created Start menu shortcut"

$uninstallString = "powershell.exe -NoProfile -ExecutionPolicy Bypass -File `"$InstallPath\install.ps1`" -Uninstall -Silent"
$installDate = Get-Date -Format "yyyyMMdd"

New-Item -Path $uninstallKey -Force | Out-Null
Set-ItemProperty -Path $uninstallKey -Name "DisplayName" -Value $appName
Set-ItemProperty -Path $uninstallKey -Name "DisplayVersion" -Value "1.0.0"
Set-ItemProperty -Path $uninstallKey -Name "Publisher" -Value $companyName
Set-ItemProperty -Path $uninstallKey -Name "InstallDate" -Value $installDate
Set-ItemProperty -Path $uninstallKey -Name "UninstallString" -Value $uninstallString
Set-ItemProperty -Path $uninstallKey -Name "DisplayIcon" -Value $icoPath
Set-ItemProperty -Path $uninstallKey -Name "InstallLocation" -Value $InstallPath
Write-Info "  Registered uninstall entry"

Copy-Item (Join-Path $scriptDir "install.ps1") (Join-Path $InstallPath "install.ps1") -Force

Write-Info ""
Write-Info "Installation complete!" -ForegroundColor Green
Write-Info "  Location: $InstallPath"
Write-Info "  To uninstall, run: $InstallPath\install.ps1 -Uninstall"
Write-Info "  Or use Settings > Apps > Installed Apps"
Write-Info ""
Write-Info "Launching $appName..." -ForegroundColor Cyan
Start-Process (Join-Path $InstallPath $exeName) -WindowStyle Normal
