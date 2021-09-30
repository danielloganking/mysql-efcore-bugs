# MySQL/EF Core Connector Bugs

This example project is used to demonstrate a number of bugs found in the interactions between MySQL.EntityFrameworkCore and EF Core.

## Version Info

MySQL version: 8.0.25
.NET runtime version: Core 3.1
EF Core version: 5.0.5
MySQL.EntityFrameworkCore version: 5.0.5

## Initial Setup

1. Clone Repo

    First we need to set up an example project we can use. A minimum example is available in this [git repo](https://github.com/danielloganking/mysql-efcore-bugs).

2. Add Connection String

    In the example project, navigate to `Database/ExampleDbContext.cs` and replace the placeholder `<YOUR_CONNECTION_STRING_HERE>` with the connection string to a MySQL 8.0.23+ instance. It is strongly recommended to specify a new, named database to avoid data corruption--eg `database=test`.

3. Install EF

    This is only necessary you have not already installed EF as a global dotnet tool. From the repo root run:

    ```sh
    dotnet tool restore
    ```

4. Run Migrations to Setup Database

    From the repo root run:

    ```sh
    dotnet ef database update
    ```

## 1. DateTimeOffset Inappropriate Time Shift

### Problem

When using a `DateTimeOffset` column on an entity, MySQL is returning data shifted from the original timestamp (eg 12:00 -0400) to the UTC timestamp without updating the timezone info (eg returning 16:00 -0400 when it should be either 16:00 +0000 or the originally provided 12:00 -0400). Note, this applies regardless if the original `DateTimeOffset` is in UTC or local time.

Further, changing the timezone of the database (either globally or for the session) does not seem to affect this behavior.

Note, the correct value is stored in the database, so this appears to be a deserialization issue.

### Steps to Reproduce

1. From the repo root run:

    ```sh
    dotnet test --filter FullyQualifiedName~App.Tests.DateTimeOffsetBug
    ```

### Expected Behavior

We expect the timestamp data to be returned either as the local `DateTime` with a local `Offset` _or_ as the UTC `DateTime` with a UTC `Offset`. Both tests should pass.

### Actual Behavior

Timestamp data is returned as the UTC `DateTime` with a _local_ `Offset`. For example, storing 12:00-0400 is shifted to 16:00-0400 when it should be remain as is or be 16:00+0000.
