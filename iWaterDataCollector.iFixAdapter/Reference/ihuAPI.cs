///
/// Historian UserAPI C# Wrapper
/// (C) COPYRIGHT 2011 GE Intelligent Platforms, Inc. All rights reserved.
/// 
/// Refer to IHUAPI.h header and online documentation for usage instructions.
///
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Proficy.Historian.UserAPI;

namespace Proficy.Historian.UserAPI
{
  public enum ihuErrorCode
  {
    OK = 0,
    FAILED = 100,
    API_TIMEOUT = 101,
    NOT_CONNECTED = 102,
    INTERFACE_NOT_FOUND = 103,
    NOT_SUPPORTED = 104,
    DUPLICATE_DATA = 105,
    NOT_VALID_USER = 106,
    ACCESS_DENIED = 107,
    WRITE_IN_FUTURE = 108,
    WRITE_ARCH_OFFLINE = 109,
    ARCH_READONLY = 110,
    WRITE_OUTSIDE_ACTIVE = 111,
    WRITE_NO_ARCH_AVAIL = 112,
    INVALID_TAGNAME = 113,
    LIC_TOO_MANY_TAGS = 114,
    LIC_TOO_MANY_USERS = 115,
    LIC_INVALID_LIC_DLL = 116,
    NO_VALUE = 117,
    NOT_LICENSED = 118,
    CALC_CIRC_REFERENCE = 119,
    DUPLICATE_INTERFACE = 120,
    BACKUP_EXCEEDED_SPACE = 121,
    INVALID_SERVER_VERSION = 122,
    DATA_RETRIEVAL_COUNT_EXCEEDED = 123,
    INVALID_PARAMETER = 124,
    MAX_ERROR_NUM = 124
  }

  public enum ihuDataType
  {
    Undefined = 0,
    Short,
    Integer,
    Float,
    DoubleFloat,
    String,
    Scaled,
    MaxDataType
  }

  public enum ihuQualityStatus
  {
    OPCBad = 0,
    OPCUncertain,
    OPCNA,
    OPCGood,
  }

  public enum ihuQualitySubStatus
  {
    OPCNonspecific = 0,
    OPCConfigurationError,
    OPCNotConnected,
    OPCDeviceFailure,
    OPCSensorFailure,
    OPCLastKnownValue,
    OPCCommFailure,
    OPCOutOfService,
    ScaledOutOfRange,
    OffLine,
    NoValue,
    CalculationError,
    ConditionCollectionHalted,
    CalculationTimeout,
  }

  public enum ihuTagProperties
  {
    Tagname = 1,
    Description,
    EngineeringUnits,
    Comment,
    DataType,
    FixedStringLength,
    InterfaceName,
    SourceAddress,
    CollectionType,
    CollectionInterval,
    CollectionOffset,
    LoadBalancing,
    SpikeLogic,
    SpikeLogicOverride,
    TimeStampType,
    HighEngineeringUnits,
    LowEngineeringUnits,
    InputScaling,
    HighScale,
    ihutagpropLowScale,
    CollectorCompression,
    CollectorDeadbandPercentRange,
    ArchiveCompression,
    ArchiveDeadbandPercentRange,
    Spare1,
    Spare2,
    Spare3,
    Spare4,
    Spare5,
    ReadSecurityGroup,
    WriteSecurityGroup,
    AdministratorSecurityGroup,
    LastModified,
    LastModifiedUser,
    InterfaceType,
    StoreMilliseconds,
    UTCBias,
    NumberOfCalculationDependencies,
    CalculationDependencies,
    AverageCollectionTime,
    CollectionDisabled,
    ArchiveCompressionTimeout,
    CollectorCompressionTimeout,
    InterfaceAbsoluteDeadbanding,
    InterfaceAbsoluteDeadband,
    ArchiveAbsoluteDeadbanding,
    ArchiveAbsoluteDeadband,
    StepValue,
    TimeResolution,
    ConditionCollectionEnabled,
    ConditionCollectionTriggerTag,
    ConditionCollectionComparison,
    ConditionCollectionCompareValue,
    ConditionCollectionMarkers,
    Id,
    MaxPropertyNum
  }

  public enum ihuSamplingMode
  {
    Undefined = 0,
    Reserved,
    Interpolated,
    Trend,
    Lab,
    InterpolatedtoRaw,
    TrendtoRaw,
    LabtoRaw,
    RawByTime,
    MaxSamplingMode
  }

  public enum ihuCalculationMode
  {
    Undefined = 0,
    Average,
    StandardDeviation,
    Total,
    Minimum,
    Maximum,
    Count,
    RawAverage,
    RawStandardDeviation,
    RawTotal,
    MinimumTime,
    MaximumTime,
    TimeGood,
    MaxCalculationMode
  }

  public enum ihuIntervalType
  {
    IntervalTypeUndefined = 0,
    IntervalWeek,
    IntervalDay,
    IntervalHour,
    IntervalMinute,
    IntervalSecond,
    IntervalMillisecond,
    IntervalMicrosecond
  }

  [StructLayout(LayoutKind.Explicit, Pack = 1)]
  public struct ihuValue
  {
    [FieldOffset(0)]
    public short Short;
    [FieldOffset(0)]
    public int Integer;
    [FieldOffset(0)]
    public float Float;
    [FieldOffset(0)]
    public double DoubleFloat;
    [FieldOffset(0)]
    IntPtr StringPtr;

    public void Dispose()
    {
      Marshal.FreeCoTaskMem(StringPtr);
      StringPtr = IntPtr.Zero;
    }

    public string StringValue
    {
      get
      {
        return Marshal.PtrToStringAnsi(StringPtr);
      }
      set
      {
        Marshal.FreeCoTaskMem(StringPtr);
        StringPtr = Marshal.StringToCoTaskMemAnsi(value);
      }
    }

