using System;
using UnityEngine;
using System.IO;
using System.Text;

namespace SurvivalOfTheAlturist {

    public class SimulationReport {

        public enum SaveType {
            All,
            TextOnly,
            CsvOnly
        }

#region Class fields

        public const string PathToReports = "Reports";
        public const string ReportsFileName = "Report - {0}";

        private static Simulation currentSimulation;

#endregion

#region Properties

        public static bool IsSimulationRunning { get { return currentSimulation != null; } }

#endregion

        public static Simulation StartSimulationReport(MainController mainController) {
            if (currentSimulation != null) {
                throw new Exception("A simulation report is already in progress!");
            }

            currentSimulation = new Simulation(mainController, Time.time);
            return currentSimulation;
        }

        public static Simulation StopSimulationReport() {
            if (currentSimulation == null) {
                throw new Exception("No simulation started yet!");
            }

            currentSimulation.EndTime = Time.time;

            Simulation simulation = currentSimulation;
            currentSimulation = null;
            return simulation;
        }

        public static Simulation GetCurrentSimulation() {
            if (currentSimulation == null) {
                throw new Exception("No simulation started yet!");
            }

            return currentSimulation;
        }

        public static void PrintCurrentReport() {
            if (currentSimulation == null) {
                Debug.LogError("No simulation started yet!");
                return;
            }

            Debug.Log(currentSimulation.GetReport());
        }

        public static void SaveToFile(Simulation simulation, SaveType saveType = SaveType.All) {
            //            string path = PathToReports + "/" + string.Format(ReportsFileName, simulation.Date).Replace(":", ".") + ".txt";
            //            string path = PathToReports + "/" + string.Format(ReportsFileName, simulation.Date).Replace(":", ".") + " - G[" + simulation.groups.Count + "].txt";
            string name = string.Format(ReportsFileName, simulation.Date).Replace(":", ".");
            //            name += string.Format(" - G[{0}].txt", simulation.groups.Count);
            name += string.Format(" - {0} - {1} - {2}.txt", simulation.EnergyStorageCapacityTagFormat, simulation.GeneratorTagFormat, simulation.GetGroupsTags());
            string path = PathToReports + "/" + name;

            // save all or text only format
            switch (saveType) {
                case SaveType.All:
                case SaveType.TextOnly:
                    string report = simulation.GetReport();

                    using (var fileStream = File.CreateText(path)) {
                        fileStream.WriteLine(report);
                    }
                    Debug.LogFormat("Report saved to: {0}\nReport:\n{1}", path, report);
                    break;
            }

            // save all or cvs only
            switch (saveType) {
                case SaveType.All:
                case SaveType.CsvOnly:
                    StringBuilder builder = new StringBuilder();
                    name = name.Replace(".txt", ".csv");
                    path = PathToReports + "/" + name;

                    // get header
                    builder.AppendFormat("{1}{0}{2}\n", Export.Separator, currentSimulation.groups[0][0].GetHeader(), GetHeader());

                    foreach (var group in currentSimulation.groups) {
                        foreach (var creature in group.Creatures) {
                            builder.AppendFormat("{1}{0}{2}\n", Export.Separator, creature.GetExport(), GetExport(simulation));
                        }
                    }

                    using (var fileStream = File.CreateText(path)) {
                        fileStream.WriteLine(builder);
                    }
                    break;
            }
        }

#region IExport implementation

        public static string GetHeader() {
            return string.Format("esc{0}generator", Export.Separator);
        }

        public static string GetExport(Simulation simulation) {
            return string.Format("{1}{0}\"{2}\"", Export.Separator, simulation.EnergyStorageCapacity, simulation.GeneratorTag);
        }

#endregion
    }
}
