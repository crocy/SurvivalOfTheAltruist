using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SurvivalOfTheAlturist.Environment {

    public class EnvironmentController : MonoBehaviour, IController, IReport {

#region Class fields

        private readonly List<EnvironmentObject> environmentObjects = new List<EnvironmentObject>();

        private float energyStorageMaxCapacity = 1;
        private float energyStorage = 0;

        private int numOfAllGeneratedEnergy = 0;
        private float sumOfAllGeneratedEnergy = 0;

#endregion

#region Serialized fields

        [SerializeField]
        private MainController mainContoller = null;
        /// How much energy per second is generated.
        [SerializeField]
        private EnergyGenerator energyGenerator = null;

        [Header("Generation logic")]

        [SerializeField]
        private Energy energyPrefab = null;

#endregion

#region Properties

        public float CurrentEnergyGenerationRate { 
            get {
                float time = SimulationReport.GetCurrentSimulation().CurrentTime;
                if (!energyGenerator.GetEnergyGeneratorEnabled(time)) {
                    return 0;
                }

                float rate = energyGenerator.GetEnergyGenerationRate(time);

                switch (energyGenerator.Type) {
                    case EnergyGenerator.EnergyGeneratorType.NetFlow:
                        // add (not subtract) the two values because EnergyDepletionRateSum is a positive value (but generation rate wise should be negative)
                        return rate + mainContoller.CreatureController.EnergyDepletionRateSum;
                        
                    default:
                        return rate;
                }
            }
        }

        public int NumOfAllGeneratedEnergy { get { return numOfAllGeneratedEnergy; } }

        public float SumOfAllGeneratedEnergy { get { return sumOfAllGeneratedEnergy; } }

#endregion

#region Unity override

        private void FixedUpdate() {
            if (!SimulationReport.IsSimulationRunning) {
                return;
            }

            energyStorage += CurrentEnergyGenerationRate * Time.fixedDeltaTime;

            if (energyStorage >= energyStorageMaxCapacity) {
                GenerateEnergy();
                energyStorage = 0;
            }
        }

#endregion

#region IController implementation

        public void Reset() {
            GenerateEnvironment();

            gameObject.SetActive(true);

            numOfAllGeneratedEnergy = 0;
            sumOfAllGeneratedEnergy = 0;
        }

#endregion

#region IReport implementation

        public string GetReport() {
            return string.Format("Environment controller: numOfAllGeneratedEnergy = {0}, sumOfAllGeneratedEnergy = {1},\n  generator = {2}", 
                numOfAllGeneratedEnergy, sumOfAllGeneratedEnergy, energyGenerator.GetReport());
        }

#endregion

        private void GenerateEnvironment() {
            RemoveAllEnvironmentObjects();

            for (int i = 0; i < energyGenerator.StartEnergy; i++) {
                GenerateEnergy();
            }
        }

        public void RemoveAllEnvironmentObjects() {
            while (environmentObjects.Count > 0) {
                RemoveEnvironmentObject(environmentObjects[0]);
            }
        }

        public bool RemoveEnvironmentObject(EnvironmentObject envObj) {
            Object.Destroy(envObj.gameObject);
            return environmentObjects.Remove(envObj);
        }

        private Energy GenerateEnergy() {
            Energy energy = UnityEngine.Object.Instantiate(energyPrefab);
//            energy.name = "Energy (" + i + ")";
            energy.name = "Energy (" + (numOfAllGeneratedEnergy++) + ")";
            energy.transform.position = mainContoller.GetRandomWorldPosition();
            sumOfAllGeneratedEnergy += energy.EnergyAmount;

            environmentObjects.Add(energy);
            return energy;
        }
    }
}

