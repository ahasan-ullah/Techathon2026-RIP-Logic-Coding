using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public RoomType Type { get; set; }

        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
