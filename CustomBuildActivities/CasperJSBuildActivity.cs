using Microsoft.TeamFoundation.Build.Client;

namespace CustomBuildActivities
{
    using System;
    using System.Activities;
    using Microsoft.TeamFoundation.Build.Workflow.Activities;

    /// <summary>
    /// Executes CasperJs with all files in SourcesDirectory
    /// </summary>
    [BuildActivity(HostEnvironmentOption.Agent)]
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
            context.TrackBuildMessage("This is a message.");
            context.TrackBuildWarning("This is a warning.");
            context.TrackBuildError("This is an error.");
        }
    }
}
