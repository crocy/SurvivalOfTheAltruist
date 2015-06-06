using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SurvivalOfTheAlturist.Environment;
using System;

namespace SurvivalOfTheAlturist.Creatures {

    public class CreatureController : MonoBehaviour {

#region Class fields

        public const float MinTresholdDistance = 0.01f;

        private readonly List<Creature> creatures = new List<Creature>();
        private readonly Dictionary<int, List<Creature>> groupIdToCreatures = new Dictionary<int, List<Creature>>();

#endregion

#region Serialized fields

        [SerializeField]
        private MainController mainContoller = null;
        [SerializeField]
        private Curves curves = null;

        [Header("Prefabs")]

        [SerializeField]
        private Creature creaturePrefab = null;

        [Header("Generation logic")]

        [SerializeField]
        private int generateCreatures = 10;

        [Header("Creature logic")]

        [SerializeField]
        [Range(0, 1)]
        private float energyLevelLow = 0.3f;
        [SerializeField]
        [Range(0, 1)]
        private float energyLevelCritical = 0.1f;

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

        private void OnEnergyLevelLow(Creature creatureInNeed) {
//            Debug.LogFormat("OnEnergyLevelLow: creature = {0}", creatureInNeed);
            NeedEnergy(creatureInNeed);
        }

        private void OnEnergyLevelCritical(Creature creatureInNeed) {
//            Debug.LogFormat("OnEnergyLevelCritical: creature = {0}", creatureInNeed);
            NeedEnergy(creatureInNeed);
        }

        private void OnEnergyDepleted(Creature creature) {
//            Debug.LogFormat("OnEnergyDepleted: creature = {0}", creature);
            RemoveCreature(creature);
        }

#endregion

#region Generate/update/remove logic

        public void Reset() {
            GenerateCreatures();
        }

        private void GenerateCreatures() {
            RemoveAllCreatures();

            Creature creature;
            List<Creature> list;

            for (int i = 0; i < generateCreatures; i++) {
                creature = UnityEngine.Object.Instantiate(creaturePrefab);
                creature.name += " (" + i + ")";
                creature.InitToRandomValues();
                creature.transform.position = mainContoller.GetRandomWorldPosition(); // start position
                creature.MoveTo = mainContoller.GetRandomWorldPosition(); // random go to position

                creature.OnTriggerEntered = this.OnTriggerEntered;
                creature.OnEnergyDetected = this.OnEnergyDetected;
                creature.OnEnergyCollect = this.OnEnergyCollect;
//                creature.OnEnergyDepleted = this.OnEnergyDepleted; // not using atm

                creatures.Add(creature);

                if (!groupIdToCreatures.TryGetValue(creature.GroupID, out list)) {
                    list = new List<Creature>();
                    groupIdToCreatures.Add(creature.GroupID, list);
                }
                list.Add(creature);
            }
        }

        public void RemoveAllCreatures() {
            while (creatures.Count > 0) {
                RemoveCreature(creatures[0]);
            }
        }

        public bool RemoveCreature(Creature creature) {
            Debug.LogWarningFormat("Removing creature: {0}", creature);
            UnityEngine.Object.Destroy(creature.gameObject);
            return creatures.Remove(creature);
        }

        private void UpdateCreatures() {
            float prevEnergy;
            float currentEnergy;

            for (int i = 0; i < creatures.Count; i++) {
                var creature = creatures[i];
                prevEnergy = creature.Energy;
                currentEnergy = creature.UpdateEnergy();

                if (creature.Energy <= 0) {
                    OnEnergyDepleted(creature);
                    i--;
                    continue;
                } else if (currentEnergy <= energyLevelCritical && prevEnergy > energyLevelCritical) {
                    // trigger only once, once the threshold has been reached
                    OnEnergyLevelCritical(creature);
                } else if (creature.Energy <= energyLevelLow && prevEnergy > energyLevelLow) {
                    // trigger only once, once the threshold has been reached
                    OnEnergyLevelLow(creature);
                }

                if (!creature.UpdatePosition()) {
                    creature.MoveTo = mainContoller.GetRandomWorldPosition();
                }
            }
        }

#endregion

#region Behavior logic

        /// <summary>
        /// Would the "creatureToGive" share any of it's energy with the "creatureInNeed"?
        /// </summary>
        /// <returns>The amount of energy "creatureToGive" is prepared to share. A return value
        /// grater than zero means that amount of enery can be shared. Zero or less means no
        /// sharing.</returns>
        /// <param name="creatureInNeed">Creature in need.</param>
        /// <param name="creatureToGive">Creature to give.</param>
        private float WouldShareEnergy(Creature creatureInNeed, Creature creatureToGive) {
            // check if creatures from different groups share energy
            // TODO: should expand this
            if (creatureToGive.GroupID != creatureInNeed.GroupID) {
                return -1;
            }

            // don't share enery if below or at critical (this may change later)
            if (creatureToGive.Energy <= energyLevelCritical) {
                return -1;
            }

            // TODO: add a different curves for when creature above and below "low energy level"?

            // never give so much energy that would push the creature below energy level critical
            float energyAvailable = creatureToGive.Energy - energyLevelCritical;
            return energyAvailable * curves.AltruismToEnergyGivePercent(creatureToGive.Altruism);
        }

        private float EnergyNeededAmount(Creature creatureInNeed, float energyOffered) {
            float energyNeeded = 0;

            // TODO: add a different curves for when creature above and below "low energy level"?

            // take as much energy as needed to get you out of critical level
            if (creatureInNeed.Energy < energyLevelCritical) {
                energyNeeded += energyLevelCritical - creatureInNeed.Energy;
            }

            if (energyNeeded >= energyOffered) {
                return energyOffered;
            }

            return (energyOffered - energyNeeded) * curves.AltruismToEnergyTakePercent(creatureInNeed.Altruism);
        }

        private void NeedEnergy(Creature creatureInNeed) {
            List<Creature> list;
            if (!groupIdToCreatures.TryGetValue(creatureInNeed.GroupID, out list)) {
                throw new Exception("No group found with ID = " + creatureInNeed.GroupID);
            }

            float energyOffered;
            float bestEnergyOffered = 0;
            Creature bestMatch = null;

            foreach (var creature in list) {
                if (creature == creatureInNeed) {
                    continue;
                }

                // check how much energy can the creatureInNeed get from creature
                energyOffered = WouldShareEnergy(creatureInNeed, creature);

                if (energyOffered > bestEnergyOffered) {
                    bestEnergyOffered = energyOffered;
                    bestMatch = creature;
                }
            }

            if (bestMatch != null) {
                // check how much of the offered energy would creatureInNeed take
                float energyNeeded = EnergyNeededAmount(creatureInNeed, bestEnergyOffered);

                // exchange energy
                // TODO: don't have this as an instant transfer?
                Debug.LogFormat("Energy to be shared: creatureInNeed = {0}, creatureToGive = {1}", creatureInNeed, bestMatch);
                creatureInNeed.Energy += energyNeeded;
                bestMatch.Energy -= energyNeeded;
                Debug.LogFormat("Energy shared: energy exchanged = {0}, creatureInNeed = {1}, creatureToGive = {2}", energyNeeded, creatureInNeed, bestMatch);

            } else {
                Debug.LogFormat("No creature would to share energy with: {0}", creatureInNeed);
            }
        }

#endregion

    }
}

