using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class SwordAttack : WalkToTargetAndExecuteAction
    {
        private int hpChange;
        private int xpChange;

        public SwordAttack(AutonomousCharacter character, GameObject target) : base("SwordAttack",character,target)
        {
           
            if (target.tag.Equals("Skeleton"))
            {
                this.hpChange = -5;
                this.xpChange = 5;
            }
            else if (target.tag.Equals("Orc"))
            {
                this.hpChange = -10;
                this.xpChange = 10;
            }
            else if (target.tag.Equals("Dragon"))
            {
                this.hpChange = -20;
                this.xpChange = 20;
            }
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change += -this.hpChange;
            }
            else if (goal.Name == AutonomousCharacter.GAIN_XP_GOAL)
            {
                change += -this.xpChange;
            }
            
            return change;
        }


        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.SwordAttack(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var xpValue = worldModel.GetGoalValue(AutonomousCharacter.GAIN_XP_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GAIN_XP_GOAL,xpValue-this.xpChange); 

            var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL,surviveValue-this.hpChange);

            var hp = (int)worldModel.GetProperty(Properties.HP);
            worldModel.SetProperty(Properties.HP,hp + this.hpChange);
            var xp = (int)worldModel.GetProperty(Properties.XP);
            worldModel.SetProperty(Properties.XP, xp + this.xpChange);
           

            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name,false);
        }

        public override float f(int featureIndex, WorldModel state)
        {
            var hp = (int)state.GetProperty(Properties.HP);
            var maxHP = (int)state.GetProperty(Properties.MAXHP);
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
                        return lvl == 3 ? 0 : 20;
                    }
                    return 10f;
                case 1:     //HP
                    //Avoid death at all costs
                    if (this.Target.tag.Equals("Skeleton"))
                    {
                        return hp-5 <= 0 ? 0 : hp - 5 ;
                    }
                    else if (this.Target.tag.Equals("Orc"))
                    {
                        return hp-10 <= 0 ? 0 : hp - 10; ;
                    }
                    else if (this.Target.tag.Equals("Dragon"))
                    {
                        return hp-20 <= 0 ? 0 : hp - 20; ;
                    }
                    return 0;
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
