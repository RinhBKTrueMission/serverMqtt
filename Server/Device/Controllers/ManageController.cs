using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Device.Models;
using Newtonsoft.Json;
namespace Device.Controllers
{
    class ManageController : Vst.Server.SlaveController
    {
        public object DeviceList()
        {
            var uc = new ManageController();
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            var lstDevice = uc.NodeDb.GetAll();           
            return Response(lstDevice);

        }
        public object ControlDevice()
        {

            var data = this.ServerContext.Value.ToString();
            List<string> cs = data.Split('/').ToList<string>();
            var clientId = cs[0];
            var SetDivce = cs[1];
            var topic = "response/default/" + clientId;
            return Response(topic, SetDivce);
        }
        public object UpdateDevice()
        {
            var d = this.ServerContext.Value;
            var data = Json.Convert<DeviceModel>(d);
            var uc = new ManageController();
            uc.DeviceDb = new Vst.Server.Data.DeviceData(uc.MainDb.PhysicalPath);
            uc.DeviceDb.Update(data.Id, data);
            return Response(null);
        }
        public object UpdateNode()
        {
            var d = this.ServerContext.Value.ToString();
            var data = JsonConvert.DeserializeObject<NodeModel>(d);
            var uc = new ManageController();
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            var newV = uc.NodeDb.FindById(data.Id);
            var newValue = JsonConvert.DeserializeObject<NodeModel>(newV.ToString());
            if (newValue.listData.Count==0 && data.listData.Count!=0)
            {
                newValue.listData = new List<setData>();
                newValue.listData.AddRange(data.listData);
                //newValue.function = data.function;
            }
            else if(newValue.listData.Count != 0 && data.listData.Count != 0)
            {
                newValue.listData.AddRange(data.listData);
                //newValue.function = data.function;
            }
            else if (newValue.listData.Count == 0 && data.listData.Count == 0)
            {
                newValue.listData = new List<setData>();
            }
            else
            {

            }
            newValue.status = data.status;
            newValue.location = data.location;
            newValue.function = data.function;
                uc.NodeDb.Update(data.Id, newValue);
            return STATUS(200);
        }
        public object UpdateData()
        {
            var d = this.ServerContext.Value.ToString();
            var data = JsonConvert.DeserializeObject<ObjValue>(d);
            var uc = new ManageController();
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            var newV = uc.NodeDb.FindById(data.Id);
            var newValue = JsonConvert.DeserializeObject<NodeModel>(newV.ToString());
            if (newValue.listData.Count == 0)
            {
                newValue.listData = new List<setData>();
                newValue.listData.Add(data.Value);
               
            }
            else
            {
                newValue.listData.Add(data.Value);
                
            }

            uc.NodeDb.Update(data.Id, newValue);
            return STATUS(200);
        }
        public object UpdateDataList()
        {
            var d = this.ServerContext.Value.ToString();
            var data = JsonConvert.DeserializeObject<ObjValueList>(d);
            var uc = new ManageController();
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            var newV = uc.NodeDb.FindById(data.Id);
            var newValue = JsonConvert.DeserializeObject<NodeModel>(newV.ToString());
            if (newValue.listData.Count == 0)
            {
                newValue.listData = new List<setData>();
                newValue.listData.AddRange(data.Value);

            }
            else
            {
                newValue.listData.AddRange(data.Value);

            }

            uc.NodeDb.Update(data.Id, newValue);
            return STATUS(200);
        }

        public object Find()
        {
            var Id = this.ServerContext.Value.ToString();
            var uc = new ManageController();
            uc.DeviceDb = new Vst.Server.Data.DeviceData(uc.MainDb.PhysicalPath);
            var data = uc.DeviceDb.FindById(Id);
            var vm = Json.Convert<DeviceModel>(data);
            vm.Name = "200";
            return Response(vm);
        }

        public object NodeSum()
        {
            var uc = new ManageController();
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            var lstDevice = uc.NodeDb.GetAll();
            var sum = new nodeSum();
            
