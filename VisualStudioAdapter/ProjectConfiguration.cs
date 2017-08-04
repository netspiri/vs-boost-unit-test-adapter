// (C) Copyright 2015 ETAS GmbH (http://www.etas.com/)
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)

// This file has been modified by Microsoft on 8/2017.

using System;
using System.Linq;

using Microsoft.VisualStudio.VCProjectEngine;

namespace VisualStudioAdapter
{
    /// <summary>
    /// Adapter class for a Visual Studio Project Configuration
    /// </summary>
    class ProjectConfiguration
    {
        private VSDebugConfiguration _configuration = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">The base Visual Studio project configuration which is to be adapted</param>
        public ProjectConfiguration(VSDebugConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            this._configuration = configuration;

            this.PrimaryOutput = this._configuration.VCConfiguration.PrimaryOutput;
        }

        public string PrimaryOutput { get; private set; }
       
        public VSDebugConfiguration VSDebugConfiguration
        {
            get
            {
                return _configuration;
            }
        }
    }
}