using iWaterDataCollector.Global;
using iWaterDataCollector.Model.INI;
using System.ComponentModel;

/********************************************
 * Kafka Server(Consumer) Setting Page
 * Kafka(Consumer) 연결 정보설정 화면 ViewModel
 * (20240707) 소스 정리 완료
 ********************************************/
namespace iWaterDataCollector.ViewModel.UserControl
{
    public class KafkaSettingUCViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Binding Data
        /// <summary>
        /// Kafka 설정정보
        /// </summary>
        public KafkaModel Kafka { get; set; } = new KafkaModel();
        #endregion
        /// <summary>
        /// 생성자
        /// </summary>
        public KafkaSettingUCViewModel()
        {
            BindData();
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
            Kafka = AppData.Instance.KAFKA;
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
