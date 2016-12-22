using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using State = System.Collections.Generic.Dictionary<string, int>;

namespace Inzynierka
{
    

    public class Program
    {
        static void Main(string[] args)
        {
			State state = StateInit();
			State maxValues = MaxValuesInit();
			List<Test> tests = TestsInit();
			DecisionTree tree = new DecisionTree(new List<int> { 100, 100, 50, 0 }, state, maxValues, tests);
			Console.WriteLine("udao sie");
			Console.WriteLine(tree.decide(state).ToString());

			//petla walki
			int turnsLeft = 100;
			while(turnsLeft > 0 || state["HP"] <= 0 || state["enemyHP"] <= 0)
			{
				MakeMove(tree.decide(state), state); //TODO tree wrzucone na chwile, powinien byc osobnik z populacji
				//EnemyMakeMove(enemy.decide(state), state); //TODO jakies drzewo przechowujace przeciwnika albo ustalonego z gory albo z poprzedniej populacji
				turnsLeft--;
			}

			Console.ReadLine();
		}

		static void MakeMove(Move move, State state)
		{
			if(state["enemyIsInDanger"] == 1)
			{
				if (state["enemyIsDefending"] == 1)
				{
					state["enemyHP"] -= Const.RocketDmg / 2;
				}
				else
				{
					state["enemyHP"] -= Const.RocketDmg;
				}
				state["enemyIsInDanger"] = 0;
            }
			switch (move)
			{
				case Move.SHORT_MOVE_FORWARD:
					{
						if (state["distance"] >= 1)
						{
							state["distance"] -= 1;
							state["isInDanger"] = 0;
						}
					}
					break;
				case Move.SHORT_MOVE_BACK:
					{
						state["distance"] += 1;
						state["isInDanger"] = 0;
					}
					break;
				case Move.LONG_MOVE_FORWARD:
					{
						if (state["distance"] >= 2)
						{
							state["distance"] -= 2;
							state["isInDanger"] = 0;
						}
						else if (state["distance"] >= 1)
						{
							state["distance"] -= 1;
							state["isInDanger"] = 0;
						}
					}
					break;
				case Move.LONG_MOVE_BACK:
					{
						state["distance"] += 2;
						state["isInDanger"] = 0;
					}
					break;
				case Move.DEFEND:
					{
						state["isDefending"] = 1;
					}
					break;
				case Move.ATTACK_FAR:
					{
						if(state["distance"] <= Const.FarRange)
						{
							if (state["enemyIsDefending"] == 1)
							{
								state["enemyHP"] -= Const.FarDmg / 2;
							}
							else
							{
								state["enemyHP"] -= Const.FarDmg;
							}
						}
					}
					break;
				case Move.ATTACK_CLOSE:
					{
						if (state["distance"] <= Const.CloseRange)
						{
							if (state["enemyIsDefending"] == 1)
							{
								state["enemyHP"] -= Const.CloseDmg / 2;
							}
							else
							{
								state["enemyHP"] -= Const.CloseDmg;
							}
						}
					}
					break;
				case Move.ATTACK_ROCKET:
					{
						state["enemyIsInDanger"] = 1;
					}
					break;
				case Move.SHORT_MOVE_FORWARD_THEN_ATTACK:
					{
						if (state["distance"] >= 1)
						{
							state["distance"] -= 1;
							state["isInDanger"] = 0;
						}
						if (state["distance"] <= Const.CloseRange)
						{
							if (state["enemyIsDefending"] == 1)
							{
								state["enemyHP"] -= Const.CloseDmg / 2;
							}
							else
							{
								state["enemyHP"] -= Const.CloseDmg;
							}
						}
					}
					break;
				case Move.SHORT_MOVE_BACK_THEN_ATTACK:
					{
						state["distance"] += 1;
						state["isInDanger"] = 0;

						if (state["distance"] <= Const.CloseRange)
						{
							if (state["enemyIsDefending"] == 1)
							{
								state["enemyHP"] -= Const.CloseDmg / 2;
							}
							else
							{
								state["enemyHP"] -= Const.CloseDmg;
							}
						}
					}
					break;
				case Move.ATTACK_THEN_SHORT_MOVE_FORWARD:
					{
						if (state["distance"] <= Const.CloseRange)
						{
							if (state["enemyIsDefending"] == 1)
							{
								state["enemyHP"] -= Const.CloseDmg / 2;
							}
							else
							{
								state["enemyHP"] -= Const.CloseDmg;
							}
						}

						if (state["distance"] >= 1)
						{
							state["distance"] -= 1;
							state["isInDanger"] = 0;
						}
					}
					break;
				case Move.ATTACK_THEN_SHORT_MOVE_BACK:
					{
						if (state["distance"] <= Const.CloseRange)
						{
							if (state["enemyIsDefending"] == 1)
							{
								state["enemyHP"] -= Const.CloseDmg / 2;
							}
							else
							{
								state["enemyHP"] -= Const.CloseDmg;
							}
						}

						state["distance"] += 1;
						state["isInDanger"] = 0;
					}
					break;
				default:
					break;
			}
		}

