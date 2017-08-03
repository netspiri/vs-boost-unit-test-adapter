// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using BoostTestShared;
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
        void IBoostTestPackageService.Placeholder()
        {
        }
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
