$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:Seeding_RunOnStartup = "true"
$env:Auth_RequireConfirmedEmail = "false"
$env:SuperAdmin_Password="1234qQ"

docker compose up -d
