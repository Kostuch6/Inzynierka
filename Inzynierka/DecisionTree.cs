﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using State = System.Collections.Generic.Dictionary<string, int>;

namespace Inzynierka
{
    public class DecisionTree
    {
        static Random r = new Random();

		public int elementCount { get; set; } = 0;

		private Node root;

        public DecisionTree(List<int> probabilityList, State state, State maxValues, List<Test> tests)
        {
            DecisionNode parent = generateDecisionNode(state, maxValues, tests);
			//Console.WriteLine("root: Stat -> {0}, Test -> {1}, Param -> {2}",parent.Decision.Statistic, parent.Decision.Test.ToString(), parent.Decision.Param);
            root = parent;
            generateTree(probabilityList, parent, 1, state, maxValues, tests);
        }

        public Move decide(State state)
        {
            return root.decide(state);
        }

        //utworzenie wezla (tylko w rootcie)

        // sprawdzenie czy powinien miec lewy wezel
            //TAK
                //utworzenie wezla, podczepienie do leftChild
                //rekurencja od leftChild
            //NIE
                //utworzenie liscia

        // sprawdzenie czy powinien miec prawy wezel
            //TAK
                //utworzenie wezla, podczepienie do rightChild
                //rekurencja od rightChild
            //NIE
                //utworzenie liscia
            
        public void generateTree(List<int> probabilityList, Node parent, int levelIndex, State state, State maxValues, List<Test> tests)
        {
			//Console.WriteLine("Poziom: {0}", levelIndex);
			//Console.ReadLine();
			if (probabilityList[levelIndex] >= r.Next(1, 101))
            {
				//Console.WriteLine("Generowanie lewego wezla");
                DecisionNode leftChild = generateDecisionNode(state, maxValues, tests);
                parent.leftChild = leftChild;
                generateTree(probabilityList, leftChild, levelIndex + 1, state, maxValues, tests);
            }
            else
            {
				//Console.WriteLine("Generowanie lewego liscia");
				parent.leftChild = generateResultNode();
            }

			//Console.WriteLine("Powrot na poziom: {0}", levelIndex);

			if (probabilityList[levelIndex] >= r.Next(1, 101))
            {
				//Console.WriteLine("Generowanie prawego wezla");
				DecisionNode rightChild = generateDecisionNode(state, maxValues, tests);
                parent.rightChild = rightChild;
                generateTree(probabilityList, rightChild, levelIndex + 1, state, maxValues, tests);
            }
            else
            {
				//Console.WriteLine("Generowanie prawego liscia");
				parent.rightChild = generateResultNode();
            }
        }

        public DecisionNode generateDecisionNode(State state, State maxValues, List<Test> tests)
        {
			List<string> statistics = state.Keys.ToList(); // lista wszystkich statystyk
            string stat = statistics[r.Next(statistics.Count)]; // wybranie losowo jednej statystyki dla decyzji
            int param = r.Next(maxValues[stat]+1); // wylosowanie parametru do porownania statystyki z odpowiedniego przedzialu
			Test test = tests[r.Next(tests.Count)]; // wylosowanie operatora
			Decision decision = new Decision(stat, test, param);
			DecisionNode node = new DecisionNode(decision);
			//Console.WriteLine("Node: Stat -> {0}, Test -> {1}, Param -> {2}", node.Decision.Statistic, node.Decision.Test.ToString(), node.Decision.Param);

			return node;
        }

        public ResultNode generateResultNode()
        {
            Array values = Enum.GetValues(typeof(Move));
            ResultNode Result = new ResultNode((Move)values.GetValue(r.Next(values.Length)));
			//Console.WriteLine("Result: {0}", Result.Move.ToString());
            return Result;
        }
    }
}
