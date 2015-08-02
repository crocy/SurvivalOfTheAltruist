using System;
using UnityEngine;
using System.Text;

namespace SurvivalOfTheAlturist.Environment {

    public class EnergyGenerator : ScriptableObject, IReport {

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
        private string tag;

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

        public string Tag { get { return string.Format("EG[{0}]", tag); } }

        public EnergyGeneratorType Type { get { return type; } }

        public int StartEnergy { get { return startEnergy; } }

#endregion

#region IReport implementation

        public string GetReport() {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("[Energy generator: '{0}', type = {1}, startEnergy = {2}, disableGeneratorWhenNetFlowNegative = {3}]", 
                name, type, startEnergy, disableGeneratorWhenNetFlowNegative);
//            builder.AppendFormat("\n  Generator curve: {0}", generationCurve);

            return builder.ToString();
        }

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

