/********************************************
 * 전역 Code 모음 (struct 및 Enum)
 ********************************************/
namespace iWaterDataCollector.Global
{
    public static class Code
    {
        public static string INFO = "Info";
        public static string RECOVERY = "Recovery";
        public static string BACKUP = "FileBackup";
    }
    /// <summary>
    /// ini File Section 이름
    /// </summary>
    public enum SECTION_NAME
    {
        KAFKA,
        REDUNDANCY,
        IWATER,
        DIRECTORY,
        LOG,
        COMMON
    }
    /// <summary>
    /// 이중화 상태
    /// </summary>
    public enum REDUNDANCY_STATE
    {
        None,
        Active,
        StandBy,
        Alone
    }
    /// <summary>
    /// Log Section 이름
    /// </summary>
    public enum LOG_SECTION
    {
        Program,
        Net,
        Historian,
        Kafka,
        PDB,
        PDB_Error,
        Error
    }
    /// <summary>
    /// Log Level
    /// </summary>
    public enum LOG_LEVEL
    {
        All,
        Debug,
        Info,
        Warn,
        Error,
        Fatal,
        Off
    }
    /// <summary>
    /// Network 에러Code
    /// </summary>
    public enum NET_CODE
    {
        OK,
        CLOSING,
        ERROR
    }
    /// <summary>
    /// DataGrid Unique Eroor Code
    /// </summary>
    public enum GRID_WARN
    {
        NORMAL,
        NULL,
        UNIQUE,
        UNIQUE_CHECK
    }
    /// <summary>
    /// SCADA Setting Error Code
    /// </summary>
    public enum SETTING_WARN
    {
        NORMAL,
        NODE_NULL
    }
    /// <summary>
    /// iWater Event Code
    /// </summary>
    public enum EVENT_CODE
    {
        Normal,
        Action,
        Fail,
        Error,
        Disconnect,
        Recovery
    }
}
