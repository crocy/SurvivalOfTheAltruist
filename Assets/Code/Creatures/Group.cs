using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace SurvivalOfTheAlturist.Creatures {

    public class Group : ScriptableObject, IReport {

#region Class fields

        private readonly int groupId = UnityEngine.Random.Range(0, int.MaxValue);

        private float groupAltruism;

#endregion

#region Serialized fields

        [SerializeField]
        [Range(1, 100)]
        private int genNumOfCreatures = 10;

        [SerializeField]
        private Color groupColor = Color.green;

        [SerializeField]
        [Range(Creature.AltruismMin, Creature.AltruismMax)]
        private float altruismMin = Creature.AltruismMin;
        [SerializeField]
        [Range(Creature.AltruismMin, Creature.AltruismMax)]
        private float altruismMax = Creature.AltruismMax;

        [SerializeField]
        [Range(Creature.EnergyStartMin, Creature.EnergyStartMax)]
        private float energyStartMin = Creature.EnergyStartMin;
        [SerializeField]
        [Range(Creature.EnergyStartMin, Creature.EnergyStartMax)]
        private float energyStartMax = Creature.EnergyStartMax;

        [SerializeField]
        private List<Creature> creatures = new List<Creature>();

#endregion

#region Properties

        public int GroupId { get { return groupId; } }

        public int GenNumOfCreatures { get { return genNumOfCreatures; } }

        public Color GroupColor { get { return groupColor; } }

        public float GroupAltruism { get { return groupAltruism; } }

        public float AltruismMin {
            get { return altruismMin; }
            set { altruismMin = value; }
        }

        public float AltruismMax {
            get { return altruismMax; }
            set { altruismMax = value; }
        }

        public float EnergyStartMin {
            get { return energyStartMin; }
            set { energyStartMin = value; }
        }

        public float EnergyStartMax {
            get { return energyStartMax; }
            set { energyStartMax = value; }
        }

        public int CreaturesCount { get { return creatures.Count; } }

        public float EnergyCollectedSum {
            get {
                float energyCollectedSum = 0;
                foreach (var creature in creatures) {
                    energyCollectedSum += creature.EnergyCollected;
                }
                return energyCollectedSum;
            }
        }

        public float EnergySharedSum {
            get {
                float energySharedSum = 0;
                foreach (var creature in creatures) {
                    energySharedSum += creature.EnergyShared;
                }
                return energySharedSum;
            }
        }

#endregion

#region Indexers

        public Creature this[int i] { get { return creatures[i]; } }

#endregion

#region IReport implementation

        public string GetReport() {
            StringBuilder builder = new StringBuilder();

            // sort creatures first by endTime and if that not applicable, by energy
            creatures.Sort(delegate(Creature x, Creature y) {
                if (x.State != CreatureState.Dead) {
                    if (y.State != CreatureState.Dead) {
                        return (int)Mathf.Sign(y.Energy - x.Energy);
                    } else {
                        return -1;
                    }
                } else if (y.State != CreatureState.Dead) {
                    return 1;
                }

                return (int)Mathf.Sign(y.EndTime - x.EndTime);
            });

            builder.AppendFormat("{0}\n", ToString());
            builder.AppendFormat("  (Energy sum: collected = {0}, shared = {1})\n", EnergyCollectedSum, EnergySharedSum);
            builder.Append("-------------------------------------------------------------------------------\n");
            foreach (var item in creatures) {
                builder.AppendFormat("  {0}\n", item.GetReport());
            }
            builder.Append("-------------------------------------------------------------------------------\n");

            return builder.ToString();
        }

#endregion

        public override string ToString() {
            return string.Format("[Group: \"{0}\", GroupId = {1}, AltruismMin = {2:0.000}, AltruismMax = {3:0.000}, EnergyStartMin = {4:0.000}, EnergyStartMax = {5:0.000}, CreaturesCount = {6}]",
                name, GroupId, AltruismMin, AltruismMax, EnergyStartMin, EnergyStartMax, CreaturesCount);
        }

        public void InitGroup(CreatureController creatureController) {
            Validate();

            Creature creature;

            creatures.Clear();
            for (int i = 0; i < genNumOfCreatures; i++) {
//                creature = UnityEngine.Object.Instantiate(creaturePrefab);
                creature = creatureController.GenerateCreature(this);
                creature.name = string.Format("Creature ({0:00})", i);
                creature.Altruism = UnityEngine.Random.Range(altruismMin, altruismMax);
                creature.CollectEnergy(UnityEngine.Random.Range(energyStartMin, energyStartMax));

                groupAltruism += creature.Altruism;

                creatures.Add(creature);
            }

            groupAltruism = groupAltruism / genNumOfCreatures; // average out the group altruism (score)
        }

        private void Validate() {
            if (altruismMin < 0) {
                throw new Exception("Group altruismMin can't be lower than 0.");
            } else if (altruismMax > 1) {
                throw new Exception("Group altruismMax can't be greater than 1.");
            } else if (altruismMin > altruismMax) {
                throw new Exception("Group altruismMin can't be greater than altruismMax.");
            }

            if (energyStartMin < 0) {
                throw new Exception("Group energyStartMin can't be lower than 0.");
            } else if (energyStartMax > 1) {
                throw new Exception("Group energyStartMax can't be greater than 1.");
            } else if (energyStartMin > energyStartMax) {
                throw new Exception("Group energyStartMin can't be greater than energyStartMax.");
            }
        }

    }
}

