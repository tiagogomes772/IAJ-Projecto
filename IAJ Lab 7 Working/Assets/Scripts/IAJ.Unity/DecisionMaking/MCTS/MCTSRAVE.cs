using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSRAVE : MCTS
    {
        protected const float b = 0.5f; //FIXME: verificar este valor 
        protected List<Pair<int,Action>> ActionHistory { get; set; }
        public MCTSRAVE(CurrentStateWorldModel worldModel) :base(worldModel)
        {
        }

        protected override MCTSNode BestUCTChild(MCTSNode node)
        {
            float MCTSValue;
            float RAVEValue;
            float UCTValue;
            float bestUCT = float.MinValue;
            MCTSNode bestNode = null;

            //step 1, calculate beta and 1-beta. beta does not change from child to child. So calculate this only once
            // Mean Squared Error
            float beta = node.NRAVE / (node.N + node.NRAVE + 4 * node.N * node.NRAVE * b * b);

            //float k = this.MaxIterations / 2;
            //float beta = (float)Math.Sqrt(k / (3 * node.N + k));

            float oneMinusBeta = 1 - beta;

            //step 2, calculate the MCTS value, the RAVE value, and the UCT for each child and determine the best one
            foreach (MCTSNode child in node.ChildNodes)
            {
                MCTSValue = child.Q/child.N;
                RAVEValue = child.QRAVE / child.NRAVE;
                UCTValue = (oneMinusBeta * MCTSValue + beta * RAVEValue) + C * (float)Math.Sqrt(Math.Log(node.N) / child.N);

                if(bestUCT < UCTValue)
                {
                    bestUCT = UCTValue;
                    bestNode = child;
                }
            }
            return bestNode;
        }


        protected override Reward Playout(WorldModel initialPlayoutState)
        {
            ActionHistory = new List<Pair<int, GOB.Action>>();

            GOB.Action action;

            var currentState = initialPlayoutState.GenerateChildWorldModel();

            CurrentDepth = 0;
            while (!currentState.IsTerminal())
            {
                var actions = currentState.GetExecutableActions();

                int index = RandomGenerator.Next(actions.Length);
                action = actions[index];
                ActionHistory.Add(new Pair<int, GOB.Action>(currentState.GetNextPlayer(), action));
                action.ApplyActionEffects(currentState);
                currentState.CalculateNextPlayer();
                CurrentDepth++;
            }
            if (CurrentDepth > MaxPlayoutDepthReached)
                MaxPlayoutDepthReached = CurrentDepth;

            Reward r = new Reward();
            //TODO Verify if reward is this score
            r.Value = currentState.GetScore();
            if(r.Value == 1.0f)
            {
                r.Value = 1.0f;
            }
            r.PlayerID = currentState.GetNextPlayer();
            return r;
        }

        protected override void Backpropagate(MCTSNode node, Reward reward)
        {
            while (node != null)
            {
                node.N++;
                //TODO VERIFY this
                node.Q = node.Q + reward.GetRewardForNode(node);


                if (node.Parent != null) ActionHistory.Add(new Pair<int, GOB.Action>(node.Parent.PlayerID, node.Action));
                node = node.Parent;

                if (node != null)
                {
                    int player = node.PlayerID;
                    foreach(MCTSNode c in node.ChildNodes)
                    {
                        
                        if(ActionHistory.FirstOrDefault( x => x.Left.Equals( player) && x.Right.Equals(c.Action)) != null)
                        {
                            c.NRAVE++;
                            c.QRAVE = c.QRAVE + reward.GetRewardForNode(c);
                        }
                    }

                }
            }
        }
    }
}
