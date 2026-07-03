using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Alert
    {
        public int Id { get; set; }
        public int? RoomId { get; set; }
        public int? DeviceId { get; set; }
        public AlertType Type { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime TriggeredAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public Room? Room { get; set; } = null!;
        public Device? Device { get; set; } = null!;
    }
}
