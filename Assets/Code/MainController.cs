using UnityEngine;
using System.Collections;

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
}
