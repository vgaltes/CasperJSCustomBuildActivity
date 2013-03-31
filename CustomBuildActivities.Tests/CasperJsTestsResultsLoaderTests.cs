namespace CustomBuildActivities.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    [TestClass]
    public class CasperJsTestsResultsLoaderTests
    {
        [TestMethod]
        public void EmptyTestsResults_CreateNoTests()
        {
            CasperJsTestsResults results = CasperJsTestsResultsLoader.Load(GetEmptyTestsResults());

            results.PassedTests.Should().HaveCount(0);
            results.FailedTests.Should().HaveCount(0);
        }

        [TestMethod]
        public void TestSuiteWithOnePassedTest_CreateOnePassedTest()
        {
            CasperJsTestsResults results = CasperJsTestsResultsLoader.Load(GetTestsResultsOnePassing());

            results.PassedTests.Should().HaveCount(1);
            results.FailedTests.Should().HaveCount(0);
        }

        [TestMethod]
        public void TestSuiteWithOnePassedTest_CreateOnePassedTestWithPopertiesFilled()
        {
            CasperJsTestsResults results = CasperJsTestsResultsLoader.Load(GetTestsResultsOnePassing());

            results.PassedTests.Should().HaveCount(1);
            results.FailedTests.Should().HaveCount(0);
            results.PassedTests[0].Should().Be(new CasperJsTest
                {
                    File = "testCasperJS.js",
                    Name = "url is the one expected",
                    Time = 0.459
                });
        }

        [TestMethod]
        public void TestSuiteWithTwoPassedTest_CreateTwoPassedTestWithPopertiesFilled()
        {
            CasperJsTestsResults results = CasperJsTestsResultsLoader.Load(GetTestsResultsTwoPassing());

            results.PassedTests.Should().HaveCount(2);
            results.FailedTests.Should().HaveCount(0);
            results.PassedTests[0].Should().Be(new CasperJsTest
            {
                File = "testCasperJS.js",
                Name = "url is the one expected",
                Time = 0.459
            });
            results.PassedTests[1].Should().Be(new CasperJsTest
            {
                File = "testCasperJS.js",
                Name = "error.js script was loaded",
                Time = 0.11
            });
        }

        [TestMethod]
        public void TestSuiteWithOneFailedTest_CreateOneFailedTestWithPopertiesFilled()
        {
            CasperJsTestsResults results = CasperJsTestsResultsLoader.Load(GetTestsResultsOneFailing());

            results.PassedTests.Should().HaveCount(0);
            results.FailedTests.Should().HaveCount(1);
            results.FailedTests[0].Should().Be(new CasperJsTest
            {
                File = "testCasperJS.js",
                Name = "url is the one expected",
                Time = 0.459
            });
        }

        [TestMethod]
        public void TestSuiteWithOneFailedTestAndOnePassingTest_CreateOneFailedTestAndOnePassedTestWithPopertiesFilled()
        {
            CasperJsTestsResults results = CasperJsTestsResultsLoader.Load(GetTestsResultsOnePassingOneFailing());

            results.PassedTests.Should().HaveCount(1);
            results.PassedTests[0].Should().Be(new CasperJsTest
            {
                File = "testCasperJS.js",
                Name = "url is the one expected",
                Time = 0.459
            });
            results.FailedTests.Should().HaveCount(1);
            results.FailedTests[0].Should().Be(new CasperJsTest
            {
                File = "testCasperJS.js",
                Name = "url is the one expected",
                Time = 0.459
            });
        }

        private string GetEmptyTestsResults()
        {
            return "<testsuite></testsuite>";
        }

        private string GetTestsResultsOnePassing()
        {
            return "<testsuite><testcase classname=\"testCasperJS.js\" name=\"url is the one expected\" time=\"0.459\"></testcase></testsuite>";
        }

        private string GetTestsResultsOneFailing()
        {
            return "<testsuite><testcase classname=\"testCasperJS.js\" name=\"url is the one expected\" time=\"0.459\"><failure type=\"assertResourceExists\">Expected resource has been found</failure></testcase></testsuite>";
        }

        private string GetTestsResultsTwoPassing()
        {
            return "<testsuite><testcase classname=\"testCasperJS.js\" name=\"url is the one expected\" time=\"0.459\"></testcase><testcase classname=\"testCasperJS.js\" name=\"error.js script was loaded\" time=\"0.11\"></testcase></testsuite>";
        }

        private string GetTestsResultsOnePassingOneFailing()
        {
            return "<testsuite><testcase classname=\"testCasperJS.js\" name=\"url is the one expected\" time=\"0.459\"></testcase><testcase classname=\"testCasperJS.js\" name=\"url is the one expected\" time=\"0.459\"><failure type=\"assertResourceExists\">Expected resource has been found</failure></testcase></testsuite>";
        }
    }
}
