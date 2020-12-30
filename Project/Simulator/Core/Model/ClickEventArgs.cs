using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Core.Model
{
    public class ClickEventArgs : EventArgs
    {
        public Point Point { get; set; }
    }
}
