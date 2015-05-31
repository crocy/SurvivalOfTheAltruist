using UnityEngine;
using System.Collections;

namespace SurvivalOfTheAlturist.Environment {

    public class Energy : EnvironmentObject {

#region Class fields

#endregion

#region Serialized fields

        [SerializeField]
        [Range(0, 1)]
        public float energy = 1f;

#endregion

#region Properties

#endregion

#region Unity override

        //        private void OnTriggerEnter2D(Collider2D other) {
        //            Debug.LogFormat("OnTriggerEnter: other = {0}", other);
        //        }
        //
        //        private void OnCollisionEnter2D(Collision2D collision) {
        //            Debug.LogFormat("OnCollisionEnter: collision = {0}", collision);
        //        }

#endregion

        public override string ToString() {
            return string.Format("{0}: energy = {1}", name, energy);
        }
    }
}
