using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inzynierka
{
    class ResultNode : Node
    {
        public String Result { get; set; }

        public override string decide(int[] myState, int[] enemyState)
        {
            return Result;
        }
    }
}
