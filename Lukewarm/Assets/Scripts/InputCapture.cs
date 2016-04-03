using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputCapture : MonoBehaviour {
	public int TimeLimit = 5;
	public int updatesPerSecond = 32;
	public GameObject player;
	public GameObject playerView;


	// switch to private after development
	public int ticker;
  public bool capturing;
  public bool doJump, doFire, doSprint, doPickup;

	public List<ActionSet> actions;
	ActionSet currentActionSet;

	// Use this for initialization
	void Start () {
		ticker = 0;
		capturing = false;
		actions = new List<ActionSet> ();
	}

	public void startCapture() {
		if (capturing)
			return;
		Debug.Log ("start recording");
		ticker = 0;
		currentActionSet = new ActionSet (
			TimeLimit * updatesPerSecond, 
			player.transform.position, 
			player.transform.localRotation, 
			playerView.transform.localRotation, 
			player.GetComponent<PlayerStats>().weapon.type,
      player.GetComponent<PlayerStats>().player);
		capturing = true;
	}

	public void endCapture() {
		if (!capturing)
			return;
		Debug.Log ("stopped recording");
		capturing = false;
		actions.Add (currentActionSet);
		currentActionSet = null;
	}

  void Update()
  {
    if (capturing)
    {
      if (Input.GetButtonDown("Jump"))
      {
        doJump = true;
      }
      if (Input.GetButtonDown("Fire"))
      {
        doFire = true;
      }
      if (Input.GetButtonDown("Sprint"))
      {
        doSprint = true;
      }
      if (Input.GetButtonDown("Pickup"))
      {
        doPickup = true;
      }
    }
  }

	// Update is called once per frame
	void FixedUpdate () {
		if (capturing) {
			ActionSet.Action act = new ActionSet.Action ();
			act.movement = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
			act.playerForward = player.transform.localRotation;
			act.viewForward = playerView.transform.localRotation;
			if (doJump) {
				act.isJump = true;
			}
			if (doFire) {
				act.isFire = true;
			}
			if (doSprint) {
				act.isSprint = true;
			}
			if (doPickup) {
				act.isPickup = true;
			}

      doJump = doFire = doSprint = doPickup = false;

			if (ticker < currentActionSet.length()) {
				currentActionSet.add (ticker, act);
				ticker++;
			} else {
				//TODO: remove this.
				endCapture ();
			}
		}
	}
}
