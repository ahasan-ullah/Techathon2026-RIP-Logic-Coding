using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class UsageDto
    {
        public int CurrentTotalWatts { get; set; }
        public double TodayEstimatedKwh { get; set; }
        public List<RoomUsageBreakdownDto> RoomBreakdown { get; set; } = new();
    }

    public class RoomUsageBreakdownDto
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int CurrentWatts { get; set; }
    }
    }
}
