using iWaterDataCollector.Global;
using log4net;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace iWaterDataCollector.Net
{
    public class PrimarySock
    {
        #region 싱글톤
        private static PrimarySock _Instance = null;

        public static PrimarySock Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new PrimarySock();

                return _Instance;
            }
        }

        private PrimarySock()
        {

            _bkListen.DoWork += _bkListen_DoWork;
            _bkListen.RunWorkerCompleted += _bkListen_RunWorkerCompleted;
            _bkListen.WorkerSupportsCancellation = true;

            _bkResponse.DoWork += _bkResponse_DoWork;
            _bkResponse.RunWorkerCompleted += _bkResponse_RunWorkerCompleted;
        }
        #endregion

        private TcpListener _tcpListener = null;
        private TcpClient _tcpClient = null;

        private NetworkStream _networkStream;
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;

        private BackgroundWorker _bkListen = new BackgroundWorker();
        private BackgroundWorker _bkResponse = new BackgroundWorker();

        private bool _connected;

        public bool Started => _started;
        private bool _started;

        public void Start(int port)
        {
            try
            {
                _started = true;
                _tcpListener = new TcpListener(IPAddress.Any, port);
                _tcpListener.Start();
                _bkListen.RunWorkerAsync();
            }
            catch (Exception ex)
            {
            }
        }

        public void Stop()
        {
            if(!(_tcpListener == null))
            {
                _tcpListener.Stop();
            }
            if (!(_streamReader == null))
            {
                _streamReader.Close();
            }
            if (!(_streamWriter == null))
            {
                _streamWriter.Close();
            }
            if (!(_tcpClient == null))
            {
                _tcpClient.Close();
            }
            _bkListen.CancelAsync();
            _started = false;
        }

        private void _bkListen_DoWork(object sender, DoWorkEventArgs e)
        {
            while (_started)
            {
                if (_bkListen.CancellationPending)
                {
                    _connected = false;
                    break;
                }
                
                try
                {
                    ChangedConnection(_connected);
                    _tcpClient = _tcpListener.AcceptTcpClient();
                    _connected = true;
                    _networkStream = _tcpClient.GetStream();
                    _streamWriter = new StreamWriter(_networkStream);
                    _streamReader = new StreamReader(_networkStream);
                    _bkResponse.RunWorkerAsync();
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    if (_connected)
                    { }

                    _connected = false;
                    ChangedConnection(_connected);
                    break;
                }
            }
        }

        private void _bkListen_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void _bkResponse_DoWork(object sender, DoWorkEventArgs e)
        {
            while (_connected)
            {
                if (_bkResponse.CancellationPending)
                {
                    break;
                }
                try
                {
                    if (_networkStream.CanRead)
                    {
                        var message = _streamReader.ReadLine();
                        if (string.IsNullOrEmpty(message))
                            break;

#if NET_CHECK
                        _message.SetDebug(_debugLog, $"Response Message : {message}");
#endif

                        if (message.Equals(NET_CODE.OK.ToString()))
                        {
                            var rtnMsg = "S|" + message;
#if NET_CHECK
                            _message.SetDebug(_debugLog, $"Return Message 전송 : {rtnMsg}");
#endif

                            _streamWriter.WriteLine(rtnMsg);
                            _streamWriter.Flush();
                        }
                    }
                    Thread.Sleep(1000);
                }
                catch(Exception ex)
                {
                    if (_connected)
                    {
                    }
                    _connected = false;
                    ChangedConnection(_connected);
                    break;
                }
            }
        }
        private void _bkResponse_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

#region Delegate 및 event 정의
        public event Action<bool> OnChangeConnected;
        protected virtual void ChangedConnection(bool connected)
        {
            this.OnChangeConnected?.Invoke(connected);
        }
#endregion
    }
}
