# Define the root folder and output file
$rootFolder = ".\Assets"
$outputFile = ".\context.txt"

# Clear output file if it exists
if (Test-Path $outputFile) {
    Clear-Content $outputFile
}

# Get all .cs files recursively and append their content to the output file
Get-ChildItem -Path $rootFolder -Recurse -Filter *.cs | ForEach-Object {
    $fileName = $_.Name
    Add-Content -Path $outputFile -Value "`n// ----- File: $fileName -----`n"
    Get-Content $_.FullName | Add-Content -Path $outputFile
}