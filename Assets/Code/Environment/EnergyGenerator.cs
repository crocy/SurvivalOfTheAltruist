using System;
using UnityEngine;

namespace SurvivalOfTheAlturist.Environment {

    public class EnergyGenerator : ScriptableObject {

#region Class enum

        public enum EnergyGeneratorType {
            Absolute,
            NetFlow,
        }

#endregion

#region Class fields

#endregion

#region Serialized fields

        [SerializeField]
        private EnergyGeneratorType type = default(EnergyGeneratorType);

        [SerializeField]
        private int startEnergy = 10;

        [SerializeField]
        private AnimationCurve generationCurve = null;
        /// <summary>
        /// If this curve is not set, it's default value (of 0) is ignored.
        /// </summary>
        [SerializeField]
        private AnimationCurve generatorEnabled = null;

        [SerializeField]
        private bool disableGeneratorWhenNetFlowNegative = false;

#endregion

#region Properties

        public EnergyGeneratorType Type { get { return type; } }

        public int StartEnergy { get { return startEnergy; } }

#endregion

        public float GetEnergyGenerationRate(float time) {
            return generationCurve.Evaluate(time);
        }

        public bool GetEnergyGeneratorEnabled(float time) {
            if (disableGeneratorWhenNetFlowNegative && type == EnergyGeneratorType.NetFlow) {
                return GetEnergyGenerationRate(time) >= 0;
            }

            if (generatorEnabled != null && generatorEnabled.length > 0) {
                return generatorEnabled.Evaluate(time) >= 1;
            }

            // generator is always on by default
            return true;
        }
    }
}

