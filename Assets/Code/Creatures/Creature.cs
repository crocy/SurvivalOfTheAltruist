using UnityEngine;
using System.Collections;

public class Creature : MonoBehaviour {

    #region Class fields

    #endregion

    #region Serialized fields

	[Header("Base")]

    [SerializeField]
    [Range(0, 1)]
    private float energy = 1f;

	[SerializeField]
    [Range(0, 10)]
    private float speed = 1f;

	[Header("Properties")]

	[SerializeField]
	[Range(0, 1)]
	private float kindness = 1f;

    [Header("UI")]

    [SerializeField]
    private SpriteRenderer sprite = null;

    #endregion

    #region Properties
    #endregion

    // Use this for initialization
    private void Awake() {
        
    }
	
    // Update is called once per frame
    private void Update() {
	
    }
}
