using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iWaterDataCollector.Model.Tag
{
    public class KafkaTagModel
    {
        public KafkaTagModel(string name, bool isPulse)
        {
            Name = name;
            IsPulse = isPulse;
        }

        public string Name { get; set; }

        public bool IsPulse { get; set; }
        public PulseTagModel pulse { get; set; } = new PulseTagModel();
    }
}
