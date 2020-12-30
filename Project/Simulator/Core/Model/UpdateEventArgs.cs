using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Core.Model
{
    public class UpdateEventArgs : EventArgs
    {
        public List<Point> Points { get; set; }
    }
}
