using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWineRack.Commands.Models
{
    public class Bottle
    {
        private readonly int _slot;
        private readonly string _upcCode;

        public Bottle(int slot, string upcCode)
        {
            _slot = slot;
            _upcCode = upcCode;
        }

        public string UpcCode => _upcCode;
        public int Slot => _slot;
    }
}
