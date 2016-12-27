using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using State = System.Collections.Generic.Dictionary<string, int>;

namespace Inzynierka
{
    public class DecisionNode : Node
    {
        public Decision Decision { get; set; }

		public DecisionNode(Decision decision)
		{
			Decision = decision;
		}

        public override Move decide(State state)
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
