using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inzynierka
{
    abstract class Node
    {
        public int Key { get; set; }
        public Node leftChild { get; set; }
        public Node rightChild { get; set; }

        public abstract String decide(int[] myState, int[] enemyState);
    }
}
