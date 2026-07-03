using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class RoomStatusDto
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public List<DeviceDto> Devices { get; set; } = new();
        public int DevicesOnCount { get; set; }
        public int CurrentPowerWatts { get; set; }
    }
}
