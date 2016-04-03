using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaybackController : MonoBehaviour {
	public GameObject Monitor;
	public GameObject PlayerDummy;
  	public List<GameObject> dummies;

	// Use this for initialization
	void Start () {
    dummies = new List<GameObject>();
	}

  	public void startPlayback()
  	{
	    dummies = new List<GameObject>();
		List<ActionSet> actions = Monitor.GetComponent<InputCapture> ().actions;

		foreach( ActionSet actionSet in actions) {
			// TODO: customize for each player (arguments 2 and 3).
			GameObject dummy = (GameObject) Instantiate (PlayerDummy, actionSet.startPosition, Quaternion.identity);
			dummy.GetComponent<ReplayDummy> ().player.transform.localRotation = actionSet.startDirection;
			dummy.GetComponent<ReplayDummy> ().view.transform.localRotation = actionSet.startViewDirection;
			dummy.GetComponent<ReplayDummy> ().setActionSet (actionSet);
	      dummies.Add(dummy);
		}
	}

  	public void endPlayback()
  {
    foreach (GameObject dummy in dummies)
    {
      Destroy(dummy);
    }
  }

	// Update is called once per frame
	void Update () {
		
	}
}
