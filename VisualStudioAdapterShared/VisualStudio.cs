﻿// (C) Copyright 2015 ETAS GmbH (http://www.etas.com/)
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)

// This file has been modified by Microsoft on 8/2017.

using EnvDTE80;

namespace VisualStudioAdapter.Shared
{
    /// <summary>
    /// Base class for DTE2-based Visual Studio instances.
    /// </summary>
    public abstract class VisualStudio : IVisualStudio
    {
        private DTE2 _dte = null;
        private Solution _solution = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dte">The DTE2 instance for a running Visual Studio instance</param>
        /// <param name="version">The version identifying the Visual Studio instance</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dte"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dte"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dte")]
        protected VisualStudio(DTE2 dte)
        {
            this._dte = dte;
        }

        #region IVisualStudio

        public ISolution Solution
        {
            get
            {
                if (this._solution == null)
                {
                    // Wrap the solution in a respective adapter instance
                    this._solution = new Solution(this._dte.Solution);
                }

                return this._solution;
            }
        }

        #endregion IVisualStudio
    }
}