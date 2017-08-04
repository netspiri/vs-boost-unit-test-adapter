// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using FakeItEasy;
using BoostTestShared;

namespace BoostTestAdapterNunit.Utility
{
    /// <summary>
    /// Builds fake IBoostTestPackageServiceWrapper instances
    /// </summary>
    class FakeBoostTestPackageServiceInstanceBuilder
    {
        private DebuggingProperties _debuggingProperties = new DebuggingProperties();

        /// <summary>
        /// Identifies the project's working directory
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public FakeBoostTestPackageServiceInstanceBuilder WorkingDirectory(string output)
        {
            this._debuggingProperties.WorkingDirectory = output;
            return this;
        }

        /// <summary>
        /// Identifies the project's environment
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public FakeBoostTestPackageServiceInstanceBuilder Environment(string output)
        {
            this._debuggingProperties.Environment = output;
            return this;
        }

        /// <summary>
        /// Commits any pending changes and builds a fake IBoostTestPackageServiceWrapper instance.
        /// </summary>
        /// <returns>A fake IBoostTestPackageServiceWrapper instance consisting of the previously registered output</returns>
        public IBoostTestPackageServiceWrapper Build()
        {
            IBoostTestPackageService fake = A.Fake<IBoostTestPackageService>();

            A.CallTo(() => fake.GetDebuggingProperties(A<string>._)).Returns(this._debuggingProperties);

            IBoostTestPackageServiceWrapper fakeWrapper = A.Fake<IBoostTestPackageServiceWrapper>();

            A.CallTo(() => fakeWrapper.Service).Returns(fake);

            return fakeWrapper;
        }
    }
}
