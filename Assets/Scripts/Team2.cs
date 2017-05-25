using UnityEngine;
using System.Collections;

public class Team2 : MonoBehaviour {

    //gameobjects in the scene
    //public Flag newFlag;
    public GameObject homeFlag;
    public GameObject enemyFlag;
    public GameObject flag; //used for creating a new flag after its taken off post
    public GameObject flagBase; //can go to this instead of flag when flag is taken
    public Team1[] enemies;
    public Team2[] teammates;
    public bool targetFlag = false; //player who assigned to capture flag will be true
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
    private bool canSteer = true; //if allowed to use steering arrive

    private float clamp = 0.1f; //clamp their y position
    private char heuristic; //whether kinematic or dynamic movements to start off the game
    private int orientation;
    private GameObject child; //the flag they will hold when they capture it
    private GameObject replacementFlag; //the flag that will be instantiated when a player with the flag is frozen
    public GameObject canvas;

    void Start()
    {
        SetColor(); //green
        Wander();
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, clamp, transform.position.z);

        KeepOnMap(); //wraps them around map
        Freeze(); //freeze if hit in enemy zone
        ProtectHomeTurf(); //attack enemies in home zone
        SetHeuristic(); //button pressed sets movement
        PerformBehaviour(); //perform movement
        AssignPlayerToCaptureFlag(); //assign player to capture flag
    }

    void AssignPlayerToCaptureFlag()
    {
        //if the team2controller assigned this player to capture the flag, set its target
        if (targetFlag == true)
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

    //which movment to use to capture the flag based off of which button pressed
    void PerformBehaviour()
    {

        float distance;
        float dir;
        Vector3 direction;

        if (heuristic == 'a' && (targetFlag == true || hasFlag == true))
        {

            distance = (target.transform.position - transform.position).magnitude;
            direction = (target.transform.position - transform.position).normalized;

            if (distance > targetRadius) //if too far align first
            {
                Align();
                dir = Vector3.Dot(direction, transform.forward); //wait to align with target first based off of distance
                if (dir >= 0.9f)
                {
                    Arrive();
                }
            }
            else //jump right too object
            {
                Arrive();
            }
        }
        //same as above for rest****************************
        else if (heuristic == 'b' && (targetFlag == true || hasFlag == true))
        {
            distance = (target.transform.position - transform.position).magnitude;
            direction = (target.transform.position - transform.position).normalized;

            if (distance > arrivalRadius)
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

            if (distance <= targetRadius + 1f)
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

        else if(heuristic == 'B' && (targetFlag == true || hasFlag == true))
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
        else if(frozen == false) //if neither kinematic nor dynamic and not frozen just wander
        {
            transform.position += transform.TransformDirection(Vector3.forward) * wanderSpeed * Time.deltaTime;
            if ((transform.position - waypoint).magnitude < 7)
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
        else if (distanceFromTarget > targetRadius)
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

        if (canSteer == true) //if they are not frozen than they should be allowed to move
        {
            float targetsSpeed = 0;
            if (distanceFromTarget > slowRadius) //if they are really really close, just go to target
                targetsSpeed = maxSpeed;
            else
                targetsSpeed = maxSpeed * distanceFromTarget / slowRadius; //slow down as approach target

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

            if (velocity.magnitude > maxSpeed) //do not accelerate over max speed
            {
                velocity.Normalize();
                velocity *= maxSpeed;
            }
        }
    }
    void Pursue(GameObject enemy)
    {
        SetTarget(enemy);
        direction = (target.transform.position - transform.position);
        float distance = direction.magnitude;

        float speed = new Vector3(1, 0, 1).magnitude;
        float prediction = 0f;
        float maxPrediction = 2;
        //set predictions based off of distance
        if (speed <= distance / maxPrediction) 
        {
            prediction = maxPrediction;
        }
        else
        {
            prediction = distance / speed;
        }

        Vector3 goal = enemy.transform.position + (enemy.GetComponent<Team1>().velocity * prediction);
        PerformMovement(); //perform approariate movement
    }

    void Wander()
    {
        waypoint = new Vector3(Random.Range(transform.position.x - range, transform.position.x + range),
                   transform.position.y, Random.Range(transform.position.z - range, transform.position.z + range)); //set the wander point based off of curent position and a range
        waypoint.y = transform.position.y;
        transform.LookAt(waypoint); //look at the wander, couldnt get smooth rotation to work
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
        rend.material.SetColor("_SpecColor", Color.green);
    }

    void ProtectHomeTurf() //patrol to see if enemy enters home zone
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            GameObject enemy = enemies[i].gameObject;
            if (hasFlag == false && chaseEnemy == true)
            {
                if (targetFlag == false && enemies[i].transform.position.z < 0) //if enemy crosses center into your home and you are not going after the flag
                {
                    if (enemies[i].frozen == true) //if that enemy is already frozen move on and look for another one
                        continue;

                    SetTarget(enemies[i].gameObject);
                    Pursue(enemies[i].gameObject); //pursue enemies in your territory
                }
            }
        }
    }

    void Freeze()
    {
        if (frozen == true) //this is determined when they collide
        {
            //stop and constrain their movements
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            canSteer = false; //this is to prevent steering arrive from working even when hit by an enemy
        }
        else if (frozen == false)
        {
            Unfreez();
        }
    }

    void Unfreez()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation; //reset restrictions only on rotation again
        canSteer = true; //let steering take place again if required
    }

    public void SaveTeamMate(GameObject mate) //called by teamcontroller, goes towards a frozen team member
    {
        chaseEnemy = false;
        SetTarget(mate);
        PerformMovement();
    }

    void PerformMovement()
    {
        if(heuristic == 'a' || heuristic == 'b')
        {
            Align();
            Arrive();
        }
        if(heuristic == 'A' || heuristic == 'B')
        {
            Align();
            SteerArrive();
        }
    }

    void KeepOnMap() //loops player around when reaching edge positions
    {
        if (transform.position.z > 35f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -34f);
        }
        else if (transform.position.z < -35f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 34f);
        }
        else if (transform.position.x > 15)
        {
            transform.position = new Vector3(-14.5f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -15)
        {
            transform.position = new Vector3(14.5f, transform.position.y, transform.position.z);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;
        child = transform.GetChild(1).gameObject; //what will become the flag in their hand when they hit it

        //if he gets the enemy flag
        if (obj == enemyFlag || obj == replacementFlag)
        {
            targetFlag = false;
            hasFlag = true;
            child.SetActive(true); //turn on the flag in their hand
            SetTarget(homeFlag); //go back to your base

            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);         
        }

        //if he hit enemy 
        for (int i = 0; i < enemies.Length; i++)
        {
            if (transform.position.z > 0) //in enemy territory
            {
                if (obj == enemies[i].gameObject)
                {
                    frozen = true; //freeze if it by enemy in enemy zone
                    if(hasFlag == true) //if frozen player is holding the flag
                    {
                        targetFlag = true; //set him to aim for the flag again when unfrozen
                        replacementFlag = Instantiate(flag, enemyFlag.transform.position, Quaternion.identity) as GameObject; //spawn the new flag
                        child.SetActive(false); //lose the flag in his hand if holding one
                        hasFlag = false;
                    }
                }
            }
            else //in home territory
            { 
                if(obj == enemies[i].gameObject)
                {
                    Wander();
                }
            }
        }


        for (int i = 0; i < teammates.Length; i++)
        {
            if (obj == teammates[i].gameObject)
            {
                if (frozen == true) //if your frozen and hit by teammate
                {
                    frozen = false;
                    if (targetFlag == true) //if your target is the flag go for it when unfrozen
                    {
                        SetTarget(flagBase);
                        PerformMovement();
                    }
                }
                if (teammates[i].frozen == true) //if teammate is frozen and you hit him
                {
                    chaseEnemy = true;
                    Wander();
                }
            }
        }

        if (obj == flagBase)
        {
            if (hasFlag == true)
            {
                Debug.Log("you won");
                canvas.SetActive(true);
            }
        }
    }
}
