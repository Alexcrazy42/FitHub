$MigrationName = Read-Host "Enter migration name"
$DataContextName = Read-Host "Enter dataContext name"

dotnet ef migrations add $MigrationName --startup-project src/Dashboard --project src/Data --context $DataContextName --verbose
