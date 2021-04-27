using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Core.Common.Contracts
{
    [ServiceContract]
    public interface IConfigurationChange
    {
        [OperationContract]
        void UpdateConfig(Tuple<ushort, ushort, ushort, ushort> points, Dictionary<string, ushort> pairs);
        [OperationContract]
        void SimulationSettings(bool enable);
    }
}
