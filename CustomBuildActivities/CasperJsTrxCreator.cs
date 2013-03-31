namespace CustomBuildActivities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    
    public static class CasperJsTrxCreator
    {
        public static string Create(CasperJsTestsResults casperJsTestsResults)
        {
            XNamespace testRunNamespace =
                XNamespace.Get("http://microsoft.com/schemas/VisualStudio/TeamTest/2010");

            var xml = new XElement("TestRun",
                new XAttribute("id", Guid.NewGuid().ToString()),
                new XElement("ResultSummary",
                    new XAttribute("outcome", "Completed"),
                    new XElement("Counters",
                        new XAttribute("total", (casperJsTestsResults.PassedTests.Count + casperJsTestsResults.FailedTests.Count).ToString()),
                        new XAttribute("passed", casperJsTestsResults.PassedTests.Count.ToString()),
                        new XAttribute("failed", casperJsTestsResults.FailedTests.Count.ToString()))),
                new XElement("TestDefinitions",
                    GetUnitTestsDefinitions(casperJsTestsResults)),
                new XElement("TestEntries",
                    GetUnitTestsEntries(casperJsTestsResults)),
                new XElement("Results",
                    GetUnitTestsResults(casperJsTestsResults))
                );

            var xDocument = new XDocument(xml);

            xDocument.Root.Name = testRunNamespace + xDocument.Root.Name.LocalName;

            using (var textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter))
                {
                    xml.WriteTo(xmlWriter);
                }
                return textWriter.ToString();
            }
        }

        private static XElement[] GetUnitTestsResults(CasperJsTestsResults casperJsTestsResults)
        {
            var unitTestsResults = new List<XElement>();
            var testTypeGuid = Guid.NewGuid();
            var testListGuid = Guid.NewGuid();

            foreach (var passedTest in casperJsTestsResults.PassedTests)
            {
                var xelement = new XElement("UnitTestResult",
                                            new XAttribute("testId", passedTest.Id),
                                            new XAttribute("testName", passedTest.Name),
                                            new XAttribute("executionId", passedTest.ExecutionId),
                                            new XAttribute("duration", GetTimeSpanFromTestTime(passedTest.Time)),
                                            new XAttribute("outcome", "Passed"),
                                            new XAttribute("testType", testTypeGuid),
                                            new XAttribute("testListId", testListGuid));
                unitTestsResults.Add(xelement);
            }

            foreach (var failedTest in casperJsTestsResults.FailedTests)
            {
                var xelement = new XElement("UnitTestResult",
                                            new XAttribute("testId", failedTest.Id),
                                            new XAttribute("testName", failedTest.Name),
                                            new XAttribute("executionId", failedTest.ExecutionId),
                                            new XAttribute("duration", GetTimeSpanFromTestTime(failedTest.Time)),
                                            new XAttribute("outcome", "Failed"),
                                            new XAttribute("testType", testTypeGuid),
                                            new XAttribute("testListId", testListGuid));
                unitTestsResults.Add(xelement);
            }

            return unitTestsResults.ToArray();
        }

        private static string GetTimeSpanFromTestTime(double time)
        {
            string[] timeSplitted = time.ToString().Split(new char[] { '.' });
            int seconds = (timeSplitted != null && timeSplitted.Any()) ? int.Parse(timeSplitted[0]) : 0;
            int miliseconds = (timeSplitted != null && timeSplitted.Count() == 2) ? int.Parse(timeSplitted[1]) : 0;

            var timeSpan = new TimeSpan(0, 0, 0, seconds, miliseconds);

            return timeSpan.ToString();
        }

        private static XElement[] GetUnitTestsDefinitions(CasperJsTestsResults casperJsTestsResults)
        {
            var unitTestDefinitions = new List<XElement>();

            foreach (var passedTest in casperJsTestsResults.PassedTests)
            {
                var xelement = new XElement("UnitTest",
                                            new XAttribute("name", passedTest.Name),
                                            new XAttribute("id", passedTest.Id));
                unitTestDefinitions.Add(xelement);
            }

            foreach (var failedTest in casperJsTestsResults.FailedTests)
            {
                var xelement = new XElement("UnitTest",
                                            new XAttribute("name", failedTest.Name),
                                            new XAttribute("id", failedTest.Id));
                unitTestDefinitions.Add(xelement);
            }

            return unitTestDefinitions.ToArray();
        }

        private static XElement[] GetUnitTestsEntries(CasperJsTestsResults casperJsTestsResults)
        {
            var unitTestEntries = new List<XElement>();

            foreach (var passedTest in casperJsTestsResults.PassedTests)
            {
                var xelement = new XElement("TestEntry",
                                            new XAttribute("testId", passedTest.Id),
                                            new XAttribute("executionId", passedTest.ExecutionId));
                unitTestEntries.Add(xelement);
            }

            foreach (var failedTest in casperJsTestsResults.FailedTests)
            {
                var xelement = new XElement("TestEntry",
                                            new XAttribute("testId", failedTest.Id),
                                            new XAttribute("executionId", failedTest.ExecutionId));
                unitTestEntries.Add(xelement);
            }

            return unitTestEntries.ToArray();
        }
    }
}
