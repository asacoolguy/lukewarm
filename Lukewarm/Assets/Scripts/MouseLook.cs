using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour {
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

  public bool doPickup, doFire;
  public LayerMask pickupMask, fireMask;
  public float pickupRange;

  public GameObject playerStats;
	public PlayerStats playerScript;

	// Use this for initialization
	void Start () {
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
		originalRotation = transform.localRotation;
		playerScript = playerStats.GetComponent<PlayerStats> ();

    doPickup = false;

  }
	
	// Update is called once per frame
	void Update () {
		rotationX += Input.GetAxis("Mouse X") * sensitivityX;
		rotationX = ClampAngle (rotationX, minimumX, maximumX);
		Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
		player.transform.localRotation = originalRotation * xQuaternion;

		rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
		rotationY = ClampAngle (rotationY, minimumY, maximumY);
		Quaternion yQuaternion = Quaternion.AngleAxis (-rotationY, Vector3.right);
		view.transform.localRotation = originalRotation * yQuaternion;

    if (Input.GetButtonDown("Pickup"))
    {
      doPickup = true;
    }
    if (Input.GetButtonDown("Fire"))
    {
      doFire = true;
    }
	}

  void FixedUpdate()
  {
    if (doPickup)
    {
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
    doPickup = false;

    if (doFire)
    {
      PlayerStats stats = playerStats.GetComponent<PlayerStats>();
      if (stats.weapon.ammo > 0)
      {
        stats.weapon.ammo--;

		playerScript.muzzleFlash.SetActive(true);
		playerScript.muzzleFlashCountdown = playerScript.muzzleFlashLife;
        // TODO: draw the bullet trail.

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        // The mask is set to the default layer and player layer.
        if (Physics.Raycast(ray, out hit, 500, fireMask))
        {
          if (hit.collider.gameObject.tag != null && hit.collider.gameObject.tag == "Player")
          {
            PlayerStats player = hit.collider.gameObject.GetComponent<PlayerStats>();
            player.health -= stats.weapon.bodyShotDMG;

            // TODO: Move the animation stuff into here.
						playerScript.crosshair.SetActive(false);
						playerScript.crosshair_hit.SetActive(true);
						playerScript.crosshairHitCountdown = playerScript.crosshairHitLife;
          }
        }
        // check ammo
        if (stats.weapon.ammo <= 0)
        {
          stats.switchWeapon(PlayerStats.WeaponType.unarmed);
        }
      }
      
    }
    doFire = false;

	// cool down stuff

		// cool down the muzzleFlash
		playerScript.muzzleFlashCountdown -= Time.deltaTime;
		if (playerScript.muzzleFlashCountdown < 0f)
		{
			playerScript.muzzleFlashCountdown = 0f;
			playerScript.muzzleFlash.SetActive(false);
		}

		// cool down the crosshair_hit
		playerScript.crosshairHitCountdown -= Time.deltaTime;
		if (playerScript.crosshairHitCountdown < 0f)
		{
			playerScript.crosshairHitCountdown = 0f;
			playerScript.crosshair.SetActive(true);
			playerScript.crosshair_hit.SetActive(false);
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
