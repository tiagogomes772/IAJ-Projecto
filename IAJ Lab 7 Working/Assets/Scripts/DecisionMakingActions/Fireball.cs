﻿using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class Fireball : WalkToTargetAndExecuteAction
    {
        private int xpChange;
        private int manaChange = 5;

        public Fireball(AutonomousCharacter character, GameObject target) : base("Fireball", character, target)
        {
            if (target.tag.Equals("Skeleton"))
            {
                this.xpChange = 5;
            }
            else if (target.tag.Equals("Orc"))
            {
                this.xpChange = 10;
            }
            else if (target.tag.Equals("Dragon"))
            {
                this.xpChange = 0;
            }
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.GAIN_XP_GOAL)
            {
                change += -this.xpChange;
            }

            return change;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.Mana >= 5;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            var mana = (int)worldModel.GetProperty(Properties.MANA);
            return mana >= 5;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.Fireball(this.Target);
        }


        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var xpValue = worldModel.GetGoalValue(AutonomousCharacter.GAIN_XP_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GAIN_XP_GOAL, xpValue - this.xpChange);

            var xp = (int)worldModel.GetProperty(Properties.XP);
            worldModel.SetProperty(Properties.XP, xp + this.xpChange);
            var mana = (int)worldModel.GetProperty(Properties.MANA);
            worldModel.SetProperty(Properties.MANA, mana + this.manaChange);

            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }



        public override float f(int featureIndex, WorldModel state)
        {
            var lvl = (int)state.GetProperty(Properties.LEVEL);
            switch (featureIndex)
            {
                case 0:     //XP
                    if (this.Target.tag.Equals("Skeleton"))
                    {
                        return lvl == 3 ? 0 : 5;
                    }
                    else if (this.Target.tag.Equals("Orc"))
                    {
                        return lvl == 3 ? 0 : 10;
                    }
                    else if (this.Target.tag.Equals("Dragon"))
                    {
                        return 0;
                    }
                    return lvl == 3 ? 0 : (5f / lvl);
                case 1:     //HP
                    //If the target is a dragon we wont do any damage and we will most certainly be hit with a -20hp attack 
                    //So avoid at all costs
                    if (this.Target.tag.Equals("Skeleton"))
                    {
                        return 10f;
                    }
                    else if (this.Target.tag.Equals("Orc"))
                    {
                        return 20f;
                    }
                    else if (this.Target.tag.Equals("Dragon"))
                    {
                        return 0;
                    }
                    return 10f;
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
