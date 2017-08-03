// (C) Copyright 2015 ETAS GmbH (http://www.etas.com/)
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)

// This file has been modified by Microsoft on 8/2017.

using BoostTestAdapter.Boost.Runner;
using BoostTestAdapter.Settings;
using BoostTestShared;
using System;
using System.IO;

namespace BoostTestAdapter.Utility
{
    public static class BoostTestRunnerCommandLineArgsEx
    {
        /// <summary>
        /// Allows specification of an environment via a line separated string
        /// </summary>
        /// <param name="args">The arguments to populate</param>
        /// <param name="environment">The line separated environment string</param>
        public static void SetEnvironment(this BoostTestRunnerCommandLineArgs args, string environment)
        {
            Code.Require(args, "args");

            if (!string.IsNullOrEmpty(environment))
            {
                foreach (string entry in environment.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] keyValuePair = entry.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if ((keyValuePair != null) && (keyValuePair.Length == 2))
                    {
                        args.Environment[keyValuePair[0]] = keyValuePair[1];
                    }
                }
            }
        }

        /// <summary>
        /// Sets the working environment (i.e. WorkingDirectory and Environment) properties of the command line arguments
        /// based on the provided details
        /// </summary>
        /// <param name="args">The arguments which to set</param>
        /// <param name="source">The base source which will be executed</param>
        /// <param name="settings">The BoostTestAdapterSettings which are currently applied</param>
        /// <param name="packageService">The current BoostTestPackageService (if available)</param>
        public static void SetWorkingEnvironment(this BoostTestRunnerCommandLineArgs args, string source, BoostTestAdapterSettings settings, IBoostTestPackageServiceWrapper packageService)
        {
            Code.Require(args, "args");
            Code.Require(source, "source");
            Code.Require(settings, "settings");

            // Default working directory
            args.WorkingDirectory = Path.GetDirectoryName(source);

            // Working directory extracted from test settings
            if (!string.IsNullOrEmpty(settings.WorkingDirectory) && Directory.Exists(settings.WorkingDirectory))
            {
                args.WorkingDirectory = settings.WorkingDirectory;
            }

            if (packageService != null)
            {
                // Visual Studio configuration has higher priority over settings (if available)
                args.WorkingDirectory = packageService.Service.GetWorkingDirectory(source) ?? args.WorkingDirectory;
                args.SetEnvironment(packageService.Service.GetEnvironment(source));
            }
            // Enforce windows style backward slashes
            args.WorkingDirectory = args.WorkingDirectory.Replace('/', '\\');
        }
    }
}
