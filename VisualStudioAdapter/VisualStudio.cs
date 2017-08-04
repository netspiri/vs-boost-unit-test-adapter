// (C) Copyright ETAS 2015.
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)

// This file has been modified by Microsoft on 8/2017.

using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using VSProject = EnvDTE.Project;
using VSProjectItem = EnvDTE.ProjectItem;

namespace VisualStudioAdapter
{
    /// <summary>
    /// Base class for DTE2-based Visual Studio instances.
    /// </summary>
    public class VisualStudio : IVisualStudio
    {
        /// <summary>
        /// Constant strings which distinguish Solution item kinds.
        /// </summary>
        private static class EnvDTEProjectKinds
        {
            /// <summary>
            /// Solution folder item kind label
            /// </summary>
            public const string VsProjectKindSolutionFolder = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";

            /// <summary>
            /// C++ project item kind label
            /// </summary>
            public const string VsProjectKindVCpp = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
        }

        private DTE2 _dte = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dte">The DTE2 instance for a running Visual Studio instance</param>
        /// <param name="version">The version identifying the Visual Studio instance</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dte"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dte"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dte")]
        public VisualStudio(DTE2 dte)
        {
            this._dte = dte;
        }

        #region IVisualStudio

        public DebuggingProperties GetDebuggingProperties(string binary)
        {
            foreach (VSProject folderOrProject in this._dte.Solution.Projects.OfType<VSProject>())
            {
                //Call 364853
                //Loop through the solution folders (if any) to get all the projects within a solution
                foreach (var dteproject in GetProjects(folderOrProject))
                {
                    if (dteproject.Kind == EnvDTEProjectKinds.VsProjectKindVCpp)
                    {
                        var project = new Project(dteproject);
                        var configuration = project.ActiveConfiguration;
                        if (string.Equals(binary, configuration.PrimaryOutput, StringComparison.Ordinal))
                        {
                            return new DebuggingProperties
                            {
                                Environment = configuration.VSDebugConfiguration.Environment,
                                WorkingDirectory = configuration.VSDebugConfiguration.WorkingDirectory
                            };
                        }
                    }
                }
            }
            return null;
        }

        #endregion IVisualStudio

        /// <summary>
        /// Recursively retrieves projects form the provided Visual Studio project
        /// </summary>
        /// <param name="folderOrProject">A reference to a Visual Studio Project or Solution Folder</param>
        /// <returns>An enumeration of all Visual Studio projects (only)</returns>
        private IEnumerable<VSProject> GetProjects(VSProject folderOrProject)
        {
            // it is a solution folder
            if (folderOrProject.Kind == EnvDTEProjectKinds.VsProjectKindSolutionFolder)
            {
                foreach (VSProjectItem item in folderOrProject.ProjectItems)
                {
                    // it is a project
                    if (item.SubProject != null)
                    {
                        foreach (var project in GetProjects(item.SubProject))
                        {
                            yield return project;
                        }
                    }
                }
            }
            else
            {
                yield return folderOrProject;
            }
        }
    }
}
