using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Model.INI
{
    public class DirectoryModel : ModelBase
    {
        public DirectoryModel()
        {
            Default = string.Empty;
            PDB = string.Empty;
            Kafka = string.Empty;
        }
        /// <summary>
        /// 프로그램 기본 경로
        /// </summary>
        public string Default
        {
            get => _default;
            set
            {
                if (_default != value)
                {
                    _default = value;
                    RaisePropertyChanged(nameof(Default));
                }
            }
        }
        private string _default;
        /// <summary>
        /// PDB 파일 경로
        /// </summary>
        public string PDB
        {
            get => _pdb;
            set
            {
                if (_pdb != value)
                {
                    _pdb = value;
                    RaisePropertyChanged(nameof(PDB));
                }
            }
        }
        private string _pdb;
        /// <summary>
        /// Kafka 파일 경로
        /// </summary>
        public string Kafka
        {
            get => _kafka;
            set
            {
                if (_kafka != value)
                {
                    _kafka = value;
                    RaisePropertyChanged(nameof(Kafka));
                }
            }
        }
        private string _kafka;
    }
}
