using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inzynierka
{
    public delegate bool Test(int stat, int param);

    class Decision
    {
        public string Statistic { get; set; }
        public Test Test { get; set; }
        public int Param { get; set; }

        public bool test(Dictionary<string, int> state)
        {
            return Test(state[Statistic], Param);
        }
    }
}
