using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SurvivalOfTheAlturist.Creatures {

    [CustomEditor(typeof(CreatureController))]
    public class CreatureControllerEditor : Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            CreatureController controller = target as CreatureController;
            GUIStyle styleBold = GUI.skin.label;
            styleBold.fontStyle = FontStyle.Bold;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Params:", styleBold);

            if (SimulationReport.IsSimulationRunning) {
                EditorGUILayout.LabelField("Simulation time", SimulationReport.GetCurrentSimulation().CurrentTime + "");

                float sumEnergy = 0;
                foreach (var creature in controller.Creatures) {
                    sumEnergy += creature.Energy;
                }

                EditorGUILayout.LabelField("Energy sum", sumEnergy + "");
            } else {
                EditorGUILayout.LabelField("Simulation not running!", styleBold);
            }
        }

        public override bool RequiresConstantRepaint() {
            return true;
        }
    }
}

