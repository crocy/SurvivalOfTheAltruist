using UnityEngine;
using System.Collections;
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

        //        [SerializeField]
        //        private int numOfCreaturesPerGroup = 10;
        //        [SerializeField]
        //        [Range(Creature.AltruismMin, Creature.AltruismMax)]
        //        private float altruismMin = Creature.AltruismMin;
        //        [SerializeField]
        //        [Range(Creature.AltruismMin, Creature.AltruismMax)]
        //        private float altruismMax = Creature.AltruismMax;
        //        [SerializeField]
        //        [Range(Creature.EnergyStartMin, Creature.EnergyStartMax)]
        //        private float energyStartMin = Creature.EnergyStartMin;
        //        [SerializeField]
        //        [Range(Creature.EnergyStartMin, Creature.EnergyStartMax)]
        //        private float energyStartMax = Creature.EnergyStartMax;

#endregion

#region Properties

        public MainController MainContoller { get { return mainContoller; } }

#endregion

#region Unity override

#endregion

#region IController implementation

        public void Reset() {
            InitGroups();
            gameObject.SetActive(true);
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

