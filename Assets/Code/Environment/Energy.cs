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

        private void OnTriggerEnter(Collider other) {
            Debug.LogFormat("OnTriggerEnter: other = {0}", other);
        }

#endregion

        //	// Use this for initialization
        //	void Start () {
        //
        //	}
        //
        //	// Update is called once per frame
        //	void Update () {
        //
        //	}
    }
}
