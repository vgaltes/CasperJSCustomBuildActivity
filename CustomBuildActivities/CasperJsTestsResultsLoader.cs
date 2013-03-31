namespace CustomBuildActivities
{
    using System.Collections.Generic;
    using System.Xml;

    public static class CasperJsTestsResultsLoader
    {
        public static CasperJsTestsResults Load(string casperJsResults)
        {
            var results = new CasperJsTestsResults();

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(casperJsResults);

            foreach (XmlNode node in xmlDocument.ChildNodes[0])
            {
                var test = new CasperJsTest
                {
                    File = node.Attributes["classname"].Value,
                    Name = node.Attributes["name"].Value,
                    Time = double.Parse(node.Attributes["time"].Value)
                };

                if (node.ChildNodes.Count == 1)
                {
                    if (node.FirstChild.Name == "failure")
                        results.FailedTests.Add(test);
                }
                else
                {
                    results.PassedTests.Add(test);
                }
            }

            return results;
        }
    }
}
