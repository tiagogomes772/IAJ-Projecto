using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    class MCTSHybridRAVEBiased: MCTS
    {
        protected const float b = 1; //FIXME: verificar este valor 
        protected List<Pair<int, GOB.Action>> ActionHistory { get; set; }

        public MCTSHybridRAVEBiased(CurrentStateWorldModel worldModel) :base(worldModel)
        {
        }

        protected override Reward Playout(WorldModel initialPlayoutState)
        {
            ActionHistory = new List<Pair<int, GOB.Action>>();

            GOB.Action action=null;

            var currentState = initialPlayoutState.GenerateChildWorldModel();

            CurrentDepth = 0;
            while (!currentState.IsTerminal())
            {
                var actions = currentState.GetExecutableActions();
                #region BiasedPlayout
                List<float> H = new List<float>();
                foreach (GOB.Action a in actions)
                {
                    H.Add(h(a, currentState));
                }
                List<double> softmax = Softmax(H);

                int randSample = RandomGenerator.Next(100);

                double cumulativeSoftmax = 0;

                for (int i = 0; i < softmax.Count; i++)
                {
                    cumulativeSoftmax += softmax[i] * 100;

                    if (cumulativeSoftmax > randSample)
                    {
                        action = actions[i];
                        break;
                    }
                }

                if (action == null)
                {
                    action = actions[actions.Length - 1];
                }
                #endregion

                #region RAVE
                ActionHistory.Add(new Pair<int, GOB.Action>(currentState.GetNextPlayer(), action));
                #endregion
                action.ApplyActionEffects(currentState);
                currentState.CalculateNextPlayer();
                CurrentDepth++;
            }
            if (CurrentDepth > MaxPlayoutDepthReached)
                MaxPlayoutDepthReached = CurrentDepth;

            Reward r = new Reward();
            //TODO Verify if reward is this score
            r.Value = currentState.GetScore();
            return r;
        }

        #region RAVE Specific

        protected override MCTSNode BestUCTChild(MCTSNode node)
        {
            float MCTSValue;
            float RAVEValue;
            float UCTValue;
            float bestUCT = float.MinValue;
            MCTSNode bestNode = null;

            //step 1, calculate beta and 1-beta. beta does not change from child to child. So calculate this only once
            float beta = node.NRAVE / (node.N + node.NRAVE + 4 * node.N * node.NRAVE * b * b);

            //step 2, calculate the MCTS value, the RAVE value, and the UCT for each child and determine the best one
            foreach (MCTSNode child in node.ChildNodes)
            {
                MCTSValue = child.Q / child.N;
                RAVEValue = child.QRAVE / child.NRAVE;
                UCTValue = ((1 - beta) * MCTSValue + beta * RAVEValue) + C * (float)Math.Sqrt(Math.Log(node.N) / child.N);

                if (bestUCT < UCTValue)
                {
                    bestUCT = UCTValue;
                    bestNode = child;
                }
            }
            return bestNode;
        }


        

        protected override void Backpropagate(MCTSNode node, Reward reward)
        {
            while (node != null)
            {
                node.N++;
                //TODO VERIFY this
                node.Q = node.Q + reward.GetRewardForNode(node);


                if (node.Parent != null) ActionHistory.Add(new Pair<int, GOB.Action>(node.Parent.State.GetNextPlayer(), node.Action));
                node = node.Parent;

                if (node != null)
                {
                    int player = node.State.GetNextPlayer();
                    foreach (MCTSNode c in node.ChildNodes)
                    {

                        if (ActionHistory.FirstOrDefault(x => x.Left.Equals(player) && x.Right.Equals(c.Action)) != null)
                        {
                            c.NRAVE++;
                            c.QRAVE = c.QRAVE + reward.GetRewardForNode(c);
                        }
                    }

                }
            }
        }
        #endregion



        #region BiasedPlayout Specific

        public List<double> Softmax(List<float> H)
        {
            List<double> softmax = new List<double>();

            double normalizationSum = 0;

            foreach (float hi in H)
            {
                normalizationSum += Math.Exp(hi);
            }

            foreach (float hi in H)
            {
                softmax.Add(Math.Exp(hi) / normalizationSum);
            }

            return softmax;
        }

        public float w(int featureIndex)
        {
            switch (featureIndex)
            {
                case 0:     //XP
                    return 0.30f;
                case 1:     //HP
                    return 0.2f;
                case 2:     //Money
                    return 0.30f;
                case 3:     //Time
                    return 0.1f;
                default:
                    return 0f;
            }
        }


        public float f(int featureIndex, GOB.Action action, WorldModel state)
        {
            return action.f(featureIndex, state);
        }

        public float h(GOB.Action action, WorldModel state)
        {
            float result = 0;
            for (int i = 0; i < 4; i++)
            {
                result += w(i) * f(i, action, state);
            }

            return result;
        }

        protected MCTSNode Expand(WorldModel parentState, GOB.Action action)
        {
            //MCTSNode new_child = new MCTSNode(parentState.GenerateChildWorldModel());
            //action.ApplyActionEffects(new_child.State);
            //new_child.Action = action;
            //new_child.Parent = parentState.;

            //parent.ChildNodes.Add(new_child);
            //return new_child;
            throw new NotImplementedException();
        }
        #endregion

    }
}
