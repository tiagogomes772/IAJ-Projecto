﻿using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    class MCTSHybridRAVEBiased: MCTS
    {
        protected const float b = 1.0f; //FIXME: verificar este valor 
        protected List<Pair<int, GOB.Action>> ActionHistory { get; set; }

        public MCTSHybridRAVEBiased(CurrentStateWorldModel worldModel) :base(worldModel)
        {
        }

        /*protected override Reward Playout(WorldModel initialPlayoutState)
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
        }*/

        protected override Reward Playout(WorldModel initialPlayoutState)
        {
            ActionHistory = new List<Pair<int, GOB.Action>>();

            GOB.Action action = null;

            var currentState = initialPlayoutState.GenerateChildWorldModel();

            CurrentDepth = 0;
            while (!currentState.IsTerminal())
            {
                var actions = currentState.GetExecutableActions();

                //if (currentState.IsImmediateWin())
                //{
                //    #region Immediate win
                //    foreach (GOB.Action a in actions)
                //    {
                //        if (a.TypeOfAction.Equals("PickUpChest"))
                //        {
                //            action = a;
                //            break;
                //        }
                //    }
                //    #endregion
                //}


                //else
                //{
                    #region BiasedPlayout
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
                    #endregion
                //}

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
            // Mean Squared Error
            float beta = node.NRAVE / (node.N + node.NRAVE + 4 * node.N * node.NRAVE * b * b);

            //float k = this.MaxIterations / 2;
            //float beta = (float)Math.Sqrt(k / (3 * node.N + k));

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
                node.Q = node.Q + reward.GetRewardForNode(node);


                if (node.Parent != null) ActionHistory.Add(new Pair<int, GOB.Action>(node.Parent.PlayerID, node.Action));
                node = node.Parent;

                if (node != null)
                {
                    int player = node.PlayerID;
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

       
        #endregion

    }
}
