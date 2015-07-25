using UnityEditor;
using UnityEngine;

namespace SurvivalOfTheAlturist.Environment {

    [CustomEditor(typeof(EnergyGenerator))]
    public class EnergyGeneratorEditor : Editor {

        public const string PathToEnergyGenerators = "Assets/Generators/";

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
        
            EnergyGenerator generator = target as EnergyGenerator;
            GUIStyle styleBold = GUI.skin.label;
            styleBold.fontStyle = FontStyle.Bold;
        
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Params:", styleBold);
        
            if (SimulationReport.IsSimulationRunning) {
                float time = SimulationReport.GetCurrentSimulation().CurrentTime;
                EditorGUILayout.LabelField("Simulation time", time + "");
                EditorGUILayout.LabelField("Generator enabled", generator.GetEnergyGeneratorEnabled(time) + "");
                EditorGUILayout.LabelField("Energy generation rate", generator.GetEnergyGenerationRate(time) + "");
            } else {
                EditorGUILayout.LabelField("Simulation not running!", styleBold);
            }
        }

        public override bool RequiresConstantRepaint() {
            return SimulationReport.IsSimulationRunning;
        }

        [MenuItem("Custom/Create/Energy generator")]
        public static void CreateEnergyGenerator() {
            EnergyGenerator generator = ScriptableObject.CreateInstance<EnergyGenerator>();
            AssetDatabase.CreateAsset(generator, PathToEnergyGenerators + "EnergyGenerator.asset");
            AssetDatabase.SaveAssets();
            Selection.activeObject = generator;
        }

    }
}

