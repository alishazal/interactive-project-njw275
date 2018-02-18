using UnityEngine;
//using System.Collections;
using System;

public class ReceivePosition : MonoBehaviour {

  public OSC osc;

  //See if trigger is pressed
	public bool triggerPressed = false;

  //gesture booleans that are set if Wekinator recognizes a gesture
	public bool gesture1Triggered = false;
	public bool gesture2Triggered = false;
	public bool gesture3Triggered = false;

  //Prefabs that users instantiate with the gestures
	public GameObject cubePrefab;
	public GameObject spherePrefab;

	// Use this for initialization
	void Start () {

       osc.SetAddressHandler("/wek/outputs", OnReceiveMessage);

  }

	// Update is called once per frame
	void Update () {


    //The left controller
		GameObject obj_location  = GameObject.Find ("Controller (left)");

    //If the user is done with their gesture
		if (!triggerPressed) {

      //If Wekinator identified the gesture as "Gesture 1"
			if (gesture1Triggered) {

        /* 1. Create a cube prefab at the location of the left controller
           2. Set all gesture triggers to false
           3. Log out where the cube was placed (for user's to create one outside of "play" mode)
        */
				Instantiate (cubePrefab, new Vector3(obj_location.transform.position.x,obj_location.transform.position.y,obj_location.transform.position.z), Quaternion.identity);
				gesture1Triggered = false;
				gesture2Triggered = false;
				Debug.Log ("*** New cube prefab made at location x: " + obj_location.transform.position.x + " y: " + obj_location.transform.position.y + " z: " + obj_location.transform.position.z + " ***");

      }
      //If Wekinator identified the gesture as "Gesture 2"
      else if (gesture2Triggered) {
        /* 1. Create a sphere prefab at the location of the left controller
           2. Set all gesture triggers to false
           3. Log out where the sphere was placed (for user's to create one outside of "play" mode)
        */
				Instantiate (spherePrefab, new Vector3(obj_location.transform.position.x,obj_location.transform.position.y,obj_location.transform.position.z), Quaternion.identity);
				gesture2Triggered = false;
				gesture1Triggered = false;
				Debug.Log ("*** New sphere prefab made at location x: " + obj_location.transform.position.x + " y: " + obj_location.transform.position.y + " z: " + obj_location.transform.position.z + " ***");

      }
      /* If Wekinator identified the gesture as "Gesture 3"
      not used but can be set up for a third gesture */
      else if (gesture3Triggered) {

				gesture3Triggered = false;

			}
		}

	}



  void OnReceiveMessage(OscMessage message) {
		//Debug.Log ("OSC MESSAGE IS ----> " + message);

    //Get the values for comparison to gesture groups

		float gesture1 = message.GetFloat(0); //square
		float gesture2 = message.GetFloat(1); //circle
		float gesture3 = message.GetFloat(2);

    //Put the values in an array and sort them
    //The value closest to 0 is what Wekinator believes is the gesture
    // (so compare to inputs[0] [the smallest of the sorted values])
		float[] inputs = {gesture1,gesture2,gesture3};
		Array.Sort (inputs);

    //Set them all to false first -- to make sure you dont make multiple objects at the same time
		gesture1Triggered = false;
		gesture2Triggered = false;
		gesture3Triggered = false;


		if (triggerPressed) {
			//If gesture1 is the smallest value
			if (inputs [0] == gesture1) {
			     gesture1Triggered = true;
			}
      //If gesture2 is the smallest value
      else if (inputs [0] == gesture2) {
			     gesture2Triggered = true;
			}
    //If gesture3 is the smallest value [not used]
      else if (inputs [0] == gesture3) {
			     gesture3Triggered = true;
			}
		}

  }

}
