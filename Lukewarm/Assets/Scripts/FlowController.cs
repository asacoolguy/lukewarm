using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public class FlowController : MonoBehaviour {
	public enum Phase {
		Overview,
		Countdown,
		Turn,
		Results,
		End,
	};

	public Camera overviewCam, playerCam;
	public int countdownTime = 3;
	public int turnDuration = 30;
	public int startingLives = 3;

	public Phase currentPhase;
	public GameObject player;
	public GameObject captureController;
	public GameObject playbackController;
	public GameObject stagePrefab;
	public GameObject spawn1, spawn2;
	public GameObject countdownOverlay;
	public GameObject resultsOverlay;
	public GameObject turnOverlay;
	public GameObject pickups;

	public PlayerStats.Player currentPlayer;
	public int [] lives = new int [2];

	// set to private after
	public float countdownTicker;
	public float turnTicker;

	// Use this for initialization
	void Start () {
		lives[(int)PlayerStats.Player.one] = startingLives;
		lives[(int)PlayerStats.Player.two] = startingLives + 1;
		currentPhase = Phase.Overview;
		overviewCam.enabled = true;
		playerCam.enabled = false;
		player.GetComponent<MovementControl> ().setCanMove (false);
	}

	// starting countdown, spawn player in to environment
	// start regular timer, enable player movement, start recording
	// after 30 seconds, disable character movement and determine the state of the objective.
	// display round results, press 'space' to continue
	// swap players and repeat.
	// Update is called once per frame
	void Update () {
		if (currentPhase == Phase.Overview) {
			if (Input.GetButtonDown ("Jump")) {
				overviewCam.enabled = false;
				playerCam.enabled = true;
				countdownTicker = countdownTime;
				countdownOverlay.SetActive (true);
				currentPhase = Phase.Countdown;
				// Spawns the character in
				player.GetComponent<PlayerStats>().switchWeapon(PlayerStats.WeaponType.unarmed);
				if (currentPlayer == PlayerStats.Player.one)
				{
					player.GetComponent<PlayerStats>().player = PlayerStats.Player.one;
					player.transform.position = spawn1.transform.position;
				}
				else
				{
					player.GetComponent<PlayerStats>().player = PlayerStats.Player.two;
					player.transform.position = spawn2.transform.position;
				}
			}
		} else if (currentPhase == Phase.Countdown) {
			// decrement the countdown timer
			// take action if it reaches 0
			if (countdownTicker > 0)
			{
				// TODO: animate a better timer.
				countdownOverlay.GetComponent<Text> ().text = ((int)countdownTicker + 1) + "";
				countdownTicker -= Time.deltaTime;
			} else {
				// countdown is finished.
				countdownOverlay.SetActive (false);
				captureController.GetComponent<InputCapture> ().startCapture ();
				player.GetComponent<MovementControl> ().setCanMove (true);
				turnTicker = turnDuration;
				currentPhase = Phase.Turn;
				// start the playback
				playbackController.GetComponent<PlaybackController>().startPlayback();
				turnOverlay.SetActive (true);
			}
		} else if (currentPhase == Phase.Turn) {
			// decrement the turn counter
			// take action if it reaches 0
			if (turnTicker > 0) {
				turnOverlay.GetComponent<Text> ().text = turnTicker + "";
				turnTicker -= Time.deltaTime;
				// TODO: animate a timer.
			} else {
				// Turn over.
				turnOverlay.SetActive (false);
				overviewCam.enabled = true;
				playerCam.enabled = false;
				captureController.GetComponent<InputCapture> ().endCapture ();
				player.GetComponent<MovementControl> ().setCanMove (false);
				player.transform.position = new Vector3(0, 0, -99);

				// TODO: figure out results & display them as an overlay
				List<GameObject> allPlayers = playbackController.GetComponent<PlaybackController>().dummies;
				allPlayers.Add (player);
				countScore(allPlayers);
				print ("turn over happened");
				// TODO: reset the stage's pickups
				resetPickups();
				resultsOverlay.SetActive (true);
				resultsOverlay.GetComponent<Text>().text = "Player 1: " + lives[(int)PlayerStats.Player.one] + "  Player 2: " + lives[(int)PlayerStats.Player.two];
				playbackController.GetComponent<PlaybackController>().startPlayback();

				// TODO: if the game has ended, go to Phase.End instead of Phase.Results
				print("pre-lives check");
				if (lives [(int)PlayerStats.Player.one] <= 0 || lives [(int)PlayerStats.Player.two] <= 0) {
					currentPhase = Phase.End;
					print ("end it");
				} else {
					currentPhase = Phase.Results;
				}

			}
		} else if (currentPhase == Phase.Results) {
			if (Input.GetButtonDown ("Jump")) {
				// end the playback that's occuring on the summary screen.
				playbackController.GetComponent<PlaybackController> ().endPlayback ();
				overviewCam.enabled = false;
				playerCam.enabled = true;
				countdownTicker = countdownTime;
				resultsOverlay.SetActive (false);
				if (currentPlayer == PlayerStats.Player.one) {
					currentPlayer = PlayerStats.Player.two;
				} else {
					currentPlayer = PlayerStats.Player.one;
				}

				// TODO: reset the stage's pickups
				resetPickups();
				countdownOverlay.SetActive (true);
				player.GetComponent<PlayerStats> ().reset ();

				if (currentPlayer == PlayerStats.Player.one) {
					player.transform.position = spawn1.transform.position;
					player.GetComponent<PlayerStats> ().player = PlayerStats.Player.one;
				} else {
					player.transform.position = spawn2.transform.position;
					player.GetComponent<PlayerStats> ().player = PlayerStats.Player.two;
				}
				currentPhase = Phase.Countdown;
			}
		} else if (currentPhase == Phase.End) {
			// Display an endgame thing
			string winText;
			if (lives [(int)PlayerStats.Player.one] <= 0) {
				// player 2 wins
				winText = "Player 1: " + lives[(int)PlayerStats.Player.one] + "  Player 2: " + lives[(int)PlayerStats.Player.two] + "\nPlayer 2 wins! Nicely done!";
			} else {
				// player 1 wins
				winText = "Player 1: " + lives[(int)PlayerStats.Player.one] + "  Player 2: " + lives[(int)PlayerStats.Player.two] + "\nPlayer 1 wins! Wicked sick!";
			}

			resultsOverlay.SetActive (true);
			resultsOverlay.GetComponent<Text> ().text = winText;
		}
	}

	// takes in a list of all the player objects, deal with the scores accordingly
	void countScore(List<GameObject> players){
		foreach (GameObject obj in players) {
			if (obj) {
				PlayerStats player = obj.GetComponent<PlayerStats> ();
				if (player.flag.activeInHierarchy) {
					if (player.player == PlayerStats.Player.one) {
						lives [(int)PlayerStats.Player.two] -= 1;
					} else {
						lives [(int)PlayerStats.Player.one] -= 1;
					}
				}
			}
		}
	}

	// reactivates all pickup objs in the scene
	void resetPickups(){
		print ("pickups reset");
		foreach (Transform child in pickups.transform) {
			child.gameObject.SetActive (true);
		}

		foreach (Transform child in GameObject.FindGameObjectWithTag("SpawnedPickup").transform) {
			if (child) {
				Destroy (child.gameObject);
			}
		}
	}
	/*
	 * void resetPickups(){
		print ("pickups reset");
		GameObject p = Instantiate (pickups, Vector3.zero, Quaternion.identity) as GameObject;
		GameObject stage = GameObject.FindGameObjectWithTag ("Stage").gameObject;
		p.transform.parent = stage.transform;
		p.transform.localScale = new Vector3 (2, 2, 2);
	}*/
}