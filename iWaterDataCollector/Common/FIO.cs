using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using System.IO;

namespace iWaterDataCollector.Common
{
    /// <summary>
    ///     File IO Class.
    /// </summary>  
    /// <remarks>
    ///     OpenFileDialog가 사용되는 Function 모음
    /// </remarks>
    public class FIO
    {
       
        /// <summary>
        /// 사용자 정의 로그 파일 만들기
        /// rname : 롤러 이름(로그파일prefix)
        /// path : 로그 파일이 저장될 폴더 경로
        /// level : 로그 레벨
        /// </summary>
        public ILog NewRollerSetup(string fileName, string rollerName, string path, string minLevel, string maxLevel, string pattern)
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.CreateRepository(rollerName);
            var tlogger = hierarchy.LoggerFactory.CreateLogger(hierarchy, rollerName);

            PatternLayout patternLayout = new PatternLayout
            {
                ConversionPattern = pattern
            };
            patternLayout.ActivateOptions();

            LevelRangeFilter levelRangeFilter = new LevelRangeFilter
            {
                AcceptOnMatch = true,
                LevelMin = GetLevel(minLevel),
                LevelMax = GetLevel(maxLevel)
            };

            RollingFileAppender roller = new RollingFileAppender();
            roller.File = Path.Combine(path, rollerName, fileName);
            roller.ImmediateFlush = true;
            roller.LockingModel = new FileAppender.MinimalLock();
            roller.AppendToFile = true;
            roller.DatePattern = "-yyyy-MM-dd-HH'.log'";
            roller.RollingStyle = RollingFileAppender.RollingMode.Composite;
            roller.AddFilter(levelRangeFilter);
            roller.Layout = patternLayout;
            roller.ActivateOptions();

            tlogger.Level = GetLevel(minLevel);
            tlogger.Hierarchy = hierarchy;
            tlogger.AddAppender(roller);
            tlogger.Repository.Configured = true;

            return new LogImpl(tlogger);
        }

        public Level GetLevel(string level)
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
