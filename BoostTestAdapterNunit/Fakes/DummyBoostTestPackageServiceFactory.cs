// (C) Copyright ETAS 2015.
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)

// This file has been modified by Microsoft on 4/2018.

using BoostTestAdapter.Utility.VisualStudio;
using BoostTestShared;

namespace BoostTestAdapterNunit.Utility
{
    /// <summary>
    /// A stub implementation for the IBoostTestPackageServiceFactory interface.
    /// Provides an IBoostTestPackageServiceWrapper instance which is defined at the construction time.
    /// </summary>
    public class DummyBoostTestPackageServiceFactory : IBoostTestPackageServiceFactory
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="packageService">The IBoostTestPackageServiceWrapper instance which is to be provided on the Create call</param>
        public DummyBoostTestPackageServiceFactory(IBoostTestPackageServiceWrapper packageService)
        {
            this.PackageService = packageService;
        }

        #region IBoostTestPackageServiceFactory

        private IBoostTestPackageServiceWrapper PackageService { get; set; }

        public IBoostTestPackageServiceWrapper Create(int processId)
        {
            return PackageService;
        }

        #endregion IBoostTestPackageServiceFactory

        private static DummyBoostTestPackageServiceFactory _default = new DummyBoostTestPackageServiceFactory(null);

        /// <summary>
        /// Default DummyBoostTestPackageServiceFactory which returns a null service instance
        /// </summary>
        public static DummyBoostTestPackageServiceFactory Default
        {
            get
            {
                return _default;
            }
        }
    }

}
