using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SurvivalOfTheAlturist.Environment {

    [CustomEditor(typeof(EnvironmentController))]
    public class EnvironmentControllerEditor : Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EnvironmentController controller = target as EnvironmentController;
            GUIStyle styleBold = GUI.skin.label;
            styleBold.fontStyle = FontStyle.Bold;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Params:", styleBold);

            if (SimulationReport.IsSimulationRunning) {
                EditorGUILayout.LabelField("Simulation time", SimulationReport.GetCurrentSimulation().CurrentTime + "");
                EditorGUILayout.LabelField("Energy generation rate", controller.CurrentEnergyGenerationRate + "");
            } else {
                EditorGUILayout.LabelField("Simulation not running!", styleBold);
            }
        }

        public override bool RequiresConstantRepaint() {
            return SimulationReport.IsSimulationRunning;
        }
    }
}

