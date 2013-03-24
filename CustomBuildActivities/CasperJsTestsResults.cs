namespace CustomBuildActivities
{
    using System.Collections.Generic;

    public class CasperJsTestsResults
    {
        public IList<CasperJsTest> PassedTests
        {
            get;
            set;
        }

        public IList<CasperJsTest> FailedTests
        {
            get;
            set;
        }

        public CasperJsTestsResults()
        {
            PassedTests = new List<CasperJsTest>();
            FailedTests = new List<CasperJsTest>();
        }
    }
}
