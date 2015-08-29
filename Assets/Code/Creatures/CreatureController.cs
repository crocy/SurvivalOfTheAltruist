using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SurvivalOfTheAlturist.Environment;
using System;

namespace SurvivalOfTheAlturist.Creatures {

    public class CreatureController : MonoBehaviour, IController, IReport {

#region Class fields

        public const float MinTresholdDistance = 0.01f;

        private readonly List<Creature> creatures = new List<Creature>();
        private readonly Dictionary<Group, List<Creature>> groupToCreatures = new Dictionary<Group, List<Creature>>();

        private Action onAllCreaturesDied;

#endregion

#region Serialized fields

        [Header("Links")]

        [SerializeField]
        private MainController mainContoller = null;
        [SerializeField]
        private Curves curves = null;

        [Header("Prefabs")]

        [SerializeField]
        private Creature creaturePrefab = null;

        [Header("Creature logic")]

        [SerializeField]
        [Range(0, 20)]
        private float energyStorageCapacity = 2f;
        [SerializeField]
        [Range(0, 1)]
        private float energyLevelLow = 0.3f;
        [SerializeField]
        [Range(0, 1)]
        private float energyLevelCritical = 0.1f;
        [SerializeField]
        private bool takeAllOfferedEnergy = false;
        [SerializeField]
        private bool shareEneryToLevelCritical = false;
        [SerializeField]
        [Range(0, 1)]
        private float shareExcessEnergyAltruismThreshold = 0.5f;
        [SerializeField]
        [Range(0, 1)]
        private float dontShareLassThanXEnergy = 0.01f;

#endregion

#region Properties

        public List<Creature> Creatures { get { return creatures; } }

        public Action OnAllCreaturesDied {
            get { return onAllCreaturesDied; }
            set { onAllCreaturesDied = value; }
        }

        public float EnergyDepletionRateSum {
            get {
                float depletionSum = 0;
                foreach (var creature in Creatures) {
                    depletionSum += creature.EnergyDepletionRate;
                }
                return depletionSum;
            }
        }

        public bool TakeAllOfferedEnergy { get { return takeAllOfferedEnergy; } }

        public float EnergyStorageCapacity { get { return energyStorageCapacity; } }

        public float DontShareLassThanXEnergy { get { return dontShareLassThanXEnergy; } }

#endregion

#region Unity override

        // Update is called once per frame
        private void FixedUpdate() {
            if (!SimulationReport.IsSimulationRunning) {
                return;
            }

            UpdateCreatures();
        }

#endregion

#region IController implementation

        public void Reset() {
            //            GenerateCreatures();
            RemoveAllCreatures();
            gameObject.SetActive(true);
        }

#endregion

#region IReport implementation

        public string GetReport() {
            return string.Format("Creature controller:" +
            "\n - Creatures = {0}," +
            "\n - Energy: storageCapacity = {1}, takeAllOffered = {2}, shareToLevelCritical = {3}, shareExcessEnergyAltruismThreshold = {4}, dontShareLassThanXEnergy = {5}", 
                Creatures.Count,
                energyStorageCapacity, takeAllOfferedEnergy, shareEneryToLevelCritical, shareExcessEnergyAltruismThreshold, dontShareLassThanXEnergy);
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
            if (creature.State == CreatureState.CollectingEnergy) {
                // already collecting energy, don't get distracted ;)
                return;
            }
            creature.State = CreatureState.CollectingEnergy;

            creature.MoveTo = energy.transform.position;
        }

        private void OnEnergyCollect(Creature creature, Energy energy) {
//            Debug.LogFormat("OnEnergyCollect: creature = {0}, energy = {1}", creature, energy);
            creature.CollectEnergy(energy.EnergyAmount);
            mainContoller.RemoveEnvironmentObject(energy);
            creature.State = CreatureState.Foraging;
            ShareExcessEnergyMaybe(creature);
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
            creature.KillCreature();
            RemoveCreature(creature);
        }

#endregion

#region Generate/update/remove logic

        public Creature GenerateCreature(Group group) {
            Creature creature;
            List<Creature> list;

            creature = UnityEngine.Object.Instantiate(creaturePrefab);
            creature.InitToRandomValues(group, energyLevelLow, energyLevelCritical, energyStorageCapacity);
            creature.transform.position = mainContoller.GetRandomWorldPosition(); // start position
            creature.MoveTo = mainContoller.GetRandomWorldPosition(); // random go to position

            creature.OnTriggerEntered = this.OnTriggerEntered;
            creature.OnEnergyDetected = this.OnEnergyDetected;
            creature.OnEnergyCollect = this.OnEnergyCollect;
            //                creature.OnEnergyDepleted = this.OnEnergyDepleted; // not using atm

            creatures.Add(creature);

            if (!groupToCreatures.TryGetValue(creature.Group, out list)) {
                list = new List<Creature>();
                groupToCreatures.Add(creature.Group, list);
            }
            list.Add(creature);

            //                SimulationReport.GetCurrentSimulation().creatures.Add(creature);
            return creature;
        }

