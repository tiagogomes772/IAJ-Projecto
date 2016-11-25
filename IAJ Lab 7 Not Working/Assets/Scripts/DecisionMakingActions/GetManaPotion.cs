using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class GetManaPotion : WalkToTargetAndExecuteAction
    {
        public GetManaPotion(AutonomousCharacter character, GameObject target) : base("GetManaPotion",character,target)
        {
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.Mana < 10;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;

            var mana = (int)worldModel.GetProperty(Properties.MANA);
            return mana < 10;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.GetManaPotion(this.Target);
        }


        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);
            worldModel.SetProperty(Properties.MANA, 10);
            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

        public float f(int featureIndex, WorldModel state)
        {
            switch (featureIndex)
            {
                case 0:     //XP
                    return 2.5f;
                case 1:     //HP
                    return 0f;
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
