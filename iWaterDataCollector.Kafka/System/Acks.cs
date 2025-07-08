using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Kafka.System
{
    /// <summary>
    ///     Acks enum values
    /// </summary>
    public enum Acks : int
    {
        /// <summary>
        ///     None
        /// </summary>
        None = 0,

        /// <summary>
        ///     Leader
        /// </summary>
        Leader = 1,

        /// <summary>
        ///     All
        /// </summary>
        All = -1
    }
}
