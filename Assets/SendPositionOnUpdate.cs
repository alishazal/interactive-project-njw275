using UnityEngine;
using System.Collections;

public class SendPositionOnUpdate : MonoBehaviour {

	public OSC osc;

	//Boolean to check if user has pulled the trigger to indicate a gesture
	public bool triggerPressed = false;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	  OscMessage message = new OscMessage();

		//Find the HTC Vive Headset
		GameObject headset = GameObject.Find ("[CameraRig]/Camera (eye)");
		//Debug.Log (headset);

		//if user is making a gesture
		if (triggerPressed) {
			message.address = "/wek/inputs";
			Vector3 distanceFromHeadset;
			float distanceX = transform.position.x - headset.transform.position.x;
			float distanceY = transform.position.y - headset.transform.position.y;
			float distanceZ = transform.position.z - headset.transform.position.z;

			/*
			Here, the messages (namely: distanceX, distanceY, distanceZ)
			are values that measure the x,y,z position of the controller
			the script is attached to in RELATION TO the headset
			*/
			message.values.Add (distanceX);
			message.values.Add (distanceY);
			message.values.Add (distanceZ);

			//Send the values over OSC
			osc.Send (message);
		}


  }


}