    public object AsObject(ihuDataType datatype)
    {
      switch (datatype)
      {
        case ihuDataType.Short:
          return Short;
        case ihuDataType.Integer:
          return Integer;
        case ihuDataType.Float:
          return Float;
        case ihuDataType.DoubleFloat:
          return DoubleFloat;
        case ihuDataType.String:
          return StringValue;
        default:
          throw new ArgumentException("Unsupported ValueDataType");
      }
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct IHU_RAW_QUALITY
  {
    [MarshalAs(UnmanagedType.I1)]
    public bool Deleted;
    [MarshalAs(UnmanagedType.I1)]
    public char Replaced;
    public ihuQualityStatus QualityStatus;
    public ihuQualitySubStatus QualitySubStatus;

    public override string ToString()
    {
      if (QualityStatus == ihuQualityStatus.OPCGood)
        return String.Format("{0}", QualityStatus);
      else
        return String.Format("{0}.{1}", QualityStatus, QualitySubStatus);
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct IHU_COMMENT
  {
    public IHU_TIMESTAMP StoredOnTimeStamp;	// Stored time.  (Set by archiver)
    public IHU_TIMESTAMP CommentTimeStamp;	// Timestamp 
    public string SuppliedUsername;         // Supplied username (optionally given in ihCommentAdd)
    public string Username;			            // OS user name of writer.  (Set by archiver)
    public string CommentString;

    public override string ToString()
    {
      return String.Format("{0}.{1}: {2} by {3}", StoredOnTimeStamp.Seconds, StoredOnTimeStamp.Subseconds, CommentString, Username);
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  struct IHU_COMMENT_ARRAY : IDisposable
  {
    int NumberOfComments;
    IntPtr CommentArray;

    public void Dispose()
    {
      if (CommentArray == IntPtr.Zero)
        return;

      IntPtr ptr = CommentArray;
      for (int i = 0; i < NumberOfComments; i++)
      {
        Marshal.DestroyStructure(ptr, typeof(IHU_COMMENT));
        ptr = IntPtr.Add(ptr, Marshal.SizeOf(typeof(IHU_COMMENT)));
      }
      Marshal.FreeCoTaskMem(CommentArray);
      CommentArray = IntPtr.Zero;
    }

    internal IHU_COMMENT[] Comments
    {
      get
      {
        if (CommentArray == IntPtr.Zero || NumberOfComments == 0)
          return null;

        IHU_COMMENT[] comments = new IHU_COMMENT[NumberOfComments];
        IntPtr ptr = CommentArray;
        for (int i = 0; i < NumberOfComments; i++)
        {
          comments[i] = (IHU_COMMENT)Marshal.PtrToStructure(ptr, typeof(IHU_COMMENT));
          ptr = IntPtr.Add(ptr, Marshal.SizeOf(comments[i]));
        }
        return comments;
      }
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct IHU_DATA_SAMPLE : IDisposable
  {
    public IHU_TIMESTAMP TimeStamp;
    public string Tagname;
    public ihuDataType ValueDataType;
    public ihuValue Value;
    public IHU_RAW_QUALITY Quality;
    IntPtr CommentsPtr;

    public void Dispose()
    {
      if (ValueDataType == ihuDataType.String)
        Value.Dispose();

      if (CommentsPtr != IntPtr.Zero)
      {
        IHU_COMMENT_ARRAY array = (IHU_COMMENT_ARRAY)Marshal.PtrToStructure(CommentsPtr, typeof(IHU_COMMENT_ARRAY));
        array.Dispose();
        Marshal.FreeCoTaskMem(CommentsPtr);
        CommentsPtr = IntPtr.Zero;
      }
    }

    public object ValueObject
    {
      get
      {
        return Value.AsObject(ValueDataType);
      }
      set
      {
        Type type = value.GetType();
        if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(short))
        {
          ValueDataType = ihuDataType.Short;
          Value.Short = (short)value;
        }
        else if (type == typeof(ushort) || type == typeof(int))
        {
          ValueDataType = ihuDataType.Integer;
          Value.Integer = (int)value;
        }
        else if (type == typeof(float))
        {
          ValueDataType = ihuDataType.Float;
          Value.Float = (float)value;
        }
        else if (type == typeof(double))
        {
          ValueDataType = ihuDataType.DoubleFloat;
          Value.DoubleFloat = (double)value;
        }
        else if (type == typeof(string))
        {
          ValueDataType = ihuDataType.String;
          Value.StringValue = value.ToString();
        }
        else
        {
          throw new ArgumentException("Unsupported type " + value.GetType());
        }
      }
    }

    public IHU_COMMENT[] Comments
    {
      get
      {
        if (CommentsPtr == IntPtr.Zero)
          return null;
        IHU_COMMENT_ARRAY array = (IHU_COMMENT_ARRAY)Marshal.PtrToStructure(CommentsPtr, typeof(IHU_COMMENT_ARRAY));
        return array.Comments;
      }
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct IHU_TIMESTAMP
  {
    public int Seconds;		    // seconds since Jan 1, 1970
    public int Subseconds;		// nanoseconds, fractional part of a second

    public const int SubsecondsPerMillisecond = 1000000;

    static DateTime EpochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    const int SubsecondsPerTick = SubsecondsPerMillisecond / (int)TimeSpan.TicksPerMillisecond;

    public IHU_TIMESTAMP(DateTime time)
    {
      IHUAPI.IHU_TIMESTAMP_FromParts(time, out this);
    }

    public DateTime ToDateTime()
    {
      return EpochStart.AddSeconds(Seconds).AddTicks(Subseconds / SubsecondsPerTick);
    }

    public override string ToString()
    {
      return String.Format("{0}.{1:000000000}", Seconds, Subseconds);
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct IHU_DATA_INTERVAL
  {
    public uint Interval;
    public ihuIntervalType IntervalType;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  struct IHU_RETRIEVED_DATA_RECORDS : IDisposable
  {
    int NumberOfTags;
    IntPtr ErrorsPtr;
    IntPtr TagValuePtr;

    public IHU_RETRIEVED_DATA_RECORDS(string[] tags)
    {
      NumberOfTags = tags.Length;
      ErrorsPtr = Marshal.AllocCoTaskMem(tags.Length * Marshal.SizeOf(typeof(int)));

      int size = Marshal.SizeOf(typeof(IHU_RETRIEVED_DATA_VALUES));
      TagValuePtr = Marshal.AllocCoTaskMem(tags.Length * size);

      byte[] zeroBuffer = new byte[size];
      IntPtr ptr = TagValuePtr;
      for (int i = 0; i < tags.Length; i++)
      {
        Marshal.Copy(zeroBuffer, 0, ptr, size);
        Marshal.WriteIntPtr(ptr, Marshal.StringToCoTaskMemAnsi(tags[i]));
        ptr = IntPtr.Add(ptr, size);
      }
    }

    public void Dispose()
    {
      Marshal.FreeCoTaskMem(ErrorsPtr);
      ErrorsPtr = IntPtr.Zero;

      Marshal.FreeCoTaskMem(TagValuePtr);
      TagValuePtr = IntPtr.Zero;
    }

    public ihuErrorCode[] ErrorResults
    {
      get
      {
        int[] errors = new int[NumberOfTags];
        Marshal.Copy(ErrorsPtr, errors, 0, NumberOfTags);
        return Array.ConvertAll(errors, value => (ihuErrorCode)value);
      }
    }

    public IHU_RETRIEVED_DATA_VALUES[] TagValues
    {
      get
      {
        IHU_RETRIEVED_DATA_VALUES[] values = new IHU_RETRIEVED_DATA_VALUES[NumberOfTags];
        IntPtr ptr = TagValuePtr;
        for (int i = 0; i < NumberOfTags; i++)
        {
          values[i] = (IHU_RETRIEVED_DATA_VALUES)Marshal.PtrToStructure(ptr, typeof(IHU_RETRIEVED_DATA_VALUES));
          Marshal.DestroyStructure(ptr, typeof(IHU_RETRIEVED_DATA_VALUES));
          ptr = IntPtr.Add(ptr, Marshal.SizeOf(values[i]));
        }
        return values;
      }
    }

    public IHU_RETRIEVED_DATA_VALUES_EX[] TagValuesEx
    {
      get
      {
        IHU_RETRIEVED_DATA_VALUES_EX[] values = new IHU_RETRIEVED_DATA_VALUES_EX[NumberOfTags];
        IntPtr ptr = TagValuePtr;
        for (int i = 0; i < NumberOfTags; i++)
        {
          values[i] = (IHU_RETRIEVED_DATA_VALUES_EX)Marshal.PtrToStructure(ptr, typeof(IHU_RETRIEVED_DATA_VALUES_EX));
          Marshal.DestroyStructure(ptr, typeof(IHU_RETRIEVED_DATA_VALUES_EX));
          ptr = IntPtr.Add(ptr, Marshal.SizeOf(values[i]));
        }
        return values;
      }
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct IHU_RETRIEVED_DATA_VALUES : IDisposable
  {
    public string Tagname;
    public ihuDataType ValueDataType;
    public int NumberOfValues;          // number of items in the arrays that follow
    IntPtr TimeStampPtr;                // allocated array of timestamps of samples
    IntPtr DataValuePtr;                // allocated array of values for the timestamps
    IntPtr PercentGoodPtr;              // allocated array of qualities for the timestamps
    IntPtr CommentPtr;                  // null if no comments

    public void Dispose()
    {
      Dispose(NumberOfValues, ValueDataType, ref TimeStampPtr, ref DataValuePtr, ref PercentGoodPtr, ref CommentPtr);
    }

    public IHU_TIMESTAMP[] TimeStamps
    {
      get
      {
        return TimeStampsFromPtr(TimeStampPtr, NumberOfValues);
      }
    }

    public ihuValue[] DataValues
    {
      get
      {
        return DataValuesFromPtr(DataValuePtr, NumberOfValues);
      }
    }

    public float[] PercentGoods
    {
      get
      {
        return PercentGoodsFromPtr(PercentGoodPtr, NumberOfValues);
      }
    }

    public IHU_COMMENT[][] Comments
    {
      get
      {
        return CommentsFromPtr(CommentPtr, NumberOfValues);
      }
    }

    internal static void Dispose(int num, ihuDataType datatype, ref IntPtr times, ref IntPtr data, ref IntPtr percents, ref IntPtr comments)
    {
      Marshal.FreeCoTaskMem(times);
      times = IntPtr.Zero;

      if (datatype == ihuDataType.String)
      {
        IntPtr ptr = data;
        for (int i = 0; i < num; i++)
        {
          ihuValue value = (ihuValue)Marshal.PtrToStructure(ptr, typeof(ihuValue));
          value.Dispose();
          ptr = IntPtr.Add(ptr, Marshal.SizeOf(value));
        }
      }
      Marshal.FreeCoTaskMem(data);
      data = IntPtr.Zero;

      Marshal.FreeCoTaskMem(percents);
      percents = IntPtr.Zero;

      if (comments != IntPtr.Zero)
      {
        IntPtr ptr = comments;
        for (int i = 0; i < num; i++)
        {
          IHU_COMMENT_ARRAY array = (IHU_COMMENT_ARRAY)Marshal.PtrToStructure(ptr, typeof(IHU_COMMENT_ARRAY));
          array.Dispose();
          ptr = IntPtr.Add(ptr, Marshal.SizeOf(array));
        }
        Marshal.FreeCoTaskMem(comments);
        comments = IntPtr.Zero;
      }
    }

    internal static IHU_TIMESTAMP[] TimeStampsFromPtr(IntPtr sourcePtr, int num)
    {
      IHU_TIMESTAMP[] times = new IHU_TIMESTAMP[num];
      IntPtr ptr = sourcePtr;
      for (int i = 0; i < times.Length; i++)
      {
        times[i] = (IHU_TIMESTAMP)Marshal.PtrToStructure(ptr, typeof(IHU_TIMESTAMP));
        ptr = IntPtr.Add(ptr, Marshal.SizeOf(times[i]));
      }
      return times;
    }

    internal static ihuValue[] DataValuesFromPtr(IntPtr sourcePtr, int num)
    {
      ihuValue[] values = new ihuValue[num];
      IntPtr ptr = sourcePtr;
      for (int i = 0; i < values.Length; i++)
      {
        values[i] = (ihuValue)Marshal.PtrToStructure(ptr, typeof(ihuValue));
        ptr = IntPtr.Add(ptr, Marshal.SizeOf(values[i]));
      }
      return values;
    }

    internal static float[] PercentGoodsFromPtr(IntPtr sourcePtr, int num)
    {
      float[] percents = new float[num];
      if (num > 0)
        Marshal.Copy(sourcePtr, percents, 0, num);
      return percents;
    }

    internal static IHU_COMMENT[][] CommentsFromPtr(IntPtr sourcePtr, int num)
    {
      if (sourcePtr == IntPtr.Zero)
        return null;

      IHU_COMMENT[][] comments = new IHU_COMMENT[num][];

      IntPtr ptr = sourcePtr;
      for (int i = 0; i < num; i++)
      {
        IHU_COMMENT_ARRAY array = (IHU_COMMENT_ARRAY)Marshal.PtrToStructure(ptr, typeof(IHU_COMMENT_ARRAY));
        comments[i] = array.Comments;
        ptr = IntPtr.Add(ptr, Marshal.SizeOf(array));
      }
      return comments;
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct IHU_RETRIEVED_DATA_VALUES_EX : IDisposable
  {
    public string Tagname;
    public ihuDataType ValueDataType;
    public int NumberOfValues;          // number of items in the arrays that follow
    IntPtr TimeStampPtr;   // allocated array of timestamps of samples
    IntPtr DataValuePtr;            // allocated array of values for the timestamps
    IntPtr PercentGoodPtr;   // allocated array of qualities for the timestamps
    IntPtr CommentPtr;           //null if no comments
    public ihuSamplingMode SamplingMode;
    public ihuCalculationMode CalculationMode;

    public void Dispose()
    {
      IHU_RETRIEVED_DATA_VALUES.Dispose(NumberOfValues, ValueDataType, ref TimeStampPtr, ref DataValuePtr, ref PercentGoodPtr, ref CommentPtr);
    }

    public IHU_TIMESTAMP[] TimeStamps
    {
      get
      {
        return IHU_RETRIEVED_DATA_VALUES.TimeStampsFromPtr(TimeStampPtr, NumberOfValues);
      }
    }

    public ihuValue[] DataValues
    {
      get
      {
        return IHU_RETRIEVED_DATA_VALUES.DataValuesFromPtr(DataValuePtr, NumberOfValues);
      }
    }

    public float[] PercentGoods
    {
      get
      {
        return IHU_RETRIEVED_DATA_VALUES.PercentGoodsFromPtr(PercentGoodPtr, NumberOfValues);
      }
    }

    public IHU_COMMENT[][] Comments
    {
      get
      {
        return IHU_RETRIEVED_DATA_VALUES.CommentsFromPtr(CommentPtr, NumberOfValues);
      }
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct IHU_RETRIEVED_RAW_VALUES : IDisposable
  {
    public string Tagname;
    public ihuDataType ValueDataType;
    public uint NumberOfValues;          // number of items in the arrays that follow
    IntPtr ValuesPtr;                    // allocated array of values for the timestamps

    public IHU_RETRIEVED_RAW_VALUES(string tagname)
      : this()
    {
      Tagname = tagname;
    }

    public void Dispose()
    {
      if (ValuesPtr == IntPtr.Zero)
        return;

      IntPtr ptr = ValuesPtr;
      for (int i = 0; i < NumberOfValues; i++)
      {
        IHU_DATA_SAMPLE value = (IHU_DATA_SAMPLE)Marshal.PtrToStructure(ptr, typeof(IHU_DATA_SAMPLE));
        value.Dispose();
        Marshal.DestroyStructure(ptr, typeof(IHU_DATA_SAMPLE));
        ptr = IntPtr.Add(ptr, Marshal.SizeOf(value));
      }

      Marshal.FreeCoTaskMem(ValuesPtr);
      ValuesPtr = IntPtr.Zero;
    }

    public IHU_DATA_SAMPLE[] Values
    {
      get
      {
        IHU_DATA_SAMPLE[] values = new IHU_DATA_SAMPLE[NumberOfValues];
        IntPtr ptr = ValuesPtr;
        for (int i = 0; i < values.Length; i++)
        {
          values[i] = (IHU_DATA_SAMPLE)Marshal.PtrToStructure(ptr, typeof(IHU_DATA_SAMPLE));
          ptr = IntPtr.Add(ptr, Marshal.SizeOf(values[i]));
        }
        return values;
      }
    }
  }

  public enum ihuCollectorType
  {
	  Undefined = 0,
	  iFix,
	  Simulation,
	  OPC,
	  File,
	  iFixLabData,
	  ManualEntry,
	  Other,
	  Calculation,
	  ServerToServer,
	  PI,
	  OPCAE,
	  CimplicityPE,
	  PIDistributor,
    CimplicityME,
    MaxCollectorType
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct IHU_COLLECTOR
  {
    public string CollectorName;
    public ihuCollectorType CollectorType;

    public override string ToString()
    {
      return String.Format("{0} [{1}]", CollectorName, CollectorType);
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct IHU_CONNECTION_PARAMETERS
  {
	  int Size;
	  int TCPConnectionWindow;

    public IHU_CONNECTION_PARAMETERS(int tcpConnectionWindow)
      : this()
    {
      Size = Marshal.SizeOf(Marshal.SizeOf(this));
      TCPConnectionWindow = tcpConnectionWindow;
    }
  }

  public class IHUAPI
  {
    public const string ARCHIVER_PROP_CREATEOFFLINEARCHIVES = "CreateOfflineArchives";
    public const string ARCHIVER_PROP_CONFIGSERIALNUMBER = "ConfigurationSerialNumber";
    public const string ARCHIVER_PROP_ACTIVEHOURS = "ActiveHours";

    static bool Is32bit
    {
      get
      {
        return IntPtr.Size == 4;
      }
    }

    static IntPtr AllocDataSamples(string[] tags, IHU_TIMESTAMP[] times)
    {
      int size = Marshal.SizeOf(typeof(IHU_DATA_SAMPLE));
      IntPtr data = Marshal.AllocCoTaskMem(tags.Length * size);
      byte[] zeroBuffer = new byte[size];

      IntPtr ptr = data;
      for (int i = 0; i < tags.Length; i++)
      {
        Marshal.Copy(zeroBuffer, 0, ptr, size);
        if (times != null)
          Marshal.WriteInt32(ptr, times[i].Seconds);
        ptr = IntPtr.Add(ptr, 4);
        if (times != null)
          Marshal.WriteInt32(ptr, times[i].Subseconds);
        ptr = IntPtr.Add(ptr, 4);
        Marshal.WriteIntPtr(ptr, Marshal.StringToCoTaskMemAnsi(tags[i]));
        ptr = IntPtr.Add(ptr, size - 8);
      }
      return data;
    }

    static IHU_DATA_SAMPLE[] DataSamplesFromPtr(IntPtr data, int number)
    {
      if (data == IntPtr.Zero)
        return null;

      IHU_DATA_SAMPLE[] samples = new IHU_DATA_SAMPLE[number];
      IntPtr ptr = data;
      for (int i = 0; i < number; i++)
      {
        samples[i] = (IHU_DATA_SAMPLE)Marshal.PtrToStructure(ptr, typeof(IHU_DATA_SAMPLE));
        Marshal.DestroyStructure(ptr, typeof(IHU_DATA_SAMPLE));
        ptr = IntPtr.Add(ptr, Marshal.SizeOf(samples[i]));
      }

      return samples;
    }

    static IntPtr AllocRawValues(string[] tags)
    {
      int size = Marshal.SizeOf(typeof(IHU_RETRIEVED_RAW_VALUES));
      IntPtr data = Marshal.AllocCoTaskMem(tags.Length * size);
      byte[] zeroBuffer = new byte[size];

      IntPtr ptr = data;
      for (int i = 0; i < tags.Length; i++)
      {
        Marshal.Copy(zeroBuffer, 0, ptr, size);
        Marshal.WriteIntPtr(ptr, Marshal.StringToCoTaskMemAnsi(tags[i]));
        ptr = IntPtr.Add(ptr, size);
      }
      return data;
    }

    static IHU_RETRIEVED_RAW_VALUES[] RawValuesFromPtr(IntPtr data, int number)
    {
      if (data == IntPtr.Zero)
        return null;

      IHU_RETRIEVED_RAW_VALUES[] values = new IHU_RETRIEVED_RAW_VALUES[number];

      IntPtr ptr = data;
      for (int i = 0; i < number; i++)
      {
        values[i] = (IHU_RETRIEVED_RAW_VALUES)Marshal.PtrToStructure(ptr, typeof(IHU_RETRIEVED_RAW_VALUES));
        Marshal.DestroyStructure(ptr, typeof(IHU_RETRIEVED_RAW_VALUES));
        ptr = IntPtr.Add(ptr, Marshal.SizeOf(values[i]));
      }
      return values;
    }

    const string DLLNAME = "\\Reference\\IHUAPI.dll";

    static class IHU32
    {
      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuConnect@16")]
      public static extern ihuErrorCode ihuConnect(string server, string username, string password, out int serverhandle);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuConnectEx@28")]
      public static extern ihuErrorCode ihuConnectEx(string server, string username, string password, string buffername, uint maxMemoryMB, uint minDiskFreeMB, out int serverhandle);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuDisconnect@4")]
      public static extern ihuErrorCode ihuDisconnect(int serverhandle);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuIsServerConnected@4")]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool ihuIsServerConnected(int serverhandle);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuSetConnectionParameters@4")]
      public static extern ihuErrorCode ihuSetConnectionParameters(ref IHU_CONNECTION_PARAMETERS parameters);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuRestoreDefaultConnectionParameters@0")]
      public static extern ihuErrorCode ihuRestoreDefaultConnectionParameters();

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "IHU_TIMESTAMP_FromParts@32")]
      public static extern ihuErrorCode IHU_TIMESTAMP_FromParts(int year, int month, int day, int hour, int minute, int second, int subsecond, out IHU_TIMESTAMP time);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "IHU_TIMESTAMP_ToParts@32")]
      public static extern ihuErrorCode IHU_TIMESTAMP_ToParts(ref IHU_TIMESTAMP time, out int year, out int month, out int day, out int hour, out int minute, out int second, out int subsecond);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagCacheCriteriaClear@0")]
      public static extern void ihuTagCacheCriteriaClear();

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagCacheCriteriaSetStringProperty@8")]
      public static extern ihuErrorCode ihuTagCacheCriteriaSetStringProperty(ihuTagProperties property, string value);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagCacheCriteriaSetNumericProperty@12")]
      public static extern ihuErrorCode ihuTagCacheCriteriaSetNumericProperty(ihuTagProperties property, double value);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuFetchTagCache@12")]
      public static extern ihuErrorCode ihuFetchTagCache(int serverhandle, string tagnamemask, out int numberOfTags);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuFetchTagCacheEx@8")]
      public static extern ihuErrorCode ihuFetchTagCacheEx(int serverhandle, out int numberOfTags);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuCloseTagCache@0")]
      public static extern ihuErrorCode ihuCloseTagCache();

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuGetTagnameCacheIndex@8")]
      public static extern ihuErrorCode ihuGetTagnameCacheIndex(string tagname, out int cacheindex);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuGetNumericTagPropertyByTagname@12")]
      public static extern ihuErrorCode ihuGetNumericTagPropertyByTagname(string tagname, ihuTagProperties property, out double value);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuGetNumericTagPropertyByIndex@12")]
      public static extern ihuErrorCode ihuGetNumericTagPropertyByIndex(int index, ihuTagProperties property, out double value);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuGetStringTagPropertyByTagname@16")]
      public static extern ihuErrorCode ihuGetStringTagPropertyByTagname(string tagname, ihuTagProperties property, StringBuilder builder, int valuelength);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuGetStringTagPropertyByIndex@16")]
      public static extern ihuErrorCode ihuGetStringTagPropertyByIndex(int index, ihuTagProperties property, StringBuilder builder, int valuelength);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagClearProperties@0")]
      public static extern void ihuTagClearProperties();

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagSetStringProperty@8")]
      public static extern ihuErrorCode ihuTagSetStringProperty(ihuTagProperties property, string value);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagSetNumericProperty@12")]
      public static extern ihuErrorCode ihuTagSetNumericProperty(ihuTagProperties property, double value);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagAdd@4")]
      public static extern ihuErrorCode ihuTagAdd(int serverhandle);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagDelete@8")]
      public static extern ihuErrorCode ihuTagDelete(int serverhandle, string tagname);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagRename@12")]
      public static extern ihuErrorCode ihuTagRename(int serverhandle, string oldname, string newname);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuGetArchiverProperty@12")]
      public static extern ihuErrorCode ihuGetArchiverProperty(int serverhandle, string property, StringBuilder builder);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuSetArchiverProperty@12")]
      public static extern ihuErrorCode ihuSetArchiverProperty(int serverhandle, string property, string value);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuBrowseCollectors@16")]
      public static extern ihuErrorCode ihuBrowseCollectors(int serverhandle, string collectornamemask, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3, ArraySubType = UnmanagedType.LPStruct)] out IHU_COLLECTOR[] collectors, out int number);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuReadRawDataByTime@24")]
      public static extern ihuErrorCode ihuReadRawDataByTime(int serverhandle, string tagname, ref IHU_TIMESTAMP start, ref IHU_TIMESTAMP end, out int numberOfSamples, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4, ArraySubType = UnmanagedType.LPStruct)] out IHU_DATA_SAMPLE[] samples);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuReadRawDataByCount@24")]
      public static extern ihuErrorCode ihuReadRawDataByCount(int serverhandle, string tagname, ref IHU_TIMESTAMP start, ref int numberOfSamples, [MarshalAs(UnmanagedType.Bool)] bool forwardTimeOrder, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3, ArraySubType = UnmanagedType.LPStruct)] out IHU_DATA_SAMPLE[] samples);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuRetrieveSampledData@36")]
      public static extern ihuErrorCode ihuRetrieveSampledData(int serverhandle, IHU_TIMESTAMP start, IHU_TIMESTAMP end, ihuSamplingMode samplingMode, uint numberOfSamples, uint intervalMilliSeconds, ref IHU_RETRIEVED_DATA_RECORDS datarecords);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuRetrieveSampledDataEx@40")]
      public static extern ihuErrorCode ihuRetrieveSampledDataEx(int serverhandle, IHU_TIMESTAMP start, IHU_TIMESTAMP end, ihuSamplingMode samplingMode, uint numberOfSamples, IHU_DATA_INTERVAL interval, ref IHU_RETRIEVED_DATA_RECORDS datarecords);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuRetrieveCalculatedData@36")]
      public static extern ihuErrorCode ihuRetrieveCalculatedData(int serverhandle, IHU_TIMESTAMP start, IHU_TIMESTAMP end, ihuCalculationMode calculationMode, uint numberOfSamples, uint intervalMilliSeconds, ref IHU_RETRIEVED_DATA_RECORDS datarecords);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuRetrieveCalculatedDataEx@40")]
      public static extern ihuErrorCode ihuRetrieveCalculatedDataEx(int serverhandle, IHU_TIMESTAMP start, IHU_TIMESTAMP end, ihuCalculationMode calculationMode, uint numberOfSamples, IHU_DATA_INTERVAL interval, ref IHU_RETRIEVED_DATA_RECORDS datarecords);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuReadCurrentValue@16")]
      public static extern ihuErrorCode ihuReadCurrentValue(int serverhandle, int numberOfTags, IntPtr samples, ihuErrorCode[] results);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuReadInterpolatedValue@16")]
      public static extern ihuErrorCode ihuReadInterpolatedValue(int serverhandle, int numberOfTags, IntPtr samples, ihuErrorCode[] results);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuReadMultiTagRawDataByTime@24")]
      public static extern ihuErrorCode ihuReadMultiTagRawDataByTime(int serverhandle, int numberOfTags, ref IHU_TIMESTAMP start, ref IHU_TIMESTAMP end, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1, ArraySubType = UnmanagedType.I4)] out ihuErrorCode[] results, IntPtr samples);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuReadMultiTagRawDataByCount@28")]
      public static extern ihuErrorCode ihuReadMultiTagRawDataByCount(int serverhandle, int numberOfTags, ref IHU_TIMESTAMP start, ref int numberOfSamples, [MarshalAs(UnmanagedType.Bool)] bool forwardTimeOrder, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1, ArraySubType = UnmanagedType.I4)] out ihuErrorCode[] results, IntPtr data);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuWriteData@24")]
      public static extern ihuErrorCode ihuWriteData(int serverhandle, int numberOfSamples, IHU_DATA_SAMPLE[] data_values, ihuErrorCode[] results, [MarshalAs(UnmanagedType.Bool)] bool waitForReply, [MarshalAs(UnmanagedType.Bool)] bool errorOnReplace);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuWriteComment@24")]
      public static extern ihuErrorCode ihuWriteComment(int serverhandle, string tagname, ref IHU_TIMESTAMP timestamp, string comment, string supplieduser, string suppliedpassword);
    }

    static class IHU64
    {
      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuConnect")]
      public static extern ihuErrorCode ihuConnect(string server, string username, string password, out int serverhandle);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuConnectEx")]
      public static extern ihuErrorCode ihuConnectEx(string server, string username, string password, string buffername, uint maxMemoryMB, uint minDiskFreeMB, out int serverhandle);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuDisconnect")]
      public static extern ihuErrorCode ihuDisconnect(int serverhandle);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuIsServerConnected")]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool ihuIsServerConnected(int serverhandle);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuSetConnectionParameters")]
      public static extern ihuErrorCode ihuSetConnectionParameters(ref IHU_CONNECTION_PARAMETERS parameters);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuRestoreDefaultConnectionParameters")]
      public static extern ihuErrorCode ihuRestoreDefaultConnectionParameters();

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "IHU_TIMESTAMP_FromParts")]
      public static extern ihuErrorCode IHU_TIMESTAMP_FromParts(int year, int month, int day, int hour, int minute, int second, int subsecond, out IHU_TIMESTAMP time);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "IHU_TIMESTAMP_ToParts")]
      public static extern ihuErrorCode IHU_TIMESTAMP_ToParts(ref IHU_TIMESTAMP time, out int year, out int month, out int day, out int hour, out int minute, out int second, out int subsecond);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagCacheCriteriaClear")]
      public static extern void ihuTagCacheCriteriaClear();

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagCacheCriteriaSetStringProperty")]
      public static extern ihuErrorCode ihuTagCacheCriteriaSetStringProperty(ihuTagProperties property, string value);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagCacheCriteriaSetNumericProperty")]
      public static extern ihuErrorCode ihuTagCacheCriteriaSetNumericProperty(ihuTagProperties property, double value);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuFetchTagCache")]
      public static extern ihuErrorCode ihuFetchTagCache(int serverhandle, string tagnamemask, out int numberOfTags);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuFetchTagCacheEx")]
      public static extern ihuErrorCode ihuFetchTagCacheEx(int serverhandle, out int numberOfTags);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuCloseTagCache")]
      public static extern ihuErrorCode ihuCloseTagCache();

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuGetTagnameCacheIndex")]
      public static extern ihuErrorCode ihuGetTagnameCacheIndex(string tagname, out int cacheindex);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuGetNumericTagPropertyByTagname")]
      public static extern ihuErrorCode ihuGetNumericTagPropertyByTagname(string tagname, ihuTagProperties property, out double value);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuGetNumericTagPropertyByIndex")]
      public static extern ihuErrorCode ihuGetNumericTagPropertyByIndex(int index, ihuTagProperties property, out double value);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuGetStringTagPropertyByTagname")]
      public static extern ihuErrorCode ihuGetStringTagPropertyByTagname(string tagname, ihuTagProperties property, StringBuilder builder, int valuelength);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuGetStringTagPropertyByIndex")]
      public static extern ihuErrorCode ihuGetStringTagPropertyByIndex(int index, ihuTagProperties property, StringBuilder builder, int valuelength);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagClearProperties")]
      public static extern void ihuTagClearProperties();

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagSetStringProperty")]
      public static extern ihuErrorCode ihuTagSetStringProperty(ihuTagProperties property, string value);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagSetNumericProperty")]
      public static extern ihuErrorCode ihuTagSetNumericProperty(ihuTagProperties property, double value);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagAdd")]
      public static extern ihuErrorCode ihuTagAdd(int serverhandle);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagDelete")]
      public static extern ihuErrorCode ihuTagDelete(int serverhandle, string tagname);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuTagRename")]
      public static extern ihuErrorCode ihuTagRename(int serverhandle, string oldname, string newname);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuGetArchiverProperty")]
      public static extern ihuErrorCode ihuGetArchiverProperty(int serverhandle, string property, StringBuilder builder);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuSetArchiverProperty")]
      public static extern ihuErrorCode ihuSetArchiverProperty(int serverhandle, string property, string value);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuBrowseCollectors")]
      public static extern ihuErrorCode ihuBrowseCollectors(int serverhandle, string collectornamemask, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3, ArraySubType = UnmanagedType.LPStruct)] out IHU_COLLECTOR[] collectors, out int number);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuReadRawDataByTime")]
      public static extern ihuErrorCode ihuReadRawDataByTime(int serverhandle, string tagname, ref IHU_TIMESTAMP start, ref IHU_TIMESTAMP end, out int numberOfSamples, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4, ArraySubType = UnmanagedType.LPStruct)] out IHU_DATA_SAMPLE[] samples);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuReadRawDataByCount")]
      public static extern ihuErrorCode ihuReadRawDataByCount(int serverhandle, string tagname, ref IHU_TIMESTAMP start, ref int numberOfSamples, [MarshalAs(UnmanagedType.Bool)] bool forwardTimeOrder, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3, ArraySubType = UnmanagedType.LPStruct)] out IHU_DATA_SAMPLE[] samples);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuRetrieveSampledData")]
      public static extern ihuErrorCode ihuRetrieveSampledData(int serverhandle, IHU_TIMESTAMP start, IHU_TIMESTAMP end, ihuSamplingMode samplingMode, uint numberOfSamples, uint intervalMilliSeconds, ref IHU_RETRIEVED_DATA_RECORDS datarecords);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuRetrieveSampledDataEx")]
      public static extern ihuErrorCode ihuRetrieveSampledDataEx(int serverhandle, IHU_TIMESTAMP start, IHU_TIMESTAMP end, ihuSamplingMode samplingMode, uint numberOfSamples, IHU_DATA_INTERVAL interval, ref IHU_RETRIEVED_DATA_RECORDS datarecords);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuRetrieveCalculatedData")]
      public static extern ihuErrorCode ihuRetrieveCalculatedData(int serverhandle, IHU_TIMESTAMP start, IHU_TIMESTAMP end, ihuCalculationMode calculationMode, uint numberOfSamples, uint intervalMilliSeconds, ref IHU_RETRIEVED_DATA_RECORDS datarecords);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuRetrieveCalculatedDataEx")]
      public static extern ihuErrorCode ihuRetrieveCalculatedDataEx(int serverhandle, IHU_TIMESTAMP start, IHU_TIMESTAMP end, ihuCalculationMode calculationMode, uint numberOfSamples, IHU_DATA_INTERVAL interval, ref IHU_RETRIEVED_DATA_RECORDS datarecords);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuReadCurrentValue")]
      public static extern ihuErrorCode ihuReadCurrentValue(int serverhandle, int numberOfTags, IntPtr samples, ihuErrorCode[] results);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuReadInterpolatedValue")]
      public static extern ihuErrorCode ihuReadInterpolatedValue(int serverhandle, int numberOfTags, IntPtr samples, ihuErrorCode[] results);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuReadMultiTagRawDataByTime")]
      public static extern ihuErrorCode ihuReadMultiTagRawDataByTime(int serverhandle, int numberOfTags, ref IHU_TIMESTAMP start, ref IHU_TIMESTAMP end, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1, ArraySubType = UnmanagedType.I4)] out ihuErrorCode[] results, IntPtr samples);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuReadMultiTagRawDataByCount")]
      public static extern ihuErrorCode ihuReadMultiTagRawDataByCount(int serverhandle, int numberOfTags, ref IHU_TIMESTAMP start, ref int numberOfSamples, [MarshalAs(UnmanagedType.Bool)] bool forwardTimeOrder, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1, ArraySubType = UnmanagedType.I4)] out ihuErrorCode[] results, IntPtr data);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuWriteData")]
      public static extern ihuErrorCode ihuWriteData(int serverhandle, int numberOfSamples, IHU_DATA_SAMPLE[] data_values, ihuErrorCode[] results, [MarshalAs(UnmanagedType.Bool)] bool waitForReply, [MarshalAs(UnmanagedType.Bool)] bool errorOnReplace);

      [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, EntryPoint = "ihuWriteComment")]
      public static extern ihuErrorCode ihuWriteComment(int serverhandle, string tagname, ref IHU_TIMESTAMP timestamp, string comment, string supplieduser, string suppliedpassword);
    }

    //
    // Server Connection
    //
    public static ihuErrorCode ihuConnect(string server, string username, string password, out int serverhandle)
    {
      if (Is32bit)
        return IHU32.ihuConnect(server, username, password, out serverhandle);
      else
        return IHU64.ihuConnect(server, username, password, out serverhandle);
    }

    public static ihuErrorCode ihuConnectEx(string server, string username, string password, string buffername, uint maxMemoryMB, uint minDiskFreeMB, out int serverhandle)
    {
      if (Is32bit)
        return IHU32.ihuConnectEx(server, username, password, buffername, maxMemoryMB, minDiskFreeMB, out serverhandle);
      else
        return IHU64.ihuConnectEx(server, username, password, buffername, maxMemoryMB, minDiskFreeMB, out serverhandle);
    }

    public static ihuErrorCode ihuDisconnect(int serverhandle)
    {
      if (Is32bit)
        return IHU32.ihuDisconnect(serverhandle);
      else
        return IHU64.ihuDisconnect(serverhandle);
    }

    public static bool ihuIsServerConnected(int serverhandle)
    {
      if (Is32bit)
        return IHU32.ihuIsServerConnected(serverhandle);
      else
        return IHU64.ihuIsServerConnected(serverhandle);
    }

    public static ihuErrorCode ihuSetConnectionParameters(IHU_CONNECTION_PARAMETERS parameters)
    {
      if (Is32bit)
        return IHU32.ihuSetConnectionParameters(ref parameters);
      else
        return IHU64.ihuSetConnectionParameters(ref parameters);
    }

    public static ihuErrorCode ihuRestoreDefaultConnectionParameters()
    {
      if (Is32bit)
        return IHU32.ihuRestoreDefaultConnectionParameters();
      else
        return IHU64.ihuRestoreDefaultConnectionParameters();
    }

    //
    // Timestamps
    //
    public static ihuErrorCode IHU_TIMESTAMP_FromParts(DateTime datetime, out IHU_TIMESTAMP time)
    {
      datetime = datetime.ToLocalTime();
      if (Is32bit)
        return IHU32.IHU_TIMESTAMP_FromParts(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond * IHU_TIMESTAMP.SubsecondsPerMillisecond, out time);
      else
        return IHU64.IHU_TIMESTAMP_FromParts(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond * IHU_TIMESTAMP.SubsecondsPerMillisecond, out time);
    }

    public static ihuErrorCode IHU_TIMESTAMP_FromParts(int year, int month, int day, int hour, int minute, int second, int subsecond, out IHU_TIMESTAMP time)
    {
      if (Is32bit)
        return IHU32.IHU_TIMESTAMP_FromParts(year, month, day, hour, minute, second, subsecond, out time);
      else
        return IHU64.IHU_TIMESTAMP_FromParts(year, month, day, hour, minute, second, subsecond, out time);
    }

    public static ihuErrorCode IHU_TIMESTAMP_ToParts(IHU_TIMESTAMP time, out DateTime datetime)
    {
      int year, month, day, hour, minute, second, subsecond;
      ihuErrorCode result;
      if (Is32bit)
        result = IHU32.IHU_TIMESTAMP_ToParts(ref time, out year, out month, out day, out hour, out minute, out second, out subsecond);
      else
        result = IHU64.IHU_TIMESTAMP_ToParts(ref time, out year, out month, out day, out hour, out minute, out second, out subsecond);
      datetime = new DateTime(year, month, day, hour, minute, second, subsecond / IHU_TIMESTAMP.SubsecondsPerMillisecond, DateTimeKind.Local);
      return result;
    }

    public static ihuErrorCode IHU_TIMESTAMP_ToParts(IHU_TIMESTAMP time, out int year, out int month, out int day, out int hour, out int minute, out int second, out int subsecond)
    {
      if (Is32bit)
        return IHU32.IHU_TIMESTAMP_ToParts(ref time, out year, out month, out day, out hour, out minute, out second, out subsecond);
      else
        return IHU64.IHU_TIMESTAMP_ToParts(ref time, out year, out month, out day, out hour, out minute, out second, out subsecond);
    }

    //
    // Tag browsing
    //
    public static void ihuTagCacheCriteriaClear()
    {
      if (Is32bit)
        IHU32.ihuTagCacheCriteriaClear();
      else
        IHU64.ihuTagCacheCriteriaClear();
    }

    public static ihuErrorCode ihuTagCacheCriteriaSetStringProperty(ihuTagProperties property, string value)
    {
      if (Is32bit)
        return IHU32.ihuTagCacheCriteriaSetStringProperty(property, value);
      else
        return IHU64.ihuTagCacheCriteriaSetStringProperty(property, value);
    }

    public static ihuErrorCode ihuTagCacheCriteriaSetNumericProperty(ihuTagProperties property, double value)
    {
      if (Is32bit)
        return IHU32.ihuTagCacheCriteriaSetNumericProperty(property, value);
      else
        return IHU64.ihuTagCacheCriteriaSetNumericProperty(property, value);
    }

    //
    // Tag properties
    //
    public static ihuErrorCode ihuFetchTagCache(int serverhandle, string tagnamemask, out int numberOfTags)
    {
      if (Is32bit)
        return IHU32.ihuFetchTagCache(serverhandle, tagnamemask, out numberOfTags);
      else
        return IHU64.ihuFetchTagCache(serverhandle, tagnamemask, out numberOfTags);
    }

    public static ihuErrorCode ihuFetchTagCacheEx(int serverhandle, out int numtagsfound)
    {
      if (Is32bit)
        return IHU32.ihuFetchTagCacheEx(serverhandle, out numtagsfound);
      else
        return IHU64.ihuFetchTagCacheEx(serverhandle, out numtagsfound);
    }

    public static ihuErrorCode ihuCloseTagCache()
    {
      if (Is32bit)
        return IHU32.ihuCloseTagCache();
      else
        return IHU64.ihuCloseTagCache();
    }

    public static ihuErrorCode ihuGetTagnameCacheIndex(string tagname, out int numberOfTags)
    {
      if (Is32bit)
        return IHU32.ihuGetTagnameCacheIndex(tagname, out numberOfTags);
      else
        return IHU64.ihuGetTagnameCacheIndex(tagname, out numberOfTags);
    }

    public static ihuErrorCode ihuGetNumericTagPropertyByTagname(string tagname, ihuTagProperties property, out double value)
    {
      if (Is32bit)
        return IHU32.ihuGetNumericTagPropertyByTagname(tagname, property, out value);
      else
        return IHU64.ihuGetNumericTagPropertyByTagname(tagname, property, out value);
    }

    public static ihuErrorCode ihuGetNumericTagPropertyByIndex(int index, ihuTagProperties property, out double value)
    {
      if (Is32bit)
        return IHU32.ihuGetNumericTagPropertyByIndex(index, property, out value);
      else
        return IHU64.ihuGetNumericTagPropertyByIndex(index, property, out value);
    }

    public static ihuErrorCode ihuGetStringTagPropertyByTagname(string tagname, ihuTagProperties property, out string value)
    {
      StringBuilder builder = new StringBuilder(1024);
      ihuErrorCode result;
      if (Is32bit)
        result = IHU32.ihuGetStringTagPropertyByTagname(tagname, property, builder, builder.Capacity);
      else
        result = IHU64.ihuGetStringTagPropertyByTagname(tagname, property, builder, builder.Capacity);
      value = builder.ToString();
      return result;
    }

    public static ihuErrorCode ihuGetStringTagPropertyByIndex(int index, ihuTagProperties property, out string value)
    {
      StringBuilder builder = new StringBuilder(1024);
      ihuErrorCode result;
      if (Is32bit)
        result = IHU32.ihuGetStringTagPropertyByIndex(index, property, builder, builder.Capacity);
      else
        result = IHU64.ihuGetStringTagPropertyByIndex(index, property, builder, builder.Capacity);
      value = builder.ToString();
      return result;
    }

    //
    // Tag utility
    //
    public static void ihuTagClearProperties()
    {
      if (Is32bit)
        IHU32.ihuTagClearProperties();
      else
        IHU64.ihuTagClearProperties();
    }

    public static ihuErrorCode ihuTagSetStringProperty(ihuTagProperties property, string value)
    {
      if (Is32bit)
        return IHU32.ihuTagSetStringProperty(property, value);
      else
        return IHU64.ihuTagSetStringProperty(property, value);
    }

    public static ihuErrorCode ihuTagSetNumericProperty(ihuTagProperties property, double value)
    {
      if (Is32bit)
        return IHU32.ihuTagSetNumericProperty(property, value);
      else
        return IHU64.ihuTagSetNumericProperty(property, value);
    }

    public static ihuErrorCode ihuTagAdd(int serverhandle)
    {
      if (Is32bit)
        return IHU32.ihuTagAdd(serverhandle);
      else
        return IHU64.ihuTagAdd(serverhandle);
    }

    public static ihuErrorCode ihuTagDelete(int serverhandle, string tagname)
    {
      if (Is32bit)
        return IHU32.ihuTagDelete(serverhandle, tagname);
      else
        return IHU64.ihuTagDelete(serverhandle, tagname);
    }

    public static ihuErrorCode ihuTagRename(int serverhandle, string oldname, string newname)
    {
      if (Is32bit)
        return IHU32.ihuTagRename(serverhandle, oldname, newname);
      else
        return IHU64.ihuTagRename(serverhandle, oldname, newname);
    }

    //
    // Archiver properites
    //
    public static ihuErrorCode ihuGetArchiverProperty(int serverhandle, string property, out string value)
    {
      StringBuilder builder = new StringBuilder(1024);
      ihuErrorCode result;
      if (Is32bit)
        result = IHU32.ihuGetArchiverProperty(serverhandle, property, builder);
      else
        result = IHU64.ihuGetArchiverProperty(serverhandle, property, builder);
      value = builder.ToString();
      return result;
    }

    public static ihuErrorCode ihuSetArchiverProperty(int serverhandle, string property, string value)
    {
      if (Is32bit)
        return IHU32.ihuSetArchiverProperty(serverhandle, property, value);
      else
        return IHU64.ihuSetArchiverProperty(serverhandle, property, value);
    }

    //
    // Collector browse
    //
    public static ihuErrorCode ihuBrowseCollectors(int serverhandle, string collectornamemask, out IHU_COLLECTOR[] collectors)
    {
      int number;
      if (Is32bit)
        return IHU32.ihuBrowseCollectors(serverhandle, collectornamemask, out collectors, out number);
      else
        return IHU64.ihuBrowseCollectors(serverhandle, collectornamemask, out collectors, out number);
    }

    //
    // Data query
    //
    public static ihuErrorCode ihuReadRawDataByTime(int serverhandle, string tagname, IHU_TIMESTAMP start, IHU_TIMESTAMP end, out IHU_DATA_SAMPLE[] samples)
    {
      int numberOfSamples;
      if (Is32bit)
        return IHU32.ihuReadRawDataByTime(serverhandle, tagname, ref start, ref end, out numberOfSamples, out samples);
      else
        return IHU64.ihuReadRawDataByTime(serverhandle, tagname, ref start, ref end, out numberOfSamples, out samples);
    }

    public static ihuErrorCode ihuReadRawDataByCount(int serverhandle, string tagname, IHU_TIMESTAMP start, int numberOfSamples, bool forwardTimeOrder, out IHU_DATA_SAMPLE[] samples)
    {
      if (Is32bit)
        return IHU32.ihuReadRawDataByCount(serverhandle, tagname, ref start, ref numberOfSamples, forwardTimeOrder, out samples);
      else
        return IHU64.ihuReadRawDataByCount(serverhandle, tagname, ref start, ref numberOfSamples, forwardTimeOrder, out samples);
    }

    public static ihuErrorCode ihuRetrieveSampledData(int serverhandle, string[] tagnames, IHU_TIMESTAMP start, IHU_TIMESTAMP end, ihuSamplingMode mode, uint numberOfSamples, uint intervalMilliSeconds, out IHU_RETRIEVED_DATA_VALUES[] values, out ihuErrorCode[] results)
    {
      IHU_RETRIEVED_DATA_RECORDS records = new IHU_RETRIEVED_DATA_RECORDS(tagnames);

      ihuErrorCode result;
      if (Is32bit)
        result = IHU32.ihuRetrieveSampledData(serverhandle, start, end, mode, numberOfSamples, intervalMilliSeconds, ref records);
      else
        result = IHU64.ihuRetrieveSampledData(serverhandle, start, end, mode, numberOfSamples, intervalMilliSeconds, ref records);

      values = records.TagValues;
      results = records.ErrorResults;
      records.Dispose();

      return result;
    }

    public static ihuErrorCode ihuRetrieveSampledDataEx(int serverhandle, string[] tagnames, IHU_TIMESTAMP start, IHU_TIMESTAMP end, ihuSamplingMode samplingMode, uint numberOfSamples, IHU_DATA_INTERVAL interval, out IHU_RETRIEVED_DATA_VALUES_EX[] values, out ihuErrorCode[] results)
    {
      IHU_RETRIEVED_DATA_RECORDS records = new IHU_RETRIEVED_DATA_RECORDS(tagnames);

      ihuErrorCode result;
      if (Is32bit)
        result = IHU32.ihuRetrieveSampledDataEx(serverhandle, start, end, samplingMode, numberOfSamples, interval, ref records);
      else
        result = IHU64.ihuRetrieveSampledDataEx(serverhandle, start, end, samplingMode, numberOfSamples, interval, ref records);

      values = records.TagValuesEx;
      results = records.ErrorResults;
      records.Dispose();

      return result;
    }

    public static ihuErrorCode ihuRetrieveCalculatedData(int serverhandle, string[] tagnames, IHU_TIMESTAMP start, IHU_TIMESTAMP end, ihuCalculationMode samplingMode, uint numberOfSamples, uint intervalMilliSeconds, out IHU_RETRIEVED_DATA_VALUES[] values, out ihuErrorCode[] results)
    {
      IHU_RETRIEVED_DATA_RECORDS records = new IHU_RETRIEVED_DATA_RECORDS(tagnames);

      ihuErrorCode result;
      if (Is32bit)
        result = IHU32.ihuRetrieveCalculatedData(serverhandle, start, end, samplingMode, numberOfSamples, intervalMilliSeconds, ref records);
      else
        result = IHU64.ihuRetrieveCalculatedData(serverhandle, start, end, samplingMode, numberOfSamples, intervalMilliSeconds, ref records);

      values = records.TagValues;
      results = records.ErrorResults;
      records.Dispose();

      return result;
    }

    public static ihuErrorCode ihuRetrieveCalculatedDataEx(int serverhandle, string[] tagnames, IHU_TIMESTAMP start, IHU_TIMESTAMP end, ihuCalculationMode samplingMode, uint numberOfSamples, IHU_DATA_INTERVAL interval, out IHU_RETRIEVED_DATA_VALUES_EX[] values, out ihuErrorCode[] results)
    {
      IHU_RETRIEVED_DATA_RECORDS records = new IHU_RETRIEVED_DATA_RECORDS(tagnames);

      ihuErrorCode result;
      if (Is32bit)
        result = IHU32.ihuRetrieveCalculatedDataEx(serverhandle, start, end, samplingMode, numberOfSamples, interval, ref records);
      else
        result = IHU64.ihuRetrieveCalculatedDataEx(serverhandle, start, end, samplingMode, numberOfSamples, interval, ref records);

      values = records.TagValuesEx;
      results = records.ErrorResults;
      records.Dispose();

      return result;
    }

    public static ihuErrorCode ihuReadCurrentValue(int serverhandle, string[] tagnames, out IHU_DATA_SAMPLE[] samples, out ihuErrorCode[] results)
    {
      IntPtr data = IntPtr.Zero;
      try
      {
        data = AllocDataSamples(tagnames, null);
        results = new ihuErrorCode[tagnames.Length];

        ihuErrorCode result;
        if (Is32bit)
            result = IHU32.ihuReadCurrentValue(serverhandle, tagnames.Length, data, results);

        else
            result = IHU64.ihuReadCurrentValue(serverhandle, tagnames.Length, data, results);

        samples = DataSamplesFromPtr(data, tagnames.Length);

        return result;
      }
      finally
      {
        Marshal.FreeCoTaskMem(data);
      }

    }

    public static ihuErrorCode ihuReadInterpolatedValue(int serverhandle, string[] tagnames, IHU_TIMESTAMP[] timestamps, out IHU_DATA_SAMPLE[] samples, out ihuErrorCode[] results)
    {
      if (tagnames.Length != timestamps.Length)
        throw new ArgumentException("Inconsistent number of tagnames and timestamps");

      IntPtr data = IntPtr.Zero;
      try
      {
        data = AllocDataSamples(tagnames, timestamps);
        results = new ihuErrorCode[tagnames.Length];

        ihuErrorCode result;
        if (Is32bit)
          result = IHU32.ihuReadInterpolatedValue(serverhandle, tagnames.Length, data, results);
        else
          result = IHU64.ihuReadInterpolatedValue(serverhandle, tagnames.Length, data, results);

        samples = DataSamplesFromPtr(data, tagnames.Length);

        return result;
      }
      finally
      {
        Marshal.FreeCoTaskMem(data);
      }
    }

    public static ihuErrorCode ihuReadMultiTagRawDataByTime(int serverhandle, string[] tagnames, IHU_TIMESTAMP start, IHU_TIMESTAMP end, out IHU_RETRIEVED_RAW_VALUES[] samples, out ihuErrorCode[] results)
    {
      IntPtr data = IntPtr.Zero;
      try
      {
        data = AllocRawValues(tagnames);

        ihuErrorCode result;
        if (Is32bit)
          result = IHU32.ihuReadMultiTagRawDataByTime(serverhandle, tagnames.Length, ref start, ref end, out results, data);
        else
          result = IHU64.ihuReadMultiTagRawDataByTime(serverhandle, tagnames.Length, ref start, ref end, out results, data);

        samples = RawValuesFromPtr(data, tagnames.Length);

        return result;
      }
      finally
      {
        Marshal.FreeCoTaskMem(data);
      }
    }

    public static ihuErrorCode ihuReadMultiTagRawDataByCount(int serverhandle, string[] tagnames, IHU_TIMESTAMP start, int numberOfSamples, bool forwardTimeOrder, out IHU_RETRIEVED_RAW_VALUES[] samples, out ihuErrorCode[] results)
    {
      IntPtr data = IntPtr.Zero;
      try
      {
        data = AllocRawValues(tagnames);

        ihuErrorCode result;
        if (Is32bit)
          result = IHU32.ihuReadMultiTagRawDataByCount(serverhandle, tagnames.Length, ref start, ref numberOfSamples, forwardTimeOrder, out results, data);
        else
          result = IHU64.ihuReadMultiTagRawDataByCount(serverhandle, tagnames.Length, ref start, ref numberOfSamples, forwardTimeOrder, out results, data);

        samples = RawValuesFromPtr(data, tagnames.Length);

        return result;
      }
      finally
      {
        Marshal.FreeCoTaskMem(data);
      }
    }

    //
    // Data insert
    //
    public static ihuErrorCode ihuWriteData(int serverhandle, IHU_DATA_SAMPLE[] samples, ihuErrorCode[] results, bool waitForReply, bool errorOnReplace)
    {
      if (Is32bit)
        return IHU32.ihuWriteData(serverhandle, samples.Length, samples, results, waitForReply, errorOnReplace);
      else
        return IHU64.ihuWriteData(serverhandle, samples.Length, samples, results, waitForReply, errorOnReplace);
    }

    public static ihuErrorCode ihuWriteComment(int serverhandle, string tagname, IHU_TIMESTAMP timestamp, string comment, string username, string password)
    {
      if (Is32bit)
        return IHU32.ihuWriteComment(serverhandle, tagname, ref timestamp, comment, username, password);
      else
        return IHU64.ihuWriteComment(serverhandle, tagname, ref timestamp, comment, username, password);
    }
  }
}
