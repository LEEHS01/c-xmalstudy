using iWaterDataCollector.Global;
using iWaterDataCollector.Global.Handler;
using iWaterDataCollector.Model.INI;
using Microsoft.Toolkit.Mvvm.Input;
using System.ComponentModel;

/********************************************
 * Directory Setting Page
 * 경로 설정 화면 ViewModel
 * Log Set Name → Program
 * (20240707) 소스 정리 완료
 ********************************************/
namespace iWaterDataCollector.ViewModel.UserControl
{
    public class DirectorySettingUCViewModel : ViewModelBase, IDataErrorInfo
    {
        #region User CommandParameter
        /// <summary>
        /// Default Path Setting Button
        /// </summary>
        private const string COMMAND_DEFAULT = "Default";
        /// <summary>
        /// Kafak Path Setting Button
        /// </summary>
        private const string COMMAND_KAFKA = "Kafka";
        /// <summary>
        /// PDB Path Setting Button
        /// </summary>
        private const string COMMAND_PDB = "PDB";
        #endregion
        #region Binding Data
        /// <summary>
        /// Directory Class Variable
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public DirectoryModel Directory { get; set; } = new DirectoryModel();
        #endregion
        /// <summary>
        /// 생성자
        /// </summary>
        public DirectorySettingUCViewModel()
        {
            BindData();
            BindCommand();
        }
        
        #region 초기값 등록
        /// <summary>
        /// Binding 되는 Variable 설정 함수
        /// </summary>
        /// <remarks>
        /// 화면에 보여지는 초기값을 설정하는 Function
        /// </remarks>
        private void BindData()
        {
            //설정파일의 경로 정보 값으로 초기화
            Directory = AppData.Instance.DIRECTORY;
        }
        #endregion
        #region 명령등록
        private void BindCommand()
        {
            Command = new RelayCommand<object>(Command_Handler);
        }
        /// <summary>
        /// 화면 버튼 움직임에 대한 Command Handler 함수
        /// </summary>
        /// <remarks>
        /// 화면에서 버튼을 클릭할 시 연결되는 Command Handler Function
        /// </remarks>
        /// <param name="obj">버튼 CommandName</param>
        private void Command_Handler(object obj)
        {
            if (obj == null)
                return;
            var command = obj.ToString();
            var path = string.Empty;
            switch(command)
            {
                //기본경로에 대한 설정
                case COMMAND_DEFAULT:
                    
                    if (DirectoryHandler.GetDirectory(Directory.Default, out path))
                    {
                        Directory.Default = path;
                    }
                    break;
                //Kafka Tag 파일 경로에 대한 설정
                case COMMAND_KAFKA:
                    if (DirectoryHandler.GetFileDirectory(Directory.Kafka, out path))
                    {
                        Directory.Kafka = path;
                    }
                    break;
                //PDB Load Tag 설정 파일 경로에 대한 설정
                case COMMAND_PDB:
                    if (DirectoryHandler.GetFileDirectory(Directory.PDB, out path))
                    {
                        Directory.PDB = path;
                    }
                    break;
                default:
                    break;
            }
            
        }
        #endregion

        #region IDataErrorInfo 인터페이스 Member
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    default: return string.Empty;
                }
            }
        }
        public string Error
        {
            get
            {
                return string.Empty;
            }
        }
        #endregion
    }
}
