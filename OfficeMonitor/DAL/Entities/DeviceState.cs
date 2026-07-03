using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class DeviceState
    {
        public int DeviceId { get; set; }
        public bool IsOn { get; set; }
        public DateTime LastChangedAt { get; set; }

        public Device Device { get; set; } = null!;
    }
}
