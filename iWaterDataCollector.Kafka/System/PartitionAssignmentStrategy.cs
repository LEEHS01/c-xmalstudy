using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Kafka.System
{
    /// <summary>
    ///     PartitionAssignmentStrategy enum values
    /// </summary>
    public enum PartitionAssignmentStrategy
    {
        /// <summary>
        ///     Range
        /// </summary>
        Range,

        /// <summary>
        ///     RoundRobin
        /// </summary>
        RoundRobin,

        /// <summary>
        ///     CooperativeSticky
        /// </summary>
        CooperativeSticky
    }
}
