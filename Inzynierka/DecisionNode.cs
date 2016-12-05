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

        public override String decide(int[] myState, int[] enemyState)
        {
            if (Decision.test(myState, enemyState))
            {
                return leftChild.decide(myState, enemyState);
            }
            else
            {
                return rightChild.decide(myState, enemyState);
            }
        }
    }
}
