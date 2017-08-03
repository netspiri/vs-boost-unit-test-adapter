// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.ServiceModel;

namespace BoostTestShared
{
    /// <summary>
    /// Helper class for configuring BoostTestPackageService
    /// </summary>
    public sealed class BoostTestPackageServiceConfiguration
    {
        /// <summary>
        /// Relative address of the endpoint interface.
        /// </summary>
        public static readonly Uri InterfaceAddress = new Uri(nameof(IBoostTestPackageService), UriKind.Relative);

        /// <summary>
        /// Constructs the Uri at which the service is made available.
        /// </summary>
        /// <param name="processId">Identifier of the process offering the service</param>
        /// <returns></returns>
        public static Uri ConstructPipeUri(int processId)
        {
            return new Uri(string.Format(CultureInfo.InvariantCulture,
                "net.pipe://localhost/BoostTestPackageService.d57607bb-6c87-4279-91bb-e06ca10e5c3d.{0}/",
                processId));
        }

        /// <summary>
        /// Creates proxy object for the service.
        /// </summary>
        /// <remarks>
        /// The object must be handled as a <ref>System.ServiceModel.IClientChannel</ref>.
        /// </remarks>
        /// <param name="processId">Identifier of the process offering the service</param>
        /// <returns></returns>
        public static IBoostTestPackageService CreateProxy(int processId)
        {
            var endpointUri = new Uri(ConstructPipeUri(processId), InterfaceAddress);
            var endpointAddress = new EndpointAddress(endpointUri);
            return ChannelFactory<IBoostTestPackageService>.CreateChannel(new NetNamedPipeBinding(), endpointAddress);
        }

        /// <summary>
        /// Private default constructor to prevent from creating instances of this class.
        /// </summary>
        private BoostTestPackageServiceConfiguration()
        {
        }
    }
}
