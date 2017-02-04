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
			//State state = StateInit();
			State state = new State();
			StateInit(state);
			State maxValues = MaxValuesInit();
			List<Test> tests = TestsInit();
			int iterations = 100;
            int populationSize = 20;
			double crossoverProb = 0.1;
			double mutationProb = 0.1;
			double averageFitness = 0;
			CryptoRandom r = new CryptoRandom();
            List<DecisionTree> population = new List<DecisionTree>();
			DecisionTree enemy = new DecisionTree(new List<int> { 100, 100, 50, 0 }, state, maxValues, tests, r); 
			//DecisionTree enemy = CreateEnemy(r, state, tests);

			//population init
			for (int i = 0; i < populationSize; i++)
            {
                population.Add(new DecisionTree(new List<int> { 100, 100, 50, 0 }, state, maxValues, tests, r));
				//Console.WriteLine("Wielkosc drzewa: {0}", population[i].elementCount);
				//Console.WriteLine("Wielkosc drzewa': {0}", population[i].CountElements());
			}

			while (iterations > 0) //glowna petla programu, ustalona ilosc iteracji, pozniej dodac zatrzymanie po stagnacji
			{
				for (int j = 0; j < populationSize; j++)
				{
					//petla walki
					int turn = 1;
					while (turn <= 99 && state["HP"] > 0 && state["enemyHP"] > 0)
					{
						//Console.WriteLine("Osobnik {0}, tura: {1}", j, turn);
						EnemyMakeMove(enemy.decide(state), state);
						MakeMove(population[j].decide(state), state);
						turn++;
					}
					//Console.WriteLine("HP -> {0}", state["HP"]);
					//Console.WriteLine("enemyHP -> {0}", state["enemyHP"]);
					//Console.WriteLine("distance -> {0}", state["distance"]);
					//Console.WriteLine("isInDanger -> {0}", state["isInDanger"]);
					//Console.WriteLine("enemyIsInDanger -> {0}", state["enemyIsInDanger"]);
					//Console.WriteLine("isDefending -> {0}", state["isDefending"]);
					//Console.WriteLine("enemyIsDefending -> {0}", state["enemyIsDefending"]);
					//Console.ReadLine();

					population[j].fitness = Evaluate(state, maxValues, turn, population[j].elementCount);

					//state = StateInit();
					StateInit(state);
				}

				averageFitness = population.Sum(p => p.fitness) / populationSize;
				Console.WriteLine("srednia wartosc funkcji oceny: " + averageFitness);
				Console.WriteLine("najgorszy osobnik: " + population.Min(p => p.fitness));
				Console.WriteLine("najlepszy osobnik: " + population.Max(p => p.fitness));

				//selekcja
				List<DecisionTree> SelectedIndividuals = EliteRankingSelection(population, populationSize, r);

				//wybranie najlepszego z poprzedniej populacji i ustawienie na enemy
				//enemy = population.OrderByDescending(p => p.fitness).First().Clone();

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
					population.Add(IndividualsToCross.Last());
					IndividualsToCross.RemoveAt(IndividualsToCross.Count - 1);
				}

				Console.WriteLine("Skrzyzowane osobniki: " + IndividualsToCross.Count);
				//krzyzowanie
				while(IndividualsToCross.Count > 0)
				{
					DecisionTree Individual1 = IndividualsToCross[r.Next(0,IndividualsToCross.Count)];
					IndividualsToCross.Remove(Individual1);
					DecisionTree Individual2 = IndividualsToCross[r.Next(0,IndividualsToCross.Count)];
					IndividualsToCross.Remove(Individual2);
					Crossover(Individual1, Individual2, r);
					population.Add(Individual1);
					population.Add(Individual2);
				}

				int mutationCount = 0;
				//mutacja
				for (int i = 0; i < populationSize; i++)
				{
					if(r.NextDouble() <= mutationProb)
					{
						Mutation(population[i], r, state, maxValues, tests);
						mutationCount++;
					}
				}
				Console.WriteLine("Zmutowane osobniki: " + mutationCount);
				
				iterations--;
				Console.ReadLine();
			}
            
			Console.ReadLine();
		}

		static void MakeMove(Move move, State state)
		{
			//Console.WriteLine("Ruch osobnika: " + move.ToString());
			if (state["enemyIsInDanger"] == 1)
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
			//Console.WriteLine("Ruch przeciwnika: " + move.ToString());

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

		public static void StateInit(State state)
        {
			//State state = new State();
			state.Clear();
			state.Add("HP", 100);
            state.Add("enemyHP", 100);
            state.Add("distance", 5);
            state.Add("isInDanger", 0);
            state.Add("enemyIsInDanger", 0);
            state.Add("isDefending", 0);
            state.Add("enemyIsDefending", 0);
			//return state;
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

		public static double Evaluate(State state, State maxValues, int turn, int elementCount) //TODO kara za wielkosc 
		{
			double fitness = (((maxValues["enemyHP"] - state["enemyHP"]) * 2) + state["HP"] - turn);
			if (state["HP"] == 0)
			{
				fitness /= 2;
			}
			if(elementCount > 10)
			{
				fitness *= 10.0 / elementCount;
			}
			return fitness;
		}

		public static void Crossover( DecisionTree individual1, DecisionTree individual2, CryptoRandom r)
		{
			//wybranie punktu przeciecia z zakresu elementCount
			//dla obu osobnikow
			//zamiana miejscami tylko poddrzew wybranych elementow

			int locus1;
			int locus2;
			Node node1;
			Node node2;

			do
			{
				locus1 = r.Next(1, individual1.elementCount + 1);
				node1 = individual1.Find(locus1, individual1.root);

			} while (node1 is ResultNode);

			do
			{
				locus2 = r.Next(1, individual2.elementCount + 1);
				node2 = individual2.Find(locus2, individual2.root);

			} while (node2 is ResultNode);

			Node tempNode = null;

			tempNode = node1.leftChild;
			node1.leftChild = node2.leftChild;
			node2.leftChild = tempNode;

			tempNode = null;

			tempNode = node1.rightChild;
			node1.rightChild = node2.rightChild;
			node2.rightChild = tempNode;

			individual1.RecalculateKeys();
			individual2.RecalculateKeys();
		}

		public static void CrossoverWithParent(DecisionTree individual1, DecisionTree individual2, CryptoRandom r)
		{
			int locus1;
			int locus2;
			Node node1;
			Node node2;
			Node parent1;
			Node parent2;

			locus1 = r.Next(2, individual1.elementCount + 1);
			node1 = individual1.Find(locus1, individual1.root);
			parent1 = individual1.FindParentOfNode(locus1, individual1.root);

			locus2 = r.Next(2, individual2.elementCount + 1);
			node2 = individual2.Find(locus2, individual2.root);
			parent2 = individual2.FindParentOfNode(locus2, individual2.root);

			if(parent1.leftChild.Equals(node1))
			{
				parent1.leftChild = node2;
			}
			else
			{
				parent1.rightChild = node2;
			}

			if (parent2.leftChild.Equals(node2))
			{
				parent2.leftChild = node1;
			}
			else
			{
				parent2.rightChild = node1;
			}

			individual1.RecalculateKeys();
			individual2.RecalculateKeys();
		}

		public static List<DecisionTree> EliteRankingSelection(List<DecisionTree> population, int populationSize, CryptoRandom r)
		{
			population = population.OrderBy(p => p.fitness).ToList();
			List<DecisionTree> SelectedIndividuals = new List<DecisionTree>();
			//SelectedIndividuals.Add(population[populationSize-1]); //przepisanie najlepszego osobnika bez zmian

			int totalSum = 0;
			for (int i = 1; i < populationSize; i++)
			{
				totalSum += i;
			}
			for (int i = 1; i <= populationSize; i++) //petla wybierajaca osobniki
			{
				int selectedInt = r.Next(1, totalSum + 1); // losowy int z przedzialu
				int sum = 0;
				for (int j = 1; j <= populationSize; j++) //petla sprawdzaja ktory osobnik zostal wybrany
				{
					sum += j;
					if(selectedInt <=sum)
					{
						SelectedIndividuals.Add(population[j].Clone());
						break;
					}
				}
			}

			return SelectedIndividuals;
		}

		public static void Mutation(DecisionTree individual, CryptoRandom r, State state, State maxValues, List<Test> tests)
		{
			int node = r.Next(1, individual.elementCount + 1);
			Node nodeToMutate = individual.Find(node, individual.root);
			if(nodeToMutate is DecisionNode)
			{
				DecisionNode newNode = individual.generateDecisionNode(state, maxValues, tests);
				((DecisionNode)nodeToMutate).Decision = newNode.Decision;
			}
			else if(nodeToMutate is ResultNode)
			{
				ResultNode newNode = individual.GenerateResultNode();
				((ResultNode)nodeToMutate).Move = newNode.Move;
			}
		}

		public static DecisionTree CreateEnemy(CryptoRandom r, State state, List<Test> tests)
		{
			ResultNode node3 = new ResultNode { Key = 3, Move = Move.LONG_MOVE_FORWARD };
			ResultNode node5 = new ResultNode { Key = 5, Move = Move.LONG_MOVE_FORWARD };
			ResultNode node6 = new ResultNode { Key = 6, Move = Move.ATTACK_ROCKET };
			ResultNode node9 = new ResultNode { Key = 9, Move = Move.SHORT_MOVE_FORWARD_THEN_ATTACK };
			ResultNode node10 = new ResultNode { Key = 10, Move = Move.SHORT_MOVE_FORWARD };
			ResultNode node11 = new ResultNode { Key = 11, Move = Move.ATTACK_FAR };
			DecisionNode node4 = new DecisionNode { Key = 4, Decision = new Decision { Statistic = "enemyIsInDanger", Param = 0, Test = tests[0] }, leftChild = node5, rightChild = node6 };
			DecisionNode node8 = new DecisionNode { Key = 8, Decision = new Decision { Statistic = "distance", Param = 4, Test = tests[2] }, leftChild = node9, rightChild = node10 };
			DecisionNode node2 = new DecisionNode { Key = 2, Decision = new Decision { Statistic = "enemyIsDefending", Param = 0, Test = tests[0] }, leftChild = node3, rightChild = node4 };
			DecisionNode node7 = new DecisionNode { Key = 7, Decision = new Decision { Statistic = "enemyIsInDanger", Param = 0, Test = tests[0] }, leftChild = node8, rightChild = node11 };
			DecisionNode node1 = new DecisionNode { Key = 1, Decision = new Decision { Statistic = "distance", Param = 6, Test = tests[0] }, leftChild = node2, rightChild = node7 };
			return new DecisionTree { R = r, elementCount = 11, fitness = 0, root = node1 };
		}

		//TODO metoda wyswietlajaca drzewo
	}
}
