// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using BoostTestShared;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics;
using System.ServiceModel;

namespace BoostTestPackage
{
    /// <summary>
    /// Implements IBoostTestPackageService to expose Visual Studio interfaces for the out-of-process adapter.
    /// </summary>
    class BoostTestPackageService : IBoostTestPackageService
    {
        private VisualStudioAdapter.IVisualStudio _visualStudio;

        /// <summary>
        /// Default constructor
        /// </summary>
        BoostTestPackageService()
        {
            var dte = (DTE2)Package.GetGlobalService(typeof(EnvDTE.DTE));
            _visualStudio = new VisualStudio2015Adapter.VisualStudio(dte);
        }

        #region IBoostTestPackageService

        public string GetEnvironment(string binary)
        {
            foreach (var project in _visualStudio.Solution.Projects)
            {
                var configuration = project.ActiveConfiguration;

                if (string.Equals(binary, configuration.PrimaryOutput, StringComparison.Ordinal))
                {
                    return configuration.VSDebugConfiguration.Environment;
                }
            }
            return null;
        }

        public string GetWorkingDirectory(string binary)
        {
            foreach (var project in _visualStudio.Solution.Projects)
            {
                var configuration = project.ActiveConfiguration;

                if (string.Equals(binary, configuration.PrimaryOutput, StringComparison.Ordinal))
                {
                    return configuration.VSDebugConfiguration.WorkingDirectory;
                }
            }
            return null;
        }

        #endregion
    }

    /// <summary>
    /// Host class for BoostTestPackageService.
    /// </summary>
    class BoostTestPackageServiceHost : ServiceHost
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BoostTestPackageServiceHost() :
            base(typeof(BoostTestPackageService), new Uri[] {
                BoostTestPackageServiceConfiguration.ConstructPipeUri(Process.GetCurrentProcess().Id)
            })
        {
            AddServiceEndpoint(typeof(IBoostTestPackageService), new NetNamedPipeBinding(), BoostTestPackageServiceConfiguration.InterfaceAddress);
        }
    }
}
