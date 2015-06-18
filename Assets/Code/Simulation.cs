using System;
using System.Collections.Generic;
using SurvivalOfTheAlturist.Creatures;
using System.Text;
using UnityEngine;

namespace SurvivalOfTheAlturist {

    public class Simulation {

#region Class fields

        private readonly int id = UnityEngine.Random.Range(0, int.MaxValue);
        private readonly string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public float startTime;
        public float endTime;

        public int numOfAllEnergy;
        public float sumOfAllEnergy;

        public List<Creature> creatures = new List<Creature>();

#endregion

#region Properties

        public int Id { get { return id; } }

        public string Date { get { return date; } }

        public float CurrentTime { get { return UnityEngine.Time.time - startTime; } }

        public float SimulationDuration { get { return endTime - startTime; } }

        public float SimulationCurrentDuration { 
            get { 
                if (endTime > 0) {
                    return SimulationDuration;
                }
                return Time.time - startTime;
            }
        }

#endregion

        public string GetReport() {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Simulation: ID = {0}, date = {1}, duration = {2} (start = {3}, end = {4})\n", id, date, SimulationCurrentDuration, startTime, endTime);
            builder.AppendFormat("Energy generated: num = {0}, sum = {1}\n", numOfAllEnergy, sumOfAllEnergy);

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

            builder.AppendFormat("Creatures: num = {0}\n", creatures.Count);
            foreach (var item in creatures) {
                builder.AppendFormat("{0}\n", item.GetReportString());
            }

            return builder.ToString();
        }

    }
}

