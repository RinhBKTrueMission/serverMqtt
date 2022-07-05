using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Models
{
    public class Building
    {
        public string Id { get; set; }
        public List<Flooring> floors { get; set; }
    }
    public class Flooring
    {
        public string Id { get; set; }
        public List<Room> rooms { get; set; }

    }
    public class Room
    {
        public string Id { get; set; }
        public List<string> NodeIds { get; set; }

    }
}
