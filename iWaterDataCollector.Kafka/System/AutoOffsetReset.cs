using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Kafka.System
{
    /// <summary>
    ///     AutoOffsetReset enum values
    /// </summary>
    public enum AutoOffsetReset
    {
        /// <summary>
        ///     Latest
        /// </summary>
        Latest,

        /// <summary>
        ///     Earliest
        /// </summary>
        Earliest,

        /// <summary>
        ///     Error
        /// </summary>
        Error
    }
}
