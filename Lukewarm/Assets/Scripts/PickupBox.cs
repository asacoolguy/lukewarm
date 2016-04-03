using UnityEngine;
using System.Collections;

public class PickupBox : MonoBehaviour {
	public enum PickupType{
		pistol,
		automatic,
		flag
	};

	public Color pistolColor = Color.blue;
	public Color automaticColor = Color.red;
	public Color flagColor = Color.yellow;

	public PickupType pickupType = PickupType.pistol;

	public float rotationSpeed = 15f;

	// Use this for initialization
	void Start () {
    transform.localEulerAngles = new Vector3(45, 45, 45);

    if (pickupType == PickupType.pistol)
    {
      GetComponent<Renderer>().material.color = pistolColor;
    }
    else if (pickupType == PickupType.automatic)
    {
      GetComponent<Renderer>().material.color = automaticColor;
    }
    else if (pickupType == PickupType.flag)
    {
      GetComponent<Renderer>().material.color = flagColor;
    }
	}
	
	// Update is called once per frame
	void Update () {

		transform.Rotate (new Vector3 (0, rotationSpeed * Time.deltaTime, 0));
	}
}
