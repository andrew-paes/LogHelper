using System;
#if USE_MSTEST
namespace LogHelper.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MSLogHelperTests
    {
        Lazy<LogHelperTests> _tester = new Lazy<LogHelperTests>(() => { return new LogHelperTests(); });

        [TestMethod]
        public void TestOne()
        {
            _tester.Value.TestOne();
        }
    }
}
#endif

namespace LogHelper.Tests
{
    using Xunit;

    public class LogHelperTests
    {
        [Fact]
        public void TestOne() { }
    }
}