﻿// (C) Copyright 2015 ETAS GmbH (http://www.etas.com/)
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)

namespace BoostTestAdapter.Boost.Results.LogEntryTypes
{
    /// <summary>
    /// Log entry for a message emitted to standard error.
    /// </summary>
    public class LogEntryStandardErrorMessage : LogEntry
    {
        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public LogEntryStandardErrorMessage()
        {
        }

        #endregion Constructors

        #region object overrides

        public override string ToString()
        {
            return "Standard Error";
        }

        #endregion object overrides
    }
}