        public void RemoveAllCreatures() {
            while (creatures.Count > 0) {
                RemoveCreature(creatures[0]);
            }
        }

        public bool RemoveCreature(Creature creature) {
//            Debug.LogFormat("Removing creature: {0}", creature);
            creature.transform.parent = transform;
            creature.gameObject.SetActive(false);
            return creatures.Remove(creature);
        }

        private void UpdateCreatures() {
            if (creatures.Count == 0) {
                if (onAllCreaturesDied != null) {
                    onAllCreaturesDied();
                }
                gameObject.SetActive(false);
                return;
            }

            float prevEnergy;
            float currentEnergy;

            for (int i = 0; i < creatures.Count; i++) {
                var creature = creatures[i];
                prevEnergy = creature.Energy;
                currentEnergy = creature.UpdateEnergy();

                if (currentEnergy <= 0) {
                    OnEnergyDepleted(creature);
                    i--;
                    continue;
                } else if (currentEnergy <= energyLevelCritical && prevEnergy > energyLevelCritical) {
                    // trigger only once, once the threshold has been reached
                    OnEnergyLevelCritical(creature);
                } else if (currentEnergy <= energyLevelLow && prevEnergy > energyLevelLow) {
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
            // check if creatures from different groups share energy (not for now)
            // TODO: should expand this
            if (creatureToGive.Group != creatureInNeed.Group) {
                return -1;
            }

            // by default, don't share enery if at or below low level (this may change later)
            float shareToEnergyLevel = energyLevelLow;
            if (shareEneryToLevelCritical) {
                // don't share enery if at or below critical level (this may change later)
                shareToEnergyLevel = energyLevelCritical;
            }

            // TODO: add a different curves for when creature above and below "low energy level"?

            // never give so much energy that would push the creature below shareToEnergyLevel
            float energyAvailable = (creatureToGive.Energy - shareToEnergyLevel) * curves.AltruismToEnergyGivePercent(creatureToGive.Altruism);

            // don't share energy if all of it would be lost in transaction penalties
            if (energyAvailable <= dontShareLassThanXEnergy + creatureToGive.EnergySharingLoss(creatureInNeed)) {
                return 0;
            }

            return energyAvailable;
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
            if (!groupToCreatures.TryGetValue(creatureInNeed.Group, out list)) {
                throw new Exception("No group found with ID = " + creatureInNeed.Group);
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
                float energyNeeded = bestEnergyOffered;

                if (!takeAllOfferedEnergy) {
                    energyNeeded = EnergyNeededAmount(creatureInNeed, bestEnergyOffered);
                }
//                Debug.LogFormat("Energy to be shared: {0}, energyLoss = {1}", energyNeeded, energyLoss);

                // exchange energy
//                Debug.LogFormat("Energy to be shared: creatureInNeed = {0}, creatureToGive = {1}", creatureInNeed, bestMatch);
                bestMatch.ShareEnergy(energyNeeded, creatureInNeed);
//                Debug.LogFormat("Energy shared: energy exchanged = {0}, creatureInNeed = {1}, creatureToGive = {2}", energyNeeded, creatureInNeed, bestMatch);
            } else {
//                Debug.LogFormat("No creature would to share energy with: {0}", creatureInNeed);
            }
        }

        /// <summary>
        /// Shares the excess energy maybe.
        /// </summary>
        /// <returns><c>true</c>, if excess energy was shared, <c>false</c> otherwise.</returns>
        /// <param name="creatureToGive">Creature to give.</param>
        public bool ShareExcessEnergyMaybe(Creature creatureToGive) {
            if (creatureToGive.Altruism < shareExcessEnergyAltruismThreshold) {
                return false;
            }

            float excessEnergy = creatureToGive.Energy - creatureToGive.EnergyStorageCapacity;
            if (excessEnergy <= 0) {
                return false;
            }

            float energyMin = creatureToGive.Energy;
            int energyMinIndex = -1; // index of a creature with lowest energy
            Group ownGroup = creatureToGive.Group;
            for (int i = 0; i < ownGroup.CreaturesCount; i++) {
                if (ownGroup[i].Energy < energyMin) {
                    energyMin = ownGroup[i].Energy;
                    energyMinIndex = i;
                }
            }

            if (energyMinIndex > 0) {
                creatureToGive.ShareEnergy(excessEnergy, ownGroup[energyMinIndex]);
                return true;
            }

            return false;
        }

#endregion

    }
}

