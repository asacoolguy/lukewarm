using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementControl : MonoBehaviour {
	public float speed = 10f;
	public float jumpPower = 10f;
	public float gravity = 29.8f;
	float jumpThreshold = 0.01f;
	float jumpingSpeed;
	bool grounded, sprinting;
  bool doJump, doSprint;
	Vector3 priorMovement;

	public LayerMask floorMask;

	public List<GameObject> collidingWalls;
	private Rigidbody rbody;
	bool canMove;

	// Use this for initialization
	void Start () {
		jumpingSpeed = 0;
		jumpThreshold = 0.01f;
		sprinting = false;
		grounded = true;

    // doJump and doSprint keep track of whether we should initiate a jump/sprint on the next fixedframe
    doJump = doSprint = false;

		priorMovement = new Vector3 ();
		collidingWalls = new List<GameObject> ();
		rbody = GetComponent<Rigidbody> ();
		canMove = false;
	}

	public void setCanMove (bool canMove) {
		this.canMove = canMove;
	}

  void Update()
  {
    if (!canMove)
    {
      return;
    }
    if (Input.GetButtonDown("Sprint"))
    {
      doSprint = true;
    }
    if (Input.GetButtonDown("Jump"))
    {
      doJump = true;
    }
  }

	// Update is called once per frame
	void FixedUpdate () {
		if (!canMove) {
			return;
		}

		// TODO: move sprint and jump initial inputs to Update() for better responsiveness.  Toggle a bool true and false based on the input detected in update().
		// check sprinting
    if (doSprint)
    {
      sprinting = true;
      doSprint = false;
    }
    if (sprinting && Input.GetAxis("Vertical") <= 0.01)
    {
      sprinting = false; // this makes sprinting persist until you stop moving "forward"
    }
		
		Vector3 move = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));

		move *= speed * Time.fixedDeltaTime;
		if (sprinting)
			move *= 1.5f;

		if (!grounded) {
			move = priorMovement * 0.75f + move * 0.25f;	
		}

		priorMovement = move;

		// jump
		if (doJump && grounded) {
			jumpingSpeed = jumpPower;
			grounded = false;
		}
    doJump = false;

    // landing
		// if you're in the air, do a raycast to check if you've hit the ground
		if (!grounded) {
			Ray ray = new Ray (transform.position, new Vector3 (0, -1, 0));
			RaycastHit hit;
			// If the raycast hits, then set grounded to true and move the player character just above the 
			//   raycast threshold.
			if (Physics.Raycast (ray, out hit, 1 + jumpThreshold, floorMask)) {
				grounded = true;
				jumpingSpeed = 0;
				move.y = 1 + jumpThreshold - hit.distance + 0.005f;
			} else {
				// If the raycast misses, then keep moving the player down (simulated gravity).
				move.y = jumpingSpeed * Time.fixedDeltaTime;
				jumpingSpeed -= gravity * Time.fixedDeltaTime;
			}
		} else  {
			// This checks to see if you've walked off a platform.
			Ray ray = new Ray (transform.position, new Vector3 (0, -1, 0));
			RaycastHit hit;
      if (!Physics.Raycast(ray, out hit, 1 + jumpThreshold + 0.01f, floorMask))
      {
				grounded = false;
				move.y = 1 + jumpThreshold - hit.distance + 0.005f;
				jumpingSpeed = 0;
				jumpingSpeed -= gravity * Time.fixedDeltaTime;
				move.y = jumpingSpeed * Time.fixedDeltaTime;
			}
			
		}
		if (collidingWalls.Count == 0) {
			rbody.velocity = Vector3.zero;
		}


		transform.Translate (move);
	}

	void OnCollisionEnter(Collision c){
		if (c.gameObject.tag == "Wall") {
			collidingWalls.Add (c.gameObject);
		}

	}

	void OnCollisionExit(Collision c){
		if (c.gameObject.tag == "Wall") {
			collidingWalls.Remove (c.gameObject);
		}
	}

}
