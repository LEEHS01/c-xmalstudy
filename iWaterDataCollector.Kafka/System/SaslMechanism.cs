using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Kafka.System
{
    /// <summary>
    ///     SaslMechanism enum values
    /// </summary>
    public enum SaslMechanism
    {
        /// <summary>
        ///     GSSAPI
        /// </summary>
        Gssapi,

        /// <summary>
        ///     PLAIN
        /// </summary>
        Plain,

        /// <summary>
        ///     SCRAM-SHA-256
        /// </summary>
        ScramSha256,

        /// <summary>
        ///     SCRAM-SHA-512
        /// </summary>
        ScramSha512,

        /// <summary>
        ///     OAUTHBEARER
        /// </summary>
        OAuthBearer
    }
}
