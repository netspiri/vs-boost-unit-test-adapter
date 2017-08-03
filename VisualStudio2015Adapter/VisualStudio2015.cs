// (C) Copyright 2015 ETAS GmbH (http://www.etas.com/)
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)

// This file has been modified by Microsoft on 8/2017.

namespace VisualStudio2015Adapter
{
    /// <summary>
    /// Adapter for a Visual Studio 2015 and newer running instances.
    /// </summary>
    public class VisualStudio : VisualStudioAdapter.Shared.VisualStudio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dte")]
        public VisualStudio(EnvDTE80.DTE2 dte) :
            base(dte)
        {
        }
    }
}