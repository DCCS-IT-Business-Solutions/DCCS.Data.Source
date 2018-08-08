using System;
using NUnit.Framework;

namespace DCCS.REST.Data
{

    [TestFixture]
    public class ParamsTest
    {
        [Test]
        public void CopyConstructor_Should_copy()
        {
            var src = new Params { Page = 1, Count = 2, OrderBy = "order", Desc = true };
            var sut = new Params(src);

            Assert.AreEqual(src.Page, sut.Page);
            Assert.AreEqual(src.Count, sut.Count);
            Assert.AreEqual(src.OrderBy, sut.OrderBy);
            Assert.AreEqual(src.Desc, sut.Desc);
        }
    }
}