		static void EnemyMakeMove(Move move, State state)
		{
			if (state["isInDanger"] == 1)
			{
				if (state["isDefending"] == 1)
				{
					state["HP"] -= Const.RocketDmg / 2;
				}
				else
				{
					state["HP"] -= Const.RocketDmg;
				}
				state["isInDanger"] = 0;
			}
			switch (move)
			{
				case Move.SHORT_MOVE_FORWARD:
					{
						if (state["distance"] >= 1)
						{
							state["distance"] -= 1;
							state["enemyIsInDanger"] = 0;
						}
					}
					break;
				case Move.SHORT_MOVE_BACK:
					{
						state["distance"] += 1;
						state["enemyIsInDanger"] = 0;
					}
					break;
				case Move.LONG_MOVE_FORWARD:
					{
						if (state["distance"] >= 2)
						{
							state["distance"] -= 2;
							state["enemyIsInDanger"] = 0;
						}
						else if (state["distance"] >= 1)
						{
							state["distance"] -= 1;
							state["enemyIsInDanger"] = 0;
						}
					}
					break;
				case Move.LONG_MOVE_BACK:
					{
						state["distance"] += 2;
						state["enemyIsInDanger"] = 0;
					}
					break;
				case Move.DEFEND:
					{
						state["enemyIsDefending"] = 1;
					}
					break;
				case Move.ATTACK_FAR:
					{
						if (state["distance"] <= Const.FarRange)
						{
							if (state["isDefending"] == 1)
							{
								state["HP"] -= Const.FarDmg / 2;
							}
							else
							{
								state["HP"] -= Const.FarDmg;
							}
						}
					}
					break;
				case Move.ATTACK_CLOSE:
					{
						if (state["distance"] <= Const.CloseRange)
						{
							if (state["isDefending"] == 1)
							{
								state["HP"] -= Const.CloseDmg / 2;
							}
							else
							{
								state["HP"] -= Const.CloseDmg;
							}
						}
					}
					break;
				case Move.ATTACK_ROCKET:
					{
						state["isInDanger"] = 1;
					}
					break;
				case Move.SHORT_MOVE_FORWARD_THEN_ATTACK:
					{
						if (state["distance"] >= 1)
						{
							state["distance"] -= 1;
							state["enemyIsInDanger"] = 0;
						}
						if (state["distance"] <= Const.CloseRange)
						{
							if (state["isDefending"] == 1)
							{
								state["HP"] -= Const.CloseDmg / 2;
							}
							else
							{
								state["HP"] -= Const.CloseDmg;
							}
						}
					}
					break;
				case Move.SHORT_MOVE_BACK_THEN_ATTACK:
					{
						state["distance"] += 1;
						state["enemyIsInDanger"] = 0;

						if (state["distance"] <= Const.CloseRange)
						{
							if (state["isDefending"] == 1)
							{
								state["HP"] -= Const.CloseDmg / 2;
							}
							else
							{
								state["HP"] -= Const.CloseDmg;
							}
						}
					}
					break;
				case Move.ATTACK_THEN_SHORT_MOVE_FORWARD:
					{
						if (state["distance"] <= Const.CloseRange)
						{
							if (state["isDefending"] == 1)
							{
								state["HP"] -= Const.CloseDmg / 2;
							}
							else
							{
								state["HP"] -= Const.CloseDmg;
							}
						}

						if (state["distance"] >= 1)
						{
							state["distance"] -= 1;
							state["enemyIsInDanger"] = 0;
						}
					}
					break;
				case Move.ATTACK_THEN_SHORT_MOVE_BACK:
					{
						if (state["distance"] <= Const.CloseRange)
						{
							if (state["isDefending"] == 1)
							{
								state["HP"] -= Const.CloseDmg / 2;
							}
							else
							{
								state["HP"] -= Const.CloseDmg;
							}
						}

						state["distance"] += 1;
						state["enemyIsInDanger"] = 0;
					}
					break;
				default:
					break;
			}
		}

		public static State StateInit()
        {
			State state = new State();
			state.Add("HP", 100);
            state.Add("enemyHP", 100);
            state.Add("distance", 5);
            state.Add("isInDanger", 0);
            state.Add("enemyIsInDanger", 0);
            state.Add("isDefending", 0);
            state.Add("enemyIsDefending", 0);
			return state;
        }

        public static State MaxValuesInit()
        {
			State maxValues = new State();
			maxValues.Add("HP", 100);
            maxValues.Add("enemyHP", 100);
            maxValues.Add("distance", 20);
            maxValues.Add("isInDanger", 1);
            maxValues.Add("enemyIsInDanger", 1);
            maxValues.Add("isDefending", 1);
            maxValues.Add("enemyIsDefending", 1);
			return maxValues;
        }

		public static List<Test> TestsInit()
		{
			List<Test> Tests = new List<Test>();
            Tests.Add((a, b) => a > b);
			Tests.Add((a, b) => a >= b);
			Tests.Add((a, b) => a < b);
			Tests.Add((a, b) => a <= b);
			Tests.Add((a, b) => a == b);
			Tests.Add((a, b) => a != b);
			return Tests;
		}
    }
}
