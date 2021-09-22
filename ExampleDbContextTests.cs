using System;
using FluentAssertions;
using Xunit;

namespace App.Tests
{
    public class ExampleDbContextTests : IDisposable
    {
        private readonly ExampleDbContext dbContext;

        public ExampleDbContextTests()
        {
            dbContext = new ExampleDbContext();
        }

        [Fact]
        public void StandardTimestamp_BehavesAsExpected()
        {
            var now = DateTimeOffset.Now;
            var entity = new Entity() { Id = Guid.NewGuid() };
            dbContext.Entities.Add(entity);
            dbContext.SaveChanges();

            entity.Example.Should().BeCloseTo(now, 30_000);
        }

        [Fact]
        public void UtcTimestamp_BehavesAsExpected()
        {
            var now = DateTimeOffset.UtcNow;
            var entity = new Entity() { Id = Guid.NewGuid() };
            dbContext.Entities.Add(entity);
            dbContext.SaveChanges();

            entity.Example.Should().BeCloseTo(now, 30_000);
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}