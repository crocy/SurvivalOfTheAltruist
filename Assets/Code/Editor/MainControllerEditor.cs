using UnityEngine;
using System.Collections;
using UnityEditor;

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
            } else {
                EditorGUILayout.LabelField("Simulation not running!", styleBold);
            }
        }

        //        public override bool RequiresConstantRepaint() {
        //            return true;
        //        }
    }
}

