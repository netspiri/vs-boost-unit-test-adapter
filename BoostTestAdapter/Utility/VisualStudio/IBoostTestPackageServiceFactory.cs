// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using BoostTestShared;

namespace BoostTestAdapter.Utility.VisualStudio
{
    /// <summary>
    /// Abstract factory which provides IBoostTestPackageServiceWrapper instances.
    /// </summary>
    public interface IBoostTestPackageServiceFactory
    {
        /// <summary>
        /// Creates an IBoostTestPackageServiceWrapper instance.
        /// </summary>
        /// <returns></returns>
        IBoostTestPackageServiceWrapper Create(int processId);
    }
}
