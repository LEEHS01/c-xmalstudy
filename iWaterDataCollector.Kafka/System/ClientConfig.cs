// *** Auto-generated from librdkafka v1.9.2 *** - do not modify manually.
//
// Copyright 2018-2022 Confluent Inc.
//
// Licensed under the Apache License, Version 2.0 (the 'License');
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an 'AS IS' BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Refer to LICENSE for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Kafka.System
{
    /// <summary>
    ///     Configuration common to all clients
    /// </summary>
    public class ClientConfig : Config
    {

        /// <summary>
        ///     Initialize a new empty <see cref="ClientConfig" /> instance.
        /// </summary>
        public ClientConfig() : base() { }

        /// <summary>
        ///     Initialize a new <see cref="ClientConfig" /> instance wrapping
        ///     an existing <see cref="ClientConfig" /> instance.
        ///     This will change the values "in-place" i.e. operations on this class WILL modify the provided collection
        /// </summary>
        public ClientConfig(ClientConfig config) : base(config) { }

        /// <summary>
        ///     Initialize a new <see cref="ClientConfig" /> instance wrapping
        ///     an existing key/value pair collection.
        ///     This will change the values "in-place" i.e. operations on this class WILL modify the provided collection
        /// </summary>
        public ClientConfig(IDictionary<string, string> config) : base(config) { }

        /// <summary>
        ///     SASL mechanism to use for authentication. Supported: GSSAPI, PLAIN, SCRAM-SHA-256, SCRAM-SHA-512. **NOTE**: Despite the name, you may not configure more than one mechanism.
        /// </summary>
        public SaslMechanism? SaslMechanism
        {
            get
            {
                var r = Get("sasl.mechanism");
                if (r == null) { return null; }
                if (r == "GSSAPI") { return System.SaslMechanism.Gssapi; }
                if (r == "PLAIN") { return System.SaslMechanism.Plain; }
                if (r == "SCRAM-SHA-256") { return System.SaslMechanism.ScramSha256; }
                if (r == "SCRAM-SHA-512") { return System.SaslMechanism.ScramSha512; }
                if (r == "OAUTHBEARER") { return System.SaslMechanism.OAuthBearer; }
                throw new ArgumentException($"Unknown sasl.mechanism value {r}");
            }
            set
            {
                if (value == null) { this.properties.Remove("sasl.mechanism"); }
                else if (value == System.SaslMechanism.Gssapi) { this.properties["sasl.mechanism"] = "GSSAPI"; }
                else if (value == System.SaslMechanism.Plain) { this.properties["sasl.mechanism"] = "PLAIN"; }
                else if (value == System.SaslMechanism.ScramSha256) { this.properties["sasl.mechanism"] = "SCRAM-SHA-256"; }
                else if (value == System.SaslMechanism.ScramSha512) { this.properties["sasl.mechanism"] = "SCRAM-SHA-512"; }
                else if (value == System.SaslMechanism.OAuthBearer) { this.properties["sasl.mechanism"] = "OAUTHBEARER"; }
                else throw new ArgumentException($"Unknown sasl.mechanism value {value}");
            }
        }


        /// <summary>
        ///     This field indicates the number of acknowledgements the leader broker must receive from ISR brokers
        ///     before responding to the request: Zero=Broker does not send any response/ack to client, One=The
        ///     leader will write the record to its local log but will respond without awaiting full acknowledgement
        ///     from all followers. All=Broker will block until message is committed by all in sync replicas (ISRs).
        ///     If there are less than min.insync.replicas (broker configuration) in the ISR set the produce request
        ///     will fail.
        /// </summary>
        public Acks? Acks
        {
            get
            {
                var r = Get("acks");
                if (r == null) { return null; }
                if (r == "0") { return System.Acks.None; }
                if (r == "1") { return System.Acks.Leader; }
                if (r == "-1" || r == "all") { return System.Acks.All; }
                return (Acks)(int.Parse(r));
            }
            set
            {
                if (value == null) { this.properties.Remove("acks"); }
                else if (value == System.Acks.None) { this.properties["acks"] = "0"; }
                else if (value == System.Acks.Leader) { this.properties["acks"] = "1"; }
                else if (value == System.Acks.All) { this.properties["acks"] = "-1"; }
                else { this.properties["acks"] = ((int)value.Value).ToString(); }
            }
        }

        /// <summary>
        ///     Client identifier.
        ///
        ///     default: rdkafka
        ///     importance: low
        /// </summary>
        public string ClientId { get { return Get("client.id"); } set { this.SetObject("client.id", value); } }

        /// <summary>
        ///     Initial list of brokers as a CSV list of broker host or host:port. The application may also use `rd_kafka_brokers_add()` to add brokers during runtime.
        ///
        ///     default: ''
        ///     importance: high
        /// </summary>
        public string BootstrapServers { get { return Get("bootstrap.servers"); } set { this.SetObject("bootstrap.servers", value); } }

        /// <summary>
        ///     Maximum Kafka protocol request message size. Due to differing framing overhead between protocol versions the producer is unable to reliably enforce a strict max message limit at produce time and may exceed the maximum size by one message in protocol ProduceRequests, the broker will enforce the the topic's `max.message.bytes` limit (see Apache Kafka documentation).
        ///
        ///     default: 1000000
        ///     importance: medium
        /// </summary>
        public int? MessageMaxBytes { get { return GetInt("message.max.bytes"); } set { this.SetObject("message.max.bytes", value); } }

        /// <summary>
        ///     Maximum size for message to be copied to buffer. Messages larger than this will be passed by reference (zero-copy) at the expense of larger iovecs.
        ///
        ///     default: 65535
        ///     importance: low
        /// </summary>
        public int? MessageCopyMaxBytes { get { return GetInt("message.copy.max.bytes"); } set { this.SetObject("message.copy.max.bytes", value); } }

        /// <summary>
        ///     Maximum Kafka protocol response message size. This serves as a safety precaution to avoid memory exhaustion in case of protocol hickups. This value must be at least `fetch.max.bytes`  + 512 to allow for protocol overhead; the value is adjusted automatically unless the configuration property is explicitly set.
        ///
        ///     default: 100000000
        ///     importance: medium
        /// </summary>
        public int? ReceiveMessageMaxBytes { get { return GetInt("receive.message.max.bytes"); } set { this.SetObject("receive.message.max.bytes", value); } }

        /// <summary>
        ///     Maximum number of in-flight requests per broker connection. This is a generic property applied to all broker communication, however it is primarily relevant to produce requests. In particular, note that other mechanisms limit the number of outstanding consumer fetch request per broker to one.
        ///
        ///     default: 1000000
        ///     importance: low
        /// </summary>
        public int? MaxInFlight { get { return GetInt("max.in.flight"); } set { this.SetObject("max.in.flight", value); } }

        /// <summary>
        ///     Period of time in milliseconds at which topic and broker metadata is refreshed in order to proactively discover any new brokers, topics, partitions or partition leader changes. Use -1 to disable the intervalled refresh (not recommended). If there are no locally referenced topics (no topic objects created, no messages produced, no subscription or no assignment) then only the broker list will be refreshed every interval but no more often than every 10s.
        ///
        ///     default: 300000
        ///     importance: low
        /// </summary>
        public int? TopicMetadataRefreshIntervalMs { get { return GetInt("topic.metadata.refresh.interval.ms"); } set { this.SetObject("topic.metadata.refresh.interval.ms", value); } }

        /// <summary>
        ///     Metadata cache max age. Defaults to topic.metadata.refresh.interval.ms * 3
        ///
        ///     default: 900000
        ///     importance: low
        /// </summary>
        public int? MetadataMaxAgeMs { get { return GetInt("metadata.max.age.ms"); } set { this.SetObject("metadata.max.age.ms", value); } }

        /// <summary>
        ///     When a topic loses its leader a new metadata request will be enqueued with this initial interval, exponentially increasing until the topic metadata has been refreshed. This is used to recover quickly from transitioning leader brokers.
        ///
        ///     default: 250
        ///     importance: low
        /// </summary>
        public int? TopicMetadataRefreshFastIntervalMs { get { return GetInt("topic.metadata.refresh.fast.interval.ms"); } set { this.SetObject("topic.metadata.refresh.fast.interval.ms", value); } }

        /// <summary>
        ///     Sparse metadata requests (consumes less network bandwidth)
        ///
        ///     default: true
        ///     importance: low
        /// </summary>
        public bool? TopicMetadataRefreshSparse { get { return GetBool("topic.metadata.refresh.sparse"); } set { this.SetObject("topic.metadata.refresh.sparse", value); } }

        /// <summary>
        ///     Apache Kafka topic creation is asynchronous and it takes some time for a new topic to propagate throughout the cluster to all brokers. If a client requests topic metadata after manual topic creation but before the topic has been fully propagated to the broker the client is requesting metadata from, the topic will seem to be non-existent and the client will mark the topic as such, failing queued produced messages with `ERR__UNKNOWN_TOPIC`. This setting delays marking a topic as non-existent until the configured propagation max time has passed. The maximum propagation time is calculated from the time the topic is first referenced in the client, e.g., on produce().
        ///
        ///     default: 30000
        ///     importance: low
        /// </summary>
        public int? TopicMetadataPropagationMaxMs { get { return GetInt("topic.metadata.propagation.max.ms"); } set { this.SetObject("topic.metadata.propagation.max.ms", value); } }

        /// <summary>
        ///     Topic blacklist, a comma-separated list of regular expressions for matching topic names that should be ignored in broker metadata information as if the topics did not exist.
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string TopicBlacklist { get { return Get("topic.blacklist"); } set { this.SetObject("topic.blacklist", value); } }

        /// <summary>
        ///     A comma-separated list of debug contexts to enable. Detailed Producer debugging: broker,topic,msg. Consumer: consumer,cgrp,topic,fetch
        ///
        ///     default: ''
        ///     importance: medium
        /// </summary>
        public string Debug { get { return Get("debug"); } set { this.SetObject("debug", value); } }

        /// <summary>
        ///     Default timeout for network requests. Producer: ProduceRequests will use the lesser value of `socket.timeout.ms` and remaining `message.timeout.ms` for the first message in the batch. Consumer: FetchRequests will use `fetch.wait.max.ms` + `socket.timeout.ms`. Admin: Admin requests will use `socket.timeout.ms` or explicitly set `rd_kafka_AdminOptions_set_operation_timeout()` value.
        ///
        ///     default: 60000
        ///     importance: low
        /// </summary>
        public int? SocketTimeoutMs { get { return GetInt("socket.timeout.ms"); } set { this.SetObject("socket.timeout.ms", value); } }

        /// <summary>
        ///     Broker socket send buffer size. System default is used if 0.
        ///
        ///     default: 0
        ///     importance: low
        /// </summary>
        public int? SocketSendBufferBytes { get { return GetInt("socket.send.buffer.bytes"); } set { this.SetObject("socket.send.buffer.bytes", value); } }

        /// <summary>
        ///     Broker socket receive buffer size. System default is used if 0.
        ///
        ///     default: 0
        ///     importance: low
        /// </summary>
        public int? SocketReceiveBufferBytes { get { return GetInt("socket.receive.buffer.bytes"); } set { this.SetObject("socket.receive.buffer.bytes", value); } }

        /// <summary>
        ///     Enable TCP keep-alives (SO_KEEPALIVE) on broker sockets
        ///
        ///     default: false
        ///     importance: low
        /// </summary>
        public bool? SocketKeepaliveEnable { get { return GetBool("socket.keepalive.enable"); } set { this.SetObject("socket.keepalive.enable", value); } }

        /// <summary>
        ///     Disable the Nagle algorithm (TCP_NODELAY) on broker sockets.
        ///
        ///     default: false
        ///     importance: low
        /// </summary>
        public bool? SocketNagleDisable { get { return GetBool("socket.nagle.disable"); } set { this.SetObject("socket.nagle.disable", value); } }

        /// <summary>
        ///     Disconnect from broker when this number of send failures (e.g., timed out requests) is reached. Disable with 0. WARNING: It is highly recommended to leave this setting at its default value of 1 to avoid the client and broker to become desynchronized in case of request timeouts. NOTE: The connection is automatically re-established.
        ///
        ///     default: 1
        ///     importance: low
        /// </summary>
        public int? SocketMaxFails { get { return GetInt("socket.max.fails"); } set { this.SetObject("socket.max.fails", value); } }

        /// <summary>
        ///     How long to cache the broker address resolving results (milliseconds).
        ///
        ///     default: 1000
        ///     importance: low
        /// </summary>
        public int? BrokerAddressTtl { get { return GetInt("broker.address.ttl"); } set { this.SetObject("broker.address.ttl", value); } }

        /// <summary>
        ///     Allowed broker IP address families: any, v4, v6
        ///
        ///     default: any
        ///     importance: low
        /// </summary>
        public BrokerAddressFamily? BrokerAddressFamily { get { return (BrokerAddressFamily?)GetEnum(typeof(BrokerAddressFamily), "broker.address.family"); } set { this.SetObject("broker.address.family", value); } }

        /// <summary>
        ///     Maximum time allowed for broker connection setup (TCP connection setup as well SSL and SASL handshake). If the connection to the broker is not fully functional after this the connection will be closed and retried.
        ///
        ///     default: 30000
        ///     importance: medium
        /// </summary>
        public int? SocketConnectionSetupTimeoutMs { get { return GetInt("socket.connection.setup.timeout.ms"); } set { this.SetObject("socket.connection.setup.timeout.ms", value); } }

        /// <summary>
        ///     Close broker connections after the specified time of inactivity. Disable with 0. If this property is left at its default value some heuristics are performed to determine a suitable default value, this is currently limited to identifying brokers on Azure (see librdkafka issue #3109 for more info).
        ///
        ///     default: 0
        ///     importance: medium
        /// </summary>
        public int? ConnectionsMaxIdleMs { get { return GetInt("connections.max.idle.ms"); } set { this.SetObject("connections.max.idle.ms", value); } }

        /// <summary>
        ///     The initial time to wait before reconnecting to a broker after the connection has been closed. The time is increased exponentially until `reconnect.backoff.max.ms` is reached. -25% to +50% jitter is applied to each reconnect backoff. A value of 0 disables the backoff and reconnects immediately.
        ///
        ///     default: 100
        ///     importance: medium
        /// </summary>
        public int? ReconnectBackoffMs { get { return GetInt("reconnect.backoff.ms"); } set { this.SetObject("reconnect.backoff.ms", value); } }

        /// <summary>
        ///     The maximum time to wait before reconnecting to a broker after the connection has been closed.
        ///
        ///     default: 10000
        ///     importance: medium
        /// </summary>
        public int? ReconnectBackoffMaxMs { get { return GetInt("reconnect.backoff.max.ms"); } set { this.SetObject("reconnect.backoff.max.ms", value); } }

        /// <summary>
        ///     librdkafka statistics emit interval. The application also needs to register a stats callback using `rd_kafka_conf_set_stats_cb()`. The granularity is 1000ms. A value of 0 disables statistics.
        ///
        ///     default: 0
        ///     importance: high
        /// </summary>
        public int? StatisticsIntervalMs { get { return GetInt("statistics.interval.ms"); } set { this.SetObject("statistics.interval.ms", value); } }

        /// <summary>
        ///     Disable spontaneous log_cb from internal librdkafka threads, instead enqueue log messages on queue set with `rd_kafka_set_log_queue()` and serve log callbacks or events through the standard poll APIs. **NOTE**: Log messages will linger in a temporary queue until the log queue has been set.
        ///
        ///     default: false
        ///     importance: low
        /// </summary>
        public bool? LogQueue { get { return GetBool("log.queue"); } set { this.SetObject("log.queue", value); } }

        /// <summary>
        ///     Print internal thread name in log messages (useful for debugging librdkafka internals)
        ///
        ///     default: true
        ///     importance: low
        /// </summary>
        public bool? LogThreadName { get { return GetBool("log.thread.name"); } set { this.SetObject("log.thread.name", value); } }

        /// <summary>
        ///     If enabled librdkafka will initialize the PRNG with srand(current_time.milliseconds) on the first invocation of rd_kafka_new() (required only if rand_r() is not available on your platform). If disabled the application must call srand() prior to calling rd_kafka_new().
        ///
        ///     default: true
        ///     importance: low
        /// </summary>
        public bool? EnableRandomSeed { get { return GetBool("enable.random.seed"); } set { this.SetObject("enable.random.seed", value); } }

        /// <summary>
        ///     Log broker disconnects. It might be useful to turn this off when interacting with 0.9 brokers with an aggressive `connections.max.idle.ms` value.
        ///
        ///     default: true
        ///     importance: low
        /// </summary>
        public bool? LogConnectionClose { get { return GetBool("log.connection.close"); } set { this.SetObject("log.connection.close", value); } }

        /// <summary>
        ///     Signal that librdkafka will use to quickly terminate on rd_kafka_destroy(). If this signal is not set then there will be a delay before rd_kafka_wait_destroyed() returns true as internal threads are timing out their system calls. If this signal is set however the delay will be minimal. The application should mask this signal as an internal signal handler is installed.
        ///
        ///     default: 0
        ///     importance: low
        /// </summary>
        public int? InternalTerminationSignal { get { return GetInt("internal.termination.signal"); } set { this.SetObject("internal.termination.signal", value); } }

        /// <summary>
        ///     Request broker's supported API versions to adjust functionality to available protocol features. If set to false, or the ApiVersionRequest fails, the fallback version `broker.version.fallback` will be used. **NOTE**: Depends on broker version >=0.10.0. If the request is not supported by (an older) broker the `broker.version.fallback` fallback is used.
        ///
        ///     default: true
        ///     importance: high
        /// </summary>
        public bool? ApiVersionRequest { get { return GetBool("api.version.request"); } set { this.SetObject("api.version.request", value); } }

        /// <summary>
        ///     Timeout for broker API version requests.
        ///
        ///     default: 10000
        ///     importance: low
        /// </summary>
        public int? ApiVersionRequestTimeoutMs { get { return GetInt("api.version.request.timeout.ms"); } set { this.SetObject("api.version.request.timeout.ms", value); } }

        /// <summary>
        ///     Dictates how long the `broker.version.fallback` fallback is used in the case the ApiVersionRequest fails. **NOTE**: The ApiVersionRequest is only issued when a new connection to the broker is made (such as after an upgrade).
        ///
        ///     default: 0
        ///     importance: medium
        /// </summary>
        public int? ApiVersionFallbackMs { get { return GetInt("api.version.fallback.ms"); } set { this.SetObject("api.version.fallback.ms", value); } }

        /// <summary>
        ///     Older broker versions (before 0.10.0) provide no way for a client to query for supported protocol features (ApiVersionRequest, see `api.version.request`) making it impossible for the client to know what features it may use. As a workaround a user may set this property to the expected broker version and the client will automatically adjust its feature set accordingly if the ApiVersionRequest fails (or is disabled). The fallback broker version will be used for `api.version.fallback.ms`. Valid values are: 0.9.0, 0.8.2, 0.8.1, 0.8.0. Any other value >= 0.10, such as 0.10.2.1, enables ApiVersionRequests.
        ///
        ///     default: 0.10.0
        ///     importance: medium
        /// </summary>
        public string BrokerVersionFallback { get { return Get("broker.version.fallback"); } set { this.SetObject("broker.version.fallback", value); } }

        /// <summary>
        ///     Protocol used to communicate with brokers.
        ///
        ///     default: plaintext
        ///     importance: high
        /// </summary>
        public SecurityProtocol? SecurityProtocol { get { return (SecurityProtocol?)GetEnum(typeof(SecurityProtocol), "security.protocol"); } set { this.SetObject("security.protocol", value); } }

        /// <summary>
        ///     A cipher suite is a named combination of authentication, encryption, MAC and key exchange algorithm used to negotiate the security settings for a network connection using TLS or SSL network protocol. See manual page for `ciphers(1)` and `SSL_CTX_set_cipher_list(3).
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslCipherSuites { get { return Get("ssl.cipher.suites"); } set { this.SetObject("ssl.cipher.suites", value); } }

        /// <summary>
        ///     The supported-curves extension in the TLS ClientHello message specifies the curves (standard/named, or 'explicit' GF(2^k) or GF(p)) the client is willing to have the server use. See manual page for `SSL_CTX_set1_curves_list(3)`. OpenSSL >= 1.0.2 required.
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslCurvesList { get { return Get("ssl.curves.list"); } set { this.SetObject("ssl.curves.list", value); } }

        /// <summary>
        ///     The client uses the TLS ClientHello signature_algorithms extension to indicate to the server which signature/hash algorithm pairs may be used in digital signatures. See manual page for `SSL_CTX_set1_sigalgs_list(3)`. OpenSSL >= 1.0.2 required.
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslSigalgsList { get { return Get("ssl.sigalgs.list"); } set { this.SetObject("ssl.sigalgs.list", value); } }

        /// <summary>
        ///     Path to client's private key (PEM) used for authentication.
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslKeyLocation { get { return Get("ssl.key.location"); } set { this.SetObject("ssl.key.location", value); } }

        /// <summary>
        ///     Private key passphrase (for use with `ssl.key.location` and `set_ssl_cert()`)
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslKeyPassword { get { return Get("ssl.key.password"); } set { this.SetObject("ssl.key.password", value); } }

        /// <summary>
        ///     Client's private key string (PEM format) used for authentication.
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslKeyPem { get { return Get("ssl.key.pem"); } set { this.SetObject("ssl.key.pem", value); } }

        /// <summary>
        ///     Path to client's public key (PEM) used for authentication.
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslCertificateLocation { get { return Get("ssl.certificate.location"); } set { this.SetObject("ssl.certificate.location", value); } }

        /// <summary>
        ///     Client's public key string (PEM format) used for authentication.
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslCertificatePem { get { return Get("ssl.certificate.pem"); } set { this.SetObject("ssl.certificate.pem", value); } }

        /// <summary>
        ///     File or directory path to CA certificate(s) for verifying the broker's key. Defaults: On Windows the system's CA certificates are automatically looked up in the Windows Root certificate store. On Mac OSX this configuration defaults to `probe`. It is recommended to install openssl using Homebrew, to provide CA certificates. On Linux install the distribution's ca-certificates package. If OpenSSL is statically linked or `ssl.ca.location` is set to `probe` a list of standard paths will be probed and the first one found will be used as the default CA certificate location path. If OpenSSL is dynamically linked the OpenSSL library's default path will be used (see `OPENSSLDIR` in `openssl version -a`).
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslCaLocation { get { return Get("ssl.ca.location"); } set { this.SetObject("ssl.ca.location", value); } }

        /// <summary>
        ///     CA certificate string (PEM format) for verifying the broker's key.
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslCaPem { get { return Get("ssl.ca.pem"); } set { this.SetObject("ssl.ca.pem", value); } }

        /// <summary>
        ///     Comma-separated list of Windows Certificate stores to load CA certificates from. Certificates will be loaded in the same order as stores are specified. If no certificates can be loaded from any of the specified stores an error is logged and the OpenSSL library's default CA location is used instead. Store names are typically one or more of: MY, Root, Trust, CA.
        ///
        ///     default: Root
        ///     importance: low
        /// </summary>
        public string SslCaCertificateStores { get { return Get("ssl.ca.certificate.stores"); } set { this.SetObject("ssl.ca.certificate.stores", value); } }

        /// <summary>
        ///     Path to CRL for verifying broker's certificate validity.
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslCrlLocation { get { return Get("ssl.crl.location"); } set { this.SetObject("ssl.crl.location", value); } }

        /// <summary>
        ///     Path to client's keystore (PKCS#12) used for authentication.
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslKeystoreLocation { get { return Get("ssl.keystore.location"); } set { this.SetObject("ssl.keystore.location", value); } }

        /// <summary>
        ///     Client's keystore (PKCS#12) password.
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslKeystorePassword { get { return Get("ssl.keystore.password"); } set { this.SetObject("ssl.keystore.password", value); } }

        /// <summary>
        ///     Path to OpenSSL engine library. OpenSSL >= 1.1.0 required.
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SslEngineLocation { get { return Get("ssl.engine.location"); } set { this.SetObject("ssl.engine.location", value); } }

        /// <summary>
        ///     OpenSSL engine id is the name used for loading engine.
        ///
        ///     default: dynamic
        ///     importance: low
        /// </summary>
        public string SslEngineId { get { return Get("ssl.engine.id"); } set { this.SetObject("ssl.engine.id", value); } }

        /// <summary>
        ///     Enable OpenSSL's builtin broker (server) certificate verification. This verification can be extended by the application by implementing a certificate_verify_cb.
        ///
        ///     default: true
        ///     importance: low
        /// </summary>
        public bool? EnableSslCertificateVerification { get { return GetBool("enable.ssl.certificate.verification"); } set { this.SetObject("enable.ssl.certificate.verification", value); } }

        /// <summary>
        ///     Endpoint identification algorithm to validate broker hostname using broker certificate. https - Server (broker) hostname verification as specified in RFC2818. none - No endpoint verification. OpenSSL >= 1.0.2 required.
        ///
        ///     default: none
        ///     importance: low
        /// </summary>
        public SslEndpointIdentificationAlgorithm? SslEndpointIdentificationAlgorithm { get { return (SslEndpointIdentificationAlgorithm?)GetEnum(typeof(SslEndpointIdentificationAlgorithm), "ssl.endpoint.identification.algorithm"); } set { this.SetObject("ssl.endpoint.identification.algorithm", value); } }

        /// <summary>
        ///     Kerberos principal name that Kafka runs as, not including /hostname@REALM
        ///
        ///     default: kafka
        ///     importance: low
        /// </summary>
        public string SaslKerberosServiceName { get { return Get("sasl.kerberos.service.name"); } set { this.SetObject("sasl.kerberos.service.name", value); } }

        /// <summary>
        ///     This client's Kerberos principal name. (Not supported on Windows, will use the logon user's principal).
        ///
        ///     default: kafkaclient
        ///     importance: low
        /// </summary>
        public string SaslKerberosPrincipal { get { return Get("sasl.kerberos.principal"); } set { this.SetObject("sasl.kerberos.principal", value); } }

        /// <summary>
        ///     Shell command to refresh or acquire the client's Kerberos ticket. This command is executed on client creation and every sasl.kerberos.min.time.before.relogin (0=disable). %{config.prop.name} is replaced by corresponding config object value.
        ///
        ///     default: kinit -R -t "%{sasl.kerberos.keytab}" -k %{sasl.kerberos.principal} || kinit -t "%{sasl.kerberos.keytab}" -k %{sasl.kerberos.principal}
        ///     importance: low
        /// </summary>
        public string SaslKerberosKinitCmd { get { return Get("sasl.kerberos.kinit.cmd"); } set { this.SetObject("sasl.kerberos.kinit.cmd", value); } }

        /// <summary>
        ///     Path to Kerberos keytab file. This configuration property is only used as a variable in `sasl.kerberos.kinit.cmd` as ` ... -t "%{sasl.kerberos.keytab}"`.
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SaslKerberosKeytab { get { return Get("sasl.kerberos.keytab"); } set { this.SetObject("sasl.kerberos.keytab", value); } }

        /// <summary>
        ///     Minimum time in milliseconds between key refresh attempts. Disable automatic key refresh by setting this property to 0.
        ///
        ///     default: 60000
        ///     importance: low
        /// </summary>
        public int? SaslKerberosMinTimeBeforeRelogin { get { return GetInt("sasl.kerberos.min.time.before.relogin"); } set { this.SetObject("sasl.kerberos.min.time.before.relogin", value); } }

        /// <summary>
        ///     SASL username for use with the PLAIN and SASL-SCRAM-.. mechanisms
        ///
        ///     default: ''
        ///     importance: high
        /// </summary>
        public string SaslUsername { get { return Get("sasl.username"); } set { this.SetObject("sasl.username", value); } }

        /// <summary>
        ///     SASL password for use with the PLAIN and SASL-SCRAM-.. mechanism
        ///
        ///     default: ''
        ///     importance: high
        /// </summary>
        public string SaslPassword { get { return Get("sasl.password"); } set { this.SetObject("sasl.password", value); } }

        /// <summary>
        ///     SASL/OAUTHBEARER configuration. The format is implementation-dependent and must be parsed accordingly. The default unsecured token implementation (see https://tools.ietf.org/html/rfc7515#appendix-A.5) recognizes space-separated name=value pairs with valid names including principalClaimName, principal, scopeClaimName, scope, and lifeSeconds. The default value for principalClaimName is "sub", the default value for scopeClaimName is "scope", and the default value for lifeSeconds is 3600. The scope value is CSV format with the default value being no/empty scope. For example: `principalClaimName=azp principal=admin scopeClaimName=roles scope=role1,role2 lifeSeconds=600`. In addition, SASL extensions can be communicated to the broker via `extension_NAME=value`. For example: `principal=admin extension_traceId=123`
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SaslOauthbearerConfig { get { return Get("sasl.oauthbearer.config"); } set { this.SetObject("sasl.oauthbearer.config", value); } }

        /// <summary>
        ///     Enable the builtin unsecure JWT OAUTHBEARER token handler if no oauthbearer_refresh_cb has been set. This builtin handler should only be used for development or testing, and not in production.
        ///
        ///     default: false
        ///     importance: low
        /// </summary>
        public bool? EnableSaslOauthbearerUnsecureJwt { get { return GetBool("enable.sasl.oauthbearer.unsecure.jwt"); } set { this.SetObject("enable.sasl.oauthbearer.unsecure.jwt", value); } }

        /// <summary>
        ///     Set to "default" or "oidc" to control which login method to be used. If set to "oidc", the following properties must also be be specified: `sasl.oauthbearer.client.id`, `sasl.oauthbearer.client.secret`, and `sasl.oauthbearer.token.endpoint.url`.
        ///
        ///     default: default
        ///     importance: low
        /// </summary>
        public SaslOauthbearerMethod? SaslOauthbearerMethod { get { return (SaslOauthbearerMethod?)GetEnum(typeof(SaslOauthbearerMethod), "sasl.oauthbearer.method"); } set { this.SetObject("sasl.oauthbearer.method", value); } }

        /// <summary>
        ///     Public identifier for the application. Must be unique across all clients that the authorization server handles. Only used when `sasl.oauthbearer.method` is set to "oidc".
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SaslOauthbearerClientId { get { return Get("sasl.oauthbearer.client.id"); } set { this.SetObject("sasl.oauthbearer.client.id", value); } }

        /// <summary>
        ///     Client secret only known to the application and the authorization server. This should be a sufficiently random string that is not guessable. Only used when `sasl.oauthbearer.method` is set to "oidc".
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SaslOauthbearerClientSecret { get { return Get("sasl.oauthbearer.client.secret"); } set { this.SetObject("sasl.oauthbearer.client.secret", value); } }

        /// <summary>
        ///     Client use this to specify the scope of the access request to the broker. Only used when `sasl.oauthbearer.method` is set to "oidc".
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SaslOauthbearerScope { get { return Get("sasl.oauthbearer.scope"); } set { this.SetObject("sasl.oauthbearer.scope", value); } }

        /// <summary>
        ///     Allow additional information to be provided to the broker. Comma-separated list of key=value pairs. E.g., "supportFeatureX=true,organizationId=sales-emea".Only used when `sasl.oauthbearer.method` is set to "oidc".
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SaslOauthbearerExtensions { get { return Get("sasl.oauthbearer.extensions"); } set { this.SetObject("sasl.oauthbearer.extensions", value); } }

        /// <summary>
        ///     OAuth/OIDC issuer token endpoint HTTP(S) URI used to retrieve token. Only used when `sasl.oauthbearer.method` is set to "oidc".
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string SaslOauthbearerTokenEndpointUrl { get { return Get("sasl.oauthbearer.token.endpoint.url"); } set { this.SetObject("sasl.oauthbearer.token.endpoint.url", value); } }

        /// <summary>
        ///     List of plugin libraries to load (; separated). The library search path is platform dependent (see dlopen(3) for Unix and LoadLibrary() for Windows). If no filename extension is specified the platform-specific extension (such as .dll or .so) will be appended automatically.
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string PluginLibraryPaths { get { return Get("plugin.library.paths"); } set { this.SetObject("plugin.library.paths", value); } }

        /// <summary>
        ///     A rack identifier for this client. This can be any string value which indicates where this client is physically located. It corresponds with the broker config `broker.rack`.
        ///
        ///     default: ''
        ///     importance: low
        /// </summary>
        public string ClientRack { get { return Get("client.rack"); } set { this.SetObject("client.rack", value); } }

    }
}
