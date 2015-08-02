using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SurvivalOfTheAlturist.Creatures {

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Group))]
    public class GroupEditor : Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var group = target as Group;
            GUIStyle styleBold = GUI.skin.label;
            styleBold.fontStyle = FontStyle.Bold;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Group ID:", group.GroupId + "", styleBold);
        }

    }

}

