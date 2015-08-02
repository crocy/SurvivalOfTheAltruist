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
        private readonly MainController mainController;
        private readonly float startTime;

        private float endTime;

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

        public float StartTime { get { return startTime; } }

        public float EndTime { 
            get { return endTime; }
            set { endTime = value; }
        }

#endregion

        public Simulation(MainController mainController, float startTime) {
            this.mainController = mainController;
            this.startTime = startTime;
        }

#region IReport implementation

        public string GetReport() {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Simulation: ID = {0}, date = {1}, duration = {2}, random seed = {3}", id, date, SimulationCurrentDuration, randomSeed);
//            if (startTime > 0) {
//                builder.AppendFormat(" (start = {0}, end = {1})", startTime, endTime);
//            }
            builder.AppendLine("\n\n#################################################################");
            builder.AppendLine(mainController.GetReport());
            builder.AppendLine();
            builder.AppendLine(mainController.EnvironmentController.GetReport());
            builder.AppendLine();
            builder.AppendLine(mainController.CreatureController.GetReport());
            builder.AppendLine("#################################################################\n");

            builder.AppendFormat("Groups: num = {0}\n\n", groups.Count);
            foreach (var item in groups) {
                builder.AppendFormat("{0}\n\n", item.GetReport());
            }

            return builder.ToString();
        }

#endregion

        public string GetGroupsTags() {
            StringBuilder builder = new StringBuilder();
            foreach (var group in groups) {
                builder.AppendFormat("{0}, ", group.Tag);
            }
            builder.Length -= 2; // remove the last ", " part

            return builder.ToString();
        }

        public string GetGeneratorTag() {
            return mainController.EnvironmentController.EnergyGenerator.Tag;
        }
    }
}

