﻿using UnityEngine;
using System.Collections;
using System;
using SurvivalOfTheAlturist.Environment;

namespace SurvivalOfTheAlturist.Creatures {

    public class Creature : WorldObject {

#region Class fields

        public const float EnergyStartMin = 0.3f;
        public const float EnergyStartMax = 1f;
        public const float EnergyMax = 1f;
        public const float SpeedMax = 10f;
        public const float KindnessMax = 1f;

        private new Rigidbody2D rigidbody2D = null;
        private float collectionRadius;

        private Vector3 moveTo;
        private Action<Creature, Collider2D> onTriggerEntered;
        private Action<Creature, Energy> onEnergyDetected;
        private Action<Creature, Energy> onEnergyCollect;
        private Action<Creature> onEnergyDepleted;

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
        private int groupID = 0;
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
        private float speed = 1f;
        [SerializeField]
        private float communicationRadius = 10f;

        [Header("Properties")]

        [SerializeField]
        [Range(0, KindnessMax)]
        private float kindness = 1f;

#endregion

#region Properties

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

        public int GroupID { get { return groupID; } }

        public float Energy {
            get { return energy; }
            set { energy = value; }
        }

        public float Speed { get { return speed; } }

        public float Kindness { get { return kindness; } }

        public float DistanceToMoveTo { get { return Vector3.Distance(transform.position, moveTo); } }

#endregion

#region Unity override

        // Use this for initialization
        private void Awake() {
            rigidbody2D = GetComponent<Rigidbody2D>();
            collectionRadius = baseCollider.radius;

            EnergyDetectionRadius = energyDetectionRadius;
        }

        private void FixedUpdate() {
            if (energy > 0) {
                energy -= energyDepletionRate * Time.fixedDeltaTime;

                if (energy <= 0) {
                    energy = 0;

                    if (onEnergyDepleted != null) {
                        onEnergyDepleted(this);
                    }
                }
            }
        }

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
            sprite.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - 90f);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, moveTo);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, energyDetectionRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, communicationRadius);
        }

#endregion

        public override string ToString() {
            return string.Format("[{0}: Energy={1}, Speed={2}, Kindness={3}]", 
                name, Energy, Speed, Kindness);
        }

        public void InitToRandomValues() {
            energy = UnityEngine.Random.Range(EnergyStartMin, EnergyStartMax);
            // TODO: finish
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
    }
}
