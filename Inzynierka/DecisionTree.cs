using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inzynierka
{
    class DecisionTree
    {
        private Node root;

        public void addNode() { }

        public DecisionTree(List<int> probabilityList)
        {
            root = generateDecisionNode();
            generateTree(probabilityList, parent);
        }

        public String decide(Dictionary<string,int> state)
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
            
        public void generateTree(List<int> probabilityList, Node parent)
        {
            //chujowizna, spierdoli sie przy rekurencji
            Random r = new Random();
            int levelIndex = 2;

            if (probabilityList[levelIndex] >= r.Next(1, 101))
            {
                DecisionNode leftChild = generateDecisionNode();

            }
        }

        public DecisionNode generateDecisionNode()
        {
            //TODO tworzenie wezla decyzji, dodanie parametrow
            return null;
        }

        public ResultNode generateResultNode()
        {
            //TODO tworzenie wezla wyniku, dodanie parametrow
            return null;
        }
    }
}
