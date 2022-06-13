using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Models
{
    class Device
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Inputs { get; set; }
        public int Outputs { get; set; }
        public int Leds { get; set; } = 3;

        public event Action<int, int> StatusChanged;
        public void RaiseStatusChanged()
        {
            StatusChanged?.Invoke(Inputs, Outputs);
        }

        internal void SetLed(int index, int value)
        {
            int old = Leds;
            int mask = 1 << index;

            if (value == 0)
            {
                Leds &= ~mask;
            }
            else
            {
                Leds |= mask;
            }
            if (old != Leds)
            {
                RaiseStatusChanged();
            }    
        }
    }
}
