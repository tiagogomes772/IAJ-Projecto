using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameManager
{
    //class that represents a world model that corresponds to the current state of the world,
    //all required properties and goals are stored inside the game manager
    public class CurrentStateWorldModel : FutureStateWorldModel
    {
        private Dictionary<string, Goal> Goals { get; set; } 

        public CurrentStateWorldModel(GameManager gameManager, List<Action> actions, List<Goal> goals) : base(gameManager, actions)
        {
            this.Parent = null;
            this.Goals = new Dictionary<string, Goal>();

            foreach (var goal in goals)
            {
                this.Goals.Add(goal.Name,goal);
            }
        }

        public void Initialize()
        {
            this.ActionEnumerator.Reset();
        }

        public override object GetProperty(string propertyName)
        {
            if (propertyName.Equals(Properties.MANA)) return this.GameManager.characterData.Mana;

            if (propertyName.Equals(Properties.XP)) return this.GameManager.characterData.XP;

            if (propertyName.Equals(Properties.MAXHP)) return this.GameManager.characterData.MaxHP;

            if (propertyName.Equals(Properties.HP)) return this.GameManager.characterData.HP;

            if (propertyName.Equals(Properties.MONEY)) return this.GameManager.characterData.Money;

            if (propertyName.Equals(Properties.TIME)) return this.GameManager.characterData.Time;

            if (propertyName.Equals(Properties.LEVEL)) return this.GameManager.characterData.Level;

            if (propertyName.Equals(Properties.POSITION))
                return this.GameManager.characterData.CharacterGameObject.transform.position;

            return true;
        }

        public override void SetProperty(string propertyName, object value)
        {
            //this method does nothing, because you should not directly set a property of the CurrentStateWorldModel
        }

        public override int GetNextPlayer()
        {
            //in the current state, the next player is always player 0
            return 0;
        }

        public void updateWorld()
        {
            this.PropertiesCharacter[MANA] = this.GameManager.characterData.Mana;
            this.PropertiesCharacter[XP] = this.GameManager.characterData.XP;
            this.PropertiesCharacter[MAXHP] = this.GameManager.characterData.MaxHP;
            this.PropertiesCharacter[HP] = this.GameManager.characterData.HP;
            this.PropertiesCharacter[MONEY] = this.GameManager.characterData.Money;
            this.PropertiesCharacter[TIME] = this.GameManager.characterData.Time;
            this.PropertiesCharacter[LEVEL] = this.GameManager.characterData.Level;
            this.PropertiesCharacter[POSITION] = this.GameManager.characterData.CharacterGameObject.transform.position;

            for(int i=0; i < 14; i++)
            {
                this.PropertiesWorld[i] = false;
            }

            foreach (var chest in GameObject.FindGameObjectsWithTag("Chest"))
            {
                base.SetProperty(chest.name, true);
            }
            foreach (var orc in GameObject.FindGameObjectsWithTag("Orc"))
            {
                base.SetProperty(orc.name, true);
            }
            foreach (var skeleton in GameObject.FindGameObjectsWithTag("Skeleton"))
            {
                base.SetProperty(skeleton.name, true);
            }
            foreach (var potion in GameObject.FindGameObjectsWithTag("ManaPotion"))
            {
                base.SetProperty(potion.name, true);
            }
            foreach (var potion in GameObject.FindGameObjectsWithTag("HealthPotion"))
            {
                base.SetProperty(potion.name, true);
            }
            foreach (var dragon in GameObject.FindGameObjectsWithTag("Dragon"))
            {
                base.SetProperty(dragon.name, true);
            }

        }
    }
}
