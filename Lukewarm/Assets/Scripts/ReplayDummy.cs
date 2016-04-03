using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReplayDummy : MonoBehaviour {
	public ActionSet actionSet;
	public int ticker;
	public Vector3 spawnLocation;

	// The following variables copied from MovementControl
	public float speed = 10f;
	public float jumpPower = 10f;
	public float gravity = 29.8f;
	float jumpThreshold = 0.01f;
	float jumpingSpeed;
	bool grounded, sprinting;
	Vector3 priorMovement;
  public Material playerOneMaterial, playerTwoMaterial;

  public LayerMask floorMask;

	public List<GameObject> collidingWalls;
	private Rigidbody rbody;

	// The following variables copied from MouseLook
	public GameObject player;
	public GameObject view;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	public float minimumX = -360F;
	public float maximumX = 360F;
	public float minimumY = -60F;
	public float maximumY = 60F;
	float rotationX = 0F;
	float rotationY = 0F;
	Quaternion originalRotation;

  public LayerMask pickupMask, fireMask;
  public float pickupRange;

  public GameObject playerStats;

	public void setActionSet(ActionSet actionSet) {
		this.actionSet = actionSet;
    this.playerStats.GetComponent<PlayerStats>().switchWeapon(actionSet.weapon);
    this.playerStats.GetComponent<PlayerStats>().player = actionSet.player;
    Debug.Log(actionSet.weapon);
	}

	// Use this for initialization
	void Start () {
		if (GetComponent<Rigidbody> ()) {
			GetComponent<Rigidbody> ().freezeRotation = true;
		}
    if (actionSet == null)
    {
      actionSet = new ActionSet();
    }
    else
    {
      this.playerStats.GetComponent<PlayerStats>().switchWeapon(actionSet.weapon);
      this.playerStats.GetComponent<PlayerStats>().player = actionSet.player;
    }
		ticker = 0;

		originalRotation = transform.localRotation;
		collidingWalls = new List<GameObject> ();
		rbody = GetComponent<Rigidbody> ();

		jumpingSpeed = 0;
		jumpThreshold = 0.01f;
		grounded = true;
    priorMovement = new Vector3();
    Debug.Log(actionSet.weapon);

    if (this.playerStats.GetComponent<PlayerStats>().player == PlayerStats.Player.one)
    {
      gameObject.GetComponent<Renderer>().material = playerOneMaterial;
    }
    else
    {
      Debug.Log("p2 mat");
      gameObject.GetComponent<Renderer>().material = playerTwoMaterial;
    }
	}
	
	void FixedUpdate () {
		// copy and paste the code from MovementControl.
		/* Changes:
		 * move vector set according to action 
		 * GetButtonDown checks changed to action
		 * Time.deltaTime changed to Time.fixedDeltaTime.
		*/

		if (gameObject && ticker >= actionSet.length()) {
			Destroy (gameObject);
			return;
		}

		ActionSet.Action act = actionSet.get(ticker);

		// ---------- Turning --------------
		player.transform.localRotation = act.playerForward;

		view.transform.localRotation = act.viewForward;

		// ---------- Movement --------------
		Vector3 move = new Vector3 (act.movement.x, 0, act.movement.y);
		move *= speed * Time.fixedDeltaTime;

    if (act.isSprint)
    {
      sprinting = true;
    }
    if (sprinting && act.movement.y <= 0.01)
    {
      sprinting = false; // this makes sprinting persist until you stop moving "forward"
    }

    if (sprinting)
    {
      move *= 1.5f;
    }

		if (!grounded) {
			move = priorMovement * 0.75f + move * 0.25f;	
		}

		priorMovement = move;

		// jump
		if (act.isJump && grounded) {
			jumpingSpeed = jumpPower;
			grounded = false;
		} 
		if (act.isJump && !grounded) {
		} 

		// if you're in the air, do a raycast to check if you've hit the ground
		if (!grounded) {
			Ray ray = new Ray (transform.position, new Vector3 (0, -1, 0));
			RaycastHit hit;
			// If the raycast hits, then set grounded to true and move the player character just above the 
			//   raycast threshold.
      if (Physics.Raycast(ray, out hit, 1 + jumpThreshold, floorMask))
      {
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

    // ---------- Pick up -----------
    if (act.isPickup) {
      Ray ray = new Ray(transform.position, transform.forward);
      RaycastHit hit;
      // The mask makes the raycast only hit pickups.
      if (Physics.Raycast(ray, out hit, pickupRange, pickupMask))
      {
        // TODO: show pickup overlay hint ("'e' to pick up _____)
        PickupBox pickup = hit.collider.gameObject.GetComponent<PickupBox>();
        if (pickup.pickupType == PickupBox.PickupType.flag)
        {
          playerStats.GetComponent<PlayerStats>().switchWeapon(PlayerStats.WeaponType.flag);
        }
        else if (pickup.pickupType == PickupBox.PickupType.pistol)
        {
          playerStats.GetComponent<PlayerStats>().switchWeapon(PlayerStats.WeaponType.pistol);
        }
        else if (pickup.pickupType == PickupBox.PickupType.automatic)
        {
          playerStats.GetComponent<PlayerStats>().switchWeapon(PlayerStats.WeaponType.automatic);
        }

				// TODO: pickup box deactivates after being picked up
				print("pick up hit");
				hit.collider.gameObject.SetActive(false);
      }
    }

		// ---------- Shooting Controller -----------------
		if (act.isFire) {
      Debug.Log("puppet firing");
      PlayerStats stats = playerStats.GetComponent<PlayerStats>();
      Debug.Log(stats.weapon.type);
      if (stats.weapon.ammo > 0)
      {
        stats.weapon.ammo--;
        // TODO: draw the bullet trail.

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        // The mask is set to the default layer and player layer.
        if (Physics.Raycast(ray, out hit, 500, fireMask))
        {
          if (hit.collider.gameObject.tag == "Player")
          {
            PlayerStats pStats = hit.collider.gameObject.GetComponent<PlayerStats>();
            pStats.health -= stats.weapon.bodyShotDMG;
            // TODO: Move the animation and cooldown stuff into here.
          }
        }
        // check ammo
        if (stats.weapon.ammo <= 0)
        {
          stats.switchWeapon(PlayerStats.WeaponType.unarmed);
        }
      }
		}

		ticker++;
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

	public static float ClampAngle (float angle, float min, float max)
	{
		if (angle <= -360F)
			angle += 360F;
		if (angle >= 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}
}
