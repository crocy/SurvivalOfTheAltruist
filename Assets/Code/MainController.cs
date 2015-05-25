using UnityEngine;
using System.Collections;
using SurvivalOfTheAlturist.Creatures;
using SurvivalOfTheAlturist.Environment;

namespace SurvivalOfTheAlturist {

    public class MainController : MonoBehaviour {

#region Class fields

#endregion

#region Serialized fields

        [SerializeField]
        private Bounds worldBounds;

        [Header("Controllers")]

        [SerializeField]
        private CreatureController creatureController = null;
        [SerializeField]
        private EnvironmentController environmentController = null;

#endregion

#region Properties

        public Bounds WorldBounds { get { return worldBounds; } }

#endregion

#region Unity override

        // Use this for initialization
        private void Awake() {
            Reset();
        }
    
        // Update is called once per frame
        private void Update() {
        
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube(worldBounds.center, worldBounds.extents * 2);
        }

#endregion

        public void Reset() {
            environmentController.Reset();
            creatureController.Reset();
        }

        public Vector3 GetRandomWorldPosition() {
            float xMin = worldBounds.min.x;
            float xMax = worldBounds.max.x;
            float yMin = worldBounds.min.y;
            float yMax = worldBounds.max.y;

            Vector3 position = new Vector3();
            position.x = Random.Range(xMin, xMax);
            position.y = Random.Range(yMin, yMax);

            return position;
        }

        public void RemoveEnvironmentObject(EnvironmentObject envObj) {
            environmentController.RemoveEnvironmentObject(envObj);
        }
    }

}