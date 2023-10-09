using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class Boid : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody rigid;
    private Neighborhood neighborhood;
    
    public int numRays = 5;
    public int range = 10;
    public float turn = 25f;
    public int angle = 25;

    void Awake(){
        neighborhood = GetComponent<Neighborhood>();
        rigid = GetComponent<Rigidbody>();
        vel = Random.onUnitSphere * Spawner.SETTINGS.velocity;

        LookAhead();
        Colorize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate(){
        BoidSettings bSet = Spawner.SETTINGS;
        Vector3 sumVal = Vector3.zero;

        Vector3 delta = Attractor.POS - pos;

        if(delta.magnitude > bSet.attractPull){
            sumVal += delta.normalized * bSet.attractPull;
        } else {
            sumVal -= delta.normalized * bSet.attractPush;
        }

        //Collision avoidance
        Vector3 velAvoid = Vector3.zero;
        Vector3 tooNearPos = neighborhood.avgNearPos;
        if(tooNearPos != Vector3.zero)
        {
            velAvoid = pos - tooNearPos;
            velAvoid.Normalize();
            sumVal += velAvoid * bSet.nearAvoid;
        }

        Vector3 forwardPos = Vector3.forward;
        Vector3 leftPos = Quaternion.AngleAxis(-angle, transform.up) * transform.forward;
        Vector3 rightPos = Quaternion.AngleAxis(angle, transform.up) * transform.forward;

        Ray forwardRay = new Ray(transform.position, transform.forward * range);
        Ray leftRay = new Ray(transform.position, leftPos * range);
        Ray rightRay = new Ray(transform.position, rightPos * range);
        
        Debug.DrawRay(transform.position, transform.forward * range);
        Debug.DrawRay(transform.position, leftPos* range);
        Debug.DrawRay(transform.position, rightPos* range);

        RaycastHit hit;
        if(Physics.Raycast(forwardRay, out hit, range)){
            if(hit.collider.tag == "Avoid"){ 
                velAvoid = pos - tooNearPos;
                velAvoid.Normalize();
                sumVal += velAvoid * (turn * 2);
            }
        } else if(Physics.Raycast(leftRay, out hit, range)){
            if(hit.collider.tag == "Avoid"){ 
                velAvoid = pos - tooNearPos;
                velAvoid.Normalize();
                sumVal += velAvoid * turn;
            }
        } else if(Physics.Raycast(rightRay, out hit, range)){
            if(hit.collider.tag == "Avoid"){ 
                velAvoid = pos + tooNearPos;
                velAvoid.Normalize();
                sumVal += velAvoid * turn;
            }
        } 

        // Velocity Matching
        Vector3 velAlign = neighborhood.avgVel;
        if(velAlign != Vector3.zero)
        {
            velAlign.Normalize();
            sumVal += velAlign * bSet.velMatching;
            
        }

        //Flock Centering
        Vector3 velCenter = neighborhood.avgPos;
        if(velCenter != Vector3.zero)
        {
            velCenter -= transform.position;
            velCenter.Normalize();
            sumVal += velCenter*bSet.flockCentering;
        }
        


        sumVal.Normalize();

        vel = Vector3.Lerp(vel.normalized, sumVal, bSet.velocityEasing);
        vel *= bSet.velocity;

        LookAhead();
    }

    public Vector3 vel{
        get { return rigid.velocity; }
        private set { rigid.velocity = value;}
    }

    public Vector3 pos{
        get { return transform.position; }
        private set { transform.position = value;}
    }


    void LookAhead(){
        transform.LookAt(pos + rigid.velocity); 
    }

    void Colorize(){
        Color randColor = Random.ColorHSV(0,1,0.5f,1,0.5f,1);
        Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer r in rends){
            r.material.color = randColor;
        }

        TrailRenderer trend = GetComponent<TrailRenderer>();
        trend.startColor = randColor;
        randColor.a = 0;
        trend.endColor = randColor;
        trend.endWidth = 0;
    }
}
