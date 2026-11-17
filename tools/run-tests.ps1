# Runs the EditMode and/or PlayMode tests from a terminal and prints a summary.
# Close the Unity Editor first.
#
#   pwsh tools/run-tests.ps1                  # both suites
#   pwsh tools/run-tests.ps1 -Platform EditMode
#   pwsh tools/run-tests.ps1 -Unity "C:\path\to\Unity.exe"
#
# Exits 0 when every suite run passes, 1 otherwise.
param(
    [ValidateSet("EditMode", "PlayMode", "Both")]
    [string]$Platform = "Both",
    [string]$Unity,
    [string]$OutDir = (Join-Path ([IO.Path]::GetTempPath()) "ruined-king-tests")
)

$project = Split-Path $PSScriptRoot -Parent

function Find-Unity {
    $version = (Select-String -Path (Join-Path $project "ProjectSettings/ProjectVersion.txt") -Pattern '^m_EditorVersion: (.+)$').Matches[0].Groups[1].Value.Trim()
    $roots = @()
    $hubSetting = Join-Path $env:APPDATA "UnityHub/secondaryInstallPath.json"
    if (Test-Path $hubSetting) {
        $custom = (Get-Content $hubSetting -Raw | ConvertFrom-Json)
        if ($custom) { $roots += $custom }
    }
    $roots += Join-Path $env:ProgramFiles "Unity/Hub/Editor"
    foreach ($root in $roots) {
        $exe = Join-Path $root "$version/Editor/Unity.exe"
        if (Test-Path $exe) { return $exe }
    }
    throw "Unity $version not found under: $($roots -join ', '). Pass -Unity <path to Unity.exe>."
}

if (-not $Unity) { $Unity = Find-Unity }
if (Get-Process Unity -ErrorAction SilentlyContinue) {
    Write-Warning "A Unity process is running. Close the editor first, or the run fails on the project lock."
}
New-Item -ItemType Directory -Force $OutDir | Out-Null

$platforms = if ($Platform -eq "Both") { "EditMode", "PlayMode" } else { , $Platform }
$allPassed = $true

foreach ($p in $platforms) {
    $results = Join-Path $OutDir "$p.xml"
    $log = Join-Path $OutDir "$p.log"
    Remove-Item $results -ErrorAction SilentlyContinue

    $timer = [Diagnostics.Stopwatch]::StartNew()
    # Wait on the process, never on its output: relay_win keeps the pipe open (see AGENTS.md).
    $proc = Start-Process -FilePath $Unity -PassThru -WindowStyle Hidden -ArgumentList @(
        "-batchmode", "-nographics", "-projectPath", "`"$project`"",
        "-runTests", "-testPlatform", $p,
        "-testResults", "`"$results`"", "-logFile", "`"$log`"")
    $proc.WaitForExit()
    try { Get-Process relay_win -ErrorAction Stop | Stop-Process -Force -Confirm:$false } catch {}
    $seconds = [int]$timer.Elapsed.TotalSeconds

    if (-not (Test-Path $results)) {
        $allPassed = $false
        "${p}: no results (exit $($proc.ExitCode), ${seconds}s). Log: $log"
        Select-String -Path $log -Pattern 'error CS\d+' | Select-Object -First 20 | ForEach-Object { "  $($_.Line.Trim())" }
        continue
    }

    [xml]$xml = Get-Content $results
    $run = $xml.'test-run'
    "${p}: $($run.result) total=$($run.total) passed=$($run.passed) failed=$($run.failed) skipped=$($run.skipped) (${seconds}s)"
    foreach ($case in $xml.SelectNodes("//test-case[@result='Failed']")) {
        "  FAILED $($case.fullname)"
        $message = $case.SelectSingleNode("failure/message")
        if ($message) { "    $($message.InnerText.Trim() -replace '\s*\r?\n\s*', ' | ')" }
    }
    if ($run.result -notlike "Passed*") { $allPassed = $false }
}

if ($allPassed) { exit 0 } else { exit 1 }
