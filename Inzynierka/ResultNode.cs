using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using State = System.Collections.Generic.Dictionary<string, int>;

namespace Inzynierka
{
    public class ResultNode : Node
    {
        public Move Move { get; set; }

        public ResultNode(Move move)
        {
            Move = move;
        }

        public override Move decide(State state)
        {
            return Move;
        }
    }
}
