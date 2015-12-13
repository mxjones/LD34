using UnityEngine;
using System.Collections;

public class PickUpSpawner : MonoBehaviour
{
    // TODO: Different types of Pick up. Different effects etc.
    public float SpawnTime = 0.5f;
    public GameObject PickupItem;
    public int MaxPickupItems = 75;

	// Use this for initialization
	void Start () {
	    for (int i = 0; i <= MaxPickupItems; i++)
	    {
	        Instantiate(PickupItem);
	    }

        InvokeRepeating("Spawn", SpawnTime, SpawnTime);
	}

    void Spawn()
    {
        GameObject[] pickupItems = GameObject.FindGameObjectsWithTag("PickUpItem");
        if (pickupItems.Length < MaxPickupItems)
            Instantiate(PickupItem);

        //TODO: Spawn more than 1 Item 
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
