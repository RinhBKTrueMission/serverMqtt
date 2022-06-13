using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Controllers
{
    using Models;
    using System.Mvc;

    class HomeController : Mqtt.Client.Controller
    {
        static Device _device;
        public static Device Device => _device;
        public object Default()
        {
            if (_device == null)
            {
                _device = new Device {
                    Id = "ZERO-AKS-V2-DEMO",
                    Name = "Test Device"
                };
                _device.StatusChanged += (i, o) => {
                    Publish("device", "status/default", _device);
                };
                ClientStatus.ConnectionLost += () => {
                    _device.SetLed(2, 0);
                };
                ClientStatus.Ready += () => {
                    _device.SetLed(2, 1);
                     // Publish("device", "manage/listdevice", null);
                    if (_device.Id[0] == 'Z')
                    {
                        Publish("device", "config/deviceid", null);
                    }
                    else
                    {
                        Publish("device", "config/time", null);
                        Publish("device", "manage/find", _device.Id);

                    }
                };
                Connect(_device.Id, 5);
            }

            return View(_device);
        }
        public object Input(string index)
        {
            int mask = 1 << (index[0] & 15);
            var high = _device.Inputs & mask;
            if (high == 0)
            {
                _device.Inputs |= mask;
                _device.Outputs |= mask;
            }
            else
            {
                _device.Inputs &= ~mask;
            }
            _device.RaiseStatusChanged();
            return Done();
        }
        public object Keyboard(string key)
        {
            switch (key[key.Length - 1])
            {
                case '#':
                    _device.Outputs = 0;
                    _device.RaiseStatusChanged();
                    break;
            }
            return Done();
        }
        public object Exit()
        {
            Disconnect();
            AsyncEngine.End();

            return null;
        }
    }
}
