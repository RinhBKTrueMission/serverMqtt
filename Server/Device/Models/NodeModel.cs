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
        public nodeSum() {
            this.nhiet_do = new setData() { Time=0,value=0};
            this.gas = new setData() { Time = 0, value = 0 };
            this.khoi = new setData() { Time = 0, value = 0 };
            this.do_am = new setData() { Time = 0, value = 0 };
        }
        public nodeSum(setData nhiet_do, setData gas, setData khoi, setData do_am)
        {
            this.nhiet_do = nhiet_do;
            this.gas = gas;
            this.khoi = khoi;
            this.do_am = do_am;
        }

        public setData nhiet_do { get; set; }//0
        public setData gas { get; set; }//1
        public setData khoi { get; set; }//2
        public setData do_am { get; set; }//3
    }

    public class nodeSumlst
    {
        public List<setData> nhiet_do { get; set; }//0
        public List<setData> gas { get; set; }//1
        public List<setData> khoi { get; set; }//2
        public List<setData> do_am { get; set; }//3
    }
    public class setDataString
    {
        public string Time { get; set; }
        public double value { get; set; }
    }
    public class nodeSumlstString
    {
        public nodeSum sum { get; set; }
        public List<setDataString> nhiet_do { get; set; }//0
        public List<setDataString> gas { get; set; }//1
        public List<setDataString> khoi { get; set; }//2
        public List<setDataString> do_am { get; set; }//3
    }
    public class ObjValue
    {
        public string Id { get; set; }
        public setData Value { get; set; }
    }
    public class ObjValueList
    {
        public string Id { get; set; }
        public List<setData> Value { get; set; }
    }
    public class ObjRoom
    {
        public int Id { get; set; }
        public List<string> nodeId { get; set; }
    }
    public class ObjBuiding
    {
        public int Id { get; set; }
        public List<int> floors { get; set; }
        
    }
    public class ObjFloor
    {
        public int Id { get; set; }
        public List<int> rooms { get; set; }
    }

}
