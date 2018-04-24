// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace BoostTestAdapter.Settings
{
    public static class BoostTestSettingsConstants
    {
        public const string InternalSettingsName = "BoostTestInternalSettings";
    }

    [Export(typeof(IRunSettingsService))]
    [SettingsName(BoostTestSettingsConstants.InternalSettingsName)]
    public class RunSettingsService : IRunSettingsService
    {
        public string Name => BoostTestSettingsConstants.InternalSettingsName;

        public RunSettingsService()
        {
        }

        public IXPathNavigable AddRunSettings(IXPathNavigable runSettingDocument,
            IRunSettingsConfigurationInfo configurationInfo, ILogger logger)
        {
            XPathNavigator runSettingsNavigator = runSettingDocument.CreateNavigator();
            Debug.Assert(runSettingsNavigator != null, "userRunSettingsNavigator == null!");
            if (!runSettingsNavigator.MoveToChild(Constants.RunSettingsName, ""))
            {
                return runSettingsNavigator;
            }

            var settingsContainer = new RunSettingsContainer();
            runSettingsNavigator.AppendChild(settingsContainer.ToXml().CreateNavigator());

            runSettingsNavigator.MoveToRoot();
            return runSettingsNavigator;
        }
    }

    [XmlRoot(BoostTestSettingsConstants.InternalSettingsName)]
    public class RunSettingsContainer : TestRunSettings
    {
        public RunSettingsContainer()
            : base(BoostTestSettingsConstants.InternalSettingsName)
        {
            VSProcessId = Process.GetCurrentProcess().Id;
        }

        public int VSProcessId { get; set; }

        public override XmlElement ToXml()
        {
            var document = new XmlDocument();
            using (XmlWriter writer = document.CreateNavigator().AppendChild())
            {
                new XmlSerializer(typeof(RunSettingsContainer))
                    .Serialize(writer, this);
            }
            return document.DocumentElement;
        }
    }

    [Export(typeof(ISettingsProvider))]
    [SettingsName(BoostTestSettingsConstants.InternalSettingsName)]
    public class RunSettingsProvider : ISettingsProvider
    {
        public string Name => BoostTestSettingsConstants.InternalSettingsName;

        public int VSProcessId { get; set; }

        public void Load(XmlReader reader)
        {
            Utility.Code.Require(reader, "reader");

            var schemaSet = new XmlSchemaSet();
            var schemaStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BoostTestInternalSettings.xsd");
            schemaSet.Add(null, XmlReader.Create(schemaStream));

            var settings = new XmlReaderSettings
            {
                Schemas = schemaSet,
                ValidationType = ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
            };

            settings.ValidationEventHandler += (object o, ValidationEventArgs e) => throw e.Exception;

            using (var newReader = XmlReader.Create(reader, settings))
            {
                try
                {
                    if (newReader.Read() && newReader.Name.Equals(this.Name))
                    {
                        XmlSerializer deserializer = new XmlSerializer(typeof(RunSettingsContainer));
                        RunSettingsContainer settingsContainer = deserializer.Deserialize(newReader) as RunSettingsContainer;
                        this.VSProcessId = settingsContainer.VSProcessId;
                    }
                }
                catch (InvalidOperationException e) when (e.InnerException is XmlSchemaValidationException)
                {
                    throw new BoostTestAdapterSettingsProvider.InvalidBoostTestAdapterSettingsException(
                        String.Format(Resources.InvalidPropertyFile, BoostTestSettingsConstants.InternalSettingsName, e.InnerException.Message),
                        e.InnerException);
                }
            }
        }
    }
}
