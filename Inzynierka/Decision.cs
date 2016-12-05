using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inzynierka
{
    public delegate bool Test(int[] myState, int[] enemyState, int param);

    class Decision
    {
        public Test Test { get; set; }
        public int Param { get; set; }

        public bool test(int[] myState, int[] enemyState)
        {
            return Test(myState, enemyState, Param);
        }
    }
}
