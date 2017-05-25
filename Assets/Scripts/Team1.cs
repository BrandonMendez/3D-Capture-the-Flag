using UnityEngine;
using System.Collections;

public class Team1 : MonoBehaviour
{
    //gameobjects in the scene
    public GameObject homeFlag;
    public GameObject enemyFlag;
    public GameObject flag;
    public GameObject flagBase;
    public Team2[] enemies;
    public Team1[] teammates;
    public bool targetFlag = false;
    public bool chaseEnemy = true;
    public bool hasFlag = false;
    public bool frozen = false;
   
    //for travel distances
    Vector3 direction;
    float nearSpeed = 9;
    float nearRadius = 20f;
    float arrivalSpeed = 6f;
    float arrivalRadius = 10f;
    float targetRadius = 0.1f;
    float targetSpeed = 4f;
    float distanceFromTarget;
    float rotationSpeed = 5;
    private GameObject target;

    //for aligning
    Vector3 goalFacing;
    Quaternion lookWhereYoureGoing;
    private GameObject alignTarget;

    //for wandering
    Vector3 waypoint;
    Vector3 wanderPoint;
    Vector3 face;
    Quaternion turnToFace;
    float range = 10f;
    float wanderSpeed = 1f;

    //for steering arrive
    public Vector3 velocity;
    public float maxAcceleration;
    public float maxSpeed;
    public float targetsRadius;
    public float slowRadius;
    public float timeToTarget;
    private bool canSteer = false;

    private float clamp = 0.1f;
    private char heuristic;
    private int orientation;
    private GameObject child;
    private GameObject replacementFlag;
    public GameObject canvas;



    void Start()
    {
        SetColor(); //red
        Wander();
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, clamp, transform.position.z);//stop him from floating upwards

