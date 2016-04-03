using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerPickup : MonoBehaviour {
	public List<GameObject> triggeringObj;
	ShootingControl shootingControl;
	PlayerStats playerStats;
  bool doPickup;

	// Use this for initialization
	void Start () {
		triggeringObj = new List<GameObject> ();
		shootingControl = GetComponent<ShootingControl> ();
		playerStats = GetComponent<PlayerStats> ();
    doPickup = false;
	}
	
	// Update is called once per frame
	void Update () {
    if (Input.GetButtonDown("Pickup"))
    {
      doPickup = true;
    }
  }

  void FixedUpdate() {
		// clear Triggering obj of missing obj
		if (triggeringObj.Count > 0) {
			for (int i = triggeringObj.Count - 1; i > -1; i--) {
				if (triggeringObj [i] == null) {
					triggeringObj.RemoveAt (i);
				}
			}
		}

		if (doPickup) {
			foreach (GameObject obj in triggeringObj) {
				if (obj) {
					float angle = Vector3.Angle (obj.transform.position - transform.position, shootingControl.eyelevel.transform.forward);

					if (obj.tag == "Pickup" && angle < 90) {
						PickupBox pickup = obj.GetComponent<PickupBox> ();
						if (pickup.pickupType == PickupBox.PickupType.flag) {
							//playerStats.getFlag();
						} else if (pickup.pickupType == PickupBox.PickupType.pistol) {
							shootingControl.switchWeapon ("pistol");
						} else if (pickup.pickupType == PickupBox.PickupType.automatic) {
							shootingControl.switchWeapon ("automatic");
						}

						Destroy (obj);
					}
				}
			}
		}

    doPickup = false;
	}

	void OnTriggerEnter(Collider c){
		triggeringObj.Add (c.gameObject);
	}

	void OnTriggerExit(Collider c){
		triggeringObj.Remove (c.gameObject);
	}
}
