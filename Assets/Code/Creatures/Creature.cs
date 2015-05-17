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

#endregion

#region Unity override

        // Use this for initialization
        private void Awake() {
        
        }

        private void FixedUpdate() {
            
        }

        private void OnTriggerEnter(Collider other) {
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
    }
}