        KeepOnMap();
        Freeze();
        ProtectHomeTurf();
        SetHeuristic();
        PerformBehaviour();
        AssignPlayerToCaptureFlag();
    }

    void AssignPlayerToCaptureFlag()
    {
        if(targetFlag == true)
        {
            chaseEnemy = false;
            SetTarget(enemyFlag);
        }
    }

    void SetHeuristic()
    {
        //KINEMATIC MOVEMENTS
        if (Input.GetKey(KeyCode.A))
        {
            heuristic = 'a';
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
        if (Input.GetKey(KeyCode.B))
        {
            heuristic = 'b';
            if (targetFlag == true)
            {
                if ((target.transform.position - transform.position).magnitude > arrivalRadius)
                {
                    GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                }
            }
        }
        if (Input.GetKey(KeyCode.C))
        {
            heuristic = 'c';
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
        //STEERING MOVEMENTS
        if (Input.GetKey(KeyCode.D))
        {
            heuristic = 'A';
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            heuristic = 'B';
            if ((target.transform.position - transform.position).magnitude > arrivalRadius)
                GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
        if (Input.GetKey(KeyCode.F))
        {
            heuristic = 'C';
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
            
    }

    void PerformBehaviour()
    {
        float distance;
        float dir;
        Vector3 direction;

        if(heuristic == 'a' && (targetFlag == true || hasFlag == true))
        {

            distance = (target.transform.position - transform.position).magnitude;
            direction = (target.transform.position - transform.position).normalized;

            if (distance > targetRadius)
            {
                Align();
                dir = Vector3.Dot(direction, transform.forward);
                if (dir >= 0.9f)
                {
                    Arrive();
                }                
            }
            else
            {
                Arrive();
            }
        }
        else if(heuristic == 'b' && (targetFlag == true || hasFlag == true))
        {
            distance = (target.transform.position - transform.position).magnitude;
            direction = (target.transform.position - transform.position).normalized;

            if(distance > arrivalRadius)
            {
                Align();
                dir = Vector3.Dot(direction, transform.forward);
                if (dir >= 0.9f)
                {
                    Arrive();
                }
            }
            else
            {
                Align();
                Arrive();
            }
        }
        else if (heuristic == 'c' && (targetFlag == true || hasFlag == true))
        {
            distance = (target.transform.position - transform.position).magnitude;
            direction = (target.transform.position - transform.position).normalized;

            if(distance <= targetRadius + 1f)
            {
                Flee();
            }
            else
            {
                FleeAlign();
                dir = Vector3.Dot(direction, transform.forward);
                if (dir <= -0.9f)
                {
                    Flee();
                }
            }
        }
        else if (heuristic == 'A' && (targetFlag == true || hasFlag == true)) 
        {

            distance = (target.transform.position - transform.position).magnitude;
            direction = (target.transform.position - transform.position).normalized;

             if (distance > arrivalRadius)
             {
                 Align();
                 dir = Vector3.Dot(direction, transform.forward);
                 if (dir >= 0.9f)
                 {
                     SteerArrive();
                 }
             }
             else
             {
                 Align();
                 SteerArrive();
             }
        }
        else if (heuristic == 'B' && (targetFlag == true || hasFlag == true))
        {

            distance = (target.transform.position - transform.position).magnitude;
            direction = (target.transform.position - transform.position).normalized;

            if (distance > arrivalRadius)
            {
                Align();
                dir = Vector3.Dot(direction, transform.forward);
                if (dir >= 0.9f)
                {
                    SteerArrive();
                }
            }
            else
            {
                Align();
                SteerArrive();
            }
        }
        else if(frozen == false)
        {
            transform.position += transform.TransformDirection(Vector3.forward) * wanderSpeed * Time.deltaTime;
            if((transform.position - waypoint).magnitude < 7)
            {
                Wander();
            }
        }
    }

    void Arrive()
    {
        distanceFromTarget = (target.transform.position - transform.position).magnitude;
        direction = (target.transform.position - transform.position).normalized;

        if (distanceFromTarget > nearRadius)
        {
            GetComponent<Rigidbody>().velocity = direction * nearSpeed;
        }
        else if (distanceFromTarget > arrivalRadius)
        {
            GetComponent<Rigidbody>().velocity = direction * arrivalSpeed;
        }
        else if(distanceFromTarget > targetRadius)
        {
            GetComponent<Rigidbody>().velocity = direction * targetSpeed;
        }
    }

    void Flee()
    {
        direction = (transform.position - target.transform.position).normalized;
        GetComponent<Rigidbody>().velocity = direction * nearSpeed;
    }

    void SteerArrive()
    {
        distanceFromTarget = (target.transform.position - transform.position).magnitude;
        direction = (target.transform.position - transform.position);
        
        if(canSteer == true)
        {
            float targetsSpeed = 0;
            if (distanceFromTarget > slowRadius)
                targetsSpeed = maxSpeed;
            else
                targetsSpeed = maxSpeed * distanceFromTarget / slowRadius;

            Vector3 targetVelocity = direction;
            targetVelocity.Normalize();
            targetVelocity *= targetsSpeed;
            Vector3 linear = targetVelocity - velocity;
            linear /= timeToTarget;

            if (linear.magnitude > maxAcceleration)
            {
                linear.Normalize();
                linear *= maxAcceleration;
            }

            transform.position += velocity * Time.deltaTime;
            velocity += linear * Time.deltaTime;

            if (velocity.magnitude > maxSpeed)
            {
                velocity.Normalize();
                velocity *= maxSpeed;
            }
        }
    }

    void Pursue(GameObject enemy)
    {
        Vector3 goal;
        SetTarget(enemy);
        direction = (target.transform.position - transform.position);
        float distance = direction.magnitude;

        float speed = new Vector3(1, 0, 1).magnitude;
        float prediction = 0f;
        float maxPrediction = 2;
        if(speed <= distance / maxPrediction)
        {
            prediction = maxPrediction;
        }
        else
        {
            prediction = distance / speed;
        }

        goal = enemy.transform.position + (enemy.GetComponent<Team2>().velocity * prediction);

        PerformMovement();
    }

    void Wander()
    {
        waypoint = new Vector3(Random.Range(transform.position.x - range, transform.position.x + range), 
                   transform.position.y , Random.Range(transform.position.z - range, transform.position.z + range));
        waypoint.y = transform.position.y;
        transform.LookAt(waypoint);
     }

    void Align()
    {
        goalFacing = (alignTarget.transform.position - transform.position).normalized;
        lookWhereYoureGoing = Quaternion.LookRotation(goalFacing, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookWhereYoureGoing, rotationSpeed);
    }

    void FleeAlign()
    {
        goalFacing = (transform.position - alignTarget.transform.position).normalized;
        lookWhereYoureGoing = Quaternion.LookRotation(goalFacing, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookWhereYoureGoing, rotationSpeed);
    }

    void SetTarget(GameObject newTarget)
    {
        alignTarget = newTarget;
        target = newTarget;
    }    

    void SetColor()
    {
        child = transform.GetChild(0).gameObject;
        Renderer rend = child.GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Specular");
        rend.material.SetColor("_SpecColor", Color.red);
    }

    void ProtectHomeTurf()
    {
        for(int i = 0; i<enemies.Length; i++)
        {
            GameObject enemy = enemies[i].gameObject;
            if(hasFlag == false && chaseEnemy == true)
            {
                if (targetFlag == false && enemies[i].transform.position.z > 0)
                {
                    if (enemies[i].frozen == true)
                        continue;

                    SetTarget(enemies[i].gameObject);
                    Pursue(enemies[i].gameObject);
                }
            } 
        }
    }

    void Freeze()
    {
        if(frozen == true)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            canSteer = false;
        }
        else if(frozen == false)
        {
            Unfreez();
        }
    }

    void Unfreez()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        canSteer = true;
    }

    public void SaveTeamMate(GameObject mate)
    {
        chaseEnemy = false;
        SetTarget(mate);
        PerformMovement();
    }

    void PerformMovement()
    {
        if (heuristic == 'a' || heuristic == 'b')
        {
            Align();
            Arrive();
        }
        if (heuristic == 'A' || heuristic == 'B')
        {
            Align();
            SteerArrive();
        }
    }

    void KeepOnMap()
    {
        if(transform.position.z > 35f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -34f);
        }
        else if(transform.position.z < -35f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 34f);
        }
        else if(transform.position.x > 15)
        {
            transform.position = new Vector3(-14.5f, transform.position.y, transform.position.z);
        }
        else if(transform.position.x < -15)
        {
            transform.position = new Vector3(14.5f, transform.position.y, transform.position.z);
        }
    }

    void OnCollisionEnter (Collision collision)
    {
        child = transform.GetChild(1).gameObject;
        GameObject obj = collision.gameObject;

        //if he gets the enemy flag
        if (obj == enemyFlag || obj == replacementFlag)
        {
            targetFlag = false;
            hasFlag = true;
            child.SetActive(true);
            SetTarget(homeFlag);
        }

        //if he gets hit by enemy in enemy territory
        for (int i = 0; i < enemies.Length; i++)
        {
            if(transform.position.z < 0)
            {
                if (obj == enemies[i].gameObject)
                {
                    frozen = true;
                    if (hasFlag == true)
                    {
                        Debug.Log("lost flag");
                        targetFlag = true;
                        replacementFlag = Instantiate(flag, enemyFlag.transform.position, Quaternion.identity) as GameObject;
                        child.SetActive(false);
                        hasFlag = false;
                    }
                }
            }
            else //in home territory
            {
                if (obj == enemies[i].gameObject)
                {
                    Wander();
                }
            }
           
        }

        for (int i = 0; i < teammates.Length; i++)
        {
            if(obj == teammates[i].gameObject)
            {
                if(frozen == true) //if your frozen and hit by friend
                {
                    frozen = false;
                    if(targetFlag == true)
                    {
                        SetTarget(enemyFlag);
                        PerformMovement();
                    }

                }
                if(teammates[i].frozen == true) //if friend is frozen and you hit him
                {
                        chaseEnemy = true;
                        Wander();
                }
            }
        }

        if(obj == flagBase)
        {
            if(hasFlag == true)
            {
                Debug.Log("you won");
                canvas.SetActive(true);
            }
        }
    }
}
