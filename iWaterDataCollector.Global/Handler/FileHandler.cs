using iWaterDataCollector.Model.Tag;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

/********************************************
 * Ssystem.IO를 이용한 File 활용 Class
 * (20240707) 소스 정리 완료
 ********************************************/
namespace iWaterDataCollector.Global.Handler
{
    public static class FileHandler
    {
        /// <summary>
        /// Remote(원격지) Tag File Load
        /// </summary>
        /// <param name="path">Remote Server Tag 파일 경로</param>
        /// <returns>Remote Server Tag <see cref="List{T}"/></returns>
        public static List<LoadTagModel> ReadTagCollection(string path)
        {
            var lData = new List<LoadTagModel>();
            try
            {
                //경로 Null 여부 확인
                if (!string.IsNullOrEmpty(path))
                {
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    using (var sr = new StreamReader(fs, GetEncoding(path)))
                    {
                        string line;
                        while (sr.EndOfStream == false)
                        {
                            line = sr.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                //쉼표(,) Parsing
                                var info = line.Split(',');
                                //Tag이름 등록
                                var model = new LoadTagModel(info[0], info[1]);
                                lData.Add(model);
                            }
                        }
                    }
                    AppData.Instance.MsgIRDC.Debug(AppData.AppLog, nameof(FileHandler), $"Remote Tag Collection Load({lData.Count}개) 완료");
                }
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, nameof(FileHandler), "Remote Tag Collection Load 에러", ex);
            }

