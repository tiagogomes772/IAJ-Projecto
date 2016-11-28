using System.Collections.Generic;
using Assets.Scripts.GameManager;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class WorldModel
    {
        public const int MANA = 0;
        public const int HP = 1;
        public const int MAXHP = 2;
        public const int XP = 3;
        public const int TIME = 4;
        public const int MONEY = 5;
        public const int LEVEL = 6;
        public const int POSITION = 7;

        public const int CHEST0 = 0;
        public const int CHEST1 = 1;
        public const int CHEST2 = 2;
        public const int CHEST3 = 3;
        public const int CHEST4 = 4;
        public const int SKELETON0 = 5;
        public const int SKELETON1 = 6;
        public const int ORC0 = 7;
        public const int ORC1 = 8;
        public const int MANAPOTION0 = 9;
        public const int MANAPOTION1 = 10;
        public const int HEALTHPOTION0 = 11;
        public const int HEALTHPOTION1 = 12;
        public const int DRAGON = 13;

        private Dictionary<string, float> GoalValues { get; set; }

        public object[] PropertiesCharacter { get; set; }
        public object[] PropertiesWorld { get; set; }

        private List<Action> Actions { get; set; }
        protected IEnumerator<Action> ActionEnumerator { get; set; }

        protected WorldModel Parent { get; set; }

        public WorldModel(List<Action> actions)
        {
            this.PropertiesCharacter = fillPropertiesCharacter();
            this.PropertiesWorld = fillPropertiesOther();
            this.GoalValues = new Dictionary<string, float>();
            this.Actions = actions;
            this.ActionEnumerator = actions.GetEnumerator();
        }

        public WorldModel(WorldModel parent)
        {
            this.PropertiesCharacter = new object[8];
            this.PropertiesWorld = new object[14];
            parent.PropertiesCharacter.CopyTo(this.PropertiesCharacter, 0);
            parent.PropertiesWorld.CopyTo(this.PropertiesWorld, 0);
            this.Actions = parent.Actions;
            this.Parent = parent;
            this.ActionEnumerator = this.Actions.GetEnumerator();
        }

        public virtual object GetProperty(string propertyName)
        {
            switch (propertyName)
            {
                case Properties.MANA:
                    return this.PropertiesCharacter[MANA];
                case Properties.HP:
                    return this.PropertiesCharacter[HP];
                case Properties.XP:
                    return this.PropertiesCharacter[XP];
                case Properties.MAXHP:
                    return this.PropertiesCharacter[MAXHP];
                case Properties.LEVEL:
                    return this.PropertiesCharacter[LEVEL];
                case Properties.POSITION:
                    return this.PropertiesCharacter[POSITION];
                case Properties.MONEY:
                    return this.PropertiesCharacter[MONEY];
                case Properties.TIME:
                    return this.PropertiesCharacter[TIME];
                case "Chest":
                    return this.PropertiesWorld[CHEST0];
                case "Chest (1)":
                    return this.PropertiesWorld[CHEST1];
                case "Chest (2)":
                    return this.PropertiesWorld[CHEST2];
                case "Chest (3)":
                    return this.PropertiesWorld[CHEST3];
                case "Chest (4)":
                    return this.PropertiesWorld[CHEST4];
                case "Skeleton (2)":
                    return this.PropertiesWorld[SKELETON0];
                case "Skeleton (3)":
                    return this.PropertiesWorld[SKELETON1];
                case "Orc":
                    return this.PropertiesWorld[ORC0];
                case "Orc (1)":
                    return this.PropertiesWorld[ORC1];
                case "ManaPotion":
                    return this.PropertiesWorld[MANAPOTION0];
                case "ManaPotion (1)":
                    return this.PropertiesWorld[MANAPOTION1];
                case "HealthPotion":
                    return this.PropertiesWorld[HEALTHPOTION0];
                case "HealthPotion (1)":
                    return this.PropertiesWorld[HEALTHPOTION1];
                case "Dragon":
                    return this.PropertiesWorld[DRAGON];
                default: return null;
            }
        }

        public virtual void SetProperty(string propertyName, object value)
        {
            switch (propertyName)
            {
                case Properties.MANA:
                    this.PropertiesCharacter[MANA] = value;
                    break;
                case Properties.HP:
                    this.PropertiesCharacter[HP] = value;
                    break;
                case Properties.XP:
                    this.PropertiesCharacter[XP] = value;
                    break;
                case Properties.MAXHP:
                    this.PropertiesCharacter[MAXHP] = value;
                    break;
                case Properties.LEVEL:
                    this.PropertiesCharacter[LEVEL] = value;
                    break;
                case Properties.POSITION:
                    this.PropertiesCharacter[POSITION] = value;
                    break;
                case Properties.MONEY:
                    this.PropertiesCharacter[MONEY] = value;
                    break;
                case Properties.TIME:
                    this.PropertiesCharacter[TIME] = value;
                    break;
                case "Chest":
                    this.PropertiesWorld[CHEST0] = value;
                    break;
                case "Chest (1)":
                    this.PropertiesWorld[CHEST1] = value;
                    break;
                case "Chest (2)":
                    this.PropertiesWorld[CHEST2] = value;
                    break;
                case "Chest (3)":
                    this.PropertiesWorld[CHEST3] = value;
                    break;
                case "Chest (4)":
                    this.PropertiesWorld[CHEST4] = value;
                    break;
                case "Skeleton (2)":
                    this.PropertiesWorld[SKELETON0] = value;
                    break;
                case "Skeleton (3)":
                    this.PropertiesWorld[SKELETON1] = value;
                    break;
                case "Orc":
                    this.PropertiesWorld[ORC0] = value;
                    break;
                case "Orc (1)":
                    this.PropertiesWorld[ORC1] = value;
                    break;
                case "ManaPotion":
                    this.PropertiesWorld[MANAPOTION0] = value;
                    break;
                case "ManaPotion (1)":
                    this.PropertiesWorld[MANAPOTION1] = value;
                    break;
                case "HealthPotion":
                    this.PropertiesWorld[HEALTHPOTION0] = value;
                    break;
                case "HealthPotion (1)":
                    this.PropertiesWorld[HEALTHPOTION1] = value;
                    break;
                case "Dragon":
                    this.PropertiesWorld[DRAGON] = value;
                    break;
                default: break;
            }

        }

        public virtual WorldModel GenerateChildWorldModel()
        {
            return new WorldModel(this);
        }

        private object[] fillPropertiesOther()
        {
            object[] aux = new object[14];

            for (int i = 0; i < aux.Length; i++)
            {
                aux[i] = true;
            }

            return aux;
        }
        private object[] fillPropertiesCharacter()
        {
            object[] aux = new object[8];
            aux[MANA] = 0;
            aux[HP] = 10;
            aux[MAXHP] = 10;
            aux[XP] = 0;
            aux[TIME] = 0.0f;
            aux[MONEY] = 0;
            aux[LEVEL] = 1;
            aux[POSITION] = new Vector3((float)14.39999, 1, (float)16.6);

            return aux;
        }

        public virtual Action GetNextAction()
        {
            Action action = null;
            //returns the next action that can be executed or null if no more executable actions exist
            if (this.ActionEnumerator.MoveNext())
            {
                action = this.ActionEnumerator.Current;
            }

            while (action != null && !action.CanExecute(this))
            {
                if (this.ActionEnumerator.MoveNext())
                {
                    action = this.ActionEnumerator.Current;
                }
                else
                {
                    action = null;
                }
            }

            return action;
        }

        public virtual Action[] GetExecutableActions()
        {
            return this.Actions.Where(a => a.CanExecute(this)).ToArray();
        }

        public virtual bool IsTerminal()
        {
            return true;
        }


        public virtual float GetScore()
        {
            return 0.0f;
        }

        public virtual int GetNextPlayer()
        {
            return 0;
        }

        public virtual void CalculateNextPlayer()
        {
        }

        public virtual float GetGoalValue(string goalName)
        {
            return 1.0f;
        }

        public virtual void SetGoalValue(string goalName, float value)
        {

        }

        public float CalculateDiscontentment(List<Goal> goals)
        {
            var discontentment = 0.0f;

            foreach (var goal in goals)
            {
                var newValue = this.GetGoalValue(goal.Name);

                discontentment += goal.GetDiscontentment(newValue);
            }

            return discontentment;
        }

    }
}




