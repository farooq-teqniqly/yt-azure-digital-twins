using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWineRack.Commands.Models
{
    public class Bottle
    {
        public Bottle(int slot, string upcCode)
        {
            Slot = slot;
            UpcCode = upcCode;
        }

        public string UpcCode { get; }

        public int Slot { get; }
    }
}
