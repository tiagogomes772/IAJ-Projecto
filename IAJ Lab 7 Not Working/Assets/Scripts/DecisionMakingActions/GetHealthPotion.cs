﻿using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class GetHealthPotion : WalkToTargetAndExecuteAction
    {
        public GetHealthPotion(AutonomousCharacter character, GameObject target) : base("GetHealthPotion", character, target)
        {
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.HP < this.Character.GameManager.characterData.MaxHP;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;

            var hp = (int)worldModel.GetProperty(Properties.HP);
            return hp < this.Character.GameManager.characterData.MaxHP;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.GetHealthPotion(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);
            worldModel.SetProperty(Properties.HP, this.Character.GameManager.characterData.MaxHP);
            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

        public float f(int featureIndex, WorldModel state)
        {
            var hp = (int)state.GetProperty(Properties.HP);
            var maxHP=(int)state.GetProperty(Properties.MAXHP);

            switch (featureIndex)
            {
                case 0:     //XP
                    return 0f;
                case 1:     //HP
                    return maxHP-hp;
                case 2:     //Money
                    return 0f;
                case 3:     //Time
                    return 0.1f;
                default:
                    return 0f;
            }
        }

    }
}