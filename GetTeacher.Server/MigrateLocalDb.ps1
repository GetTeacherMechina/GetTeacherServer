# Set project paths
$projectPath = "."
$migrationsPath = ".\Services\Database\Migrations\"

# Ensure dotnet-ef CLI tools are installed
Write-Host "Checking if dotnet-ef CLI tools are installed..."
dotnet tool install --global dotnet-ef --version "*"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to install dotnet-ef. Please install manually and try again." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Generate a new migration
Write-Host "Generating a new migration..."
$migrationName = Read-Host "Enter migration name"
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
