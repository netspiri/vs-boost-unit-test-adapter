// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using BoostTestShared;
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

        public IBoostTestPackageServiceWrapper Create()
        {
            // Assuming the adapter is running in a child process of the Visual Studio process.
            int processId = Process.GetCurrentProcess().Id;
            int parentProcessId = GetParentProcessId(processId);
            var proxy = BoostTestPackageServiceConfiguration.CreateProxy(parentProcessId);
            return new BoostTestPackageServiceProxyWrapper(proxy);
        }

        #endregion

        /// <summary>
        /// Gets the process id of the parent process.
        /// </summary>
        /// <param name="processId">The process id of the child process.</param>
        /// <returns></returns>
        private static int GetParentProcessId(int processId)
        {
            string processIdString = processId.ToString(CultureInfo.InvariantCulture);
            string query = "SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = " + processIdString;
            uint parentId;

            using (ManagementObjectSearcher search = new ManagementObjectSearcher("root\\CIMV2", query))
            {
                ManagementObjectCollection.ManagementObjectEnumerator results = search.Get().GetEnumerator();
                results.MoveNext();
                ManagementBaseObject queryObj = results.Current;
                parentId = (uint)queryObj["ParentProcessId"];
            }

            return (int)parentId;
        }
    }
}
