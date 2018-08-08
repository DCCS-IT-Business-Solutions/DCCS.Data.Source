using Bogus;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DCCS.REST.Data.Tests
{

    [TestFixture]
    public class ResultTest
    {
        class Dummy
        {
            public string Name { get; set; }
        }
        public ResultTest()
        {
            Randomizer.Seed = new Random(876543);
        }

        [Test]
        public void Should_create_empty_data()
        {
            var sut = new Result<string>(new Params(), new List<string>().AsQueryable());

            Assert.IsNotNull(sut.Data);
        }

        [Test]
        public void Should_return_total()
        {
            var total = 100;
            var data = new Faker<Dummy>().Generate(total);
            var sut = new Result<Dummy>(new Params(), data.AsQueryable());

            Assert.AreEqual(total, sut.Total);
        }

        [Test]
        public void Should_return_total_with_paging()
        {
            var total = 100;
            var data = new Faker<Dummy>().Generate(total);
            var sut = new Result<Dummy>(new Params { Count = 10, Page = 2 }, data.AsQueryable());

            Assert.AreEqual(total, sut.Total);
        }

        [Test]
        public void Should_return_correct_total_if_count_bigger_than_total()
        {
            var total = 100;
            var data = new Faker<Dummy>().Generate(total);
            var sut = new Result<Dummy>(new Params { Count = 200, Page = 2 }, data.AsQueryable());

            Assert.AreEqual(total, sut.Total);
        }

        [Test]
        public void Should_return_correct_page_if_requested_page_not_available()
        {
            var total = 100;
            var data = new Faker<Dummy>().Generate(total);
            var sut = new Result<Dummy>(new Params { Count = 200, Page = 2 }, data.AsQueryable());

            Assert.AreEqual(1, sut.Page);
        }
    }
}
