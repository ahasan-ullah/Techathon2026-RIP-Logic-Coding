using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class DeviceStateHistory
    {
        public long Id { get; set; }
        public int DeviceId { get; set; }
        public bool IsOn { get; set; }
        public DateTime ChangedAt { get; set; }

        public Device Device { get; set; } = null!;
    }
}
