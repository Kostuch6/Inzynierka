using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inzynierka
{
    enum Move
    {
        SHORT_MOVE_FORWARD,
        SHORT_MOVE_BACK,
        LONG_MOVE_FORWARD,
        LONG_MOVE_BACK,
        DEFEND,
        ATTACK_FAR,
        ATTACK_CLOSE,
        ATTACK_ROCKET,
        SHORT_MOVE_FORWARD_THEN_ATTACK,
        SHORT_MOVE_BACK_THEN_ATTACK,
        ATTACK_THEN_SHORT_MOVE_FORWARD,
        ATTACK_THEN_SHORT_MOVE_BACK
    }
}
