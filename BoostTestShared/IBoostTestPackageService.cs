﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ServiceModel;

namespace BoostTestShared
{
    /// <summary>
    /// Interface of BoostTestPackageService.
    /// </summary>
    [ServiceContract]
    public interface IBoostTestPackageService
    {
        [OperationContract]
        void Placeholder();
    }

    /// <summary>
    /// Abstract wrapper around IBoostTestPackageService.
    /// </summary>
    public interface IBoostTestPackageServiceWrapper : IDisposable
    {
        /// <summary>
        /// The wrapped object.
        /// </summary>
        IBoostTestPackageService Service { get; }
    }

    /// <summary>
    /// Wrapper around IBoostTestPackageService channel proxy.
    /// </summary>
    public class BoostTestPackageServiceProxyWrapper : IBoostTestPackageServiceWrapper
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BoostTestPackageServiceProxyWrapper(IBoostTestPackageService proxy)
        {
            Service = proxy;
        }

        #region IBoostTestPackageServiceWrapper

        /// <summary>
        /// The wrapped object.
        /// </summary>
        public IBoostTestPackageService Service { get; private set; }

        #endregion

        #region IDisposable

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        ((IClientChannel)Service).Close();
                    }
                    catch (CommunicationException)
                    {
                        ((IClientChannel)Service).Abort();
                        // Not rethrowing CommunicationException
                    }
                    catch (Exception)
                    {
                        ((IClientChannel)Service).Abort();
                        throw;
                    }
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
