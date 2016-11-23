﻿using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using RAIN.Navigation.NavMesh;
using UnityEngine;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;

namespace Assets.Scripts.DecisionMakingActions
{
    public abstract class WalkToTargetAndExecuteAction : Action
    {
        protected AutonomousCharacter Character { get; set; }
        protected IHeuristic Heuristic { get; set;}
        protected GameObject Target { get; set; }
        protected ClusterGraph ClusterGraph { get; set; }
        //get the NavMeshGraph from the current scene
        NavMeshPathGraph navMesh = GameObject.Find("Navigation Mesh").GetComponent<NavMeshRig>().NavMesh.Graph;


        protected WalkToTargetAndExecuteAction(string actionName, AutonomousCharacter character, GameObject target) : base(actionName+"("+target.name+")")
        {
            ClusterGraph = Resources.Load<ClusterGraph>("ClusterGraph");
            this.Character = character;
            this.Target = target;
            Heuristic = new GatewayHeuristic(ClusterGraph);
        }

        public override float GetDuration()
        {
            //assume a velocity of 20.0f/s to get to the target
            //Gateway             
            return Heuristic.H(navMesh.QuantizeToNode(this.Target.transform.position, 1.0f), navMesh.QuantizeToNode(this.Character.transform.position, 1.0f));
            //Euclidean Distance
            //return (this.Target.transform.position - this.Character.Character.KinematicData.position).magnitude / 20.0f;
        }

        public override float GetDuration(WorldModel worldModel)
        {
            //assume a velocity of 20.0f/s to get to the target
            var position = (Vector3)worldModel.GetProperty(Properties.POSITION);
            //Gateway        
            return Heuristic.H(navMesh.QuantizeToNode(this.Target.transform.position, 1.0f), navMesh.QuantizeToNode(position, 1.0f));
            //Euclidean Distance
            //return (this.Target.transform.position - position).magnitude / 20.0f;
        }

        public override float GetGoalChange(Goal goal)
        {
            if (goal.Name == AutonomousCharacter.BE_QUICK_GOAL)
            {
                return this.GetDuration();
            }
            else return 0;
        }

        public override bool CanExecute()
        {
            return this.Target != null;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (this.Target == null) return false;
            var targetEnabled = (bool)worldModel.GetProperty(this.Target.name);
            return targetEnabled;
        }

        public override void Execute()
        {
            this.Character.StartPathfinding(this.Target.transform.position);
        }


        public override void ApplyActionEffects(WorldModel worldModel)
        {
            var duration = this.GetDuration(worldModel);

            var quicknessValue = worldModel.GetGoalValue(AutonomousCharacter.BE_QUICK_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, quicknessValue + duration*0.1f);

            var time = (float)worldModel.GetProperty(Properties.TIME);
            worldModel.SetProperty(Properties.TIME, time + duration);

            worldModel.SetProperty(Properties.POSITION, Target.transform.position);
        }

    }
}