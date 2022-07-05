using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Models
{
    public class NodeService
    {
        public static List<setData> paramLst(List<setData> lst,double t)
        {
            var dem = 1;
            var tog=new List<setData>();
            if (lst.Count == 0)
            {
                return tog;
            }
            var max = lst.Count < 6 ? lst.Count : 6;
            while(dem <= max)
            {
                foreach(var item in lst)
                {
                    if (item.Time >= (t - 3600000 * dem - 1000)  && item.Time  <= (t- 3600000 * dem + 1000))
                    {
                        tog.Add(item);
                    }
                }
                dem++;
            }
            return tog;
        }
        public static List<setData> paramAVER(List<setData> lst,long t,double he_so)
        {
            var dem = 1;
            var tog = new List<setData>();
        
            if (lst.Count == 0)
            {
                return tog;
            }
            var max = lst.Count < 6 ? lst.Count : 6;
            while (dem <= max)
            {
                var lan_dau = false;
                var xac_dinh = false;
                foreach (var item in lst)
                {
                    if (item.Time > (t - 3600000 * dem - 1000) && item.Time < (t - 3600000 * dem + 1000))
                    {
                        if (lan_dau == false)
                        {
                            tog.Add(item);
                            lan_dau = true;
                        }
                        else
                        {
                            tog[tog.Count - 1].value += item.value;
                        }
                        xac_dinh = true;
                    }

                }

                if (!xac_dinh)
                {
                    tog.Add(new setData()
                    {
                        Time = t - 3600000 * dem,
                        value = 0
                    });
                }
                tog[tog.Count - 1].value = tog[tog.Count - 1].value / he_so;
                dem++;
            }
            
            
            return tog;
        }
        public static List<setData> paramAddition(List<setData> lst)
        {
            var dem = 1;
            
            var max = lst[lst.Count - 1].Time;
            if(lst.Count == 6)
            {
                return lst;
            }
            if (lst.Count > 6)
            {
                lst.RemoveAt(0);
            }
            while(dem < 7)
            {
                var xac_dinh = false;
                foreach(var item in lst)
                {
                    if (item.Time > (max - 3600000 * dem - 1000) && item.Time < (max - 3600000* dem + 1000))
                    {
                        xac_dinh = true;

                    }
                }
                if (!xac_dinh)
                {
                    var item = new setData()
                    {
                        Time = max - 3600000 * dem,
                        value = 0
                    };
                    lst.Add(item);
                }
            }
            lst.Sort(delegate (setData x, setData y)
            {
                return x.Time.CompareTo(y.Time);
            });
            return lst;
            
        }
        public static nodeSumlstString ConvertSumToString(nodeSumlst sumlst,nodeSum nodeSum)
        {
            var lst = new nodeSumlstString();
            lst.sum = new nodeSum(nodeSum.nhiet_do, nodeSum.gas, nodeSum.khoi, nodeSum.do_am);
            lst.nhiet_do = new List<setDataString>();
            lst.gas = new List<setDataString>();
            lst.khoi = new List<setDataString>();
            lst.do_am = new List<setDataString>();
            foreach(var item in sumlst.nhiet_do)
            {
                lst.nhiet_do.Add(new setDataString() {Time=timeOfDay(item.Time),value=item.value });
            }
            foreach (var item in sumlst.gas)
            {
                lst.gas.Add(new setDataString() { Time = timeOfDay(item.Time), value = item.value });
            }
            foreach (var item in sumlst.khoi)
            {
                lst.khoi.Add(new setDataString() { Time = timeOfDay(item.Time), value = item.value });
            }
            foreach (var item in sumlst.do_am)
            {
                lst.do_am.Add(new setDataString() { Time = timeOfDay(item.Time), value = item.value });
            }
            return lst;
        }
        public static string timeOfDay(double time)
        {
            string startdatetime =time.ToString();
            long elapsed_ms = long.Parse(startdatetime);

            DateTime date = NodeService.FromUnixTimeMilliseconds(elapsed_ms).UtcDateTime;
            string timeOfDay = date.ToString("hh:mm tt");
            return timeOfDay;

        }
        public static DateTimeOffset FromUnixTimeMilliseconds(long milliseconds)
        {
            
            long ticks = milliseconds * 10000 + 621355968000000000L;
            return new DateTimeOffset(ticks, TimeSpan.Zero);
        }
    }
}
