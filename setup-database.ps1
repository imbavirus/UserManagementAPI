# setup-database.ps1

# Define project paths relative to the script location (solution root)
$applicationProjectPath = ".\Application" # Or "UserManagementAPI.Application"
$apiProjectPath = ".\API"                 # Or "UserManagementAPI.API"

dotnet ef database update --project $applicationProjectPath --startup-project $apiProjectPath
