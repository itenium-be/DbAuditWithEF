Database Auditing with EntityFramework
======================================

[itenium blog post](https://itenium.be/blog/dotnet/db-audit-ef/)

Spin up a SqlServer

```sh
docker compose up -d --build
```

Port: 5174
Login: sa / password123!


EF Migrations
-------------

```ps1
cd DbAuditWithEF.DatabaseTrigger

# Install
dotnet tool install --global dotnet-ef

# Create
dotnet ef migrations add InitialCreate
dotnet ef database update

# Delete
dotnet ef migrations remove
dotnet ef database drop -f
```

The Ways
--------

- DatabaseTrigger: Set CreatedBy/On and ModifiedBy/On with a database trigger.
- ByEF: Same but set with Entity Framework. Use a ComplexType `IAudit` and overwrite manual changes to the properties.
- ByReflection: Totally avoid setting the properties manually by making them private and using reflection.
- DatabaseTable: Keep changes in a separate table with a database trigger.
- EFTable: Same but insert records with Entity Framework.
