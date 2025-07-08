using iWaterDataCollector.INI;
using iWaterDataCollector.Model.INI;
using iWaterDataCollector.Model.View;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Windows;

/********************************************
 * 전역변수 Class (Setting 정보 Class ini파일)
 ********************************************/
namespace iWaterDataCollector.Global
{
    public class AppData
    {
        #region Singleton
        private static readonly Lazy<AppData> _instance = new Lazy<AppData>(() => new AppData());
        public static AppData Instance => _instance.Value;

        private AppData()
        {
            
        }
        #endregion

        #region Private User Variable
        /// <summary>
        /// ini Setting Class Data
        /// </summary>
        private IniFile _ini = new IniFile();
        /// <summary>
        /// Setting ini File Path
        /// </summary>
        private string _settingPath = string.Empty;
        /// <summary>
        /// 시스템 이름
        /// </summary>
        private string _currentName;
        #endregion
        #region Global Property
        #region LogManager
        public static ILog AppLog;
        public static ILog ErrorLog;
        public static ILog NetLog;
        public static ILog KafukaLog;
        public static ILog PDBLog;
        public static ILog PDBErrLog;
        public Message MsgIRDC;
        #endregion
        /// <summary>
        /// Tag 유형
        /// </summary>
        public static string F_CV = "F_CV";
        public static string A_CV = "A_CV";

        /// <summary>
        /// iWater Server Setting Info
        /// </summary>
        public IWaterModel IWATER { get; set; } = new IWaterModel();
        /// <summary>
        /// kafka Setting Info
        /// </summary>
        public KafkaModel KAFKA { get; set; } = new KafkaModel();
        /// <summary>
        /// Redundancy Setting Info
        /// </summary>
        public RedundancyModel REDUNDANCY { get; set; } = new RedundancyModel();
        /// <summary>
        /// Program Directory Setting Info
        /// </summary>
        public DirectoryModel DIRECTORY { get; set; } = new DirectoryModel();
        /// <summary>
        /// Log Default 정보
        /// </summary>
        public LogModel LOG { get; set; } = new LogModel();
        /// <summary>
        /// Server 정보
        /// </summary>
        public ObservableCollection<ServerModel> ServerCollection { get; set; } = new ObservableCollection<ServerModel>();

        public ObservableCollection<ServerModel> SettingServerCollection { get; set; } = new ObservableCollection<ServerModel>();        /// <summary>
                                                                                                                                         /// Tag 저장 주기
                                                                                                                                         /// </summary>
        public int StorageInterval => IWATER.StorageInterval;
        /// <summary>
        /// Tag 저장 주기(Double형)
        /// </summary>
        public double StorageIntervalD => (double)IWATER.StorageInterval;
        /// <summary>
        /// Tag 파일 보관 기간
        /// </summary>
        public int ArchiveDuration => IWATER.ArchiveDuration;
        /// <summary>
        /// 누실데이터 백업 기간
        /// </summary>
        public int RecoveryMaxDuration => IWATER.RecoveryMaxDuration;
        /// <summary>
        /// 누실데이터 백업 사용유무
        /// </summary>
        public bool UseRecovery => IWATER.UseRecovery;

        public int PDBSetInterval => IWATER.PDBSetInterval;
        /// <summary>
        /// Local Server Node Name
        /// </summary>
        public string LocalNodeName => _localNoadName;
        private string _localNoadName;
        /// <summary>
        /// Kafak Server Endpoint
        /// </summary>
        public string KafkaEndPoint => KAFKA.EndPoint;
        /// <summary>
        /// Kafka Server Topic
        /// </summary>
        public string Topic => KAFKA.Topic;
        /// <summary>
        /// Kafka Consumer Group ID
        /// </summary>
        public string GroupID => KAFKA.ConsumerID;
        /// <summary>
        /// 이중화 사용 여부
        /// </summary>
        public bool IsRedundancy => REDUNDANCY.IsUse;
        /// <summary>
        /// 이중화 Primary 여부
        /// </summary>
        public bool IsPrimary => REDUNDANCY.IsPrimary;
        /// <summary>
        /// 이중화 서버 IP
        /// </summary>
        public string RedundancyIP => REDUNDANCY.IP;
        /// <summary>
        /// 이중화 서버 Port
        /// </summary>
        public int RedundancyPort => REDUNDANCY.Port;
        /// <summary>
        /// Program 기본 폴더 경로
        /// </summary>
        public string DefaultPath => DIRECTORY.Default;
        /// <summary>
        /// PDB Tag 경로
        /// </summary>
        public string PDBPath => DIRECTORY.PDB;
        /// <summary>
        /// Kafak Tag 경로
        /// </summary>
        public string KafkaPath => DIRECTORY.Kafka;

        /// <summary>
        /// Kafka Tag Filter 여부
        /// </summary>
        public bool UseFilter => KAFKA.UseFilter;
        /// <summary>
        /// 자동시작 여부
        /// </summary>
        public bool IsAutoStart => IWATER.IsAutoConnection;

