using UnityEngine;
using System.Collections;
using System;

namespace SurvivalOfTheAlturist {

    public class Detector : MonoBehaviour {

#region Class fields

        public Action<Collider2D> onTriggerEnter2D = null;

#endregion

#region Serialized fields

        [SerializeField]
        private CircleCollider2D collider2d = null;

#endregion

#region Properties

        public float ColliderRadius {
            get { return collider2d.radius; }
            set { collider2d.radius = value; }
        }

#endregion

#region Unity override

        private void OnTriggerEnter2D(Collider2D other) {
            if (onTriggerEnter2D != null) {
                onTriggerEnter2D(other);
            }
        }

#endregion
    }
}

