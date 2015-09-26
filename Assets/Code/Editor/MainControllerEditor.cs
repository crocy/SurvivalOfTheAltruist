using UnityEngine;
using System.Collections;
using UnityEditor;
using SurvivalOfTheAlturist.Environment;
using SurvivalOfTheAlturist.Creatures;

namespace SurvivalOfTheAlturist {

    [CustomEditor(typeof(MainController))]
    public class MainControllerEditor : Editor {

        private bool drawCreatureController = false;
        private bool drawEnvironmentController = false;
        private bool drawGroupController = false;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            MainController controller = target as MainController;
            GUIStyle styleBold = GUI.skin.label;
            styleBold.fontStyle = FontStyle.Bold;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Params:", styleBold);
            EditorGUILayout.LabelField("Random seed: ", UnityEngine.Random.seed + "");

            if (SimulationReport.IsSimulationRunning) {
                EditorGUILayout.LabelField("Simulation progress:", (controller.SimulationsProgress * 100) + "%");
                EditorGUILayout.LabelField("Simulation time", SimulationReport.GetCurrentSimulation().CurrentTime + "");

                float generationRate = controller.EnvironmentController.CurrentEnergyGenerationRate;
                float depletionRate = controller.CreatureController.EnergyDepletionRateSum;

                EditorGUILayout.LabelField("Energy generation rate", generationRate + "");
                EditorGUILayout.LabelField("Energy depletion rate", depletionRate + "");
                EditorGUILayout.LabelField("Energy net flow rate", (generationRate - depletionRate) + "");
            } else {
                EditorGUILayout.LabelField("Simulation not running!", styleBold);
            }

            Editor editor = null;
            drawCreatureController = EditorGUILayout.Foldout(drawCreatureController, "Creature controller:");
            if (drawCreatureController) {
                editor = Editor.CreateEditor(controller.CreatureController);
                editor.DrawDefaultInspector();
            }

            drawEnvironmentController = EditorGUILayout.Foldout(drawEnvironmentController, "Environment controller:");
            if (drawEnvironmentController) {
                editor = Editor.CreateEditor(controller.EnvironmentController);
                editor.DrawDefaultInspector();
            }

            drawGroupController = EditorGUILayout.Foldout(drawGroupController, "Group controller:");
            if (drawGroupController) {
                editor = Editor.CreateEditor(controller.GroupController);
                editor.DrawDefaultInspector();
            }
        }

        public override bool RequiresConstantRepaint() {
            return SimulationReport.IsSimulationRunning;
        }

    }
}

