using AssemblyCSharp;
using System;
using UnityEngine;

namespace SurvivalOfTheAlturist {

    public class SimulationReport {

#region Class fields

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

    }
}
