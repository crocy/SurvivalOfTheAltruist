using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace SurvivalOfTheAlturist.Creatures {

    public class Group : IReport {

#region Class fields

        private readonly int groupId = UnityEngine.Random.Range(0, int.MaxValue);
        private readonly List<Creature> creatures = new List<Creature>();

        private float altruismMin = Creature.AltruismMin;
        private float altruismMax = Creature.AltruismMax;

        private float energyStartMin = Creature.EnergyStartMin;
        private float energyStartMax = Creature.EnergyStartMax;

#endregion

#region Properties

        public int GroupId { get { return groupId; } }

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
            foreach (var item in creatures) {
                builder.AppendFormat("  {0}\n", item.GetReport());
            }

            return builder.ToString();
        }

#endregion

        public override string ToString() {
            return string.Format("[Group: GroupId = {0}, AltruismMin = {1:0.000}, AltruismMax = {2:0.000}, EnergyStartMin = {3:0.000}, EnergyStartMax = {4:0.000}, CreaturesCount = {5}]",
                GroupId, AltruismMin, AltruismMax, EnergyStartMin, EnergyStartMax, CreaturesCount);
        }

        public void InitGroup(int numOfCreatures, CreatureController creatureController) {
            Creature creature;

            for (int i = 0; i < numOfCreatures; i++) {
//                creature = UnityEngine.Object.Instantiate(creaturePrefab);
                creature = creatureController.GenerateCreature(groupId);
                creature.name = "Creature (" + i + ")";
                creature.Altruism = UnityEngine.Random.Range(altruismMin, altruismMax);
                creature.CollectEnergy(UnityEngine.Random.Range(energyStartMin, energyStartMax));

                creatures.Add(creature);
            }

            SimulationReport.GetCurrentSimulation().groups.Add(this);
        }

    }
}

