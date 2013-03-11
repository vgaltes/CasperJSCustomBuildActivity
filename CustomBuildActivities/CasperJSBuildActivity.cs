namespace CustomBuildActivities
{
    using System;
    using System.Activities;

    /// <summary>
    /// Executes CasperJs with all files in SourcesDirectory
    /// </summary>
    public sealed class CasperJSBuildActivity : CodeActivity
    {
        /// <summary>
        /// Directory where there are the scripts we want to execute with CasperJS
        /// </summary>
        [RequiredArgument]
        public InArgument<string> SourcesDirectory { get; set; }

        /// <summary>
        /// Parameters to user when calling CasperJS
        /// </summary>
        public InArgument<string> CasperJSParameters { get; set; }

        /// <summary>
        /// Executes the activity
        /// </summary>
        /// <param name="context">The activity's context</param>
        protected override void Execute(CodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
