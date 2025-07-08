using iWaterDataCollector.Global;
using log4net;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace iWaterDataCollector.Net
{
    public class SecondarySock
    {
        #region 싱글톤
        private static SecondarySock _Instance = null;

        public static SecondarySock Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new SecondarySock();

                return _Instance;
            }
        }

        private SecondarySock()
        {
            _bkHeartbit.DoWork += _bkHeartbit_DoWork;
            _bkHeartbit.RunWorkerCompleted += _bkHeartbit_RunWorkerCompleted; ;
            _bkHeartbit.WorkerSupportsCancellation = true;

            _bkServerConnect.DoWork += _bkServerConnect_DoWork;
            _bkServerConnect.RunWorkerCompleted += _bkServerConnect_RunWorkerCompleted;
            _bkServerConnect.WorkerSupportsCancellation = true;
        }

        #endregion
        private TcpClient _tcpClient = null;
        private NetworkStream _networkStream;
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;

        private BackgroundWorker _bkHeartbit = new BackgroundWorker();
        private BackgroundWorker _bkServerConnect = new BackgroundWorker();

        public bool Started => _started;
        private bool _started;

        private bool _connected;

        private string _ip = string.Empty;
        private int _port = 0;


        public void Start(string ip, int port)
        {
            try
            {
                _started = true;
                _ip = ip;
                _port = port;
                _bkServerConnect.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _connected = false;
            }
            
            ChangedConnection(_connected);
        }

        public void Stop()
        {
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
            _bkHeartbit.CancelAsync();
            _bkServerConnect.CancelAsync();

            _started = false;
            _connected = false;
        }

        private void _bkServerConnect_DoWork(object sender, DoWorkEventArgs e)
        {
            while (_started)
            {
                if (_bkServerConnect.CancellationPending)
                {
                    break;
                }

                if (_connected)
                {
                    Thread.Sleep(100);
                    continue;
                }
                try
                {
                    _tcpClient = new TcpClient(_ip, _port);
                    _connected = true;
                    ChangedConnection(_connected);
                    _networkStream = _tcpClient.GetStream();
                    _streamWriter = new StreamWriter(_networkStream);
                    _streamReader = new StreamReader(_networkStream);
                    _bkHeartbit.RunWorkerAsync();
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    if (_connected)
                    {
                        _bkHeartbit.CancelAsync();
                    }

                    _connected = false;
                    ChangedConnection(_connected);
                    //10초후 재시도 2023.11.14 추가
                    Thread.Sleep(10000);
                }
            }
        }

        private void _bkHeartbit_DoWork(object sender, DoWorkEventArgs e)
        {
            while (_connected)
            {
                if(_bkHeartbit.CancellationPending)
                {
                    break;  
                }
                try
                {
                    if (_networkStream.CanRead)
                    {
                        string message = "OK";
                        _streamWriter.WriteLine(message);
                        _streamWriter.Flush();

#if NET_CHECK
                        _message.SetDebug(_debugLog, $"Client Message 전송 : {message}");
#endif

                        string messageEncrypted = _streamReader.ReadLine();
                        if (messageEncrypted.Length > 0)
                        {
#if NET_CHECK
                            _message.SetDebug(_debugLog, $"Server Message : {messageEncrypted.Split('|').Last()}"));
#endif
                            continue;
                        }
                    }

                    Thread.Sleep(100);
                }
                catch (Exception ex)
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
        private void _bkHeartbit_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private void _bkServerConnect_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
