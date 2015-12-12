using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{

    public float SpawnTime = 2.0f;
    public float MaxEnemies = 10;

    public GameObject Enemy;

	// Use this for initialization
	void Start () {
	    for (int i = 0; i <= MaxEnemies; i++)
	    {
	        Instantiate(Enemy);
	    }

        InvokeRepeating("Spawn", SpawnTime, SpawnTime);
	}

    void Spawn()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length < MaxEnemies)
            Instantiate(Enemy);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
