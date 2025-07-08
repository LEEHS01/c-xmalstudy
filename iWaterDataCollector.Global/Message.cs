using log4net;
using System;
using System.Runtime.CompilerServices;
/********************************************
 * Log 출력 Class
 ********************************************/
namespace iWaterDataCollector.Global
{
    public class Message
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="log">log Interface 변수 <see cref="ILog"/></param>
        /// <param name="name">호출한 Class Name </param>
        /// <param name="message">Log 문자열</param>
        /// <param name="memberName">호출한 Function Name <see cref="CallerMemberNameAttribute"/></param>
        public void Info(ILog log, string name, string message, [CallerMemberName] string memberName = "")
        {
            log.Info($"{name} | {memberName} | {message} ");
        }

        /// <summary>
        /// Error Level Log 입력
        /// </summary>
        /// <param name="log">log Interface 변수 <see cref="ILog"/></param>
        /// <param name="name">호출한 Class Name </param>
        /// <param name="message">Log 문자열</param>
        /// <param name="ex"><see cref="Exception"/> 정보</param>
        /// <param name="memberName">호출한 Function Name <see cref="CallerMemberNameAttribute"/></param>
        /// <param name="sourceLineNumber">호출한 SourceCode Line Number <see cref="CallerLineNumberAttribute"/></param>
        public void Error(ILog log, string name, string message, Exception ex = null, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            log.Error($"{name} | {memberName} | {message} [Line:{sourceLineNumber}]", ex);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="log">log Interface 변수 <see cref="ILog"/></param>
        /// <param name="name">호출한 Class Name </param>
        /// <param name="message">Log 문자열</param>
        /// <param name="memberName">호출한 Function Name <see cref="CallerMemberNameAttribute"/></param>
        /// <param name="sourceLineNumber">호출한 SourceCode Line Number <see cref="CallerLineNumberAttribute"/></param>
        public void Warn(ILog log, string name, string message, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            log.Warn($"{name} | {memberName} | {message} [Line:{sourceLineNumber}]");
        }

        /// <summary>
        /// Debug Level Log 입력
        /// </summary>
        /// <param name="log">log Interface 변수 <see cref="ILog"/></param>
        /// <param name="name">호출한 Class Name </param>
        /// <param name="message">Log 문자열</param>
        /// <param name="memberName">호출한 Function Name <see cref="CallerMemberNameAttribute"/></param>
        /// <param name="sourceLineNumber">호출한 SourceCode Line Number <see cref="CallerLineNumberAttribute"/></param>
        public void Debug(ILog log, string name, string message, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            log.Debug($"{name} | {memberName} | {message} [Line:{sourceLineNumber}]");
        }
    }
}
