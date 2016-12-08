using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using State = System.Collections.Generic.Dictionary<string, int>;

namespace Inzynierka
{
    class DecisionTree
    {
        static Random r = new Random();

        private Node root;

        public DecisionTree(List<int> probabilityList, State state)
        {
            DecisionNode parent = generateDecisionNode(state);
            root = parent;
            generateTree(probabilityList, parent, 1, state);
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
            
        public void generateTree(List<int> probabilityList, Node parent, int levelIndex, State state)
        {
            
            if (probabilityList[levelIndex] >= r.Next(1, 101))
            {
                DecisionNode leftChild = generateDecisionNode(state);
                parent.leftChild = leftChild;
                generateTree(probabilityList, leftChild, levelIndex + 1, state);
            }
            else
            {
                parent.leftChild = generateResultNode();
            }

            if (probabilityList[levelIndex] >= r.Next(1, 101))
            {
                DecisionNode rightChild = generateDecisionNode(state);
                parent.rightChild = rightChild;
                generateTree(probabilityList, rightChild, levelIndex + 1, state);
            }
            else
            {
                parent.rightChild = generateResultNode();
            }
        }

        public DecisionNode generateDecisionNode(State state)
        {
            //TODO tworzenie wezla decyzji, dodanie parametrow

            Decision decision = new Decision();
            //TODO losowanie wszystkich 3 elementow decyzji
            DecisionNode node = new DecisionNode();
            return null;
        }

        public ResultNode generateResultNode()
        {
            Array values = Enum.GetValues(typeof(Move));
            ResultNode Result = new ResultNode((Move)values.GetValue(r.Next(values.Length)));
            return Result;
        }
    }
}
