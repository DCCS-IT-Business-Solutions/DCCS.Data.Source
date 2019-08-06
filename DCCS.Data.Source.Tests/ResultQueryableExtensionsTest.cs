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

        [Test]
        public void Should_create_Result_without_Total()
        {
            var total = 100;
            var data = new Faker<Dummy>().Generate(total);
            var sut = data.AsQueryable().ToResultWithoutTotal(new Params());
            Assert.IsNotNull(sut);
        }


        [Test]
        public void Should_create_ResultWithoutTotal_to_less_results()
        {
            var total = 100;
            var data = new Faker<Dummy>().Generate(total);
            var sut = data.AsQueryable().ToResultWithoutTotal(new Params { Page = 1, Count = 10000 });
            Assert.AreEqual(100, sut.Data.Count());
        }

        [Test]
        public void Should_create_ResultWithoutTotal_with_first_page()
        {
            var total = 100;
            var data = new Faker<Dummy>().Generate(total);
            // We request a none existing page, this should fallback in returning the first page
            var sut = data.AsQueryable().ToResultWithoutTotal(new Params { Page = 1000, Count = 10 });
            Assert.AreEqual(1, sut.Page);
            Assert.AreEqual(10, sut.Data.Count());
        }

    }
}
