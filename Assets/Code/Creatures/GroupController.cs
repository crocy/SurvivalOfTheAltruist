using UnityEngine;
using System.Collections.Generic;
using System;

namespace SurvivalOfTheAlturist.Creatures {

    public class GroupController : MonoBehaviour, IController {

#region Class fields

#endregion

#region Serialized fields

        [SerializeField]
        private MainController mainContoller = null;

        [Header("Properties")]
        
        [SerializeField]
        private List<Group> groups = new List<Group>();

#endregion

#region Properties

        public MainController MainContoller { get { return mainContoller; } }

        public int NumOfGroups { get { return groups.Count; } }

#endregion

#region Unity override

#endregion

#region IController implementation

        public void Reset() {
//             Debug.Log("Resetting group controller...");
            InitGroups();
            gameObject.SetActive(true);
//             Debug.Log("Done.");
        }

#endregion

        private void InitGroups() {
            if (groups.Count == 0) {
                throw new Exception("No groups defined!");
            }

            SimulationReport.GetCurrentSimulation().groups.Clear();

            foreach (var group in groups) {
                group.InitGroup(MainContoller.CreatureController);
                SimulationReport.GetCurrentSimulation().groups.Add(group);
            }
        }

    }
}

