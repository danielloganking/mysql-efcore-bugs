# MySQL/EF Core Migration Bug

## Problem

When using a `DateTimeOffset` column on an entity, MySQL is returning data shifted from the original timestamp (eg 12:00 -0400) to the UTC timestamp without updating the timezone info (eg returning 16:00 -0400 when it should be either 16:00 +0000 or the originally provided 12:00 -0400). Note, this applies regardless if the original `DateTimeOffset` is in UTC or local time.

Further, changing the timezone of the database (either globally or for the session) does not seem to affect this behavior.

Note, the correct value _is_ stored in the database so this appears to be an issue with the MySQL EF Connector.

## Version Info

MySQL version: 8.0.25
.NET runtime version: Core 3.1
EF Core version: 5.0.5
MySQL.EntityFrameworkCore version: 5.0.5

## Steps to Reproduce

1. Get Example Project

    First we need to set up an example project we can use. A minimum example is available in this [git repo](https://github.com/danielloganking/timestamp-efcore-bug).

2. Add Connection String

    In the example project, navigate to `ExampleDbContext.cs` and replace the placeholder `<YOUR_CONNECTION_STRING_HERE>` with the connection string to a MySQL 8.0.23+ instance. It is strongly recommended to specify a new, named database to avoid data corruption--eg `database=test`.

3. Install EF

    This is only necessary you have not already installed EF as a global dotnet tool. From the repo root run:

    ```sh
    dotnet tool restore
    ```

4. Run Initial Migration

    From the repo root run:

    ```sh
    dotnet ef database update --project ./app.csproj
    ```

5. Run Tests

    From the repo root run:

    ```sh
    dotnet test
    ```

### Expected Behavior

We expect the timestamp data to be returned either as the local `DateTime` with a local `Offset` _or_ as the UTC `DateTime` with a UTC `Offset`. Both tests should pass.

### Actual Behavior

Timestamp data is returned as the UTC `DateTime` with a _local_ `Offset`. For example, storing 12:00-0400 is shifted to 16:00-0400 when it should be 16:00+0000.
