using UnityEngine;
using System.Collections;
using System;
using SurvivalOfTheAlturist.Environment;
using System.Text;

namespace SurvivalOfTheAlturist.Creatures {

    public enum CreatureState {
        Foraging,
        CollectingEnergy,
        Dead,
    }

    public class Creature : WorldObject, IReport {

#region Class fields

        public const float EnergyStartMin = 0.3f;
        public const float EnergyStartMax = 1f;
        public const float EnergyMax = 10f;

        public const float SpeedToEnergyDepletionRatio = 0.03f;
        public const float SpeedStartMin = 0.3f;
        public const float SpeedStartMax = 3f;
        public const float SpeedMax = 10f;
        public const float SpeedFactorOnEnergyLevelLow = 0.75f;
        public const float SpeedFactorOnEnergyLevelCritical = 0.5f;

        public const float AltruismMin = 0f;
        public const float AltruismMax = 1f;
        public const float AltruisticVery = 1f;

        private CreatureState state = CreatureState.Foraging;
        private new Rigidbody2D rigidbody2D = null;
        private float collectionRadius;

        private Vector3 moveTo;
        private Action<Creature, Collider2D> onTriggerEntered;
        private Action<Creature, Energy> onEnergyDetected;
        private Action<Creature, Energy> onEnergyCollect;
        private Action<Creature> onEnergyDepleted;

        private float startTime, endTime;
        private float energyCollected;
        private float energyShared;
        private float energyLevelLow;
        private float energyLevelCritical;

        private float speed;
        private Group group = null;

#endregion

#region Serialized fields

        [Header("Links")]

        [SerializeField]
        private SpriteRenderer sprite = null;
        [SerializeField]
        private CircleCollider2D baseCollider = null;
        [SerializeField]
        private CircleCollider2D energyDetectorCollider = null;

        [Header("Base attributes")]

        [SerializeField]
        [Range(0, EnergyMax)]
        private float energy = 1f;
        /// Energy depletion rate per second.
        [SerializeField]
        [Range(0, EnergyMax)]
        private float energyDepletionRate = 0.03f;
        [SerializeField]
        private float energyDetectionRadius = 3f;
        [SerializeField]
        [Range(0, SpeedMax)]
        private float speedBase = 1f;
        //        [SerializeField]
        //        private float communicationRadius = 10f;

        [Header("Properties")]

        [SerializeField]
        [Range(0, AltruismMax)]
        private float altruism = 1f;

#endregion

#region Properties

        public CreatureState State { 
            get { return state; }
            set { state = value; }
        }

        public Vector3 MoveTo {
            get { return moveTo; }
            set { moveTo = value; }
        }

        public Action<Creature, Collider2D> OnTriggerEntered {
            get { return onTriggerEntered; }
            set { onTriggerEntered = value; }
        }

        public Action<Creature, Energy> OnEnergyDetected {
            get { return onEnergyDetected; }
            set { onEnergyDetected = value; }
        }

        public Action<Creature, Energy> OnEnergyCollect {
            get { return onEnergyCollect; }
            set { onEnergyCollect = value; }
        }

        public Action<Creature> OnEnergyDepleted {            
            get { return onEnergyDepleted; }
            set { onEnergyDepleted = value; }
        }

        public float EnergyDetectionRadius {
            get {
                return energyDetectionRadius;
            }
            set {
                energyDetectionRadius = value;
                energyDetectorCollider.radius = energyDetectionRadius;
            }
        }

        public float Energy {
            get { return energy; }
//            set { energy = value; }
        }

        public float Speed { 
            get { return speed; }
            set {
                speed = value;
                energyDepletionRate = SpeedToEnergyDepletionRatio * speed;
            }
        }

        public Group Group { get { return group; } }

        public float Altruism { 
            get { return altruism; }
            set { altruism = value; }
        }

        public float DistanceToMoveTo { get { return Vector3.Distance(transform.position, moveTo); } }

        public float StartTime { get { return startTime; } }

        public float EndTime { get { return endTime; } }

        public float EnergyDepletionRate { get { return energyDepletionRate; } }

        public float EnergyLevelLow {
            get { return energyLevelLow; }
            set { energyLevelLow = value; }
        }

        public float EnergyLevelCritical {
            get { return energyLevelCritical; }
            set { energyLevelCritical = value; }
        }

#endregion

#region Unity override

        // Use this for initialization
        private void Awake() {
            rigidbody2D = GetComponent<Rigidbody2D>();
            collectionRadius = baseCollider.radius;

            EnergyDetectionRadius = energyDetectionRadius;

            startTime = Time.time;
        }

