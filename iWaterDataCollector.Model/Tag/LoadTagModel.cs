using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Model.Tag
{
    public class LoadTagModel
    {
        public static readonly string F_CV = "F_CV";
        public LoadTagModel()
        {
            IsCheck = false;
            Name = string.Empty;
            FullName = string.Empty;
            Description = string.Empty;
            Node = string.Empty;
            TagID = string.Empty;
            Field = F_CV;
            TimeStamp = string.Empty;
            Value = string.Empty;
            Quality = "0";
        }

        public LoadTagModel(string name, string desc)
        {
            IsCheck = false;
            Name = name;
            FullName = name;
            Description = desc;
            Node = string.Empty;
            TagID = string.Empty;
            Field = F_CV;
            TimeStamp = string.Empty;
            Value = string.Empty;
            Quality = "0";
        }

        public LoadTagModel(string name, string description, bool ischeck = false)
        {
            IsCheck = ischeck;
            Name = name;
            Description = description;
            Node = string.Empty;
            TagID = string.Empty;
            Field = F_CV;
            TimeStamp = string.Empty;
            Value = string.Empty;
            Quality = "0";
        }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string Description { get; set; }

        public string Node { get; set; }

        public string TagID { get; set; }

        public string Field { get; set; }

        public string TimeStamp { get; set; }

        public string Value { get; set; }

        public string Quality { get; set; }

        public bool IsCheck { get; set; }

        public static implicit operator string(LoadTagModel x) { return x.CSVFormat(); }

        public string CSVFormat()
        {
            //20240718
            var words = Name.Split('.');
            string tag;
            if (words.Length > 2 && words[2] == F_CV)
            {
                tag = words[1];
            }
            else
            {
                tag = Name;
            }
            return string.Join(",", tag, TimeStamp, Value, Quality);
        }

        public string CSVFormat(string date)
        {
            //20240718
            var words = Name.Split('.');
            string tag;
            if (words.Length > 2 && words[2] == F_CV)
            {
                tag = words[1];
            }
            else
            {
                tag = Name;
            }
            return string.Join(",", tag, Description, date);
        }
    }
}
