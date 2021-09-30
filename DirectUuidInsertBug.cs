using System;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace App.Tests
{
    public class DirectUuidInsertBug : IDisposable
    {
        private readonly ExampleDbContext dbContext;

        private readonly ITestOutputHelper output;

        public DirectUuidInsertBug(ITestOutputHelper outputHelper)
        {
            dbContext = new ExampleDbContext();
            output = outputHelper;
        }

        [Fact]
        public void EfInserted_EntityCanBeRetrievedViaDirectSql()
        {
            var id = Guid.NewGuid();
            var entity = dbContext.Entities.Add(new Entity { Id = id }).Entity;
            dbContext.SaveChanges();

            // this just allows us to retrieve the DML SQL UUID's byte order
            var fake = dbContext.Entities.FromSqlInterpolated(@$"
                SELECT UUID_TO_BIN({id.ToString()}) as `Id`, CURRENT_TIMESTAMP() as `Example`;
            ").AsEnumerable().FirstOrDefault();

            output.WriteLine($"EF Core byte order: {ToReadableString(entity.Id.ToByteArray())}");
            output.WriteLine($"DML SQL byte order: {ToReadableString(fake.Id.ToByteArray())}");

            var selected = dbContext.Entities.FromSqlInterpolated(@$"
                SELECT * FROM Entities WHERE Id = UUID_TO_BIN({id.ToString()});
            ").AsEnumerable().FirstOrDefault();

            selected.Should().NotBeNull();
        }

        [Fact]
        public void DirectSqlInsert_EntityCanBeRetrievedViaEf()
        {
            var id = Guid.NewGuid();
            var idString = id.ToString();
            var inserted = dbContext.Entities.FromSqlInterpolated(@$"
                INSERT INTO Entities (Id)
                VALUES (UUID_TO_BIN({idString}))

                SELECT * FROM Entities WHERE Id = UUID_TO_BIN({id.ToString()});
            ").AsEnumerable().FirstOrDefault();            

            output.WriteLine($"NETcore byte order: {ToReadableString(id.ToByteArray())}");
            output.WriteLine($"DML SQL byte order: {ToReadableString(inserted.Id.ToByteArray())}");

            dbContext.Entities.Find(id).Should().NotBeNull();
        }

        private string ToReadableString(byte[] array)
        {
            var str = "";
            foreach (var item in array)
            {
                str += item.ToString() + " ";
            }
            return str.TrimEnd();
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}