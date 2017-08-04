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
            _visualStudio = new VisualStudioAdapter.VisualStudio(dte);
        }

        #region IBoostTestPackageService

        public DebuggingProperties GetDebuggingProperties(string binary)
        {
            var debuggingProperties = _visualStudio.GetDebuggingProperties(binary);
            if (debuggingProperties != null)
            {
                return new DebuggingProperties
                {
                    Environment = debuggingProperties.Environment,
                    WorkingDirectory = debuggingProperties.WorkingDirectory
                };
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