            int x1 , x2, x3, x4;
            x1 = x2 = x3 = x4 = 0;
            var paramlst = new List<setData>();
            var paramlst1 = new List<setData>();
            var paramlst2= new List<setData>();
            var paramlst3 = new List<setData>();
            long time = 0;
            foreach (NodeModel device in JsonConvert.DeserializeObject<List<NodeModel>>(JsonConvert.SerializeObject(lstDevice)))
            {
                var lst = device.listData.ToArray();
                if (lst.Length == 0)
                {
                    continue;
                }
                time = lst[lst.Length - 1].Time;
                
                switch (device.function)
                {
                    case 0:
                        sum.nhiet_do.value += lst[lst.Length-1].value;
                        sum.nhiet_do.Time = time;
                        paramlst.AddRange(NodeService.paramLst(device.listData, time));
                        x1++;
                        break;
                    case 1:
                        sum.gas.value += lst[lst.Length - 1].value;
                        sum.gas.Time= time;
                        paramlst1.AddRange(NodeService.paramLst(device.listData, time));
                        x2++;
                        break;
                    case 2:
                        sum.khoi.value += lst[lst.Length - 1].value;
                        sum.khoi.Time= time;
                       paramlst2.AddRange(NodeService.paramLst(device.listData, time));
                        x3++;
                        break;
                    case 3:
                        sum.do_am.value += lst[lst.Length - 1].value;
                        sum.do_am.Time= time;   
                        paramlst3.AddRange(NodeService.paramLst(device.listData, time));
                        x4++;
                        break;

                }
               
                
            }
            sum.nhiet_do.value = sum.nhiet_do.value / x1;
            sum.khoi.value = sum.khoi.value / x3;
            sum.gas.value = sum.gas.value / x2;
            sum.do_am.value = sum.do_am.value / x4;
            var sumlst = new nodeSumlst()
            {
                nhiet_do = new List<setData>(),
                gas = new List<setData>(),
                do_am = new List<setData>(),    
                khoi = new List<setData>(), 
            };
            sumlst.nhiet_do.AddRange(NodeService.paramAddition(NodeService.paramAVER(paramlst,sum.nhiet_do.Time,x1)));
            sumlst.nhiet_do.Reverse();
            sumlst.nhiet_do.Add(sum.nhiet_do);
            sumlst.khoi.AddRange(NodeService.paramAddition(NodeService.paramAVER(paramlst2, sum.khoi.Time, x2)));
            sumlst.khoi.Reverse();
            sumlst.khoi.Add(sum.khoi);
            sumlst.gas.AddRange(NodeService.paramAddition(NodeService.paramAVER(paramlst1, sum.gas.Time, x3)));
            sumlst.gas.Reverse();
            sumlst.gas.Add(sum.gas);
            sumlst.do_am.AddRange(NodeService.paramAddition(NodeService.paramAVER(paramlst3,sum.do_am.Time, x4)));
            sumlst.do_am.Reverse();
            sumlst.do_am.Add(sum.do_am);

            return Response("response/tong", Json.Convert<nodeSumlstString>(NodeService.ConvertSumToString(sumlst,sum)));

        }
        public object NodeList()
        {
            var uc = new ManageController();
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            var lstDevice = uc.NodeDb.GetAll();
            return Response(lstDevice);
        }
        public object Alert()
        {
            var node = this.ServerContext.Value;
            var data = Json.Convert<NodeModel>(node);
            var uc = new ManageController();
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            //uc.NodeDb.Update(data.Id, data);

            return Response("alert",data);
        }
        public object NodeSumFloor()
        {
            var Id = this.ServerContext.Value.ToString();
            var uc = new ManageController();
            uc.BuildDb = new Vst.Server.Data.BuildingData(uc.MainDb.PhysicalPath);
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
           
            var lstDevice = uc.NodeDb.GetAll();
           
            var build =JsonConvert.DeserializeObject<Building>(uc.BuildDb.FindById("B1").ToString());
            var lst1 = new List<string>();
            foreach(var item in build.floors)
            {
                if (item.Id == Id)
                {
                    foreach(var v in item.rooms)
                    {
                        if(v.NodeIds.Count== 0)
                        {
                            continue;
                        }
                        else
                        {
                            lst1.AddRange(v.NodeIds);
                        }
                        
                        
                    }
                }
            }
            var lstNode = new List<NodeModel>();
            foreach(var item in lst1)
            {
                var value = JsonConvert.DeserializeObject<NodeModel>(uc.NodeDb.FindById(item).ToString());
                if( value != null)
                {
                    lstNode.Add(value);
                }

            }
            var sum = new nodeSum();
        
