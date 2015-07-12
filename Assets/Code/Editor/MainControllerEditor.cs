using UnityEngine;
using System.Collections;
using UnityEditor;
using SurvivalOfTheAlturist.Environment;
using SurvivalOfTheAlturist.Creatures;

namespace SurvivalOfTheAlturist {

    [CustomEditor(typeof(MainController))]
    public class MainControllerEditor : Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            MainController controller = target as MainController;
            GUIStyle styleBold = GUI.skin.label;
            styleBold.fontStyle = FontStyle.Bold;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Params:", styleBold);
            EditorGUILayout.LabelField("Random seed: ", UnityEngine.Random.seed + "");

            if (SimulationReport.IsSimulationRunning) {
                EditorGUILayout.LabelField("Simulation time", SimulationReport.GetCurrentSimulation().CurrentTime + "");

                float generationRate = controller.EnvironmentController.CurrentEnergyGenerationRate;
                float depletionRate = controller.CreatureController.EnergyDepletionRateSum;

                EditorGUILayout.LabelField("Energy generation rate", generationRate + "");
                EditorGUILayout.LabelField("Energy depletion rate", depletionRate + "");
                EditorGUILayout.LabelField("Energy flow rate", (generationRate - depletionRate) + "");
            } else {
                EditorGUILayout.LabelField("Simulation not running!", styleBold);
            }
        }

        public override bool RequiresConstantRepaint() {
            return true;
        }

    }
}

