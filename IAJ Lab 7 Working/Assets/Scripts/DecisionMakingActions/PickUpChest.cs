using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class PickUpChest : WalkToTargetAndExecuteAction
    {

        public PickUpChest(AutonomousCharacter character, GameObject target) : base("PickUpChest",character,target)
        {
        }


        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.GET_RICH_GOAL) change -= 5.0f;
            return change;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return true;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            return true;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.PickUpChest(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var goalValue = worldModel.GetGoalValue(AutonomousCharacter.GET_RICH_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GET_RICH_GOAL, goalValue - 5.0f);

            var money = (int)worldModel.GetProperty(Properties.MONEY);
            worldModel.SetProperty(Properties.MONEY, money + 5);

            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

        public override float f(int featureIndex, WorldModel state)
        {
            var money = (int)state.GetProperty(Properties.MONEY);
            var hp = (int)state.GetProperty(Properties.HP);

            switch (featureIndex)
            {
                case 0:     //XP
                    return 0;
                case 1:     //HP
                    return 0f;
                case 2:     //Money
                    //Only one chest to win and no way to die! Go for it at all costs!!!!
                    //return (money >= 20 && hp > 20) ? 10000f : 25f;     //FIXME: for some reason this crashes unity
                    return 25f;
                case 3:     //Time
                    return 0.1f;
                default:
                    return 0f;
            }
        }

        public override float h(WorldModel state)
        {
            float distance = GetDuration(state);
            var money = (int)state.GetProperty(Properties.MONEY);
            return money / 25.0f + distance;
        }

    }
}
