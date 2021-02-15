using System.Collections.Generic;
using GUI.Models.Schema;

namespace GUI.Core.Tree.Helpers
{
    public class FastLookupByMrid : FastLookup<string>
    {
        public FastLookupByMrid(EquipmentTreeNode root) : base(root, keysSelector: MridSelector)
        {
        }

        private static IEnumerable<string> MridSelector(EquipmentTreeNode node)
        {
            if (node.Item is TransformerModel transformer)
            {
                return new[]
                {
                    transformer?.MRID,
                    transformer?.PowerTransformer?.MRID,
                    transformer?.RatioTapChanger?.MRID
                };
            }

            return new[] { node.Item?.MRID };
        }
    }
}
