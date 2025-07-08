using log4net;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Reflection;

/********************************************
 * Ssystem.IO를 이용한 Directory 활용 Class
 * (20240707) 소스 정리 완료
 ********************************************/
namespace iWaterDataCollector.Global.Handler
{
    public static class DirectoryHandler
    {
        /// <summary>
        /// Info Directory Get
        /// </summary>
        /// <remarks>
        /// Info 폴더 경로 확인
        /// </remarks>
        /// <param name="path">시스템 기본 경로</param>
        /// <returns>Info 폴더 경로</returns>
        public static string InfoDirectory(string path) => Path.Combine(path, Code.INFO);
        /// <summary>
        /// Recovery Directory Get
        /// </summary>
        /// <remarks>
        /// Setting 파일 백업 경로 확인
        /// </remarks>
        /// <param name="path">시스템 기본 경로</param>
        /// <returns>파일 백업 폴더 경로</returns>
        public static string RecoveryDirectory(string path) => Path.Combine(path, Code.BACKUP);
        /// <summary>
        /// 경로 선택 Dialog
        /// </summary>
        /// <param name="current">현재 폴더 경로</param>
        /// <param name="path">선택된 경로</param>
        /// <returns>Dialog Result <see cref="Boolean"/></returns>
        /// <remarks>
        /// 폴더 경로를 선택합니다.
        /// </remarks>
        public static bool GetDirectory(string current, out string path)
        {
            bool resutl = false;
            path = string.Empty;
            try
            {
                CommonOpenFileDialog dlg = new CommonOpenFileDialog
                {
                    InitialDirectory = current,
                    IsFolderPicker = true
                };

                if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    path = dlg.FileName;
                    AppData.Instance.MsgIRDC.Info(AppData.AppLog, nameof(DirectoryHandler), "폴더 경로 선택 완료");
                    resutl = true;
                }
            }
            catch(Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, nameof(DirectoryHandler), "폴더 경로 선택 오류", ex);
                resutl = false;
            }
            return resutl;
        }
        /// <summary>
        /// 파일 경로 선택 Dialog
        /// </summary>
        /// <param name="current">현재 파일 경로</param>
        /// <param name="path">선택된 경로</param>
        /// <returns>Dialog Result <see cref="Boolean"/></returns>
        /// <remarks>
        /// 파일의 경로를 선택합니다.
        /// </remarks>
        public static bool GetFileDirectory(string current, out string path)
        {
            bool result = false;
            path = string.Empty;
            try
            {
                CommonOpenFileDialog dlg = new CommonOpenFileDialog
                {
                    Multiselect = false
                };
                if (string.IsNullOrEmpty(current) == false)
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(current);
                }

                if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    path = dlg.FileName;
                    AppData.Instance.MsgIRDC.Info(AppData.AppLog, nameof(DirectoryHandler), "파일 경로 선택 완료");
                    result = true;
                }
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, nameof(DirectoryHandler), "파일 경로 선택 오류", ex);
                result = false;
            }

            return result;
        }
        /// <summary>
        /// 기본 경로 생성
        /// </summary>
        /// <remarks>
        /// 기본 경로가 없는 경우 프로그램 실행경로를 기본경로로 하여 Path 생성
        /// </remarks>
        /// <param name="path">기본 경로</param>
        /// <param name="name">파일명</param>
        /// <returns>생성된 기본경로</returns>
        public static string GetDefaultDirectory(string path, string name)
        {
            try
            {
                var retPath = string.IsNullOrEmpty(path)
                    ? Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), name)
                    : Path.Combine(path, name);
                return retPath;
            }
            catch(Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, nameof(DirectoryHandler), "Default Directory 오류", ex);
                return string.Empty;
            }
            
        }
        /// <summary>
        /// 기본 폴더 구성 생성
        /// </summary>
        /// <remarks>
        /// 기본 경로 내에 존재하여야 하는 내부폴더를 생성
        /// </remarks>
        /// <param name="path">기본 경로</param>
        public static void CreateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    var di = Directory.CreateDirectory(path);
                    di.CreateSubdirectory(Code.INFO);
                    di.CreateSubdirectory(Code.RECOVERY);
                }
            }
            catch (Exception ex)
            {
                AppData.Instance.MsgIRDC.Error(AppData.ErrorLog, nameof(DirectoryHandler), "Create Directory 오류", ex);
            }
        }
    }
}
