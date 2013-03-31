using System;

namespace CustomBuildActivities
{
    public class CasperJsTest
    {
        public CasperJsTest()
        {
            Id = Guid.NewGuid();
            ExecutionId = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid ExecutionId { get; set; }

        public string File { get; set; }

        public string Name { get; set; }

        public double Time { get; set; }

        public override bool Equals(object obj)
        {
            var theOtherObject = obj as CasperJsTest;
            if (theOtherObject == null)
                return false;

            return (File == theOtherObject.File 
                        && Name == theOtherObject.Name 
                        && Time == theOtherObject.Time);
        }
    }
}
