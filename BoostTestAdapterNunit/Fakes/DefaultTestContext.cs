﻿// (C) Copyright 2015 ETAS GmbH (http://www.etas.com/)
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)

// This file has been modified by Microsoft on 8/2017.

using BoostTestAdapterNunit.Utility;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace BoostTestAdapterNunit.Fakes
{
    /// <summary>
    /// Default implementation of IRunContext/IDiscoveryContext/IRunSettings for testing purposes.
    /// </summary>
    public class DefaultTestContext : IRunContext, IRunSettings
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DefaultTestContext() :
            this(false, string.Empty)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="debug">Flag which identifies whether or not this RunContext is a debug run context or not.</param>
        public DefaultTestContext(bool debug) :
            this(debug, string.Empty)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="debug">Flag which identifies whether or not this RunContext is a debug run context or not.</param>
        /// <param name="settings">An Xml string which identifies the runsettings in use.</param>
        public DefaultTestContext(bool debug, string settings)
        {
            this.IsBeingDebugged = debug;
            this.SettingsXml = settings;

            this.IsDataCollectionEnabled = false;

            this.SettingProviders = new Dictionary<string, SettingProviderContext>();
        }

        /// <summary>
        /// Map of SettingProvider name to a respective SettingProvider instance.
        /// </summary>
        private IDictionary<string, SettingProviderContext> SettingProviders { get; set; }

        #region IRunContext

        public ITestCaseFilterExpression GetTestCaseFilter(IEnumerable<string> supportedProperties, Func<string, TestProperty> propertyProvider)
        {
            throw new NotImplementedException();
        }

        public bool InIsolation { get; set; }

        public bool IsBeingDebugged { get; set; }

        public bool IsDataCollectionEnabled { get; set; }

        public bool KeepAlive { get; set; }

        public string SolutionDirectory { get; set; }

        public string TestRunDirectory { get; set; }

        #region IDiscoveryContext

        public IRunSettings RunSettings
        {
            get { return this; }
        }
        
        #endregion IDiscoveryContext
        #endregion IRunContext

        #region IRunSettings

        public ISettingsProvider GetSettings(string settingsName)
        {
            SettingProviderContext context = null;
            if (this.SettingProviders.TryGetValue(settingsName, out context))
            {
                // If no Xml fragment is found for a particular provider, return null
                return (context.IsSet) ? context.Provider : null;
            }

            return null;
        }

        public string SettingsXml
        {
            get;
            set;
        }

        #endregion IRunSettings

        /// <summary>
        /// An attempt to emulate the C# MEF export.
        /// </summary>
        /// <param name="name">The settings name</param>
        /// <param name="provider">The settings provider to register under the provided name</param>
        public void RegisterSettingProvider(string name, ISettingsProvider provider)
        {
            this.SettingProviders[name] = new SettingProviderContext(provider);
        }
        
        /// <summary>
        /// Loads the embedded resource path and populates the registered providers accordingly.
        /// </summary>
        /// <param name="path">The path to the embedded resource</param>
        public void LoadEmbeddedSettings(string path)
        {
            LoadSettings(TestHelper.ReadEmbeddedResource(path));
        }

        /// <summary>
        /// Loads the xml string and populates the registered providers accordingly.
        /// </summary>
        /// <param name="settingsXml">The xml string describing the settings configuration</param>
        public void LoadSettings(string settingsXml)
        {
            this.SettingsXml = settingsXml;

            var xmlSettings = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Document,
                IgnoreComments = true,
                IgnoreWhitespace = true,
                XmlResolver = null
            };

            // Populate SettingProviders
            using (StringReader reader = new StringReader(this.SettingsXml))
            {
                XPathDocument doc = new XPathDocument(reader);
                XPathNavigator nav = doc.CreateNavigator();

                foreach (XPathNavigator child in nav.Select("/RunSettings/*"))
                {
                    if (this.SettingProviders.ContainsKey(child.LocalName))
                    {
                        var xmlReader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(child.OuterXml)), xmlSettings);
                        this.SettingProviders[child.LocalName].Load(xmlReader);
                    }
                }
            }
        }

        /// <summary>
        /// An internal class used to aggregate a SettingsProvider
        /// and a flag which states the result of the loading attempt.
        /// </summary>
        private class SettingProviderContext
        {
            public SettingProviderContext(ISettingsProvider provider)
            {
                this.Provider = provider;
                this.IsSet = false;
            }

            public ISettingsProvider Provider { get; set; }
            public bool IsSet { get; set; }

            public void Load(XmlReader reader)
            {
                this.Provider.Load(reader);
                this.IsSet = true;
            }
        }
    }
}
