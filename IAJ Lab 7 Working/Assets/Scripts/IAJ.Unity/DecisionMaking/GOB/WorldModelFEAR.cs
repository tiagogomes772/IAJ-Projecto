//using System.Collections.Generic;
//using Assets.Scripts.GameManager;
//using System.Linq;
//using UnityEngine;

//namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
//{
//    public class WorldModel
//    {
//        private Dictionary<string, float> GoalValues { get; set; }

//        private object[] Properties { get; set; }
//        private Dictionary<string, int> IndexProperties { get; set; }

//        private List<Action> Actions { get; set; }
//        protected IEnumerator<Action> ActionEnumerator { get; set; }

//        protected WorldModel Parent { get; set; }

//        public WorldModel(List<Action> actions)
//        {

//            this.Properties = new object[22];
//            this.IndexProperties = fillIndexProperties();
//            this.GoalValues = new Dictionary<string, float>();
//            this.Actions = actions;
//            this.ActionEnumerator = actions.GetEnumerator();
//        }

//        public WorldModel(WorldModel parent)
//        {
//            this.Properties = new object[22];
//            parent.Properties.CopyTo(this.Properties, 0);
//            this.IndexProperties = new Dictionary<string, int>(parent.IndexProperties); //parent.IndexProperties.ToDictionary(entry => entry.Key,
//                                                                                        //entry => entry.Value);
//            this.Actions = new List<Action>(parent.Actions);//.ToList();
//            this.Parent = parent;
//            this.ActionEnumerator = this.Actions.GetEnumerator();
//        }

//        public virtual object GetProperty(string propertyName)
//        {
//            return this.Properties[this.IndexProperties[propertyName]];

//        }

//        public virtual void SetProperty(string propertyName, object value)
//        {
//            this.Properties[this.IndexProperties[propertyName]] = value;

//        }

//        public virtual WorldModel GenerateChildWorldModel()
//        {
//            return new WorldModel(this);
//        }

//        private Dictionary<string, int> fillIndexProperties()
//        {
//            int i = 0;
//            Dictionary<string, int> aux = new Dictionary<string, int>();
//            aux.Add(Assets.Scripts.GameManager.Properties.MANA, 0);
//            this.Properties[0] = 0;
//            aux.Add(Assets.Scripts.GameManager.Properties.HP, 1);
//            this.Properties[1] = 10;
//            aux.Add(Assets.Scripts.GameManager.Properties.MAXHP, 2);
//            this.Properties[2] = 10;
//            aux.Add(Assets.Scripts.GameManager.Properties.XP, 3);
//            this.Properties[3] = 0;
//            aux.Add(Assets.Scripts.GameManager.Properties.TIME, 4);
//            this.Properties[4] = 0.0f;
//            aux.Add(Assets.Scripts.GameManager.Properties.MONEY, 5);
//            this.Properties[5] = 0;
//            aux.Add(Assets.Scripts.GameManager.Properties.LEVEL, 6);
//            this.Properties[6] = 1;
//            aux.Add(Assets.Scripts.GameManager.Properties.POSITION, 7);
//            this.Properties[7] = new Vector3((float)14.39999, 1, (float)16.6);
//            i = 7;
//            foreach (var chest in GameObject.FindGameObjectsWithTag("Chest"))
//            {
//                i = i + 1;
//                aux.Add(chest.name, i);
//                this.Properties[i] = true;
//            }
//            foreach (var skeleton in GameObject.FindGameObjectsWithTag("Skeleton"))
//            {
//                i = i + 1;
//                aux.Add(skeleton.name, i);
//                this.Properties[i] = true;
//            }
//            foreach (var orc in GameObject.FindGameObjectsWithTag("Orc"))
//            {
//                i = i + 1;
//                aux.Add(orc.name, i);
//                this.Properties[i] = true;
//            }
//            foreach (var manaPotion in GameObject.FindGameObjectsWithTag("ManaPotion"))
//            {
//                i = i + 1;
//                aux.Add(manaPotion.name, i);
//                this.Properties[i] = true;
//            }
//            foreach (var healthPotion in GameObject.FindGameObjectsWithTag("HealthPotion"))
//            {
//                i = i + 1;
//                aux.Add(healthPotion.name, i);
//                this.Properties[i] = true;
//            }

//            i++;
//            aux.Add(GameObject.FindGameObjectsWithTag("Dragon").First().name, i);
//            this.Properties[i] = true;


//            return aux;
//        }

//        public virtual Action GetNextAction()
//        {
//            Action action = null;
//            //returns the next action that can be executed or null if no more executable actions exist
//            if (this.ActionEnumerator.MoveNext())
//            {
//                action = this.ActionEnumerator.Current;
//            }

//            while (action != null && !action.CanExecute(this))
//            {
//                if (this.ActionEnumerator.MoveNext())
//                {
//                    action = this.ActionEnumerator.Current;
//                }
//                else
//                {
//                    action = null;
//                }
//            }

//            return action;
//        }

//        public virtual Action[] GetExecutableActions()
//        {
//            return this.Actions.Where(a => a.CanExecute(this)).ToArray();
//        }

//        public virtual bool IsTerminal()
//        {
//            return true;
//        }


//        public virtual float GetScore()
//        {
//            return 0.0f;
//        }

//        public virtual int GetNextPlayer()
//        {
//            return 0;
//        }

//        public virtual void CalculateNextPlayer()
//        {
//        }

//        public virtual float GetGoalValue(string goalName)
//        {
//            return 1.0f;
//        }

//        public virtual void SetGoalValue(string goalName, float value)
//        {

//        }

//        public float CalculateDiscontentment(List<Goal> goals)
//        {
//            var discontentment = 0.0f;

//            foreach (var goal in goals)
//            {
//                var newValue = this.GetGoalValue(goal.Name);

//                discontentment += goal.GetDiscontentment(newValue);
//            }

//            return discontentment;
//        }

//    }
//}
