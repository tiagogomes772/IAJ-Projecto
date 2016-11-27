using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.DecisionMakingActions
{
    public class LevelUp : Action
    {
        public AutonomousCharacter Character { get; private set; }

        public LevelUp(AutonomousCharacter character) : base("LevelUp")
        {
            this.Character = character;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            int maxHP = (int)worldModel.GetProperty(Properties.MAXHP);
            int level = (int)worldModel.GetProperty(Properties.LEVEL);

            worldModel.SetProperty(Properties.LEVEL, level + 1);
            worldModel.SetProperty(Properties.MAXHP, maxHP + 10);
            worldModel.SetProperty(Properties.HP, maxHP + 10);
        }

        public override bool CanExecute()
        {
            var level = this.Character.GameManager.characterData.Level;
            var xp = this.Character.GameManager.characterData.XP;

            if(level == 1)
            {
                return xp >= 10;
            }
            else if(level == 2)
            {
                return xp >= 30;
            }

            return false;
        }
        

        public override bool CanExecute(WorldModel worldModel)
        {
            int xp = (int)worldModel.GetProperty(Properties.XP);
            int level = (int)worldModel.GetProperty(Properties.LEVEL);

            if (level == 1)
            {
                return xp >= 10;
            }
            else if (level == 2)
            {
                return xp >= 30;
            }

            return false;
        }

        public override void Execute()
        {
            this.Character.GameManager.LevelUp();
        }

        public override float GetDuration()
        {
            return 0.0f;
        }

        public override float GetDuration(WorldModel worldModel)
        {
            return 0.0f;
        }

        public override float GetGoalChange(Goal goal)
        {
            return 0.0f;
        }

        public override float f(int featureIndex, WorldModel state)
        {
            var hp = (int)state.GetProperty(Properties.HP);
            var gainHP = (int)state.GetProperty(Properties.MAXHP) + 10;
            var lvl = (int)state.GetProperty(Properties.LEVEL);
            switch (featureIndex)
            {
                case 0:     //XP
                    return 0f;
                case 1:     //HP
                    return (lvl == 3) ? 0 : gainHP - hp;
                case 2:     //Money
                    return 0f;
                case 3:     //Time
                    return 5f;
                default:
                    return 0f;
            }
        }

        public override float h(WorldModel state)
        {
           float distance = GetDuration(state);
            var lvl = (int)state.GetProperty(Properties.LEVEL);
            if(lvl == 0)
            {
                return 0.0f + distance;
            } else if (lvl == 1)
            {
                return 0.5f + distance;
            }
            else if(lvl == 2)
            {
                return 5.0f + distance;
            } else if (lvl == 3)
            {
                return 20.0f + distance;
            }
            return 0 + distance;
        }

    }
}
