using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Device
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string Name { get; set; }
        public DeviceType Type { get; set; }
        public int RatedPowerWatts { get; set; }

        public Room Room { get; set; } = null!;
        public DeviceState? DeviceState { get; set; }
        public ICollection<DeviceStateHistory> History { get; set; } = new List<DeviceStateHistory>();
    }
}
