Set-StrictMode -Version Latest

function Get-RepositoryRoot {
    return (Resolve-Path -LiteralPath (Split-Path -Parent $PSScriptRoot)).Path
}

function Test-RepositoryPath {
    param(
        [Parameter(Mandatory)]
        [string]$Path,
        [Parameter(Mandatory)]
        [string]$RepositoryRoot
    )

    $fullPath = [IO.Path]::GetFullPath($Path)
    $rootWithSeparator = [IO.Path]::GetFullPath($RepositoryRoot).TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
    return $fullPath.StartsWith($rootWithSeparator, [StringComparison]::OrdinalIgnoreCase)
}

function Resolve-ModProject {
    param(
        [string]$Mod,
        [string]$ProjectPath
    )

    if ([string]::IsNullOrWhiteSpace($Mod) -eq [string]::IsNullOrWhiteSpace($ProjectPath)) {
        throw 'Specify exactly one of -Mod or -Project.'
    }

    $repositoryRoot = Get-RepositoryRoot
    if (-not [string]::IsNullOrWhiteSpace($Mod)) {
        if ($Mod -match '[\\/]') {
            throw "-Mod must be a Mod directory name, not a path: $Mod"
        }
        $candidatePath = Join-Path $repositoryRoot "$Mod\$Mod.csproj"
    }
    else {
        $candidatePath = if ([IO.Path]::IsPathRooted($ProjectPath)) {
            $ProjectPath
        }
        else {
            Join-Path $repositoryRoot $ProjectPath
        }
    }

    if (-not (Test-Path -LiteralPath $candidatePath -PathType Leaf)) {
        throw "Mod project was not found: $candidatePath"
    }

    $resolvedProjectPath = (Resolve-Path -LiteralPath $candidatePath).Path
    if ([IO.Path]::GetExtension($resolvedProjectPath) -ne '.csproj' -or -not (Test-RepositoryPath -Path $resolvedProjectPath -RepositoryRoot $repositoryRoot)) {
        throw "Project must be a .csproj file under the repository root: $resolvedProjectPath"
    }

    $modPath = Split-Path -Parent $resolvedProjectPath
    return [PSCustomObject]@{
        RepositoryRoot = $repositoryRoot
        ProjectPath = $resolvedProjectPath
        ModPath = $modPath
        ModName = [IO.Path]::GetFileNameWithoutExtension($resolvedProjectPath)
    }
}

function Get-ModBuildProperties {
    param(
        [Parameter(Mandatory)]
        [psobject]$ModProject,
        [Parameter(Mandatory)]
        [string]$Configuration
    )

    $arguments = @(
        'msbuild',
        $ModProject.ProjectPath,
        '-nologo',
        "-property:Configuration=$Configuration",
        '-getProperty:ElinGamePath',
        '-getProperty:OutputPath',
        '-getProperty:AssemblyName'
    )
    $output = (& dotnet @arguments | Out-String).Trim()
    if ($LASTEXITCODE -ne 0) {
        throw "Could not resolve MSBuild output properties for $($ModProject.ModName)."
    }

    try {
        $properties = ($output | ConvertFrom-Json).Properties
    }
    catch {
        throw "Could not parse MSBuild output properties for $($ModProject.ModName). Output:`n$output"
    }

    foreach ($propertyName in @('ElinGamePath', 'OutputPath', 'AssemblyName')) {
        if ([string]::IsNullOrWhiteSpace([string]$properties.$propertyName)) {
            throw "MSBuild did not resolve $propertyName for $($ModProject.ModName)."
        }
    }

    return $properties
}

function Get-ModSourceSheets {
    param(
        [Parameter(Mandatory)]
        [psobject]$ModProject
    )

    $langModPath = Join-Path $ModProject.ModPath 'package\LangMod'
    if (-not (Test-Path -LiteralPath $langModPath -PathType Container)) {
        return @()
    }

    return @(Get-ChildItem -LiteralPath $langModPath -Recurse -File -Filter 'SourceSheet.xlsx')
}

function Test-ModPackageOutput {
    param(
        [Parameter(Mandatory)]
        [psobject]$ModProject,
        [Parameter(Mandatory)]
        [string]$OutputPath,
        [Parameter(Mandatory)]
        [string]$AssemblyName
    )

    $expectedFiles = [Collections.Generic.List[string]]::new()
    $expectedFiles.Add((Join-Path $OutputPath "$AssemblyName.dll"))

    $packagePath = Join-Path $ModProject.ModPath 'package'
    if (Test-Path -LiteralPath $packagePath -PathType Container) {
        foreach ($asset in @(Get-ChildItem -LiteralPath $packagePath -Recurse -File)) {
            $relativePath = $asset.FullName.Substring($packagePath.Length).TrimStart('\', '/')
            $expectedFiles.Add((Join-Path $OutputPath $relativePath))
        }
    }

    $missingFiles = @($expectedFiles | Where-Object { -not (Test-Path -LiteralPath $_ -PathType Leaf) })
    if ($missingFiles.Count -gt 0) {
        throw "Build completed, but required output files were not found:`n$($missingFiles -join "`n")"
    }

    return $expectedFiles
}

Export-ModuleMember -Function Get-RepositoryRoot, Resolve-ModProject, Get-ModBuildProperties, Get-ModSourceSheets, Test-ModPackageOutput
