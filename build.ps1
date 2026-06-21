$projectDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$outputDir = Join-Path $projectDir "bin\Release"
$outputExe = Join-Path $outputDir "PulseBrowser.exe"

if (!(Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
}

$csc = "C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\Roslyn\csc.exe"
$refs = @(
    "System.dll",
    "System.Core.dll",
    "System.Drawing.dll",
    "System.Windows.Forms.dll",
    "System.Web.Extensions.dll"
).ForEach({ "/reference:$_" })

$sources = @(
    "Properties\AssemblyInfo.cs",
    "Program.cs",
    "MainForm.cs",
    "BookmarkManager.cs",
    "BookmarkForm.cs",
    "HistoryManager.cs",
    "HistoryForm.cs",
    "SettingsManager.cs",
    "SettingsForm.cs"
).ForEach({ Join-Path $projectDir $_ })

$iconRes = "/resource:" + (Join-Path $projectDir "Pulse.ico") + ",PulseBrowser.Pulse.ico"

$args = @(
    "/target:winexe",
    "/out:$outputExe",
    "/platform:anycpu",
    "/optimize+",
    "/nologo",
    $iconRes
) + $refs + $sources

Write-Host "Compiling Pulse Browser..."
& $csc $args

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful! Output: $outputExe" -ForegroundColor Green
} else {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}
