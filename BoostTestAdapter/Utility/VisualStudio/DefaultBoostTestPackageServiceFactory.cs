// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using BoostTestShared;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Management;

namespace BoostTestAdapter.Utility.VisualStudio
{
    /// <summary>
    /// Default implementation of an IBoostTestPackageServiceFactory. Provides IBoostTestPackageServiceWrapper instance
    /// for a proxy of the service running in the parent Visual Studio process.
    /// </summary>
    class DefaultBoostTestPackageServiceFactory : IBoostTestPackageServiceFactory
    {
        #region IBoostTestPackageServiceFactory

        public IBoostTestPackageServiceWrapper Create(int processId)
        {
            var proxy = BoostTestPackageServiceConfiguration.CreateProxy(processId);
            return new BoostTestPackageServiceProxyWrapper(proxy);
        }

        #endregion
    }
}
