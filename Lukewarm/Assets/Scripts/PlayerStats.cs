using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {
	public float MAX_HEALTH = 100f;
	public float health;
	public float muzzleFlashCountdown;
	public float muzzleFlashLife = .1f;
	public float crosshairHitLife = .1f;
	public float crosshairHitCountdown ;

  public enum Player
  {
    one = 0,
    two,
  };
  public Player player;

	public GameObject flagPickup; // the dropped flag.
	public GameObject flag;

  // TODO: move the guns to here.  why the heck are they in shootingcontrol?  Put a link to this instance in MouseLook.

  public enum WeaponType
  {
    unarmed,
    pistol,
    automatic,
    flag
  };

  public struct Weapon {
    public WeaponType type;
    public int ammo;
    public float recoilTime;
    public float currentRecoilTime;
    public float headShotDMG;
    public float bodyShotDMG;
  }
  // gun variables
  public Weapon weapon;

  // TODO: uncomment when implementing the gun shooting animations.
  
  public GameObject muzzleFlash;
  public GameObject crosshair;
  public GameObject crosshair_hit;
  public GameObject automatic;
  public GameObject pistol;

	// Use this for initialization
	void Start () {
		health = MAX_HEALTH;
    flag.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (health <= 0) {
			loseFlag ();
			Destroy (this.gameObject);
		}
	}

  public void reset()
  {
    health = MAX_HEALTH;
    flag.SetActive(false);
    switchWeapon(WeaponType.pistol);
  }

	public void loseFlag(){
    if (weapon.type != WeaponType.flag)
    {
      return;
    }

		flag.SetActive (false);
		switchWeapon (WeaponType.unarmed);
		GameObject obj = Instantiate (flagPickup, transform.position, Quaternion.identity) as GameObject;
		obj.GetComponent<PickupBox> ().pickupType = PickupBox.PickupType.flag;
		GameObject spawner = GameObject.FindGameObjectWithTag ("SpawnedPickup");
		obj.transform.parent = spawner.transform;
	}

  public void switchWeapon(WeaponType s)
  {
    Debug.Log("switching weapon to " + s);
    if (s == WeaponType.unarmed)
    {
      initUnarmed();
      pistol.SetActive(false);
      flag.SetActive(false);
    }
    else if (s == WeaponType.pistol)
    {
      initPistol();
      flag.SetActive(false);
      pistol.SetActive(true);
    }
    else if (s == WeaponType.automatic)
    {

    }
    else if (s == WeaponType.flag)
    {
      initFlag();
      pistol.SetActive(false);
      flag.SetActive(true);
    }
  }

  void initPistol()
  {
    weapon = new Weapon();
    weapon.type = WeaponType.pistol;
    weapon.recoilTime = 0.5f;
    weapon.currentRecoilTime = 0f;
    weapon.headShotDMG = 100f;
    weapon.bodyShotDMG = 25f;
    weapon.ammo = 14;
  }

  void initUnarmed()
  {
    weapon = new Weapon();
    weapon.type = WeaponType.unarmed;
    weapon.recoilTime = 0f;
    weapon.currentRecoilTime = 0f;
    weapon.headShotDMG = 50f;
    weapon.bodyShotDMG = 35f;
    weapon.ammo = 0;
  }

  void initFlag()
  {
    weapon = new Weapon();
    weapon.type = WeaponType.flag;
    weapon.recoilTime = 0f;
    weapon.currentRecoilTime = 0f;
    weapon.headShotDMG = 50f;
    weapon.bodyShotDMG = 35f;
    weapon.ammo = 0;
  }

}
