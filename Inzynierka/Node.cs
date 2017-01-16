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

		public int Count()
		{
			int left = leftChild == null ? 0 : leftChild.Count();
			int right = rightChild == null ? 0 : rightChild.Count();

			return left + right + 1;
		}

		public int RecalculateKeys(Node node, int elemCount = 0)
		{
			elemCount++;
			node.Key = elemCount;
			if(node.leftChild != null)
			{
				elemCount = RecalculateKeys(node.leftChild, elemCount);
			}
			if(node.rightChild != null)
			{
				elemCount = RecalculateKeys(node.rightChild, elemCount);
			}
			return elemCount;
		}
	}
}
