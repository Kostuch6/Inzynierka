using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using State = System.Collections.Generic.Dictionary<string, int>;

namespace Inzynierka
{
    

    class Program
    {
        static void Main(string[] args)
        {

        }

        public void StateInit(State state)
        {
            state.Add("HP", 100);
            state.Add("enemyHP", 100);
            state.Add("distance", 5);
            state.Add("isInDanger", 0);
            state.Add("enemyIsInDanger", 0);
            state.Add("isDefending", 0);
            state.Add("enemyIsDefending", 0);
        }
    }
}