        /// <summary>
        /// Log 저장기간
        /// </summary>
        public int LogArchiveDuration => IWATER.LogArchiveDuration;
        #endregion
        /// <summary>
        /// Setting INI Load
        /// </summary>
        /// <remarks>
        /// Setting Value 초기화
        /// </remarks>
        /// <param name="path">Setting 파일 경로</param>
        public void Initialize(string path)
        {
            //_settingPath = path;
            _settingPath = $"{AppDomain.CurrentDomain.BaseDirectory}{path}";
            // ini 읽기
            _ini.Load(_settingPath);

            MsgIRDC = new Message();
            //INI파일 구성을 Class로 변경
            SettingLoad();
            //iwater 정보 Load
            SetServerInfo();
        }
        public void InitializeLog(string ini)
        {
            Log.Instance.Initialize(ini);
            _currentName = Application.ResourceAssembly.ManifestModule.Name;

            AppLog = Log.Instance.GetUserLogger(LOG_SECTION.Program.ToString(), _currentName);
            ErrorLog = Log.Instance.GetUserLogger(LOG_SECTION.Error.ToString(), _currentName);
            NetLog = Log.Instance.GetUserLogger(LOG_SECTION.Net.ToString(), _currentName);
            KafukaLog = Log.Instance.GetUserLogger(LOG_SECTION.Kafka.ToString(), _currentName);
            PDBLog = Log.Instance.GetUserLogger(LOG_SECTION.PDB.ToString(), _currentName);
            PDBErrLog = Log.Instance.GetUserLogger(LOG_SECTION.PDB_Error.ToString(), _currentName);
        }

        /// <summary>
        /// ini구성 변경
        /// </summary>
        /// <remarks>
        /// ini Setting 정보를 Class로 형태로 변환
        /// </remarks>
        public void SettingLoad()
        {
            var common = new IniConverter();
            try
            {
                //iWater 정보 읽기
                common.UseGenericMethod(IWATER, (Dictionary<string, IniValue>)_ini[SECTION_NAME.IWATER.ToString()]);
                //Kafka 정보 읽기
                common.UseGenericMethod(KAFKA, (Dictionary<string, IniValue>)_ini[SECTION_NAME.KAFKA.ToString()]);
                //이중화 정보 읽기
                common.UseGenericMethod(REDUNDANCY, (Dictionary<string, IniValue>)_ini[SECTION_NAME.REDUNDANCY.ToString()]);
                //Directory 정보 읽기
                common.UseGenericMethod(DIRECTORY, (Dictionary<string, IniValue>)_ini[SECTION_NAME.DIRECTORY.ToString()]);
                //Log 정보 읽기
                common.UseGenericMethod(LOG, (Dictionary<string, IniValue>)_ini[SECTION_NAME.LOG.ToString()]);

            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// iWater Setting 정보 Collection에 입력
        /// </summary>
        private void SetServerInfo()
        {
            try
            {
                ServerCollection.Clear();
                SettingServerCollection.Clear();
                string[] aInfo = IWATER.Local.Split(',');
                ServerModel local = new ServerModel(aInfo[0], aInfo[1], true);
                _localNoadName = local.NodeName;
                ServerCollection.Add(local);
                SettingServerCollection.Add(local);
                foreach (string s in IWATER.Remote.Split('|'))
                {
                    aInfo = s.Split(',');
                    if (aInfo.Length > 1)
                    {
                        ServerModel remote = new ServerModel(aInfo[0], aInfo[1]);
                        ServerCollection.Add(remote);
                        SettingServerCollection.Add(remote);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// iWater Setting Value에 대한 예외처리
        /// </summary>
        /// <returns>Setting Value 예외처리 결과 </returns>
        public SETTING_WARN ValueException()
        {
            var result = SETTING_WARN.NORMAL;

            if (SettingServerCollection.Count(t => string.IsNullOrEmpty(t.NodeName)) > 0)
            {
                result = SETTING_WARN.NODE_NULL;
            }
            return result;
        }
        /// <summary>
        /// Class정보를 ini Setting File로 저장
        /// </summary>
        public void SettingSave()
        {
            var common = new IniConverter();
            try
            {
                ServerCollection.Clear();
                foreach(var server in SettingServerCollection)
                {
                    ServerCollection.Add(server);
                }
                //Kafka 정보 저장
                Dictionary<string, IniValue> dicKafka = common.UseGenericMethod(KAFKA);
                //Directory 정보 저장
                Dictionary<string, IniValue> dicDirectory = common.UseGenericMethod(DIRECTORY);
                //이중화 정보 저장
                Dictionary<string, IniValue> dicRedundancy = common.UseGenericMethod(REDUNDANCY);
                //iWater 정보 저장
                IWATER.Local = ServerCollection.FirstOrDefault(t => t.IsLocal);
                IWATER.Remote = string.Join("|", ServerCollection.Where(t => t.IsLocal == false).Select(t => t.CSVFormat()).ToList());
                Dictionary<string, IniValue> diciWater = common.UseGenericMethod(IWATER);
                //ini 정보에 입력
                _ini[SECTION_NAME.KAFKA.ToString()] = dicKafka;
                _ini[SECTION_NAME.DIRECTORY.ToString()] = dicDirectory;
                _ini[SECTION_NAME.REDUNDANCY.ToString()] = dicRedundancy;
                _ini[SECTION_NAME.IWATER.ToString()] = diciWater;
                //ini 파일 저장
                _ini.Save(_settingPath);
                //iWater 정보 ReLoad
                SetServerInfo();
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// 이전 Setting 정보로 돌아가기
        /// </summary>
        /// <remarks>
        /// Setting 화면에서 저장취소를 선택한 경우 동작
        /// </remarks>
        public void RollbackServerSetting()
        {
            SettingServerCollection.Clear();
            foreach(var server in ServerCollection)
            {
                SettingServerCollection.Add(server);
            }
        }
    }
}
