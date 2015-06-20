using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SurvivalOfTheAlturist.Creatures {

    public class GroupController : MonoBehaviour, IController {

#region Class fields

        private readonly List<Group> groups = new List<Group>();

#endregion

#region Serialized fields

        [SerializeField]
        private MainController mainContoller = null;

        [Header("Properties")]
        
        [SerializeField]
        private int numOfGroups = 1;
        [SerializeField]
        private int numOfCreaturesPerGroup = 10;
        [SerializeField]
        private float altruismMin = Creature.AltruismMin;
        [SerializeField]
        private float altruismMax = Creature.AltruismMax;
        [SerializeField]
        private float energyStartMin = Creature.EnergyStartMin;
        [SerializeField]
        private float energyStartMax = Creature.EnergyStartMax;

#endregion

#region Properties

#endregion

#region Unity override

#endregion

#region IController implementation

        public void Reset() {
            groups.Clear();

            GenerateGroups();
        }

#endregion

        private void GenerateGroups() {
            Group group;

            for (int i = 0; i < numOfGroups; i++) {
                group = new Group();
                group.AltruismMin = altruismMin;
                group.AltruismMax = altruismMax;
                group.EnergyStartMin = energyStartMin;
                group.EnergyStartMax = energyStartMax;
                group.InitGroup(numOfCreaturesPerGroup, mainContoller.CreatureController);

                groups.Add(group);
            }
        }

    }
}