            return lData;
        }
        /// <summary>
        /// Historian/pdb Tag Load
        /// </summary>
        /// <param name="localPath">Historian Master Tag 파일경로</param>
        /// <param name="pdbPath">PDB Setting 파일 경로</param>
        /// <param name="createDate">생성일자</param>
        /// <returns>Historian Load 대상 Tag <see cref="List{T}"/></returns>
        public static List<LoadTagModel> ReadTagCollection(string localPath, string pdbPath, out string createDate)
        {
            var lData = new List<LoadTagModel>();
            createDate = string.Empty;
            try
            {
                var di = new DirectoryInfo(localPath);
                //저장된 파일 중 가장 최신 파일을 선택
                var lfile = di.GetFiles().ToList();
                if (lfile.Count > 0)
                {
                    var fi = lfile.OrderBy(t => t.CreationTime).LastOrDefault().FullName;
                    AppData.Instance.MsgIRDC.Debug(AppData.AppLog, nameof(FileHandler), $"Tag Collection Load File : {fi}");

                    using (var fs = new FileStream(fi, FileMode.Open, FileAccess.Read))
                    using (var sr = new StreamReader(fs, new UTF8Encoding(false)))//GetEncoding(fi)))
                    {
                        string line;
                        while (sr.EndOfStream == false)
                        {
                            line = sr.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                //쉼표(,)구분 Parsing
                                var info = line.Split(',');
                                //Tag 정보 등록
                                var model = new LoadTagModel(info[0], info[1]);
                                createDate = info[2];
                                lData.Add(model);
                            }
                        }
                    }
                    AppData.Instance.MsgIRDC.Debug(AppData.AppLog, nameof(FileHandler), $"Historian Tag Load({lData.Count}개) 완료");
                    //PDB설정 Tag Name List Load
                    var names = ReadTagNameCollection(pdbPath);
                    //Historian Tag List와 PDB 설정 TagName List를 Tag Name으로 비교하여 해당되는 Tag는 PDB에서 Load해오는 것으로 체크
                    lData.Where(t => names.Contains(t.Name))
                         .ToList()
                         .ForEach(t => t.IsCheck = true);
                    AppData.Instance.MsgIRDC.Debug(AppData.AppLog, nameof(FileHandler), $"PDB Tag Check({lData.Where(t => t.IsCheck).Count()}개) 완료");
                }
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, nameof(FileHandler), "Tag Collection Load 에러", ex);
            }

            return lData;
        }
        /// <summary>
        /// PDB Setting Tag Name List Load
        /// </summary>
        /// <remarks>
        /// PDB에서 Load될 Tag Name List를 Load
        /// </remarks>
        /// <param name="path">PDB Tag 설정 파일</param>
        /// <returns>PDB Tag Name <see cref="List{T}"/></returns>
        public static List<string> ReadTagNameCollection(string path)
        {
            var lData = new List<string>();
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var sr = new StreamReader(fs, GetEncoding(path)))
                {
                    string line;
                    while (sr.EndOfStream == false)
                    {
                        line = sr.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            //쉼표(,)로 구분
                            lData.Add(line.Split(',')[0]);
                        }
                    }
                }
                AppData.Instance.MsgIRDC.Debug(AppData.AppLog, nameof(FileHandler), $"Tag Name Collection Load({lData.Count}개) 완료");
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, nameof(FileHandler), "Tag Name Collection 에러", ex);
            }

            return lData;
        }
        /// <summary>
        /// Kafka 연계 Tag List Load
        /// </summary>
        /// <param name="path">kafka Tag List 파일 경로</param>
        /// <returns>Kafka 연계 Tag <see cref="List{T}"/></returns>
        public static List<KafkaTagModel> ReadKafkaCollection(string path)
        {
            var lData = new List<KafkaTagModel>();
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var sr = new StreamReader(fs, GetEncoding(path)))
                {
                    string line;
                    while (sr.EndOfStream == false)
                    {
                        line = sr.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            //쉼표(,)로 구분
                            var tag = line.Split(',');
                            //Pulse Tag Check
                            _ = bool.TryParse(tag[2], out bool isPulse);
                            var resutl = int.TryParse(tag[4], out int duration);
                            var model = new KafkaTagModel(tag[0], isPulse);
                            //Pulse Tag인 경우 해당 정보 추가
                            if (isPulse)
                            {
                                model.pulse = new PulseTagModel(tag[3], tag[5], duration);
                            }
                            lData.Add(model);
                        }
                    }
                }
                AppData.Instance.MsgIRDC.Debug(AppData.AppLog, nameof(FileHandler), $"Kafak Tag Collection Load({lData.Count}개) 완료");
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, nameof(FileHandler), "Kafka Tag Collection 에러", ex);
            }

            return lData;
        }
        /// <summary>
        /// kafak Tag File Load
        /// </summary>
        /// <remarks>
        /// Kafka로 받아오는 Tag 정보를 등록한 파일을 Load
        /// </remarks>
        /// <param name="path">Kafka Tag List 파일경로</param>
        /// <param name="createDate">Kafka Tag List 생성일자</param>
        /// <returns>Kafka Tag Name <see cref="List{T}"/></returns>
        public static List<string> ReadKafakCollection(string path, out string createDate)
        {
            //Tag Name Dump List
            var lData = new List<string>();
            //File Create Time 등록
            createDate = File.GetCreationTime(path).ToString("d");
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var sr = new StreamReader(fs, GetEncoding(path)))
                {
                    string line;
                    //파일 끝까지 읽음.
                    while (sr.EndOfStream == false)
                    {
                        line = sr.ReadLine();
                        //Null or Empty 여부 확인
                        if (!string.IsNullOrEmpty(line))
                        {
                            //Kafka Tag 정보 등록
                            lData.Add(line);
                        }
                    }
                }
                AppData.Instance.MsgIRDC.Debug(AppData.AppLog, nameof(FileHandler), $"설정용 Kafak Tag Collection Load({lData.Count}개) 완료");
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, nameof(FileHandler), "설정용 Kafka Tag Collection 에러", ex);
            }

            return lData;
        }
        /// <summary>
        /// 기존 Setting 파일 백업
        /// </summary>
        /// <param name="file">기존경로</param>
        /// <returns></returns>
        public static bool MoveRecoveryCSVFile(string file)
        {
            var result = false;
            try
            {
                //파일 이름만 추출
                var filename = new FileInfo(file).Name;
                //백업 경로 설정
                var destDirectory = DirectoryHandler.RecoveryDirectory(AppData.Instance.DefaultPath);
                //백업 경로 유무확인
                if (!Directory.Exists(destDirectory))
                {
                    //없는 경우 생성
                    Directory.CreateDirectory(destDirectory);
                }
                //이전 백업본 경로 확인
                var dest = Path.Combine(destDirectory, filename);
                //이전 백업본 삭제
                File.Delete(dest);
                //기존파일(file) -> 백업파일(dest)로 이동
                File.Move(file, dest);
                result = true;
                AppData.Instance.MsgIRDC.Debug(AppData.AppLog, nameof(FileHandler), $"CSV File({file}) 백업완료(백업경로 : {dest})");
            }
            catch(Exception ex)
            {
                result = false;
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, nameof(FileHandler), $"CSV File({file}) Recovery 에러", ex);
            }

            return result;
        }
        /// <summary>
        /// 설정되어있는 Remote Server Tag Metadata List를 복사(삭제되는 경우 대비)
        /// </summary>
        /// <param name="path">복사할 path</param>
        /// <param name="file">Metadata 파일명</param>
        /// <returns></returns>
        public static bool MoveMetadataFile(string path, string file, List<LoadTagModel> ltag)
        {
            var result = false;
            try
            {
                //대상경로
                var destDirectory = DirectoryHandler.InfoDirectory(path);
                //대상경로 Full Path
                var dest = Path.Combine(destDirectory, $"{DateTime.Now:yyyyMMdd}_TagList.csv");

                var line = ltag.Select(t => t.CSVFormat($"{DateTime.Now:yyyy-MM-dd}")).ToArray();
                result = WriteTagCSVFile(dest, line, FileMode.Create);
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, nameof(FileHandler), $"Remote Metadata CSV File({file}) Move 에러", ex);
            }

            return result;
        }
        /// <summary>
        /// CSV 형식 파일 저장
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool WriteCSVFile(string path, string[] content)
        {
            bool result;
            try
            {
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs, Encoding.Default))
                {
                    foreach (var tag in content)
                    {
                        sw.WriteLine(tag);
                    }
                }
                AppData.Instance.MsgIRDC.Info(AppData.AppLog, nameof(FileHandler), $"CSV File({path}) Write 저장완료");
                result = true;
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, nameof(FileHandler), $"CSV File({path}) Write 에러", ex);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// CSV 형식 파일 저장
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool WriteTagCSVFile(string path, string[] content, FileMode mode = FileMode.Append)
        {
            bool result;
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach(var tag in content)
                {
                    sb.AppendLine(tag);
                }
                //Encoding 방식 변경
                using (var fs = new FileStream(path, mode, FileAccess.Write))
                using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
                {
                    foreach (var tag in content)
                    {
                        sw.WriteLine(tag);
                    }
                }
                AppData.Instance.MsgIRDC.Info(AppData.AppLog, nameof(FileHandler), $"CSV File({path}) Write 저장완료");
                result = true;
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, nameof(FileHandler), $"CSV File({path}) Write 에러", ex);
                result = false;
            }

            return result;
        }
        /// <summary>
        /// 마지막 Tag 저장 시간 입력
        /// </summary>
        /// <param name="path">SCADA별 폴더경로</param>
        /// <param name="name">SCADA Node Name</param>
        /// <param name="workingtime">Tag Load 시간</param>
        /// <returns></returns>
        public static bool WriteLastWorkingTime(string path, string name, DateTime workingtime)
        {
            bool result;
            //파일 Name 등록
            try
            {
                var fullPath = Path.Combine(path, $"{name}.config");
                using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    //마지막 Tag Load 시간 등록
                    sw.Write($"{workingtime:G}");
                }
                AppData.Instance.MsgIRDC.Info(AppData.AppLog, nameof(FileHandler), $"Config File 등록");
                result = true;
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, nameof(FileHandler), $"Config File Write 에러", ex);
                result = false;
            }

            return result;
        }
        /// <summary>
        /// 마지막 Tag 저장 시간 Load
        /// </summary>
        /// <param name="path">SCADA 폴더경로</param>
        /// <param name="name">SCADA Node Name</param>
        /// <returns>마지막 Tag Load 시간 <see cref="DateTime"/></returns>
        public static DateTime LoadLastWorkingTime(string path, string name)
        {
            DateTime dt = DateTime.Now;
            try
            {
                var lastTime = string.Empty;
                var fullPath = Path.Combine(path, $"{name}.config");
                using (var fs = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Read))
                using (var sr = new StreamReader(fs, GetEncoding(fullPath)))
                {
                    lastTime = sr.ReadLine();
                }
                dt = Convert.ToDateTime(lastTime);
                AppData.Instance.MsgIRDC.Info(AppData.AppLog, nameof(FileHandler), $"Config File Load 성공");
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, nameof(FileHandler), $"Config File Load 에러", ex);
            }

            return dt;
        }
        /// <summary>
        /// File Read 시 사용될 Encoding 함수 (해당 File의 Encoding 확인)
        /// </summary>
        /// <param name="filename">file Fullpath</param>
        /// <returns>파일 <see cref="Encoding"/></returns>
        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM(Byte Order Mark)
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
                return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe)
                return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff)
                return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
                return Encoding.UTF32;
            return Encoding.Default;
        }
    }
}
