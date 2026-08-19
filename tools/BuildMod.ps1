[CmdletBinding()]
param(
    [string]$Mod,
    [string]$Project,
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',
    [switch]$Restore,
    [switch]$ValidationOutput
)

$ErrorActionPreference = 'Stop'
Import-Module (Join-Path $PSScriptRoot 'ModTools.psm1') -Force

$modProject = Resolve-ModProject -Mod $Mod -ProjectPath $Project
$properties = Get-ModBuildProperties -ModProject $modProject -Configuration $Configuration
$gamePath = [string]$properties.ElinGamePath
$outputPath = [string]$properties.OutputPath
$assemblyName = [string]$properties.AssemblyName

if ($ValidationOutput) {
    $outputPath = Join-Path $modProject.RepositoryRoot "artifacts\$($modProject.ModName)\"
}
$outputPath = [IO.Path]::GetFullPath($outputPath)

Write-Host "Project: $($modProject.ProjectPath)"
Write-Host "Game:    $gamePath"
Write-Host "Output:  $outputPath"

if (-not $ValidationOutput -and -not (Test-Path -LiteralPath $gamePath -PathType Container)) {
    throw "ElinGamePath does not exist: $gamePath"
}

$buildArguments = @('build', $modProject.ProjectPath, '--configuration', $Configuration, '-nologo')
if (-not $Restore) {
    $buildArguments += '--no-restore'
}
if ($ValidationOutput) {
    $buildArguments += "-property:OutputPath=$outputPath"
}

& dotnet @buildArguments
if ($LASTEXITCODE -ne 0) {
    throw "Build failed with exit code $LASTEXITCODE."
}

$verifiedFiles = Test-ModPackageOutput -ModProject $modProject -OutputPath $outputPath -AssemblyName $assemblyName
Write-Host 'Build output verified:'
$verifiedFiles | ForEach-Object { Write-Host "  $_" }
