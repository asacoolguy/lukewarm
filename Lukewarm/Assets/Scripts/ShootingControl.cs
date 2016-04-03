using UnityEngine;
using System.Collections;

public class ShootingControl : MonoBehaviour {
	public GameObject eyelevel;
	public GameObject muzzleFlash;
	public GameObject crosshair;
	public GameObject crosshair_hit;
	public GameObject pistol;
	public GameObject automatic;

	public enum WeaponType
	{	unarmed,
		pistol,
		automatic
	};

	// gun variables
	public WeaponType weapon = WeaponType.unarmed;
	public int ammo = 0;
	public float recoilTime;
	public float currentRecoilTime;
	public float headShotDMG;
	public float bodyShotDMG;

	public float muzzleFlashLife = 0.1f;
	public float muzzleFlashCountdown = 0f;
	public float crosshairHitLife = 0.1f;
  public float crosshairHitCountdown = 0f;

  public bool doShoot;

	// Use this for initialization
	void Start () {
		switchWeapon ("unarmed");
    doShoot = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0)) {
      doShoot = true;
		}
	}

  void FixedUpdate()
  {
    if (doShoot)
    {
      // pistol stuff
      if (weapon == WeaponType.pistol)
      {
        if (currentRecoilTime == 0)
        {
          // shoot
          muzzleFlash.SetActive(true);
          muzzleFlashCountdown = muzzleFlashLife;
          currentRecoilTime = recoilTime;

          ammo -= 1;

          // see if we hit anything
          Ray ray = new Ray(eyelevel.transform.position, eyelevel.transform.forward);
          RaycastHit hit;

          if (Physics.Raycast(ray, out hit, 500))
          {
            if (hit.collider.gameObject.tag == "Player")
            {
              PlayerStats player = hit.collider.gameObject.GetComponent<PlayerStats>();
              player.health -= bodyShotDMG;

              crosshair.SetActive(false);
              crosshair_hit.SetActive(true);
              crosshairHitCountdown = crosshairHitLife;
            }
          }
        }
      }
    }
    doShoot = false;

    // cool down the weapon
    currentRecoilTime -= Time.deltaTime;
    if (currentRecoilTime < 0f)
    {
      currentRecoilTime = 0f;
    }

    // cool down the muzzleFlash
    muzzleFlashCountdown -= Time.deltaTime;
    if (muzzleFlashCountdown < 0f)
    {
      muzzleFlashCountdown = 0f;
      muzzleFlash.SetActive(false);
    }

    // cool down the crosshair_hit
    crosshairHitCountdown -= Time.deltaTime;
    if (crosshairHitCountdown < 0f)
    {
      crosshairHitCountdown = 0f;
      crosshair.SetActive(true);
      crosshair_hit.SetActive(false);
    }

    // check ammo
    if (ammo <= 0)
    {
      switchWeapon("unarmed");
    }
  }

	public void switchWeapon(string s){
		if (s == "unarmed") {
			initUnarmed ();
			pistol.SetActive (false);
		} else if (s == "pistol") {
			initPistol ();
			pistol.SetActive (true);
		} else if (s == "automatic") {

		}
	}
		
	void initPistol(){
		weapon = WeaponType.pistol;
		recoilTime = 0.5f;
		currentRecoilTime = 0f;
		headShotDMG = 100f;
		bodyShotDMG = 20f;
		ammo = 6;
	}

	void initUnarmed(){
		weapon = WeaponType.unarmed;
		recoilTime = 0f;
		currentRecoilTime = 0f;
		headShotDMG = 50f;
		bodyShotDMG = 35f;
		ammo = 0;
	}
}
