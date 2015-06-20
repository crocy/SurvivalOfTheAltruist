using System;
using System.Collections.Generic;
using SurvivalOfTheAlturist.Creatures;
using System.Text;
using UnityEngine;

namespace SurvivalOfTheAlturist {

    public class Simulation : IReport {

#region Class fields

        //        public readonly List<Creature> creatures = new List<Creature>();
        public readonly List<Group> groups = new List<Group>();

        private readonly int id = UnityEngine.Random.Range(0, int.MaxValue);
        private readonly string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        private readonly int randomSeed = UnityEngine.Random.seed;

        public float startTime;
        public float endTime;

        public int numOfAllEnergy;
        public float sumOfAllEnergy;

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

#region IReport implementation

        public string GetReport() {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Simulation: ID = {0}, date = {1}, duration = {2}, random seed = {3}", id, date, SimulationCurrentDuration, randomSeed);
            if (startTime > 0) {
                builder.AppendFormat(" (start = {0}, end = {1})", startTime, endTime);
            }
            builder.AppendFormat("\n\n");
            builder.AppendFormat("Energy generated: num = {0}, sum = {1}\n\n", numOfAllEnergy, sumOfAllEnergy);

            builder.AppendFormat("Groups: num = {0}\n", groups.Count);
            foreach (var item in groups) {
                builder.AppendFormat("{0}\n\n", item.GetReport());
            }

//            builder.AppendFormat("Creatures: num = {0}\n", creatures.Count);
//            foreach (var item in creatures) {
//                builder.AppendFormat("{0}\n", item.GetReport());
//            }

            return builder.ToString();
        }

#endregion

    }
}

