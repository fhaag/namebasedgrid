$scriptDir = $PSScriptRoot

$inkscapePath = $Env:INKSCAPE_PATH
if (-not $inkscapePath) {
	Write-Output 'Inkscape path not set.'
	return
}

$logoPath = [System.IO.Path]::Combine($scriptDir, 'logo.svg')
&"$inkscapePath" --export-type="png" --export-width=128 "$logoPath"

$slnDir = [System.IO.Path]::Combine($scriptDir, '..', 'src')
$slnPath = [System.IO.Path]::Combine($slnDir, 'NameBasedGrid.sln')
$pjPath = [System.IO.Path]::Combine($slnDir, 'NameBasedGrid', 'NameBasedGrid.csproj')

&dotnet build "$slnPath" -c Release

&dotnet pack "$pjPath" -o "$scriptDir"