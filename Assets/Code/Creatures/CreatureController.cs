using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SurvivalOfTheAlturist.Environment;

namespace SurvivalOfTheAlturist.Creatures {

    public class CreatureController : MonoBehaviour {

#region Class fields

        public const float MinTresholdDistance = 0.01f;

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

        // Update is called once per frame
        private void FixedUpdate() {
            UpdateCreaturePositions();
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
            RemoveAllCreatures();

            Creature creature;
            for (int i = 0; i < generateCreatures; i++) {
                creature = UnityEngine.Object.Instantiate(creaturePrefab);
                creature.InitToRandomValues();
                creature.transform.position = mainContoller.GetRandomWorldPosition();
                creature.MoveTo = mainContoller.GetRandomWorldPosition();
                creature.OnTriggerEntered = this.OnTriggerEntered;

                creatures.Add(creature);
            }
        }

        public void RemoveAllCreatures() {
            while (creatures.Count > 0) {
                RemoveCreature(creatures[0]);
            }
        }

        public bool RemoveCreature(Creature creature) {
            Object.Destroy(creature);
            return creatures.Remove(creature);
        }

        private void UpdateCreaturePositions() {
            foreach (var item in creatures) {
                if (!item.UpdatePosition()) {
                    item.MoveTo = mainContoller.GetRandomWorldPosition();
                }
            }
        }
    }
}

