using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Model.INI
{
    public class LogModel : ModelBase
    {
        public LogModel()
        {
            FileName = string.Empty;
            Path = string.Empty;
            Pattern = string.Empty;
            MinLevel = string.Empty;
            MaxLevel = string.Empty;
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                if(_fileName != value)
                {
                    _fileName = value;
                    RaisePropertyChanged(nameof(FileName));
                }
            }
        }
        private string _fileName;

        public string Path
        {
            get => _path;
            set
            {
                if(_path != value)
                {
                    _path = value;
                    RaisePropertyChanged(nameof(Path));
                }
            }
        }
        private string _path;

        public string Pattern
        {
            get => _pattern;
            set
            {
                if(_pattern != value)
                {
                    _pattern = value;
                    RaisePropertyChanged(nameof(Pattern));
                }
            }
        }
        private string _pattern;

        public string MinLevel
        {
            get => _minLevel;
            set
            {
                if(_minLevel != value)
                {
                    _minLevel = value;
                    RaisePropertyChanged(nameof(MinLevel));
                }
            }
        }
        private string _minLevel;

        public string MaxLevel
        {
            get => _maxLevel;
            set
            {
                if(_maxLevel != value)
                {
                    _maxLevel = value;
                    RaisePropertyChanged(nameof(MaxLevel));
                }
            }
        }
        private string _maxLevel;

    }

}
