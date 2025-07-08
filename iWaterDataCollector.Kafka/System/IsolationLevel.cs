using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Kafka.System
{
    /// <summary>
    ///     IsolationLevel enum values
    /// </summary>
    public enum IsolationLevel
    {
        /// <summary>
        ///     ReadUncommitted
        /// </summary>
        ReadUncommitted,

        /// <summary>
        ///     ReadCommitted
        /// </summary>
        ReadCommitted
    }
}
