using Assets.Scripts.GameManager;
using System;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSBiasedPlayout : MCTS
    {
        public MCTSBiasedPlayout(CurrentStateWorldModel currentStateWorldModel) : base(currentStateWorldModel)
        {
        }

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

        /*protected override Reward Playout(WorldModel initialPlayoutState)
        {
            GOB.Action action = null;

            var currentState = initialPlayoutState.GenerateChildWorldModel();

            CurrentDepth = 0;
            while (!currentState.IsTerminal())
            {
                var actions = currentState.GetExecutableActions();
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

                action.ApplyActionEffects(currentState);
                currentState.CalculateNextPlayer();
                CurrentDepth++;
            }
            if (CurrentDepth > MaxPlayoutDepthReached)
                MaxPlayoutDepthReached = CurrentDepth;

            Reward r = new Reward();
            //TODO Verify if reward is this score
            r.Value = currentState.GetScore();
            r.PlayerID = currentState.GetNextPlayer();
            return r;
        }*/


        protected override MCTSNode BestUCTChild(MCTSNode node)
        {

            int r = RandomGenerator.Next(node.ChildNodes.Count);
            float bestNodeValue = (node.ChildNodes[r].Q / node.ChildNodes[r].N) + C * (float)Math.Sqrt(Math.Log(node.ChildNodes[r].Parent.N) / node.N);
            MCTSNode bestChild = node.ChildNodes[r];
            //float bestNodeValue = float.MinValue;

            foreach (MCTSNode child in node.ChildNodes)
            {
                float value = (child.Q / child.N) + C * (float)Math.Sqrt(Math.Log(child.Parent.N) / node.N);

                if (value > bestNodeValue)
                {
                    bestChild = child;
                    bestNodeValue = value;
                }
            }
            return bestChild;
        }

        protected override Reward Playout(WorldModel initialPlayoutState)
        {
            GOB.Action action = null;

            var currentState = initialPlayoutState.GenerateChildWorldModel();

            CurrentDepth = 0;
            while (!currentState.IsTerminal())
            {
                var actions = currentState.GetExecutableActions();

                float sumH = 0;
                
                foreach (GOB.Action a in actions)
                {
                    sumH += a.h(currentState);
                }

                float actionValue = 0.0f;
                float gibbsProb = float.MaxValue;
                float currentGibbsProb = 0.0f;
                foreach (GOB.Action a in actions)
                {
                    actionValue = a.h(currentState);
                    
                    currentGibbsProb = actionValue / sumH;
                    if (currentGibbsProb < gibbsProb)
                    {
                        gibbsProb = currentGibbsProb;
                        action = a;
                    }
                }
               // Debug.Log(Time.realtimeSinceStartup + " " + CurrentDepth + " Action: " + action.Name + "H: " + action.h(currentState) + " L: " + actions.Length);
                action.ApplyActionEffects(currentState);
                currentState.CalculateNextPlayer();
                CurrentDepth++;
            }
            if (CurrentDepth > MaxPlayoutDepthReached)
                MaxPlayoutDepthReached = CurrentDepth;

            
            Reward r = new Reward();
            //TODO Verify if reward is this score
            r.Value = currentState.GetScore();
            r.PlayerID = currentState.GetNextPlayer();
            if (r.Value == 1.0f)
            {
                r.Value = 1.0f;
            }
            return r;
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

    }
}
