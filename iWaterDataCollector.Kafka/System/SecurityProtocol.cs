using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Kafka.System
{
    /// <summary>
    ///     SecurityProtocol enum values
    /// </summary>
    public enum SecurityProtocol
    {
        /// <summary>
        ///     Plaintext
        /// </summary>
        Plaintext,

        /// <summary>
        ///     Ssl
        /// </summary>
        Ssl,

        /// <summary>
        ///     SaslPlaintext
        /// </summary>
        SaslPlaintext,

        /// <summary>
        ///     SaslSsl
        /// </summary>
        SaslSsl
    }
}
