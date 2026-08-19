[CmdletBinding()]
param(
    [string]$Mod,
    [string]$Project,
    [string[]]$RequiredSourceId = @()
)

$ErrorActionPreference = 'Stop'
Import-Module (Join-Path $PSScriptRoot 'ModTools.psm1') -Force

function Get-ColumnName {
    param([string]$CellReference)

    return [regex]::Match($CellReference, '^[A-Z]+').Value
}

$modProject = Resolve-ModProject -Mod $Mod -ProjectPath $Project
$sourceSheets = Get-ModSourceSheets -ModProject $modProject
if ($sourceSheets.Count -eq 0) {
    if ($RequiredSourceId.Count -gt 0) {
        throw "No SourceSheet.xlsx was found for $($modProject.ModName), but required Source IDs were specified."
    }

    Write-Host "No SourceSheet.xlsx found for $($modProject.ModName). Skipped."
    return
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
$allSourceIds = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
foreach ($sourceSheet in $sourceSheets) {
    $sourceSheetPath = $sourceSheet.FullName
    $archive = [IO.Compression.ZipFile]::OpenRead($sourceSheetPath)
    try {
    $requiredEntries = @(
        '[Content_Types].xml',
        'xl/workbook.xml',
        'xl/sharedStrings.xml',
        'xl/worksheets/sheet1.xml'
    )
    foreach ($requiredEntry in $requiredEntries) {
        if ($null -eq $archive.GetEntry($requiredEntry)) {
            throw "SourceSheet is missing required entry: $requiredEntry"
        }
    }

    $sharedStringsEntry = $archive.GetEntry('xl/sharedStrings.xml')
    $sharedStringsReader = [IO.StreamReader]::new($sharedStringsEntry.Open())
    try {
        [xml]$sharedStringsXml = $sharedStringsReader.ReadToEnd()
    }
    finally {
        $sharedStringsReader.Dispose()
    }

    $namespaceManager = [Xml.XmlNamespaceManager]::new($sharedStringsXml.NameTable)
    $namespaceManager.AddNamespace('x', 'http://schemas.openxmlformats.org/spreadsheetml/2006/main')
    $sharedStrings = @($sharedStringsXml.SelectNodes('/x:sst/x:si', $namespaceManager) | ForEach-Object { $_.InnerText })

    $sheetEntry = $archive.GetEntry('xl/worksheets/sheet1.xml')
    $sheetReader = [IO.StreamReader]::new($sheetEntry.Open())
    try {
        [xml]$sheetXml = $sheetReader.ReadToEnd()
    }
    finally {
        $sheetReader.Dispose()
    }

    $sheetNamespaceManager = [Xml.XmlNamespaceManager]::new($sheetXml.NameTable)
    $sheetNamespaceManager.AddNamespace('x', 'http://schemas.openxmlformats.org/spreadsheetml/2006/main')
    $rows = @($sheetXml.SelectNodes('/x:worksheet/x:sheetData/x:row', $sheetNamespaceManager))
    if ($rows.Count -eq 0) {
        throw 'SourceSheet does not contain any rows.'
    }

    $parsedRows = [Collections.Generic.List[hashtable]]::new()
    foreach ($row in $rows) {
        $values = @{}
        foreach ($cell in @($row.SelectNodes('./x:c', $sheetNamespaceManager))) {
            $column = Get-ColumnName $cell.r
            if ($cell.t -eq 's') {
                $sharedStringIndex = [int]$cell.v
                if ($sharedStringIndex -lt 0 -or $sharedStringIndex -ge $sharedStrings.Count) {
                    throw "Cell $($cell.r) refers to missing shared string index $sharedStringIndex."
                }
                $values[$column] = $sharedStrings[$sharedStringIndex]
            }
            else {
                $values[$column] = [string]$cell.v
            }
        }
        $values['__row'] = [int]$row.r
        $parsedRows.Add($values)
    }

    $header = $parsedRows | Where-Object { $_['__row'] -eq 1 } | Select-Object -First 1
    if ($null -eq $header) {
        throw 'SourceSheet header row 1 was not found.'
    }

    $columns = @{}
    foreach ($entry in $header.GetEnumerator()) {
        if ($entry.Key -ne '__row' -and -not [string]::IsNullOrWhiteSpace([string]$entry.Value)) {
            $columns[[string]$entry.Value] = $entry.Key
        }
    }
    foreach ($requiredColumn in @('id', 'text_JP', 'text')) {
        if (-not $columns.ContainsKey($requiredColumn)) {
            throw "SourceSheet header does not contain required column: $requiredColumn"
        }
    }

    $sourceIdColumn = $columns['id']
    $japaneseColumn = $columns['text_JP']
    $englishColumn = $columns['text']
    $sourceRows = @($parsedRows | Where-Object { $_['__row'] -ge 4 -and -not [string]::IsNullOrWhiteSpace([string]$_[$sourceIdColumn]) })
    $duplicates = @($sourceRows | Group-Object { [string]$_[$sourceIdColumn] } | Where-Object Count -gt 1)
    if ($duplicates.Count -gt 0) {
        $duplicateIds = $duplicates | ForEach-Object Name
        throw "SourceSheet contains duplicate Source IDs:`n$($duplicateIds -join "`n")"
    }

    $missingTranslations = @($sourceRows | Where-Object {
        [string]::IsNullOrWhiteSpace([string]$_[$japaneseColumn]) -or
        [string]::IsNullOrWhiteSpace([string]$_[$englishColumn])
    })
    if ($missingTranslations.Count -gt 0) {
        $missingRows = $missingTranslations | ForEach-Object { "row $($_['__row']): $($_[$sourceIdColumn])" }
        throw "SourceSheet contains Source IDs without JP or EN text:`n$($missingRows -join "`n")"
    }

    foreach ($sourceRow in $sourceRows) {
        [void]$allSourceIds.Add([string]$sourceRow[$sourceIdColumn])
    }

    Write-Host "SourceSheet verified: $sourceSheetPath"
    Write-Host "  Source IDs: $($sourceRows.Count)"
    Write-Host "  Shared strings: $($sharedStrings.Count)"
    }
    finally {
        $archive.Dispose()
    }
}

$missingRequiredIds = @($RequiredSourceId | Where-Object { -not $allSourceIds.Contains($_) })
if ($missingRequiredIds.Count -gt 0) {
    throw "SourceSheet is missing required Source IDs:`n$($missingRequiredIds -join "`n")"
}
if ($RequiredSourceId.Count -gt 0) {
    Write-Host "Required Source IDs verified: $($RequiredSourceId.Count)"
}
