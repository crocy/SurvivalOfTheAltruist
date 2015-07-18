using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SurvivalOfTheAlturist.Creatures {

    [CustomEditor(typeof(GroupController))]
    public class GroupControllerEditor : Editor {

        public const string PathToGroups = "Assets/Groups/";

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GroupController controller = target as GroupController;
            GUIStyle styleBold = GUI.skin.label;
            styleBold.fontStyle = FontStyle.Bold;

            EditorGUILayout.Space();

            if (GUILayout.Button("New group")) {
                Group group = new Group();
                AssetDatabase.CreateAsset(group, PathToGroups + "Group.asset");
                AssetDatabase.SaveAssets();
                Selection.activeObject = group;
                return;
            }

//            EditorGUILayout.Space();
//            EditorGUILayout.LabelField("Params:", styleBold);
//
//            if (SimulationReport.IsSimulationRunning) {
//                EditorGUILayout.LabelField("Simulation time", SimulationReport.GetCurrentSimulation().CurrentTime + "");
//            } else {
//                EditorGUILayout.LabelField("Simulation not running!", styleBold);
//            }
        }

    }
}

