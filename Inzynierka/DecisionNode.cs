using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inzynierka
{
    class DecisionNode : Node
    {
        public Decision Decision { get; set; }

        public override String decide(Dictionary<string, int> state)
        {
            if (Decision.test(state))
            {
                return leftChild.decide(state);
            }
            else
            {
                return rightChild.decide(state);
            }
        }
    }
}
