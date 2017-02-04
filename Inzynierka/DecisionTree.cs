using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using State = System.Collections.Generic.Dictionary<string, int>;

namespace Inzynierka
{
    public class DecisionTree
    {
		public CryptoRandom R { get; set; }
		public int elementCount { get; set; } = 0;

		public Node root { get; set; }
		public double fitness { get; set; }

		public DecisionTree(List<int> probabilityList, State state, State maxValues, List<Test> tests, CryptoRandom r)
        {
			R = r;
			DecisionNode parent = generateDecisionNode(state, maxValues, tests);
			//Console.WriteLine("root: Stat -> {0}, Test -> {1}, Param -> {2}",parent.Decision.Statistic, parent.Decision.Test.ToString(), parent.Decision.Param);
			elementCount++;
			parent.Key = elementCount;
			root = parent;
            generateTree(probabilityList, parent, 1, state, maxValues, tests);
        }

		public DecisionTree()
		{

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
			if (probabilityList[levelIndex] >= R.Next(1, 101))
            {
				//Console.WriteLine("Generowanie lewego wezla");
                DecisionNode leftChild = generateDecisionNode(state, maxValues, tests);
				elementCount++;
				leftChild.Key = elementCount;
				parent.leftChild = leftChild;
                generateTree(probabilityList, leftChild, levelIndex + 1, state, maxValues, tests);
            }
            else
            {
				//Console.WriteLine("Generowanie lewego liscia");
				parent.leftChild = GenerateResultNode();
				elementCount++;
				parent.leftChild.Key = elementCount;
			}

			//Console.WriteLine("Powrot na poziom: {0}", levelIndex);

			if (probabilityList[levelIndex] >= R.Next(1, 101))
            {
				//Console.WriteLine("Generowanie prawego wezla");
				DecisionNode rightChild = generateDecisionNode(state, maxValues, tests);
				elementCount++;
				rightChild.Key = elementCount;
				parent.rightChild = rightChild;
                generateTree(probabilityList, rightChild, levelIndex + 1, state, maxValues, tests);
            }
            else
            {
				//Console.WriteLine("Generowanie prawego liscia");
				parent.rightChild = GenerateResultNode();
				elementCount++;
				parent.rightChild.Key = elementCount;
			}
		}

        public DecisionNode generateDecisionNode(State state, State maxValues, List<Test> tests)
        {
			List<string> statistics = state.Keys.ToList(); // lista wszystkich statystyk
            string stat = statistics[R.Next(0,statistics.Count)]; // wybranie losowo jednej statystyki dla decyzji
            int param = R.Next(0,maxValues[stat]+1); // wylosowanie parametru do porownania statystyki z odpowiedniego przedzialu
			Test test = tests[R.Next(0,tests.Count)]; // wylosowanie operatora
			Decision decision = new Decision(stat, test, param);
			DecisionNode node = new DecisionNode(decision);
			//Console.WriteLine("Node: Stat -> {0}, Test -> {1}, Param -> {2}", node.Decision.Statistic, node.Decision.Test.ToString(), node.Decision.Param);

			return node;
        }

        public ResultNode GenerateResultNode()
        {
            Array values = Enum.GetValues(typeof(Move));
            ResultNode Result = new ResultNode((Move)values.GetValue(R.Next(0,values.Length)));
			//Console.WriteLine("Result: {0}", Result.Move.ToString());
            return Result;
        }

		public int CountElements()
		{
			elementCount = root.Count();
			return elementCount;
		}

		public void RecalculateKeys()
		{
			elementCount = root.RecalculateKeys(root);
		}

		public Node Find(int i, Node node)
		{
			if (node != null)
			{
				if (node.Key == i)
				{
					return node;
				}
				else
				{
					Node foundNode = Find(i, node.leftChild);
					if (foundNode == null)
					{
						foundNode = Find(i, node.rightChild);
					}
					return foundNode;
				}
			}
			else
			{
				return null;
			}
		}

		public Node FindParentOfNode(int i, Node node, Node parent = null)
		{
			if (node != null)
			{
				if (node.Key == i)
				{
					return parent;
				}
				else
				{
					Node foundParent = FindParentOfNode(i, node.leftChild, node);
					if (foundParent == null)
					{
						foundParent = FindParentOfNode(i, node.rightChild, node);
					}
					return foundParent;
				}
			}
			else
			{
				return null;
			}
		}

		public DecisionTree Clone()
		{
			DecisionTree clone = new DecisionTree();
			clone.elementCount = elementCount;
			clone.fitness = fitness;
			clone.R = R;
			clone.root = root.Clone();
			return clone;
		}
	}
}
