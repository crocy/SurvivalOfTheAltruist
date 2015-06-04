using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SurvivalOfTheAlturist.Environment {

    public class EnvironmentController : MonoBehaviour {

#region Class fields

        private readonly List<EnvironmentObject> environmentObjects = new List<EnvironmentObject>();

#endregion

#region Serialized fields

        [SerializeField]
        private MainController mainContoller = null;

        [Header("Prefabs")]

        [SerializeField]
        private Energy energyPrefab = null;

        [Header("Generation logic")]

        [SerializeField]
        private int generateEnergy = 10;

#endregion

#region Properties

#endregion

#region Unity override

#endregion

        public void Reset() {
            GenerateEnvironment();
        }

        private void GenerateEnvironment() {
            RemoveAllEnvironmentObjects();

            Energy energy;
            for (int i = 0; i < generateEnergy; i++) {
                energy = UnityEngine.Object.Instantiate(energyPrefab);
                energy.name += " (" + i + ")";
                energy.transform.position = mainContoller.GetRandomWorldPosition();

                environmentObjects.Add(energy);
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
    }
}

