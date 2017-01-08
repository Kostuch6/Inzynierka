using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using State = System.Collections.Generic.Dictionary<string, int>;

namespace Inzynierka
{
    public abstract class Node
    {
		public int Key { get; set; }
		public Node leftChild { get; set; }
        public Node rightChild { get; set; }

        public abstract Move decide(State state);

		public int count()
		{
			int left = leftChild == null ? 0 : leftChild.count();
			int right = rightChild == null ? 0 : rightChild.count();

			return left + right + 1;
		}
	}
}
