using UnityEngine;
using System.Collections;

public class PickUpBehaviour : MonoBehaviour
{
    private Vector2 SpawnLocation;
	// Use this for initialization
	public virtual void Start ()
	{
	    SpawnLocation.x = Random.Range(-7, 18);
	    SpawnLocation.y = Random.Range(-5, 21);
	    transform.position = SpawnLocation;

	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
