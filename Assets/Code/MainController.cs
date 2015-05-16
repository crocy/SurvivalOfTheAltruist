using UnityEngine;
using System.Collections;
using SurvivalOfTheAlturist.Creatures;

namespace SurvivalOfTheAlturist.Main {

    public class MainController : MonoBehaviour {

#region Class fields

        private Creature[] creatures;

#endregion

#region Serialized fields

        [SerializeField]
        private Bounds worldBounds;

#endregion

        // Use this for initialization
        private void Awake() {
		
        }
	
        // Update is called once per frame
        private void Update() {
		
        }

#region Unity override

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube(worldBounds.center, worldBounds.extents);
        }

#endregion
    }

}