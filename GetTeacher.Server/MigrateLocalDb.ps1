# Always run in Development mode
$Environment = "Development"

# Validate environment
if ($Environment -ne "Development") {
    Write-Host "This script is only allowed to run in 'Development' mode." -ForegroundColor Red
    exit 1
}

# Set project paths
$projectPath = "."
$migrationsPath = ".\Services\Database\Migrations\"
$localDbPath = ".\LocalDevelopmentDb.db"

# Set the environment
$env:ASPNETCORE_ENVIRONMENT = $Environment
Write-Host "Using environment: $Environment" -ForegroundColor Yellow

# Delete everything in the Migrations folder
if (Test-Path $migrationsPath) {
    Write-Host "Deleting all files in $migrationsPath..." -ForegroundColor Yellow
    Get-ChildItem -Path $migrationsPath -Recurse | Remove-Item -Force -Recurse
    Write-Host "All files in $migrationsPath have been deleted." -ForegroundColor Green
} else {
    Write-Host "$migrationsPath does not exist. Skipping deletion." -ForegroundColor Cyan
}

# Ask user to confirm deletion of LocalDevelopmentDb.db
if (Test-Path $localDbPath) {
    $deleteDb = Read-Host "Do you want to delete LocalDB? (y/n)"
    if ($deleteDb -eq "y") {
        Write-Host "Deleting $localDbPath..." -ForegroundColor Yellow
        Remove-Item $localDbPath -Force
        Write-Host "$localDbPath has been deleted." -ForegroundColor Green
    } else {
        Write-Host "$localDbPath will not be deleted." -ForegroundColor Cyan
    }
} else {
    Write-Host "$localDbPath does not exist. Skipping deletion." -ForegroundColor Cyan
}

# Ensure dotnet-ef CLI tools are installed
Write-Host "Checking if dotnet-ef CLI tools are installed..."
dotnet tool install --global dotnet-ef --version "*"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to install dotnet-ef. Please install manually and try again." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Generate a new migration
Write-Host "Generating a new migration..."
$migrationName = "GetTeacher"
dotnet ef migrations add $migrationName --project $projectPath --output-dir $migrationsPath
if ($LASTEXITCODE -ne 0) {
    Write-Host "Migration generation failed. Please check your setup." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Update the database
Write-Host "Updating the database..."
dotnet ef database update --project $projectPath
if ($LASTEXITCODE -ne 0) {
    Write-Host "Database update failed. Please check your database connection string and setup." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "Migration and database update completed successfully!" -ForegroundColor Green