            int x1, x2, x3, x4;
            x1 = x2 = x3 = x4 = 0;
            var paramlst = new List<setData>();
            var paramlst1 = new List<setData>();
            var paramlst2 = new List<setData>();
            var paramlst3 = new List<setData>();
            long time = 0;
            foreach (NodeModel device in lstNode)
            {
                if (device.listData.Count == 0)
                {
                    continue;
                }
                var lst = device.listData.ToArray();
                time = lst[lst.Length - 1].Time;

                switch (device.function)
                {
                    case 0:
                        sum.nhiet_do.value += lst[lst.Length - 1].value;
                        sum.nhiet_do.Time = time;
                        paramlst.AddRange(NodeService.paramLst(device.listData, time));
                        x1++;
                        break;
                    case 1:
                        sum.gas.value += lst[lst.Length - 1].value;
                        sum.gas.Time = time;
                        paramlst1.AddRange(NodeService.paramLst(device.listData, time));
                        x2++;
                        break;
                    case 2:
                        sum.khoi.value += lst[lst.Length - 1].value;
                        sum.khoi.Time = time;
                        paramlst2.AddRange(NodeService.paramLst(device.listData, time));
                        x3++;
                        break;
                    case 3:
                        sum.do_am.value += lst[lst.Length - 1].value;
                        sum.do_am.Time = time;
                        paramlst3.AddRange(NodeService.paramLst(device.listData, time));
                        x4++;
                        break;

                }


            }
            sum.nhiet_do.value = sum.nhiet_do.value / x1;
            sum.khoi.value = sum.khoi.value / x3;
            sum.gas.value = sum.gas.value / x2;
            sum.do_am.value = sum.do_am.value / x4;
            var sumlst = new nodeSumlst()
            {
                nhiet_do = new List<setData>(),
                gas = new List<setData>(),
                do_am = new List<setData>(),
                khoi = new List<setData>(),
            };
            sumlst.nhiet_do.AddRange(NodeService.paramAddition(NodeService.paramAVER(paramlst, sum.nhiet_do.Time, x1)));
            sumlst.nhiet_do.Reverse();
            sumlst.nhiet_do.Add(sum.nhiet_do);
            sumlst.khoi.AddRange(NodeService.paramAddition(NodeService.paramAVER(paramlst2, sum.khoi.Time, x2)));
            sumlst.khoi.Reverse();
            sumlst.khoi.Add(sum.khoi);
            sumlst.gas.AddRange(NodeService.paramAddition(NodeService.paramAVER(paramlst1, sum.gas.Time, x3)));
            sumlst.gas.Reverse();
            sumlst.gas.Add(sum.gas);
            sumlst.do_am.AddRange(NodeService.paramAddition(NodeService.paramAVER(paramlst3, sum.do_am.Time, x4)));
            sumlst.do_am.Reverse();
            sumlst.do_am.Add(sum.do_am);

            return Response("response/tong/"+Id, Json.Convert<nodeSumlstString>(NodeService.ConvertSumToString(sumlst, sum)));

        }
        public object RoomList()
        {
            var id= this.ServerContext.Value.ToString();
            var uc = new ManageController();
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            var lstBuild = uc.BuildDb.GetAll();
            var build = JsonConvert.DeserializeObject<Building>(lstBuild.First().ToString());
            var floor= new Flooring();
            foreach(var item in build.floors)
            {
                if(item.Id == id)
                {
                    floor = item;
                }
            }
            var roomlst = floor;
            return Response("response/roomlst", roomlst);

        }
        public object NodeSumRoom()
        {
            var Id = this.ServerContext.Value.ToString();
            var uc = new ManageController();
            uc.BuildDb = new Vst.Server.Data.BuildingData(uc.MainDb.PhysicalPath);
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            var lstDevice = uc.NodeDb.GetAll();
            var build = JsonConvert.DeserializeObject<Building>(uc.BuildDb.FindById("B1").ToString());
            var lst1 = new List<string>();
            var xt = false;
            foreach (var item in build.floors)
            {
                
                    foreach (var v in item.rooms)
                    {
                        if(v.Id == Id)
                        {
                            if (v.NodeIds.Count == 0)
                            {
                                continue;
                            }
                            else
                            {
                            lst1.AddRange(v.NodeIds);
                            xt = true;
                            break;
                        }
                       
                        }
                        


                    }
                if (xt)
                {
                    break;
                }
                
            }
            var lstNode = new List<NodeModel>();
            foreach (var item in lst1)
            {
                var value = JsonConvert.DeserializeObject<NodeModel>(uc.NodeDb.FindById(item).ToString());
                if (value != null)
                {
                    lstNode.Add(value);
                }

            }
            var sum = new nodeSum();

