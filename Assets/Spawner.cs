using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class BoidSettings
{
    public int velocity = 32;
    public int neighborDist = 10;
    public int nearDist = 4;
    public int attractPushDist = 5;



    [Header("These are floats ")]

    public float velMatching = 2.5f;
    public float flockCentering = 1f;
    public float nearAvoid = 2f;
    public float attractPull = 1f;
    public float attractPush = 20f;

    [Header("How quickly the boid turns")]
    public float velocityEasing = 0.03f;
    
}

public class Spawner : MonoBehaviour
{
    static public BoidSettings SETTINGS;
    static public List<Boid> BOIDS;

    [Header("Inscribed: settings for spawning boids")]

    public GameObject boidPrefab;
    public Transform boidAnchor;
    public int numBoids = 10;
    public float spawnRadius = 100f;
    public float spawnDelay = 0.1f;

    public BoidSettings boidSettings;
    // Start is called before the first frame update

    void Awake(){
        Spawner.SETTINGS = boidSettings;
        BOIDS = new List<Boid>();
        InstantiateBoid();
    }
 
    public void InstantiateBoid(){
        GameObject go = Instantiate<GameObject>(boidPrefab);
        go.transform.position = Random.insideUnitSphere * spawnRadius;
        Boid b = go.GetComponent<Boid>();
        b.transform.SetParent(boidAnchor);
        BOIDS.Add(b);
        if(BOIDS.Count < numBoids){
            Invoke("InstantiateBoid", spawnDelay);
        }
    }

    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
