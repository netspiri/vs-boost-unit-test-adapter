// (C) Copyright 2015 ETAS GmbH (http://www.etas.com/)
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)

// This file has been modified by Microsoft on 8/2017.

using BoostTestAdapter.Boost.Runner;
using BoostTestAdapter.Settings;
using BoostTestAdapter.Utility;
using BoostTestAdapter.Utility.VisualStudio;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;

namespace BoostTestAdapter.Discoverers
{
    /// <summary>
    /// A Boost Test Discoverer which discovers tests based on configuration.
    /// </summary>
    internal class ExternalDiscoverer : IBoostTestDiscoverer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings for this instance of the discoverer.</param>
        public ExternalDiscoverer(ExternalBoostTestRunnerSettings settings)
            : this(settings, new DefaultBoostTestPackageServiceFactory())
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings for this instance of the discoverer.</param>
        /// <param name="packageServiceFactory">Boost Test Package Service factory</param>
        public ExternalDiscoverer(ExternalBoostTestRunnerSettings settings, IBoostTestPackageServiceFactory packageServiceFactory)
        {
            Settings = settings;
            PackageServiceFactory = packageServiceFactory;
        }

        /// <summary>
        /// Settings for this instance of the discoverer.
        /// </summary>
        public ExternalBoostTestRunnerSettings Settings { get; private set; }

        /// <summary>
        /// Boost Test Package Service factory
        /// </summary>
        public IBoostTestPackageServiceFactory PackageServiceFactory { get; private set; }
        
        #region IBoostTestDiscoverer

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, ITestCaseDiscoverySink discoverySink)
        {
            Code.Require(sources, "sources");
            Code.Require(discoverySink, "discoverySink");

            if (this.Settings.DiscoveryMethodType == DiscoveryMethodType.DiscoveryListContent)
            {
                // Delegate to ListContentDiscoverer
                ListContentDiscoverer discoverer = new ListContentDiscoverer(new ExternalBoostTestRunnerFactory(), PackageServiceFactory);
                discoverer.DiscoverTests(sources, discoveryContext, discoverySink);
            }
        }

        #endregion IBoostTestDiscoverer
        
        /// <summary>
        /// Internal IBoostTestRunnerFactory implementation which
        /// exclusively produces ExternalBoostTestRunner instances.
        /// </summary>
        private class ExternalBoostTestRunnerFactory : IBoostTestRunnerFactory
        {
            #region IBoostTestRunnerFactory

            public IBoostTestRunner GetRunner(string source, BoostTestRunnerFactoryOptions options)
            {
                Code.Require(source, "source");
                Code.Require(options, "options");
                Code.Require(options.ExternalTestRunnerSettings, "options.ExternalTestRunnerSettings");
                
                return new ExternalBoostTestRunner(source, options.ExternalTestRunnerSettings);
            }

            #endregion IBoostTestRunnerFactory
        }
    }
}
