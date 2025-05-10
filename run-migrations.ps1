# run-migrations.ps1

param(
    [Parameter(Mandatory=$true, HelpMessage="Enter the name for the EF Core migration.")]
    [string]$MigrationName
)

Write-Host "-----------------------------------------------------"
Write-Host "Starting EF Core Migration Process"
Write-Host "Migration Name: $MigrationName"
Write-Host "-----------------------------------------------------"

# Define project paths relative to the script location (solution root)
$applicationProjectPath = ".\Application" # Or "UserProfileBackend.Application"
$apiProjectPath = ".\API"                 # Or "UserProfileBackend.API"

# Step 1: Add Migration
Write-Host ""
Write-Host "Step 1: Adding migration '$MigrationName'..."
Write-Host "Executing: dotnet ef migrations add $MigrationName --project $applicationProjectPath --startup-project $apiProjectPath"
Write-Host ""

dotnet ef migrations add $MigrationName --project $applicationProjectPath --startup-project $apiProjectPath

# Check if the last command was successful
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to add migration '$MigrationName'. Exit code: $LASTEXITCODE"
    Write-Host "-----------------------------------------------------"
    Write-Host "EF Core Migration Process Aborted."
    Write-Host "-----------------------------------------------------"
    exit $LASTEXITCODE # Exit the script with the error code
}

Write-Host ""
Write-Host "Migration '$MigrationName' added successfully."
Write-Host "-----------------------------------------------------"

# Step 2: Update Database
Write-Host ""
Write-Host "Step 2: Applying migration and updating the database..."
Write-Host "Executing: dotnet ef database update --project $applicationProjectPath --startup-project $apiProjectPath"
Write-Host ""

dotnet ef database update --project $applicationProjectPath --startup-project $apiProjectPath

# Check if the last command was successful
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to update the database. Exit code: $LASTEXITCODE"
    Write-Host "-----------------------------------------------------"
    Write-Host "EF Core Migration Process Failed at Database Update."
    Write-Host "-----------------------------------------------------"
    exit $LASTEXITCODE # Exit the script with the error code
}

Write-Host ""
Write-Host "Database updated successfully."
Write-Host "-----------------------------------------------------"
Write-Host "EF Core Migration Process Completed Successfully!"
Write-Host "-----------------------------------------------------"