            int x1, x2, x3, x4;
            x1 = x2 = x3 = x4 = 0;
            var paramlst = new List<setData>();
            var paramlst1 = new List<setData>();
            var paramlst2 = new List<setData>();
            var paramlst3 = new List<setData>();
            long time = 0;
            foreach (NodeModel device in lstNode)
            {
                var lst = device.listData.ToArray();
                time = lst[lst.Length - 1].Time;

                switch (device.function)
                {
                    case 0:
                        sum.nhiet_do.value += lst[lst.Length - 1].value;
                        sum.nhiet_do.Time = time;
                        paramlst.AddRange(NodeService.paramLst(device.listData, time));
                        x1++;
                        break;
                    case 1:
                        sum.gas.value += lst[lst.Length - 1].value;
                        sum.gas.Time = time;
                        paramlst1.AddRange(NodeService.paramLst(device.listData, time));
                        x2++;
                        break;
                    case 2:
                        sum.khoi.value += lst[lst.Length - 1].value;
                        sum.khoi.Time = time;
                        paramlst2.AddRange(NodeService.paramLst(device.listData, time));
                        x3++;
                        break;
                    case 3:
                        sum.do_am.value += lst[lst.Length - 1].value;
                        sum.do_am.Time = time;
                        paramlst3.AddRange(NodeService.paramLst(device.listData, time));
                        x4++;
                        break;

                }


            }
            sum.nhiet_do.value = sum.nhiet_do.value / x1;
            sum.khoi.value = sum.khoi.value / x3;
            sum.gas.value = sum.gas.value / x2;
            sum.do_am.value = sum.do_am.value / x4;
            var sumlst = new nodeSumlst()
            {
                nhiet_do = new List<setData>(),
                gas = new List<setData>(),
                do_am = new List<setData>(),
                khoi = new List<setData>(),
            };
            sumlst.nhiet_do.AddRange(NodeService.paramAddition(NodeService.paramAVER(paramlst, sum.nhiet_do.Time, x1)));
            sumlst.nhiet_do.Reverse();
            sumlst.nhiet_do.Add(sum.nhiet_do);
            sumlst.khoi.AddRange(NodeService.paramAddition(NodeService.paramAVER(paramlst2, sum.khoi.Time, x2)));
            sumlst.khoi.Reverse();
            sumlst.khoi.Add(sum.khoi);
            sumlst.gas.AddRange(NodeService.paramAddition(NodeService.paramAVER(paramlst1, sum.gas.Time, x3)));
            sumlst.gas.Reverse();
            sumlst.gas.Add(sum.gas);
            sumlst.do_am.AddRange(NodeService.paramAddition(NodeService.paramAVER(paramlst3, sum.do_am.Time, x4)));
            sumlst.do_am.Reverse();
            sumlst.do_am.Add(sum.do_am);

            return Response("response/tong/" + Id, Json.Convert<nodeSumlstString>(NodeService.ConvertSumToString(sumlst, sum)));

        }
        public object NodeListinRoom()
        {
            var id = this.ServerContext.Value.ToString();
            var uc = new ManageController();
            uc.BuildDb = new Vst.Server.Data.BuildingData(uc.MainDb.PhysicalPath);
            var lstBuild = uc.BuildDb.GetAll();
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            //var  nodelist = uc.NodeDb.GetAll();
            //var lstNode = JsonConvert.DeserializeObject <List<NodeModel>> (JsonConvert.SerializeObject(nodelist));
            var build =JsonConvert.DeserializeObject<Building>(lstBuild.First().ToString());
            var room = new Room();
            var xt = false;
            foreach (var item in build.floors)
            {
                foreach(var v in item.rooms)
                {
                    if (v.Id == id)
                    {
                        room = v;
                        xt = true;
                    }
                    if (xt) { break; }
                }
                if (xt) { break; }
                
            }
            var value = new List<NodeModel>();
            var nodelst = room.NodeIds;
            foreach (var node in nodelst)
            {
                var v = uc.NodeDb.FindById(node);
                if(v != null)
                {
                    value.Add(JsonConvert.DeserializeObject<NodeModel>(v.ToString()));
                }
            }
            return Response("response/nodelstInRoom", value);
        }
    }
}
