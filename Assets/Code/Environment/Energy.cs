using UnityEngine;
using System.Collections;

namespace SurvivalOfTheAlturist.Environment {

    public class Energy : EnvironmentObject {

#region Class fields

#endregion

#region Serialized fields

        [SerializeField]
        [Range(0, 1)]
        private float energy = 1f;

#endregion

#region Properties

        public float EnergyAmount {
            get { return energy; }
            set { energy = value; }
        }

#endregion

#region Unity override

#endregion

        public override string ToString() {
            return string.Format("{0}: energy = {1}", name, energy);
        }
    }
}
