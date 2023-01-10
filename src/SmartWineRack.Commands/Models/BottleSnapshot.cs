using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartWineRack.Commands.Models
{
    public class BottleSnapshot
    {
        private readonly IList<Bottle> _bottles;

        public BottleSnapshot(IList<Bottle> bottles)
        {
            _bottles = bottles;
        }

        public IEnumerable<Bottle> Bottles => new ReadOnlyCollection<Bottle>(_bottles);
    }
}
