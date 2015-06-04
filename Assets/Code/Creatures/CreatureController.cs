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

        [Header("Creature logic")]

        [SerializeField]
        [Range(0, 1)]
        private float lowEnergyMark = 0.1f;

#endregion

#region Properties

        public List<Creature> Creatures { get { return creatures; } }

#endregion

#region Unity override

        // Update is called once per frame
        private void FixedUpdate() {
            UpdateCreatures();
        }

#endregion

#region Delegates

        private void OnTriggerEntered(Creature creature, Collider2D other) {
//            //            Debug.LogFormat("OnTriggerEnter: creature = {1}, other = {0}", other, creature);
//
//            Energy energy = other.GetComponent<Energy>();
//            if (energy != null) {
//                Debug.LogFormat("Energy collected = {0}", energy);
//                creature.Energy += energy.energy;
//                mainContoller.RemoveEnvironmentObject(energy);
//            }
        }

        private void OnEnergyDetected(Creature creature, Energy energy) {
//            Debug.LogFormat("OnEnergyDetected: creature = {0}, energy = {1}", creature, energy);
            creature.MoveTo = energy.transform.position;
        }

        private void OnEnergyCollect(Creature creature, Energy energy) {
//            Debug.LogFormat("OnEnergyCollect: creature = {0}, energy = {1}", creature, energy);
            creature.Energy += energy.energy;
            mainContoller.RemoveEnvironmentObject(energy);
        }

        private void OnEnergyLow(Creature creature) {
            //            Debug.LogFormat("OnEnergyLow: creature = {0}", creature);
        }

        private void OnEnergyDepleted(Creature creature) {
//            Debug.LogFormat("OnEnergyDepleted: creature = {0}", creature);
            RemoveCreature(creature);
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
                creature.name += " (" + i + ")";
                creature.InitToRandomValues();
                creature.transform.position = mainContoller.GetRandomWorldPosition(); // start position
                creature.MoveTo = mainContoller.GetRandomWorldPosition(); // random go to position

                creature.OnTriggerEntered = this.OnTriggerEntered;
                creature.OnEnergyDetected = this.OnEnergyDetected;
                creature.OnEnergyCollect = this.OnEnergyCollect;
//                creature.OnEnergyDepleted = this.OnEnergyDepleted;

                creatures.Add(creature);
            }
        }

        public void RemoveAllCreatures() {
            while (creatures.Count > 0) {
                RemoveCreature(creatures[0]);
            }
        }

        public bool RemoveCreature(Creature creature) {
            Object.Destroy(creature.gameObject);
            return creatures.Remove(creature);
        }

        private void UpdateCreatures() {
            for (int i = 0; i < creatures.Count; i++) {
                var creature = creatures[i];

                if (creature.Energy <= 0) {
                    OnEnergyDepleted(creature);
                    i--;
                    continue;
                } else if (creature.Energy <= lowEnergyMark) {
                    OnEnergyLow(creature);
                }

                if (!creature.UpdatePosition()) {
                    creature.MoveTo = mainContoller.GetRandomWorldPosition();
                }
            }
        }
    }
}

