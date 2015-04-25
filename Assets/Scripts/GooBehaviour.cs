using UnityEngine;
using System.Collections;

public class GooBehaviour : MonoBehaviour {

    private enum GooState
    {
        NONE,
        STUCK,
        SLIDE,
        RELEASED,
        AIRBORNE
    }

    private bool IsAlive;
    private Vector3 ShootDirection;
    private float ShootMagnitude;
    private Vector3 LaunchPointStart; //point mouse or touch started on the screen
    private Vector3 LaunchPointCurrent;
    private float MaxVelocity;

    public GameObject StartPos;

    public GameObject PowerArrow;
    private Vector3 ArrowStartPos = new Vector3(0, 0, 5);

    RigidbodyConstraints StartContraints;

    private GooState myState;
	// Use this for initialization
	void Start () 
    {
        transform.position = StartPos.transform.position;
        IsAlive = true;
        LaunchPointStart = Vector3.zero;
        LaunchPointCurrent = Vector3.zero;
        ShootDirection = Vector3.zero;
        ShootMagnitude = 0;
        myState = GooState.AIRBORNE;
        StartContraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        MaxVelocity = 15;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (IsAlive)
        {
            if (myState != GooState.AIRBORNE)
                CheckMouseState();

            
            //run through the update for each state
            if (myState == GooState.RELEASED)
            {
                //unfreeze constraints
                rigidbody.constraints = StartContraints;

                rigidbody.velocity = ShootDirection * ShootMagnitude;
                
                myState = GooState.AIRBORNE;
            }
            if (myState == GooState.STUCK)
            {
                //freeze contraints
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }

            if (rigidbody.velocity.magnitude > MaxVelocity)
            {
                rigidbody.velocity = rigidbody.velocity.normalized * MaxVelocity;
            }

        }
        else //if DEAD
        {
            //run the reset
        }
    }

    //Collision: have the collider smaller than the ball so that it looks like it sticks onto objects
    void OnCollisionEnter(Collision col)
    {
        if (myState == GooState.AIRBORNE)
        {
            //check what type of object you hit
            if (col.collider.tag == "NormalWall")
            {
                myState = GooState.STUCK;
            }
        }
    }
    void OnCollisionStay(Collision col)
    {
        //fix bug if still colliding with object despite being released
        if (myState == GooState.AIRBORNE)
        {
            transform.position += col.contacts[0].normal * 0.1f;
        }
    }

    void CheckMouseState()
    {
        //is it clicked down for the first time?
        if (Input.GetMouseButtonDown(0))
        {
            LaunchPointStart = Input.mousePosition;
        }
        //while clicked, has it dragged?
        if (Input.GetMouseButton(0))
        {
            //calc the velocity, ready for release
            LaunchPointCurrent = Input.mousePosition;
            ShootDirection = (LaunchPointStart - LaunchPointCurrent).normalized;
            ShootMagnitude = (LaunchPointStart - LaunchPointCurrent).magnitude / 8;
            
            PowerArrow.transform.position = transform.position;

            float rad = Mathf.Atan2(ShootDirection.y, ShootDirection.x); // In radians
            float deg = rad * (180 / Mathf.PI);

            Vector3 rot;
            rot.x = 0;
            rot.y = 0;
            rot.z = deg - 90;
            PowerArrow.transform.rotation = Quaternion.Euler(rot);
            PowerArrow.transform.localScale = new Vector3(1,1,1) * ShootMagnitude / 4;
            
        }
        //has it released
        if (Input.GetMouseButtonUp(0))
        {
            if (ShootMagnitude > 0.05)
            {
                myState = GooState.RELEASED;
            }
            PowerArrow.transform.position = ArrowStartPos;
            PowerArrow.transform.localScale = new Vector3(1,1,1);
        }
    }


}
