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
        Reset();
        MaxVelocity = 15;
        StartContraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    }

    void Reset() //set up the player at start on begin or after death
    {
        transform.position = StartPos.transform.position;
        rigidbody.velocity = Vector3.zero;
        IsAlive = true;
        LaunchPointStart = Vector3.zero;
        LaunchPointCurrent = Vector3.zero;
        ShootDirection = Vector3.zero;
        ShootMagnitude = 0;
        myState = GooState.AIRBORNE;
        
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
                //rigidbody.isKinematic = false;
                rigidbody.velocity = ShootDirection * ShootMagnitude;
                
                myState = GooState.AIRBORNE;
            }
            if (myState == GooState.STUCK)
            {
                //freeze contraints
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                //rigidbody.isKinematic = true;
            }

            if (rigidbody.velocity.magnitude > MaxVelocity)
            {
                rigidbody.velocity = rigidbody.velocity.normalized * MaxVelocity;
            }

        }
        else //if DEAD
        {
            //run the reset
            Reset();
        }
    }

    //Collision: have the collider smaller than the ball so that it looks like it sticks onto objects
    void OnCollisionEnter(Collision col)
    {
        if (myState == GooState.AIRBORNE)
        {
            //check what type of object you hit
            
            //Normal Wall
            if (col.collider.tag == "NormalWall")
            {
                myState = GooState.STUCK;
            }

            //Trap
            if (col.collider.tag == "Trap")
            {
                IsAlive = false;
            }

            //Slippery
            if (col.collider.tag == "SlipperyWall")
            {
                myState = GooState.SLIDE;
            }

            //Shock
            if (col.collider.tag == "ShockWall")
            {
                rigidbody.velocity = rigidbody.velocity.normalized * MaxVelocity;
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
    void OnCollisionExit(Collision col)
    {
        if (myState == GooState.SLIDE)
        {
            myState = GooState.AIRBORNE;
            PowerArrow.transform.position = ArrowStartPos;
            PowerArrow.transform.localScale = new Vector3(1, 1, 1);
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
            ShootMagnitude = (LaunchPointStart - LaunchPointCurrent).magnitude / 10;
            if (ShootMagnitude > MaxVelocity)
            {
                ShootMagnitude = MaxVelocity;
            }
            
            

            float rad = Mathf.Atan2(ShootDirection.y, ShootDirection.x); // In radians
            float deg = rad * (180 / Mathf.PI);

            Vector3 rot;
            rot.x = 0;
            rot.y = 0;
            rot.z = deg - 90;
            PowerArrow.transform.rotation = Quaternion.Euler(rot);
            PowerArrow.transform.localScale = new Vector3(1,1,1) * ShootMagnitude / 2;
            PowerArrow.transform.position = transform.position - (ShootDirection * 2);
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
