

# Interactive Project
Originally due: 2/19/2018  
Author: Nicholas White

## Prompt
Come up with some project call that you have been awarded. Perhaps you got awarded the job of coming up with a system that allows people to use gestures in their home to activate/deactivate various appliances/lights/etc. Or perhaps you got awarded the job to create a system that tracks and understands museum vistor movements in a space and triggers various sounds, visuals, and/or something else in an interactive installation. Or perhaps you were awarded a project tasked with building an interface for a new musical instrument, or maybe a game project, or.... (fill in the blank). This project could take many forms, but needs to include realtime machine learning trained with one or more sensors, and some sort of output. Be as detailed as possible about the project call, what is the setting, what the input should be, what the output should be, etc. Don't start thinking about the tech first, think about the thing you want to create first. After that, then think about what tools are needed for building the project.


## The Process

This Unity Scene is built on top of [this UnityOSC example](http://thomasfredericks.github.io/UnityOSC/)


### User Testing Phase 1

![User testing with a wiimote to create a cube in Unity](https://github.com/artintelclass/interactive-project-njw275/blob/master/GIFs/user-testing-wiimote.gif)
> Here, in early user testing, a wiimote is used to create a simple cube in Unity

![User testing with a wiimote where one gesture creates too many objects](https://github.com/artintelclass/interactive-project-njw275/blob/master/GIFs/too-many-cubes-usertesting.gif)
> Here, the user uses one gesture, yet multiple objects were created. 

### Adding the Trigger to get the gesture

First, I edited the script from SteamVR to change a boolean in my script. The boolean will allow me to know when the trigger is pressed on the controller

The **SteamVR Script** I edited is located in the SteamVR package from the Unity Asset Store:

SteamVR -> Extras -> SteamVR_TrackedController.cs

![Steam_VR Code to change the triggerPressed boolean](https://github.com/artintelclass/interactive-project-njw275/blob/master/Images/FromSteamVR.png)

> Below are the lines I added when the triggered is pressed and when it is released, respectively, to the Update function shown above.

```C#
gameObject.GetComponent<SendPositionOnUpdate> ().triggerPressed = true;
GameObject.Find ("Osc").GetComponent<ReceivePosition> ().triggerPressed = true; 
```

```C#
gameObject.GetComponent<SendPositionOnUpdate> ().triggerPressed = false;
GameObject.Find ("Osc").GetComponent<ReceivePosition> ().triggerPressed = false;
```

The triggerPressed boolean in the [SendPositionOnUpdate script](https://github.com/artintelclass/interactive-project-njw275/blob/master/Assets/SendPositionOnUpdate.cs), is used to know when a user is making a gesture. When the trigger is pressed, distanceX, distanceY, and distanceZ are calculated, added to the OSC message, and sent to Wekinator. Below, you can see this process. 

**Note:** distanceX is the x position of the controller the script is attached to calculated in relation to the headset, that way no matter where the user is in 3D space, the gesture is the same. The equivalent calculations are done for distanceY and distanceZ as well. 

```C#
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
```

### What are the gestures?
While holding the trigger:  
Pull from the left side of your body across your chest to the right side of your body == instantiate a sphere  
Pull from the right side of your body straight down == instantiate a cube

**Note:** the prefabs that are created by the gestures are simple prefabs I put into the assets folder. On the other hand, these prefabs can be anything created by the user in the assets folder. 

![Showing how to change the prefabs made on a gesture](https://github.com/artintelclass/interactive-project-njw275/blob/master/Images/prefabs.png)
>Take a prefab from your Assets folder and add it to the Receive Position script on the OSC empty game object

Next, in the [ReceivePosition script](https://github.com/artintelclass/interactive-project-njw275/blob/master/Assets/ReceivePosition.cs) Unity is receiving back data from Wekinator. Right now, the Wekinator project is set up to evaluate the data and make an estimate on one of three gestures. I put the float values that are retrieved from Wekinator into an array and sorted the array. That way, the first value (inputs[0]) will be the gesture Wekinator thinks is a match. Before setting one of the gestures to true, I set them all to false. This step is crutial as it makes sure only one object is made.

```C#
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
```

After setting all the triggers to false, we check which gesture is a match and set that trigger to true:

```C#
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
```

Finally, in the Update() function in the ReceivePosition script when triggeredPressed is false (aka the user has stopped making a gesture) we check to see which gesture trigger was set to true. Upon finding which gesture was made, a prefab is instantiated. The location of the prefab is based upon the location of the left controller. So, the gesture is done with the right controller and the prefab is then made wherever the left controller is located at the time the gesture ends (seen below).

```
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
```

After ending the current session in Unity, the objects made will disappear. I added debug log statements to accomdate for this fact. Now, when an object is made, a log statement will print out what prefab was made and its exact location. 

### User Testing Phase 2

![user testing phase 2 - making a cube on the HTC Vive](https://github.com/artintelclass/interactive-project-njw275/blob/master/GIFs/user-testing2-cubes.gif)
>User testing on the HTC Vive - making a cube
![user testing phase 2 - making a sphere on the HTC Vive](https://github.com/artintelclass/interactive-project-njw275/blob/master/GIFs/user-testing2-spheres.gif)
>User testing on the HTC Vive - making a sphere

On the HTC Vive, user testing went well. The users were able to recreate the gestures and make the simple shapes. Although, through testing it was found that at a rapid speed, the model wasnt trained enough so it had trouble with quick gestures. To account for this, I continued to train the model, adding more quick, short gestures to the training set. 

## Next Steps

In future iterations, I would like to take the log statements and make them into lines written into a file. After that, I could add a UI button in the VR space that, when clicked, would read the file and create objects based on the information in the file. That way, users could put objects they previously made into the scene they are currently working in. 

Another step is to add more gestures, right now only two are supported but there could be many more added.