        //        private void FixedUpdate() {
        //            UpdateEnergy();
        //        }

        private void OnTriggerEnter2D(Collider2D other) {
//            Debug.LogFormat("OnTriggerEnter: other = {0}", other);
            float distance = Vector3.Distance(transform.position, other.transform.position);

            Energy energyObj = other.GetComponent<Energy>();
            // if distance is grater than the collection radius, we just detected some energy (with the energyDetectorCollider)
            if (energyObj != null) {
//                Debug.LogFormat("OnTriggerEnter: other = {0}, distance = {1}, collectionRadius = {2}", other, distance, collectionRadius);
                if (distance > collectionRadius * 2) {
                    if (onEnergyDetected != null) {
                        onEnergyDetected(this, energyObj);
                        return;
                    }
                } else {
                    if (onEnergyCollect != null) {
                        onEnergyCollect(this, energyObj);
                        return;
                    }
                }
                
            }

            if (onTriggerEntered != null) {
                onTriggerEntered(this, other);
            }
        }

        private void Update() {
            Vector3 diff = MoveTo - transform.position;
            sprite.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, moveTo);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, energyDetectionRadius);

//            Gizmos.color = Color.yellow;
//            Gizmos.DrawWireSphere(transform.position, communicationRadius);
        }

#endregion

#region IReport implementation

        public string GetReport() {
            StringBuilder builder = new StringBuilder();

            if (endTime > 0) {
                builder.AppendFormat("{0}, lifetime = {1:0.00}", ToString(), (endTime - startTime));
            } else {
                builder.AppendFormat("{0}, lifetime = {1:0.00}", ToString(), (Time.time - startTime));
            }

            if (startTime > 0) {
                builder.AppendFormat(" (start = {0}, end = {1})", startTime, endTime);
            }

            builder.AppendFormat(", energy: collected = {0:0.000}, shared = {1:0.000}", energyCollected, energyShared);

            return builder.ToString();
        }

#endregion

        public override string ToString() {
            return string.Format("[{0}: State = {1}, Energy = {2:0.000}, Altruism = {3:0.000}, SpeedBase = {4:0.00}]", 
                name, state, Energy, Altruism, speedBase);
        }

        public void InitToRandomValues(Group group, float energyLevelLow, float energyLevelCritical) {
            this.group = group;
            this.energyLevelLow = energyLevelLow;
            this.energyLevelCritical = energyLevelCritical;

            altruism = UnityEngine.Random.Range(AltruismMin, AltruismMax);
            energy = UnityEngine.Random.Range(EnergyStartMin, EnergyStartMax);
            speedBase = UnityEngine.Random.Range(SpeedStartMin, SpeedStartMax);
            Speed = speedBase; // to update energy drain value

            sprite.color = group.GroupColor;
        }

        /// <summary>
        /// Moves the creature closer to the MoveTo position.
        /// </summary>
        /// <returns><c>true</c>, if position was updated, <c>false</c> otherwise.</returns>
        public bool UpdatePosition() {
            if (DistanceToMoveTo > CreatureController.MinTresholdDistance) {
                Vector3 diff = MoveTo - transform.position;
//                Vector2 diff = MoveTo - transform.position;
//                Debug.LogFormat("MoveTo = {0}, transform.position = {1}, diff = {2}, normalized = {3}, magnitude = {4}", MoveTo, transform.position, diff, diff.normalized, diff.magnitude);

                if (diff.magnitude > Speed) {
                    rigidbody2D.MovePosition(diff.normalized * Speed + transform.position);
                } else {
                    rigidbody2D.MovePosition(MoveTo);
                }
//                sprite.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - 90f);
                
                return true;
            }

            return false;
        }

        public float UpdateEnergy() {
            if (energy > 0) {
                energy -= energyDepletionRate * Time.fixedDeltaTime;

                if (energy <= 0) {
                    energy = 0;

                    if (onEnergyDepleted != null) {
                        onEnergyDepleted(this);
                    }
                }

                // reduce speed by a fixed factor if enery level low/critical
                if (energy <= energyLevelCritical) {
                    Speed = speedBase * SpeedFactorOnEnergyLevelCritical;
                } else if (energy <= energyLevelLow) {
                    Speed = speedBase * SpeedFactorOnEnergyLevelLow;
                } else {
                    Speed = speedBase;
                }
            }

            return energy;
        }

        public void KillCreature() {
            endTime = Time.time;
            state = CreatureState.Dead;
        }

        public void CollectEnergy(float energy) {
            this.energy += energy;
            energyCollected += energy;
        }

        public void ShareEnergy(float energy) {
            this.energy -= energy;
            energyShared += energy;
        }

    }
}
