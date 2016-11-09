using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class DepthLimitedGOAPDecisionMaking
    {
        public const int MAX_DEPTH = 3;
        public int ActionCombinationsProcessedPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        public int TotalActionCombinationsProcessed { get; set; }
        public bool InProgress { get; set; }

        public CurrentStateWorldModel InitialWorldModel { get; set; }
        private List<Action> Actions { get; set; }
        private List<Goal> Goals { get; set; }
        private WorldModel[] Models { get; set; }
        private Action[] ActionPerLevel { get; set; }
        public Action[] BestActionSequence { get; private set; }
        public Action BestAction { get; private set; }
        public float BestDiscontentmentValue { get; private set; }
        private int CurrentDepth {  get; set; }

        public DepthLimitedGOAPDecisionMaking(CurrentStateWorldModel currentStateWorldModel, List<Action> actions, List<Goal> goals)
        {
            this.ActionCombinationsProcessedPerFrame = 200;
            this.Actions = actions;
            this.Goals = goals;
            this.InitialWorldModel = currentStateWorldModel;
        }

        public void InitializeDecisionMakingProcess()
        {
            this.InProgress = true;
            this.TotalProcessingTime = 0.0f;
            this.TotalActionCombinationsProcessed = 0;
            this.CurrentDepth = 0;
            this.Models = new WorldModel[MAX_DEPTH + 1];
            this.Models[0] = this.InitialWorldModel;
            this.ActionPerLevel = new Action[MAX_DEPTH];
            this.BestActionSequence = new Action[MAX_DEPTH];
            this.BestAction = null;
            this.BestDiscontentmentValue = float.MaxValue;
            this.InitialWorldModel.Initialize();
        }

        public Action ChooseAction()
        {
            #region DEBUG
            var processedActions = 0;

            var startTime = Time.realtimeSinceStartup;
            #endregion
            float currentValue = 0f;
            Action nextAction;
            while(this.CurrentDepth >= 0)
            {
                if(this.CurrentDepth >= MAX_DEPTH)
                {
                    currentValue = this.Models[CurrentDepth].CalculateDiscontentment(Goals);

                    if(currentValue < this.BestDiscontentmentValue)
                    {
                        this.BestDiscontentmentValue = currentValue;
                        this.BestAction = this.ActionPerLevel[0];
                    }
                    this.CurrentDepth -= 1;
                    continue;
                }

                nextAction = Models[this.CurrentDepth].GetNextAction();
                if (nextAction != null)
                {
                    #region DEBUG
                    processedActions++;
                    #endregion
                    Models[this.CurrentDepth + 1] = Models[this.CurrentDepth].GenerateChildWorldModel();
                    nextAction.ApplyActionEffects(Models[this.CurrentDepth + 1]);
                    ActionPerLevel[this.CurrentDepth] = nextAction;
                    this.CurrentDepth++;
                }
                else
                    this.CurrentDepth--;
            }

            #region DEBUG
            this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
            #endregion

            this.InProgress = false;
            return this.BestAction;
        }
    }
}
