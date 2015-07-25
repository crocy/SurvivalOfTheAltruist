using UnityEngine;

namespace SurvivalOfTheAlturist {

    public class Curves : MonoBehaviour {

#region Class fields

#endregion

#region Serialized fields

        [Header("Creature behavior")]

        /// x axis - altruism
        /// y axis - percent of current energy willing to give/share
        [SerializeField]
        private AnimationCurve altruismToEnergyGive = null;

        /// x axis - altruism
        /// y axis - percent of energy to take
        [SerializeField]
        private AnimationCurve altruismToEnergyTake = null;

#endregion

#region Properties

#endregion

        public float AltruismToEnergyGivePercent(float altruism) {
            return altruismToEnergyGive.Evaluate(altruism);
        }

        public float AltruismToEnergyTakePercent(float altruism) {
            return altruismToEnergyTake.Evaluate(altruism);
        }
    }
}

