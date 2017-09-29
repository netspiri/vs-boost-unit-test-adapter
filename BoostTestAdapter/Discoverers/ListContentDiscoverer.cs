﻿// (C) Copyright 2015 ETAS GmbH (http://www.etas.com/)
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)

// This file has been modified by Microsoft on 8/2017.

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BoostTestAdapter.Boost.Runner;
using BoostTestAdapter.Boost.Test;
using BoostTestAdapter.Settings;
using BoostTestAdapter.Utility;
using BoostTestAdapter.Utility.ExecutionContext;
using BoostTestAdapter.Utility.VisualStudio;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace BoostTestAdapter.Discoverers
{
    /// <summary>
    /// A Boost Test Discoverer that uses the output of the source executable called with --list_content=DOT parameter 
    /// to get the list of the tests.
    /// </summary>
    internal class ListContentDiscoverer : IBoostTestDiscoverer
    {
        #region Constructors

        /// <summary>
        /// Default constructor. Default implementations of IBoostTestRunnerFactory and IBoostTestPackageServiceFactory are provided.
        /// </summary>
        public ListContentDiscoverer()
            : this(new DefaultBoostTestRunnerFactory(), new DefaultBoostTestPackageServiceFactory())
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="runnerFactory">A custom implementation of IBoostTestRunnerFactory.</param>
        /// <param name="packageServiceFactory">A custom implementation of IBoostTestPackageServiceFactory</param>
        public ListContentDiscoverer(IBoostTestRunnerFactory runnerFactory, IBoostTestPackageServiceFactory packageServiceFactory)
        {
            _runnerFactory = runnerFactory;
            _packageServiceFactory = packageServiceFactory;
        }

        #endregion

        #region Constants

        private const int EXIT_SUCCESS = 0;

        #endregion

        #region Members

        private readonly IBoostTestRunnerFactory _runnerFactory;
        private readonly IBoostTestPackageServiceFactory _packageServiceFactory;

        #endregion

        #region IBoostTestDiscoverer

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, ITestCaseDiscoverySink discoverySink)
        {
            Code.Require(sources, "sources");
            Code.Require(discoverySink, "discoverySink");

            // Populate loop-invariant attributes and settings

            BoostTestAdapterSettings settings = BoostTestAdapterSettingsProvider.GetSettings(discoveryContext);
            
            BoostTestRunnerSettings runnerSettings = new BoostTestRunnerSettings()
            {
                Timeout = settings.DiscoveryTimeoutMilliseconds
            };

            BoostTestRunnerCommandLineArgs args = new BoostTestRunnerCommandLineArgs()
            {
                ListContent = ListContentFormat.DOT
            };

            foreach (var source in sources)
            {
                try
                {
                    using (var packageService = _packageServiceFactory.Create())
                    {
                        args.SetWorkingEnvironment(source, settings, packageService);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "Could not retrieve WorkingDirectory from Visual Studio Configuration");
                }

                try
                {
                    IBoostTestRunner runner = _runnerFactory.GetRunner(source, settings.TestRunnerFactoryOptions);
                    using (TemporaryFile output = new TemporaryFile(TestPathGenerator.Generate(source, ".list.content.gv")))
                    {
                        // --list_content output is redirected to standard error
                        args.StandardErrorFile = output.Path;
                        Logger.Debug("list_content file: {0}", args.StandardErrorFile);

                        int resultCode = EXIT_SUCCESS;

                        using (var context = new DefaultProcessExecutionContext())
                        {
                            resultCode = runner.Execute(args, runnerSettings, context);
                        }

                        // Skip sources for which the --list_content file is not available
                        if (!File.Exists(args.StandardErrorFile))
                        {
                            Logger.Error("--list_content=DOT output for {0} is not available. Skipping.", source);
                            continue;
                        }

                        // If the executable failed to exit with an EXIT_SUCCESS code, skip source and notify user accordingly
                        if (resultCode != EXIT_SUCCESS)
                        {
                            Logger.Error("--list_content=DOT for {0} failed with exit code {1}. Skipping.", source, resultCode);
                            continue;
                        }

                        // Parse --list_content=DOT output
                        using (var stream = File.OpenRead(args.StandardErrorFile))
                        using (var reader = new StreamReader(stream, System.Text.Encoding.Default))
                        {
                            TestFrameworkDOTDeserialiser deserialiser = new TestFrameworkDOTDeserialiser(source);
                            TestFramework framework = deserialiser.Deserialise(reader);
                            if ((framework != null) && (framework.MasterTestSuite != null))
                            {
                                framework.MasterTestSuite.Apply(new VSDiscoveryVisitor(source, GetVersion(runner), discoverySink));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "Exception caught while discovering tests for {0} ({1} - {2})", source, ex.Message, ex.HResult);
                }
            }
        }

        /// <summary>
        /// Regular expression pattern for extracting Boost version from Boost.Test --version output
        /// </summary>
        private static readonly Regex _versionPattern = new Regex(@"Compiled from Boost version (\d+\.\d+.\d+)");

        /// <summary>
        /// Identify the version (if possible) of the Boost.Test module
        /// </summary>
        /// <param name="runner">The Boost.Test module</param>
        /// <returns>The Boost version of the Boost.Test module or the empty string if the version cannot be retrieved</returns>
        private static string GetVersion(IBoostTestRunner runner)
        {
            if (!runner.VersionSupported)
            {
                return string.Empty;
            }

            using (TemporaryFile output = new TemporaryFile(TestPathGenerator.Generate(runner.Source, ".version.stderr.log")))
            {
                BoostTestRunnerSettings settings = new BoostTestRunnerSettings();
                BoostTestRunnerCommandLineArgs args = new BoostTestRunnerCommandLineArgs()
                {
                    Version = true,
                    StandardErrorFile = output.Path
                };

                int resultCode = EXIT_SUCCESS;

                using (var context = new DefaultProcessExecutionContext())
                {
                    resultCode = runner.Execute(args, settings, context);
                }

                if (resultCode != EXIT_SUCCESS)
                {
                    Logger.Error("--version for {0} failed with exit code {1}. Skipping.", runner.Source, resultCode);
                    return string.Empty;
                }

                var info = File.ReadAllText(args.StandardErrorFile, System.Text.Encoding.Default);

                var match = _versionPattern.Match(info);
                return (match.Success) ? match.Groups[1].Value : string.Empty;
            }
        }

        #endregion IBoostTestDiscoverer
    }
}
