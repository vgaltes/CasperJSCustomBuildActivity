using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace CustomBuildActivities.Tests
{
    [TestClass]
    public class CasperJsTrxCreatorTests
    {
        [TestMethod]
        public void ShouldCreateAnXmlWithAUniqueId()
        {
            string trx = CasperJsTrxCreator.Create(new CasperJsTestsResults());
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            xmlDocument.ChildNodes[1].Name.Should().Be("TestRun");
            xmlDocument.ChildNodes[1].Attributes.Should().HaveCount(2);
            Guid resultGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].Attributes["id"].Value, out resultGuid).Should().BeTrue();
        }

        [TestMethod]
        public void ShouldCreateAnXmlWithXmlns()
        {
            string trx = CasperJsTrxCreator.Create(new CasperJsTestsResults());
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            xmlDocument.ChildNodes[1].Attributes["xmlns"].Value.Should()
                                                         .Be("http://microsoft.com/schemas/VisualStudio/TeamTest/2010");
        }

        [TestMethod]
        public void ShouldCreateAResultsSummaryNodeWithOutcomeCompleted()
        {
            string trx = CasperJsTrxCreator.Create(new CasperJsTestsResults());
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            xmlDocument.ChildNodes[1].ChildNodes[0].Name.Should().Be("ResultSummary");
            xmlDocument.ChildNodes[1].ChildNodes[0].Attributes[0].Name.Should().Be("outcome");
            xmlDocument.ChildNodes[1].ChildNodes[0].Attributes[0].Value.Should().Be("Completed");
        }

        [TestMethod]
        public void ShouldCreateACountersNodeInsideResultsWithAllValuesTo0()
        {
            string trx = CasperJsTrxCreator.Create(new CasperJsTestsResults());
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Name.Should().Be("Counters");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[0].Name.Should().Be("total");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[0].Value.Should().Be("0");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[1].Name.Should().Be("passed");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[1].Value.Should().Be("0");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[2].Name.Should().Be("failed");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[2].Value.Should().Be("0");
        }

        [TestMethod]
        public void ShouldCreateACountersNodeInsideResultsWithTotalAndPassedTo1IfThereIsOnePassedTest()
        {
            var results = new CasperJsTestsResults();
            results.PassedTests.Add(new CasperJsTest { Name = "test name" });
            string trx = CasperJsTrxCreator.Create(results);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[0].Name.Should().Be("total");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[0].Value.Should().Be("1");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[1].Name.Should().Be("passed");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[1].Value.Should().Be("1");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[2].Name.Should().Be("failed");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[2].Value.Should().Be("0");
        }

        [TestMethod]
        public void ShouldCreateACountersNodeInsideResultsWithTotalAndFailedTo1IfThereIsOneFailedTest()
        {
            var results = new CasperJsTestsResults();
            results.FailedTests.Add(new CasperJsTest{Name = "test name"});
            string trx = CasperJsTrxCreator.Create(results);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[0].Name.Should().Be("total");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[0].Value.Should().Be("1");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[1].Name.Should().Be("passed");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[1].Value.Should().Be("0");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[2].Name.Should().Be("failed");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[2].Value.Should().Be("1");
        }

        [TestMethod]
        public void ShouldCreateACountersNodeInsideResultsWithTotalTo2AndFailedAndPassedTo1IfThereIsOneFailedAndOnePassedTest()
        {
            var results = new CasperJsTestsResults();
            results.FailedTests.Add(new CasperJsTest { Name = "test name" });
            results.PassedTests.Add(new CasperJsTest { Name = "test name" });
            string trx = CasperJsTrxCreator.Create(results);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[0].Name.Should().Be("total");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[0].Value.Should().Be("2");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[1].Name.Should().Be("passed");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[1].Value.Should().Be("1");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[2].Name.Should().Be("failed");
            xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes[2].Value.Should().Be("1");
        }

        [TestMethod]
        public void ShouldCreateAnEmptyTestDefinitionsNode()
        {
            string trx = CasperJsTrxCreator.Create(new CasperJsTestsResults());
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            xmlDocument.ChildNodes[1].ChildNodes[1].Name.Should().Be("TestDefinitions");
        }

        [TestMethod]
        public void ShouldCreateATestDefinitionsNodeIfThereIsOnePassedTest()
        {
            var results = new CasperJsTestsResults();
            results.PassedTests.Add(new CasperJsTest{Name = "test name"});
            string trx = CasperJsTrxCreator.Create(results);
            
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Name.Should().Be("UnitTest");
            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["name"].Value.Should().Be("test name");
            Guid idGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["id"].Value, out idGuid)
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public void ShouldCreateTwoTestDefinitionsNodeIfThereIsTwoPassedTest()
        {
            var results = new CasperJsTestsResults();
            results.PassedTests.Add(new CasperJsTest { Name = "test name 1" });
            results.PassedTests.Add(new CasperJsTest { Name = "test name 2" });
            string trx = CasperJsTrxCreator.Create(results);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Name.Should().Be("UnitTest");
            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["name"].Value.Should().Be("test name 1");
            Guid idGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["id"].Value, out idGuid)
                .Should()
                .BeTrue();

            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[1].Name.Should().Be("UnitTest");
            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes["name"].Value.Should().Be("test name 2");
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes["id"].Value, out idGuid)
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public void ShouldCreateATestDefinitionsNodeIfThereIsOneFailedTest()
        {
            var results = new CasperJsTestsResults();
            results.FailedTests.Add(new CasperJsTest { Name = "test name" });
            string trx = CasperJsTrxCreator.Create(results);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Name.Should().Be("UnitTest");
            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["name"].Value.Should().Be("test name");
            Guid idGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["id"].Value, out idGuid)
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public void ShouldCreateTwoTestDefinitionsNodeIfThereIsTwoFailedTest()
        {
            var results = new CasperJsTestsResults();
            results.FailedTests.Add(new CasperJsTest { Name = "test name 1" });
            results.FailedTests.Add(new CasperJsTest { Name = "test name 2" });
            string trx = CasperJsTrxCreator.Create(results);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Name.Should().Be("UnitTest");
            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["name"].Value.Should().Be("test name 1");
            Guid idGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["id"].Value, out idGuid)
                .Should()
                .BeTrue();

            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[1].Name.Should().Be("UnitTest");
            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes["name"].Value.Should().Be("test name 2");
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes["id"].Value, out idGuid)
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public void ShouldCreateTwoTestDefinitionsNodeIfThereIsOneFailedTestAndOnePassedTest()
        {
            var results = new CasperJsTestsResults();
            results.PassedTests.Add(new CasperJsTest { Name = "test name passed" });
            results.FailedTests.Add(new CasperJsTest { Name = "test name failed" });
            string trx = CasperJsTrxCreator.Create(results);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Name.Should().Be("UnitTest");
            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["name"].Value.Should().Be("test name passed");
            Guid idGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["id"].Value, out idGuid)
                .Should()
                .BeTrue();

            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[1].Name.Should().Be("UnitTest");
            xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes["name"].Value.Should().Be("test name failed");
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes["id"].Value, out idGuid)
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public void ShouldCreateAnEmptyTestEntriesNode()
        {
            string trx = CasperJsTrxCreator.Create(new CasperJsTestsResults());
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            xmlDocument.ChildNodes[1].ChildNodes[2].Name.Should().Be("TestEntries");
        }

        [TestMethod]
        public void ShouldCreateATestEntryNodeIfThereIsOnePassedTest()
        {
            var results = new CasperJsTestsResults();
            results.PassedTests.Add(new CasperJsTest { Name = "test name" });
            string trx = CasperJsTrxCreator.Create(results);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            Guid testDefinitionGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["id"].Value, out testDefinitionGuid);

            Guid testEntryGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[0].Attributes["testId"].Value, out testEntryGuid)
                .Should()
                .BeTrue();

            testDefinitionGuid.Should().Be(testEntryGuid);

            Guid testExecutionIdGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[0].Attributes["executionId"].Value, out testExecutionIdGuid)
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public void ShouldCreateTwoTestEntryNodeIfThereIsTwoPassedTests()
        {
            var results = new CasperJsTestsResults();
            results.PassedTests.Add(new CasperJsTest { Name = "test name 1" });
            results.PassedTests.Add(new CasperJsTest { Name = "test name 2" });
            string trx = CasperJsTrxCreator.Create(results);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            Guid testDefinitionGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["id"].Value, out testDefinitionGuid);

            Guid testEntryGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[0].Attributes["testId"].Value, out testEntryGuid)
                .Should()
                .BeTrue();

            testDefinitionGuid.Should().Be(testEntryGuid);

            Guid testExecutionIdGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[1].Attributes["executionId"].Value, out testExecutionIdGuid)
                .Should()
                .BeTrue();

            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes["id"].Value, out testDefinitionGuid);

            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[1].Attributes["testId"].Value, out testEntryGuid)
                .Should()
                .BeTrue();

            testDefinitionGuid.Should().Be(testEntryGuid);

            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[1].Attributes["executionId"].Value, out testExecutionIdGuid)
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public void ShouldCreateATestEntryNodeIfThereIsOneFailedTest()
        {
            var results = new CasperJsTestsResults();
            results.FailedTests.Add(new CasperJsTest { Name = "test name" });
            string trx = CasperJsTrxCreator.Create(results);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            Guid testDefinitionGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["id"].Value, out testDefinitionGuid);

            Guid testEntryGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[0].Attributes["testId"].Value, out testEntryGuid)
                .Should()
                .BeTrue();

            testDefinitionGuid.Should().Be(testEntryGuid);

            Guid testExecutionIdGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[0].Attributes["executionId"].Value, out testExecutionIdGuid)
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public void ShouldCreateTwoTestEntryNodeIfThereIsTwoFailedTests()
        {
            var results = new CasperJsTestsResults();
            results.FailedTests.Add(new CasperJsTest { Name = "test name 1" });
            results.FailedTests.Add(new CasperJsTest { Name = "test name 2" });
            string trx = CasperJsTrxCreator.Create(results);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            Guid testDefinitionGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["id"].Value, out testDefinitionGuid);

            Guid testEntryGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[0].Attributes["testId"].Value, out testEntryGuid)
                .Should()
                .BeTrue();

            testDefinitionGuid.Should().Be(testEntryGuid);

            Guid testExecutionIdGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[1].Attributes["executionId"].Value, out testExecutionIdGuid)
                .Should()
                .BeTrue();

            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes["id"].Value, out testDefinitionGuid);

            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[1].Attributes["testId"].Value, out testEntryGuid)
                .Should()
                .BeTrue();

            testDefinitionGuid.Should().Be(testEntryGuid);

            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[1].Attributes["executionId"].Value, out testExecutionIdGuid)
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public void ShouldCreateTwoTestEntryNodeIfThereIsOneFailedAndOnePassedTests()
        {
            var results = new CasperJsTestsResults();
            results.PassedTests.Add(new CasperJsTest { Name = "test name 1" });
            results.FailedTests.Add(new CasperJsTest { Name = "test name 2" });
            string trx = CasperJsTrxCreator.Create(results);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            Guid testDefinitionGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["id"].Value, out testDefinitionGuid);

            Guid testEntryGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[0].Attributes["testId"].Value, out testEntryGuid)
                .Should()
                .BeTrue();

            testDefinitionGuid.Should().Be(testEntryGuid);

            Guid testExecutionIdGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[1].Attributes["executionId"].Value, out testExecutionIdGuid)
                .Should()
                .BeTrue();

            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes["id"].Value, out testDefinitionGuid);

            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[1].Attributes["testId"].Value, out testEntryGuid)
                .Should()
                .BeTrue();

            testDefinitionGuid.Should().Be(testEntryGuid);

            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[2].ChildNodes[1].Attributes["executionId"].Value, out testExecutionIdGuid)
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public void ShouldCreateAnEmptyResultsNode()
        {
            string trx = CasperJsTrxCreator.Create(new CasperJsTestsResults());
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            xmlDocument.ChildNodes[1].ChildNodes[3].Name.Should().Be("Results");
        }

        [TestMethod]
        public void ShouldCreateAUnitTestResultNodeIfThereIsOnePassedTest()
        {
            var results = new CasperJsTestsResults();
            results.PassedTests.Add(new CasperJsTest { Name = "test name", Time = 0.459});
            string trx = CasperJsTrxCreator.Create(results);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            AssertTestResult(xmlDocument, results.PassedTests[0], "Passed", 0);
        }

        [TestMethod]
        public void ShouldCreateAUnitTestResultNodeIfThereIsOneFailedTest()
        {
            var results = new CasperJsTestsResults();
            results.FailedTests.Add(new CasperJsTest { Name = "test name", Time = 0.459 });
            string trx = CasperJsTrxCreator.Create(results);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            AssertTestResult(xmlDocument, results.FailedTests[0], "Failed", 0);
        }

        [TestMethod]
        public void ShouldCreateTwoUnitTestResultNodeIfThereIsTwoPassedTest()
        {
            var results = new CasperJsTestsResults();
            results.PassedTests.Add(new CasperJsTest { Name = "test name 1", Time = 0.459 });
            results.PassedTests.Add(new CasperJsTest { Name = "test name 2", Time = 0.459 });
            string trx = CasperJsTrxCreator.Create(results);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            AssertTestResult(xmlDocument, results.PassedTests[0], "Passed", 0);
            AssertTestResult(xmlDocument, results.PassedTests[1], "Passed", 1);
        }

        [TestMethod]
        public void ShouldCreateTwoUnitTestResultNodeIfThereIsTwoFailedTest()
        {
            var results = new CasperJsTestsResults();
            results.FailedTests.Add(new CasperJsTest { Name = "test name 1", Time = 0.459 });
            results.FailedTests.Add(new CasperJsTest { Name = "test name 2", Time = 0.459 });
            string trx = CasperJsTrxCreator.Create(results);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            AssertTestResult(xmlDocument, results.FailedTests[0], "Failed", 0);
            AssertTestResult(xmlDocument, results.FailedTests[1], "Failed", 1);
        }

        [TestMethod]
        public void ShouldCreateTwoUnitTestResultNodeIfThereIsOnePassedAndOneFailedTest()
        {
            var results = new CasperJsTestsResults();
            results.PassedTests.Add(new CasperJsTest { Name = "test name 1", Time = 0.459 });
            results.FailedTests.Add(new CasperJsTest { Name = "test name 2", Time = 0.459 });
            string trx = CasperJsTrxCreator.Create(results);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(trx);

            AssertTestResult(xmlDocument, results.PassedTests[0], "Passed", 0);
            AssertTestResult(xmlDocument, results.FailedTests[0], "Failed", 1);
        }

        private static void AssertTestResult(XmlDocument xmlDocument, CasperJsTest test, string outcomeText, int numTest)
        {
            Guid testEntryGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[3].ChildNodes[numTest].Attributes["testId"].Value, out testEntryGuid)
                .Should()
                .BeTrue();

            testEntryGuid.Should().Be(test.Id);

            Guid testExecutionIdGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[3].ChildNodes[numTest].Attributes["executionId"].Value,
                          out testExecutionIdGuid)
                .Should()
                .BeTrue();
            testExecutionIdGuid.Should().Be(test.ExecutionId);

            xmlDocument.ChildNodes[1].ChildNodes[3].ChildNodes[numTest].Attributes["testName"].Value.Should().Be(test.Name);
            xmlDocument.ChildNodes[1].ChildNodes[3].ChildNodes[numTest].Attributes["duration"].Value.Should()
                                                                                        .Be("00:00:00.4590000");
            xmlDocument.ChildNodes[1].ChildNodes[3].ChildNodes[numTest].Attributes["outcome"].Value.Should().Be(outcomeText);

            Guid testTypeIdGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[3].ChildNodes[numTest].Attributes["testType"].Value, out testTypeIdGuid)
                .Should()
                .BeTrue();

            Guid testListIdGuid = new Guid();
            Guid.TryParse(xmlDocument.ChildNodes[1].ChildNodes[3].ChildNodes[0].Attributes["testListId"].Value,
                          out testListIdGuid)
                .Should()
                .BeTrue();
        }
    }
}
