using UnityEngine;
using System.Collections;

public class ActionSet {

	public Action[] tape;
	public Vector3 startPosition;
	public Quaternion startDirection;
	public Quaternion startViewDirection;
  public PlayerStats.WeaponType weapon;
  public PlayerStats.Player player;

	// models the actions taken by a player in a single frame
	public struct Action {
		public bool isJump;
		public bool isFire;
		public bool isSprint;
		public bool isPickup;
		public Vector2 movement;
		public Quaternion playerForward, viewForward;
		// TODO: update with mouse look.  Will it be another vec2 for mouse movement in each frame, or simply the direction it is firing in?
	}

	public ActionSet() {
		tape = new Action[0];
		startPosition = new Vector3 ();
		startDirection = Quaternion.identity;;
		startViewDirection = Quaternion.identity;;
    weapon = PlayerStats.WeaponType.unarmed;
    player = PlayerStats.Player.one;
	}

  public ActionSet(int tapeLength, Vector3 startPosition, Quaternion startDirection, Quaternion startViewDirection, PlayerStats.WeaponType weapon, PlayerStats.Player player)
  {
		this.tape = new Action[tapeLength];
		this.startPosition = startPosition;
		this.startDirection = startDirection;
		this.startViewDirection = startViewDirection;
    this.weapon = weapon;
    this.player = player;
	}

	public void add (int index, Action action) {
		tape [index] = action;
	}

	public int length () {
		return tape.Length;
	}

	public Action get (int index) {
		return tape [index];
	}
}
