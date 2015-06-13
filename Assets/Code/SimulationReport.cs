using AssemblyCSharp;
using System;
using UnityEngine;
using System.IO;

namespace SurvivalOfTheAlturist {

    public class SimulationReport {

#region Class fields

        public const string PathToReports = "Reports";
        public const string ReportsFileName = "Report - {0}";

        private static Simulation currentSimulation;

#endregion

#region Properties

        public static bool IsSimulationRunning { get { return currentSimulation != null; } }

#endregion

        public static Simulation StartSimulationReport() {
            if (currentSimulation != null) {
                throw new Exception("A simulation report is already in progress!");
            }

            currentSimulation = new Simulation();
            currentSimulation.startTime = Time.time;
            return currentSimulation;
        }

        public static Simulation StopSimulationReport() {
            if (currentSimulation == null) {
                throw new Exception("No simulation started yet!");
            }

            currentSimulation.endTime = Time.time;

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

        public static void SaveToFile(Simulation simulation) {
            string path = PathToReports + "/" + string.Format(ReportsFileName, simulation.Date).Replace(":", ".") + ".txt";

            using (var fileStream = File.CreateText(path)) {
                fileStream.WriteLine(simulation.GetReport());
            }

            Debug.LogFormat("Report saved to: {0}", path);
        }

    }
}
