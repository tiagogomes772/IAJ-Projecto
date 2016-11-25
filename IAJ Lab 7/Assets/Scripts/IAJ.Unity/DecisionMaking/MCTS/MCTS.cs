using Assets.Scripts.GameManager;
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
        public int MaxPlayoutDepthReached { get; protected set; }
        public int MaxSelectionDepthReached { get; private set; }
        public float TotalProcessingTime { get; private set; }
        public MCTSNode BestFirstChild { get; set; }
        public List<GOB.Action> BestActionSequence { get; private set; }


        private int CurrentIterations { get; set; }
        private int CurrentIterationsInFrame { get; set; }
        protected int CurrentDepth { get; set; }

        private CurrentStateWorldModel CurrentStateWorldModel { get; set; }
        private MCTSNode InitialNode { get; set; }
        protected System.Random RandomGenerator { get; set; }
        
        

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

            #region DEBUG
                        
            var startTime = Time.realtimeSinceStartup;
            #endregion

            this.CurrentIterationsInFrame = 0;
            int counter = 0;
            while(counter < this.MaxIterations && CurrentIterationsInFrame < this.MaxIterationsProcessedPerFrame)
            {
                selectedNode = Selection(this.InitialNode);
                reward = Playout(selectedNode.State);
                Backpropagate(selectedNode, reward);
                counter++;
                CurrentIterationsInFrame++;
            }

            

            this.BestFirstChild = BestChild(this.InitialNode);

            MCTSNode BestChildNode = this.BestFirstChild;

            while (BestChildNode != null)
            {
                this.BestActionSequence.Add(BestChildNode.Action);
                if (BestChildNode.ChildNodes.Count == 0)
                    BestChildNode = null;
                else
                    BestChildNode = BestChild(BestChildNode);
            }

            if (counter >= this.MaxIterations)
            {
                this.InProgress = false;
                this.CurrentIterations = 0;
                
            }
            #region DEBUG
            this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
            #endregion

            this.CurrentIterations = counter;

            
            return BestChild(this.InitialNode).Action;
        }

        protected MCTSNode Selection(MCTSNode initialNode)
        {
            GOB.Action nextAction;
            MCTSNode currentNode = initialNode;
            MCTSNode bestChild;


            while(!currentNode.State.IsTerminal())
            {
                var action = currentNode.State.GetNextAction();
                if (action != null)
                {
                    return Expand(currentNode, action);
                }
                else
                {
                    currentNode = BestChild(currentNode);
                }
            }
            return currentNode;
        }

        protected virtual Reward Playout(WorldModel initialPlayoutState)
        {
            GOB.Action action;

            var currentState = initialPlayoutState.GenerateChildWorldModel();

            CurrentDepth = 0;
            while (!currentState.IsTerminal())
            {
                var actions = currentState.GetExecutableActions();

                int index = RandomGenerator.Next(actions.Length);
                action = actions[index];
                action.ApplyActionEffects(currentState);
                CurrentDepth++;
            }
            if (CurrentDepth > MaxPlayoutDepthReached)
                MaxPlayoutDepthReached = CurrentDepth;

            Reward r = new Reward();
            //TODO Verify if reward is this score
            r.Value = currentState.GetScore();
            return r;
        }

        protected virtual void Backpropagate(MCTSNode node, Reward reward)
        {
            while(node!=null)
            {
                node.N++;
                //TODO VERIFY this
                node.Q = node.Q + reward.Value;
                node = node.Parent;
            }
        }

        protected MCTSNode Expand(MCTSNode parent, GOB.Action action)
        {
            MCTSNode new_child = new MCTSNode(parent.State.GenerateChildWorldModel());
            action.ApplyActionEffects(new_child.State);
            new_child.Action = action;
            new_child.Parent = parent;

            parent.ChildNodes.Add(new_child);
            return new_child;
        }

        //gets the best child of a node, using the UCT formula
        protected virtual MCTSNode BestUCTChild(MCTSNode node)
        {
            try
            {
                MCTSNode bestChild = node.ChildNodes[0];
                int bestNodeValue = (int)bestChild.Q + ((int)Math.Sqrt(2) * (int)Math.Sqrt(Math.Log(node.N) / bestChild.N));
                MCTSNode auxChild;
                int auxNodeValue;
                for (int i = 1; i < node.ChildNodes.Count; i++)
                {
                    auxChild = node.ChildNodes[i];
                    auxNodeValue = (int)auxChild.Q + ((int)Math.Sqrt(2) * (int)Math.Sqrt(Math.Log(node.N) / auxChild.N));
                    if (auxNodeValue > bestNodeValue)
                        bestChild = auxChild;
                }
                return bestChild;
            }
            catch (ArgumentOutOfRangeException e)
            {
                return null;
            }
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

        protected GOB.Action BestFinalAction(MCTSNode node)
        {
            //TODO: implement
            throw new NotImplementedException();
        }
    }
}
