using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SurvivalOfTheAlturist.Environment;

namespace SurvivalOfTheAlturist.Creatures {

    public class CreatureController : MonoBehaviour {

#region Class fields

        private readonly List<Creature> creatures = new List<Creature>();

#endregion

#region Serialized fields

        [SerializeField]
        private MainController mainContoller = null;

        [Header("Prefabs")]

        [SerializeField]
        private Creature creaturePrefab = null;

        [Header("Generation logic")]

        [SerializeField]
        private int generateCreatures = 10;

#endregion

#region Properties

        public List<Creature> Creatures { get { return creatures; } }

#endregion

#region Unity override

        // Use this for initialization
        private void Start() {
    
        }
    
        // Update is called once per frame
        private void FixedUpdate() {
    
        }

#endregion

#region Delegates

        private void OnTriggerEntered(Creature creature, Collider other) {
            Energy energy = other.gameObject.GetComponent<Energy>();
            if (energy != null) {
                creature.Energy += energy.energy;
                mainContoller.RemoveEnvironmentObject(energy);
            }
        }

#endregion

        public void Reset() {
            GenerateCreatures();
        }

        private void GenerateCreatures() {
            creatures.Clear();

            Creature creature;
            for (int i = 0; i < generateCreatures; i++) {
                creature = UnityEngine.Object.Instantiate(creaturePrefab);
                creature.transform.position = mainContoller.GetRandomWorldPosition();
                creature.OnTriggerEntered = this.OnTriggerEntered;

                creatures.Add(creature);
            }
        }

        public bool RemoveCreature(Creature creature) {
            return creatures.Remove(creature);
        }
    }
}

