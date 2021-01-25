using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;

namespace SCADA.Common.Messaging
{
    public interface IDNP3Function
    {
        byte[] PackRequest();
        Dictionary<Tuple<RegisterType, ushort>, BasePoint> PareseResponse(byte[] response);    //povratna vrednost tip registra indeks vrednost
    }
}
