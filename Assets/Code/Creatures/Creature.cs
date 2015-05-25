using UnityEngine;
using System.Collections;
using System;

namespace SurvivalOfTheAlturist.Creatures {

    public class Creature : WorldObject {

#region Class fields

        public const float EnergyStartMin = 0.3f;
        public const float EnergyStartMax = 1f;
        public const float EnergyMax = 1f;
        public const float SpeedMax = 10f;
        public const float KindnessMax = 1f;

        private Vector3 moveTo;
        private Action<Creature, Collider> onTriggerEntered;

#endregion

#region Serialized fields

        [Header("Base")]

        [SerializeField]
        [Range(0, EnergyMax)]
        private float energy = 1f;

        [SerializeField]
        [Range(0, SpeedMax)]
        private float speed = 1f;

        [Header("Properties")]

        [SerializeField]
        [Range(0, KindnessMax)]
        private float kindness = 1f;

        [Header("UI")]

        [SerializeField]
        private SpriteRenderer sprite = null;

#endregion

#region Properties

        public Vector3 MoveTo {
            get { return moveTo; }
            set { moveTo = value; }
        }

        public Action<Creature, Collider> OnTriggerEntered {
            get { return onTriggerEntered; }
            set { onTriggerEntered = value; }
        }

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
        
        }

        private void FixedUpdate() {
            
        }

        private void OnTriggerEnter(Collider other) {
            Debug.LogFormat("OnTriggerEnter: other = {0}", other);

            if (onTriggerEntered != null) {
                onTriggerEntered(this, other);
            }
        }

        // Update is called once per frame
        private void Update() {
	
        }

#endregion

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
//                Debug.LogFormat("MoveTo = {0}, transform.position = {1}, diff = {2}, normalized = {3}, magnitude = {4}", MoveTo, transform.position, diff, diff.normalized, diff.magnitude);
                if (diff.magnitude > Speed) {
                    transform.Translate(diff.normalized * Speed);
                } else {
                    transform.position = MoveTo;
                }
                
                return true;
            }

            return false;
        }
    }
}
