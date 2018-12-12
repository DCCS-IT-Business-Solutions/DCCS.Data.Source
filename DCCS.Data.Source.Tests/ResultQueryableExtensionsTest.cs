using Bogus;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DCCS.Data.Source.Tests
{

    [TestFixture]
    public class ResultQueryableExtensionsTest
    {
        class Dummy
        {
            public string Name { get; set; }
        }

        class DummyDTO
        {
            public string Name { get; set; }
            public int Length { get; set; }
        }
        public ResultQueryableExtensionsTest()
        {
            Randomizer.Seed = new Random(876543);
        }

        [Test]
        public void Should_create_Result()
        {
            var total = 100;
            var data = new Faker<Dummy>().Generate(total);
            var sut = data.AsQueryable().ToResult(new Params());

            Assert.AreEqual(total, sut.Total);
        }
    }
}
