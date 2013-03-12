using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

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
        [Editor("Microsoft.TeamFoundation.Build.Controls.ServerFolderBrowserEditor," +
          "Microsoft.TeamFoundation.Build.Controls", typeof(UITypeEditor))]
        public InArgument<string> SourcesDirectory { get; set; }

        /// <summary>
        /// Parameters to user when calling CasperJS
        /// </summary>
        public InArgument<string> CasperJSParameters { get; set; }

        [RequiredArgument]
        public InArgument<Workspace> Workspace { get; set; }

        /// <summary>
        /// Executes the activity
        /// </summary>
        /// <param name="context">The activity's context</param>
        protected override void Execute(CodeActivityContext context)
        {
            // context.TrackBuildError("This is an error.");
            WorkingFolder workingFolder = Workspace.Get(context).GetWorkingFolderForServerItem(SourcesDirectory.Get(context));

            context.TrackBuildWarning(string.Format("The sources directory is {0}.", workingFolder.LocalItem));
        }
    }
}
