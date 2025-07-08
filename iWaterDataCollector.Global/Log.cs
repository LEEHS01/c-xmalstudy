using iWaterDataCollector.INI;
using iWaterDataCollector.Model.INI;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
/********************************************
 * Log 설정(ini) 파일 생성 및 설정
 ********************************************/
namespace iWaterDataCollector.Global
{
    public class Log
    {
        #region 싱글톤
        private static Log _Instance = null;
        public static Log Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new Log();
                }

                return _Instance;
            }
        }
        private Log()
        {
            _dicHierarchy = new Dictionary<string, Hierarchy>();
            _dicRoller = new Dictionary<string, RollingFileAppender>();
        }
        #endregion
        /// <summary>
        /// ini Setting Class Data
        /// </summary>
        private readonly IniFile _ini = new IniFile();
        /// <summary>
        /// 프로그램 내 Log(Hierarchy) Dictionary
        /// </summary>
        private Dictionary<string, Hierarchy> _dicHierarchy;
        /// <summary>
        /// Appender 정보
        /// </summary>
        private Dictionary<string, RollingFileAppender> _dicRoller;
        /// <summary>
        /// Log Section Name Array
        /// </summary>
        private string[] _sections => Enum.GetNames(typeof(LOG_SECTION));
        /// <summary>
        /// Setting INI Load & Log 초기화
        /// </summary>
        /// <param name="path">Log 설정(ini) 파일 경로</param>
        public void Initialize(string path)
        {
            //_ = Setting.Instance.LOG;
            //파일 유무 확인 - Exists False
            if (File.Exists(path) == false)
            {
                CreateLogINI(path);
            }
            else
            {
                ReLoadLogINI(path);
            }
        }
        /// <summary>
        /// Log 설정(ini)파일 수정
        /// </summary>
        /// <remarks>
        /// SCADA NodeName 변경 시 Log Section 변경을 위한 함수
        /// </remarks>
        /// <param name="path">Log 설정(ini) 파일 경로</param>
        public void UpdateIWaterNode(string path)
        {
            var logPath = $"{AppDomain.CurrentDomain.BaseDirectory}{path}";
            //var common = new Converter();
            //새로 저장된 NodeName을 불러옴
            foreach (var node in AppData.Instance.ServerCollection.Select(t => t.NodeName))
            {
                //ini에 설정되어 있는 이름이 아니라면 새로 생성
                //var logModel = new LogModel();
                if (_ini.Keys.Contains(node) == false)
                {
                    var filePath = Path.Combine(AppData.Instance.LOG.Path, LOG_SECTION.Historian.ToString(), node);
#if DEBUG
                    CreateINISection(node, filePath);
#else
                    CreateINISection(node, filePath, LOG_LEVEL.Info);
#endif
                }
            }

            //ini 파일 저장
            _ini.Save(logPath);
        }
        /// <summary>
        /// Log 설정(ini) 파일 생성
        /// </summary>
        /// <param name="path">Log 설정(ini) 파일 경로</param>
        private void CreateLogINI(string path)
        {
            var logPath = $"{AppDomain.CurrentDomain.BaseDirectory}{path}";
            var filePath = string.Empty;
            //Section별 ini설정값 등록

            foreach (var section in Enum.GetValues(typeof(LOG_SECTION)))
            {
                //경로 재설정 --> Log마다 저장되는 위치가 다름
                filePath = Path.Combine(AppData.Instance.LOG.Path, section.ToString());

                switch (section)
                {
                    case LOG_SECTION.Historian:
                        //Historian의 경우 Node별로 Log 등록
                        foreach (var node in AppData.Instance.ServerCollection.Select(t => t.NodeName))
                        {
                            //경로 재설정 --> Log마다 저장되는 위치가 다름
                            filePath = Path.Combine(AppData.Instance.LOG.Path, section.ToString(), node);
#if DEBUG
                            CreateINISection(node, filePath);
#else
                        //Relese의 경우 Log Level을 Info로 함
                        CreateINISection(node, filePath, LOG_LEVEL.Info);
#endif
                        }

                        break;
                    case LOG_SECTION.PDB:   //PDB_Error도 같은 위치에 저장
                        CreateINISection(section.ToString(), filePath);
                        CreateINISection(LOG_SECTION.PDB_Error.ToString(), filePath);
                        break;
                    case LOG_SECTION.PDB_Error:
                        break;
                    case LOG_SECTION.Kafka:
                        CreateINISection(section.ToString(), filePath);
                        break;
                    case LOG_SECTION.Error:
                        //Error Log의 경우 Min/Max Level을 Error에 맞도록 설정
                        CreateINISection(section.ToString(), filePath, LOG_LEVEL.Error, LOG_LEVEL.Fatal);
                        break;

                    default:
#if DEBUG
                        CreateINISection(section.ToString(), filePath);
#else
                    //Relese의 경우 Log Level을 Info로 함
                        CreateINISection(section.ToString(), filePath, LOG_LEVEL.Info);
#endif
                        break;
                }
            }
            //ini 파일 저장
            _ini.Save(logPath);
        }
        /// <summary>
        /// ini Log 설정을 다시 가져옴
        /// </summary>
        /// <remarks>
        /// 프로그램 구동 시 Log Ini 파일을 Load해오는 기능
        /// </remarks>
        /// <param name="path">Log 설정(ini) 파일 경로</param>
        private void ReLoadLogINI(string path)
        {
            var common = new IniConverter();
            var logPath = $"{AppDomain.CurrentDomain.BaseDirectory}{path}";
            // ini 읽기
            _ini.Load(logPath);
            
            //현 시스템의 NodeName정보 확인
            var currentKeys = AppData.Instance.ServerCollection.Select(t => t.NodeName).ToList();
            //Log Ini Section명과 NodeName을 합한 List 생성(현 Log Section확인용)
            currentKeys.AddRange(_sections);
            //Section에 남아 있는 이전 Section정보 삭제 (정보일치를 위한 작업)
            var nonSection = _ini.Keys.ToList().Except(currentKeys);
            foreach (var key in nonSection)
            {
                _ini[key].Clear();
            }

            //ini 설정정보를 Class로 변경
            foreach (var section in _sections)
            {
                var logModel = new LogModel();
                //Historian Section이 아니면
                if (section.Equals(LOG_SECTION.Historian.ToString()) == false)
                {
                    //ini Section과 일치하는 Class로 변경
                    common.UseGenericMethod(logModel, (Dictionary<string, IniValue>)_ini[section]);
                    NewRollerSetup(logModel, logModel.Path, section);
                }
                else
                {
                    //NodeName별 Section확인
                    foreach (var node in AppData.Instance.ServerCollection.Select(t => t.NodeName))
                    {
                        //ini Key에 이미 등록된 경우
                        if (_ini.Keys.Contains(node))
                        {
                            //ini Section과 일치하는 Class로 변경
                            common.UseGenericMethod(logModel, (Dictionary<string, IniValue>)_ini[node]);
                            NewRollerSetup(logModel, logModel.Path, node);
                        }
                        else
                        {
                            //ini에 정보에 추가
                            var filePath = Path.Combine(AppData.Instance.LOG.Path, section, node);
#if DEBUG
                            CreateINISection(node, filePath);
#else
                            CreateINISection(node, filePath, LOG_LEVEL.Info);
#endif

                        }
                    }
                }
            }
            //ini 파일 저장
            _ini.Save(path);
        }
        /// <summary>
        /// ini Section 추가
        /// </summary>
        /// <remarks>
        /// Log용 ini Section정보 등록
        /// </remarks>
        /// <param name="name">section명</param>
        /// <param name="path">Log파일경로</param>
        /// <param name="min">Log Min Level</param>
        /// <param name="max">Log Max Level</param>
        private void CreateINISection(string name, string path, LOG_LEVEL min = LOG_LEVEL.Debug, LOG_LEVEL max = LOG_LEVEL.Info)
        {
            var common = new IniConverter();
            var value = common.UseGenericMethod(AppData.Instance.LOG);
            value["Path"] = path;
            value["MinLevel"] = min.ToString();
            value["MaxLevel"] = max.ToString();
            _ini.Add(name, value);
            NewRollerSetup(AppData.Instance.LOG, path, name);
        }
        /// <summary>
        /// 사용자 정의 로그 파일 만들기
        /// </summary>
        /// <remarks>
        /// log4net에서 사용하는 xml을 내부Class로 만들어 두는 기능
        /// </remarks>
        /// <param name="model">log 정보 class</param>
        /// <param name="path">저장 경로</param>
        /// <param name="name">hierarchy 이름</param>
        public void NewRollerSetup(LogModel model, string path, string name)
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.CreateRepository(name);
            PatternLayout patternLayout = new PatternLayout
            {
                ConversionPattern = model.Pattern
            };
            patternLayout.ActivateOptions();

            LevelRangeFilter levelRangeFilter = new LevelRangeFilter
            {
                AcceptOnMatch = true,
                LevelMin = ConvertertoLevel(model.MinLevel),
                LevelMax = ConvertertoLevel(model.MaxLevel)
            };

            RollingFileAppender roller = new RollingFileAppender
            {
                /*
                 * log파일 명을 폴더명과 일치 [2023-11-09]
                 */
                //File = Path.Combine(path, model.FileName),
                File = Path.Combine(path, name),
                ImmediateFlush = true,
                LockingModel = new FileAppender.MinimalLock(),
                AppendToFile = true,
                MaxSizeRollBackups = 100,
                DatePattern = "-yyyy-MM-dd-HH'.log'",
                RollingStyle = RollingFileAppender.RollingMode.Composite
            };

            roller.AddFilter(levelRangeFilter);
            roller.Layout = patternLayout;
            roller.ActivateOptions();

            _dicRoller.Add(name, roller);
            _dicHierarchy.Add(name, hierarchy);
        }
        /// <summary>
        /// Logger 생성 및 추출
        /// </summary>
        /// <param name="repository">hierarchy 이름</param>
        /// <param name="name">logger 이름</param>
        /// <returns>log4net.<see cref="ILog" /> 사용자 Logger </returns>
        public ILog GetUserLogger(string repository, string name)
        {
            var level = _ini[repository]["MinLevel"].ToString();
            var hierarchy = _dicHierarchy[repository];
            var roller = _dicRoller[repository];
            var tlogger = hierarchy.LoggerFactory.CreateLogger(hierarchy, name);
            tlogger.Level = ConvertertoLevel(level);
            tlogger.Hierarchy = hierarchy;
            tlogger.AddAppender(roller);
            tlogger.Repository.Configured = true;

            return new LogImpl(tlogger);
        }
        /// <summary>
        /// String to <see cref="Level" />
        /// </summary>
        /// <param name="level">level string</param>
        /// <returns>Logger <see cref="Level" /></returns>
        public Level ConvertertoLevel(string level)
        {
            Level rtnLevel = Level.Debug;
            switch (level)
            {
                case "All":
                    rtnLevel = Level.All;
                    break;
                case "Debug":
                    rtnLevel = Level.Debug;
                    break;
                case "Info":
                    rtnLevel = Level.Info;
                    break;
                case "Warn":
                    rtnLevel = Level.Warn;
                    break;
                case "Error":
                    rtnLevel = Level.Error;
                    break;
                case "Fatal":
                    rtnLevel = Level.Fatal;
                    break;
                default:
                    break;
            }
            return rtnLevel;
        }
    }
}
