using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SurvivalOfTheAlturist.Environment {

    public class EnvironmentController : MonoBehaviour {

#region Class fields

        private readonly List<EnvironmentObject> environmentObjects = new List<EnvironmentObject>();

        private float energyStorageMaxCapacity = 1;
        private float energyStorage = 0;

#endregion

#region Serialized fields

        [SerializeField]
        private MainController mainContoller = null;

        [Header("Prefabs")]

        [SerializeField]
        private Energy energyPrefab = null;

        [Header("Generation logic")]

        [SerializeField]
        private int startEnergy = 10;
        /// How much energy per second is generated.
        [SerializeField]
        private float energyGenerationRate = 0.1f;

#endregion

#region Properties

#endregion

#region Unity override

        private void FixedUpdate() {
            energyStorage += energyGenerationRate * Time.fixedDeltaTime;

            if (energyStorage >= energyStorageMaxCapacity) {
                GenerateEnergy();
                energyStorage = 0;
            }
        }

#endregion

        public void Reset() {
            GenerateEnvironment();
        }

        private void GenerateEnvironment() {
            RemoveAllEnvironmentObjects();

            for (int i = 0; i < startEnergy; i++) {
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
            energy.name = "Energy (" + (SimulationReport.GetCurrentSimulation().numOfAllEnergy++) + ")";
            energy.transform.position = mainContoller.GetRandomWorldPosition();
            SimulationReport.GetCurrentSimulation().sumOfAllEnergy += energy.EnergyAmount;

            environmentObjects.Add(energy);
            return energy;
        }
    }
}

