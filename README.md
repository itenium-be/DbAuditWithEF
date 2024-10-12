Database Auditing with EntityFramework
======================================

Spin up a SqlServer

```sh
docker compose up -d --build
```

Port: 5174
Login: sa / password123!


DatabaseTrigger
---------------

```ps1
cd DbAuditWithEF.DatabaseTrigger
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```
