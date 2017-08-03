// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using FakeItEasy;
using VisualStudioAdapter;
using BoostTestShared;

namespace BoostTestAdapterNunit.Utility
{
    /// <summary>
    /// Builds fake IBoostTestPackageServiceWrapper instances
    /// </summary>
    class FakeBoostTestPackageServiceInstanceBuilder
    {
        private string _workingDirectory = string.Empty;
        private string _environment = string.Empty;

        /// <summary>
        /// Identifies the project's working directory
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public FakeBoostTestPackageServiceInstanceBuilder WorkingDirectory(string output)
        {
            this._workingDirectory = output;
            return this;
        }

        /// <summary>
        /// Identifies the project's environment
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public FakeBoostTestPackageServiceInstanceBuilder Environment(string output)
        {
            this._environment = output;
            return this;
        }

        /// <summary>
        /// Commits any pending changes and builds a fake IBoostTestPackageServiceWrapper instance.
        /// </summary>
        /// <returns>A fake IBoostTestPackageServiceWrapper instance consisting of the previously registered output</returns>
        public IBoostTestPackageServiceWrapper Build()
        {
            IBoostTestPackageService fake = A.Fake<IBoostTestPackageService>();

            A.CallTo(() => fake.GetEnvironment(A<string>._)).Returns(this._environment);
            A.CallTo(() => fake.GetWorkingDirectory(A<string>._)).Returns(this._workingDirectory);

            IBoostTestPackageServiceWrapper fakeWrapper = A.Fake<IBoostTestPackageServiceWrapper>();

            A.CallTo(() => fakeWrapper.Service).Returns(fake);

            return fakeWrapper;
        }
    }
}
