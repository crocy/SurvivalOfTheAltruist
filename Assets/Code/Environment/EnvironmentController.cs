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

        // Use this for initialization
        private void Start() {

        }

        // Update is called once per frame
        private void Update() {

        }

#endregion

        public void Reset() {
            GenerateEnvironment();
        }

        private void GenerateEnvironment() {
            environmentObjects.Clear();

            Energy energy;
            for (int i = 0; i < generateEnergy; i++) {
                energy = UnityEngine.Object.Instantiate(energyPrefab);
                energy.transform.position = mainContoller.GetRandomWorldPosition();

                environmentObjects.Add(energy);
            }
        }

        public bool RemoveEnvironmentObject(EnvironmentObject envObj) {
            return environmentObjects.Remove(envObj);
        }
    }
}

