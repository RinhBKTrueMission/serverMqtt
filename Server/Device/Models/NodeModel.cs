using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Models
{
    public class NodeModel
    {
        public string Id { get; set; }
        /*
         0 : bình thường
         1 : sự cố
         2 : báo nguy cơ cháy
         */
        public int status { get; set; }
        /*
         0 báo nhiệt
         1 báo gas
         2 báo khói
         3 cảm biến độ ẩm
         */
        public int function { get; set; }
        public List<setData> listData { get; set; }
        public string location { get; set; }

    }

    public class setData
    {
        public long Time { get; set; }
        public double value { get; set; }
    }
    public class nodeSum
    {
        public double nhiet_doi { get; set; }//0
        public double gas { get; set; }//1
        public double khoi { get; set; }//2
        public double do_am { get; set; }//3
    }
}
