using System.Collections.Generic;
using Core.Common.ServiceBus.Dtos;

namespace GUI.Core.Tree.Helpers
{
    public class TransformerModel : IIdentifiedObject
    {
        public TransformerModel()
        {
        }

        public TransformerModel(TransformerWindingDto winding, RatioTapChangerDto tapChanger = null, PowerTransformerDto powerTransformer = null)
        {
            Description = winding.Description;
            GID = winding.GID;
            MRID = winding.MRID;
            Name = winding.Name;
            Terminals = winding.Terminals;
            EquipmentContainer = winding.EquipmentContainer;
            Measurements = winding.Measurements;
            PowerTransformer = powerTransformer;
            RatioTapChanger = tapChanger;
        }

        public string Description { get; set; }
        public long GID { get; set; }
        public string MRID { get; set; }
        public string Name { get; set; }

        public PowerTransformerDto PowerTransformer { get; set; }
        public RatioTapChangerDto RatioTapChanger { get; set; }
        public List<long> Terminals { get; set; } = new List<long>();
        public long EquipmentContainer { get; set; } = 0;
        public List<long> Measurements { get; set; } = new List<long>();
    }
}
