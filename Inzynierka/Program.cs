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
			int iterations = 100;
            int populationSize = 50;
			double crossoverProb = 0.01;
			double mutationProb = 0.001;
			Random r = new Random();
            List<DecisionTree> population = new List<DecisionTree>();
            List<int> fitness = new List<int>();
            DecisionTree enemy = new DecisionTree(new List<int> { 100, 100, 50, 0 }, state, maxValues, tests); //TODO enemy powinien byc staly
            //DecisionTree tree = new DecisionTree(new List<int> { 100, 100, 50, 0 }, state, maxValues, tests);
            //Console.WriteLine("udao sie");
            //Console.WriteLine(tree.decide(state).ToString());

            //population init
            for (int i = 0; i < populationSize; i++)
            {
                population.Add(new DecisionTree(new List<int> { 100, 100, 50, 0 }, state, maxValues, tests));
            }

			while (iterations > 0) //glowna petla programu, ustalona ilosc iteracji, pozniej dodac zatrzymanie po stagnacji
			{
				for (int j = 0; j < populationSize; j++)
				{
					//petla walki
					int turn = 1;
					while (turn <= 100 && state["HP"] > 0 && state["enemyHP"] > 0)
					{
						Console.WriteLine("Osobnik {0}, tura: {1}", j, turn);
						MakeMove(population[j].decide(state), state);
						EnemyMakeMove(enemy.decide(state), state);
						turn++;
					}
					Console.WriteLine("HP -> {0}", state["HP"]);
					Console.WriteLine("enemyHP -> {0}", state["enemyHP"]);
					Console.WriteLine("distance -> {0}", state["distance"]);
					Console.WriteLine("isInDanger -> {0}", state["isInDanger"]);
					Console.WriteLine("enemyIsInDanger -> {0}", state["enemyIsInDanger"]);
					Console.WriteLine("isDefending -> {0}", state["isDefending"]);
					Console.WriteLine("enemyIsDefending -> {0}", state["enemyIsDefending"]);
					state = StateInit();
					//Console.ReadLine();

					fitness[j] = Evaluate(state, maxValues, turn);
				}
				List<DecisionTree> SelectedIndividuals = new List<DecisionTree>();
				//selekcja

				population.Clear(); // usuniecie starej populacji

				//wybranie osobnikow do krzyzowania, przepisanie reszty do nowej populacji
				List<DecisionTree> IndividualsToCross = new List<DecisionTree>();
				for (int i = 0; i < populationSize; i++)
				{
					if(r.NextDouble() <= crossoverProb)
					{
						IndividualsToCross.Add(SelectedIndividuals[i]);
					}
					else
					{
						population.Add(SelectedIndividuals[i]);
					}
				}
				//zapewnienie parzystej ilosci osobnikow do krzyzowania
				if(IndividualsToCross.Count % 2 != 0)
				{
					IndividualsToCross.RemoveAt(IndividualsToCross.Count - 1);
				}

				//krzyzowanie
				while(IndividualsToCross.Count > 0)
				{
					DecisionTree Individual1 = IndividualsToCross[r.Next(IndividualsToCross.Count)];
					IndividualsToCross.Remove(Individual1);
					DecisionTree Individual2 = IndividualsToCross[r.Next(IndividualsToCross.Count)];
					IndividualsToCross.Remove(Individual2);
					Crossover(Individual1, Individual2);
					population.Add(Individual1);
					population.Add(Individual2);
				}

				//mutacja

				//wybranie najlepszego z poprzedniej populacji i ustawienie na enemy
				iterations--;
			}
            
			Console.ReadLine();
		}

		static void MakeMove(Move move, State state)
		{
			Console.WriteLine("Ruch osobnika: " +move.ToString());
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
			if(state["enemyHP"] < 0)
			{
				state["enemyHP"] = 0;
			}
		}

		static void EnemyMakeMove(Move move, State state)
		{
			Console.WriteLine("Ruch przeciwnika: " + move.ToString());

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
			if (state["HP"] < 0)
			{
				state["HP"] = 0;
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

		public static int Evaluate(State state, State maxValues, int turn)
		{
			int fitness = ((maxValues["enemyHP"] - state["enemyHP"]) * 2) - state["HP"] - turn;
			if(state["HP"] == 0)
			{
				fitness /= 2;
			}
			return fitness;
		}

		public static void Crossover( DecisionTree Individual1, DecisionTree Individual2)
		{
			//TODO sposob krzyzowania
		}

        //TODO metoda wyswietlajaca drzewo
    }
}
