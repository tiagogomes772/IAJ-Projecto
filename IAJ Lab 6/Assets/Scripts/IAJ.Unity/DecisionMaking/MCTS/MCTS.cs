using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTS
    {
        public const float C = 1.4f;
        public bool InProgress { get; private set; }
        public int MaxIterations { get; set; }
        public int MaxIterationsProcessedPerFrame { get; set; }
        public int MaxPlayoutDepthReached { get; private set; }
        public int MaxSelectionDepthReached { get; private set; }
        public float TotalProcessingTime { get; private set; }
        public MCTSNode BestFirstChild { get; set; }
        public List<GOB.Action> BestActionSequence { get; private set; }


        private int CurrentIterations { get; set; }
        private int CurrentIterationsInFrame { get; set; }
        private int CurrentDepth { get; set; }

        private CurrentStateWorldModel CurrentStateWorldModel { get; set; }
        private MCTSNode InitialNode { get; set; }
        private System.Random RandomGenerator { get; set; }
        
        

        public MCTS(CurrentStateWorldModel currentStateWorldModel)
        {
            this.InProgress = false;
            this.CurrentStateWorldModel = currentStateWorldModel;
            this.MaxIterations = 100;
            this.MaxIterationsProcessedPerFrame = 10;
            this.RandomGenerator = new System.Random();
        }


        public void InitializeMCTSearch()
        {
            this.MaxPlayoutDepthReached = 0;
            this.MaxSelectionDepthReached = 0;
            this.CurrentIterations = 0;
            this.CurrentIterationsInFrame = 0;
            this.TotalProcessingTime = 0.0f;
            this.CurrentStateWorldModel.Initialize();
            this.InitialNode = new MCTSNode(this.CurrentStateWorldModel)
            {
                Action = null,
                Parent = null,
                PlayerID = 0
            };
            this.InProgress = true;
            this.BestFirstChild = null;
            this.BestActionSequence = new List<GOB.Action>();
        }

        public GOB.Action Run()
        {
            MCTSNode selectedNode;
            Reward reward;

            var startTime = Time.realtimeSinceStartup;
            this.CurrentIterationsInFrame = 0;
            int Ntimes = 1000; //TODO Dont know where to get this number
            int counter = 0;
            while(counter < Ntimes)
            {
                selectedNode = Selection(this.InitialNode);
                reward = Playout(selectedNode.State);
                Backpropagate(selectedNode, reward);
                counter++;
            }

            return BestChild(this.InitialNode).Action;
        }

        private MCTSNode Selection(MCTSNode initialNode)
        {
            GOB.Action nextAction;
            MCTSNode currentNode = initialNode;
            MCTSNode bestChild;


            while(currentNode.State.GetExecutableActions().Length > 0)
            { 
                GOB.Action[] allActionAvailable = currentNode.State.GetExecutableActions();

                int childNodesNumber = currentNode.ChildNodes.Count;
                if (allActionAvailable.Length > childNodesNumber)
                {
                    currentNode.ChildNodes.Add(Expand(currentNode, allActionAvailable[childNodesNumber]));
                }
                else
                {
                    currentNode = BestChild(currentNode);
                }
            }
            return currentNode;
        }

        private Reward Playout(WorldModel initialPlayoutState)
        {
            GOB.Action action;
            while (initialPlayoutState.IsTerminal())
            {
                int index = RandomGenerator.Next(initialPlayoutState.GetExecutableActions().Length);
                action = initialPlayoutState.GetExecutableActions()[index];
                //TODO Dont know how to get a new State from a given state and an action
                initialPlayoutState = new WorldModel(initialPlayoutState);
            }
            Reward r = new Reward();
            //TODO Verify if reward is this score
            r.Value = initialPlayoutState.GetScore();
            return r;
        }

        private void Backpropagate(MCTSNode node, Reward reward)
        {
            while(!node.Equals(null))
            {
                node.N++;
                //TODO VERIFY this
                node.Q = node.Q + reward.Value;
                node = node.Parent;
            }
        }

        private MCTSNode Expand(MCTSNode parent, GOB.Action action)
        {
            //TODO Compare with professor implementation
            MCTSNode new_child = new MCTSNode(parent.State);
            new_child.Action = action;
            return new_child;
        }

        //gets the best child of a node, using the UCT formula
        private MCTSNode BestUCTChild(MCTSNode node)
        {
           
            MCTSNode bestChild = node.ChildNodes[0];
            int bestNodeValue = (int)bestChild.Q + ((int)Math.Sqrt(2) * (int)Math.Sqrt(Math.Log(node.N) / bestChild.N));
            MCTSNode auxChild;
            int auxNodeValue;
            for (int i=1; i < node.ChildNodes.Count; i++)
            {
                auxChild = node.ChildNodes[i];
                auxNodeValue = (int)auxChild.Q + ((int)Math.Sqrt(2) * (int)Math.Sqrt(Math.Log(node.N) / auxChild.N));
                if (auxNodeValue > bestNodeValue)
                    bestChild = auxChild;  
            }
            return bestChild;
        }

        //this method is very similar to the bestUCTChild, but it is used to return the final action of the MCTS search, and so we do not care about
        //the exploration factor
        private MCTSNode BestChild(MCTSNode node)
        {
            MCTSNode bestChild = node.ChildNodes[0];
            int bestNodeValue = (int)bestChild.Q + ((int)Math.Sqrt(2) * (int)Math.Sqrt(Math.Log(node.N) / bestChild.N));
            MCTSNode auxChild;
            int auxNodeValue;
            for (int i = 1; i < node.ChildNodes.Count; i++)
            {
                auxChild = node.ChildNodes[i];
                auxNodeValue = (int)auxChild.Q + (int)Math.Sqrt(Math.Log(node.N) / auxChild.N);
                if (auxNodeValue > bestNodeValue)
                    bestChild = auxChild;
            }
            return bestChild;
        }
    }
}
