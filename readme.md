

# Interactive-Project
This project is due Monday, February 19th.

## Prompt
Come up with some project call that you have been awarded. Perhaps you got awarded the job of coming up with a system that allows people to use gestures in their home to activate/deactivate various appliances/lights/etc. Or perhaps you got awarded the job to create a system that tracks and understands museum vistor movements in a space and triggers various sounds, visuals, and/or something else in an interactive installation. Or perhaps you were awarded a project tasked with building an interface for a new musical instrument, or maybe a game project, or.... (fill in the blank). This project could take many forms, but needs to include realtime machine learning trained with one or more sensors, and some sort of output. Be as detailed as possible about the project call, what is the setting, what the input should be, what the output should be, etc. Don't start thinking about the tech first, think about the thing you want to create first. After that, then think about what tools are needed for building the project.


## The Process

This Unity Scene is based off of [this UnityOSC example](http://thomasfredericks.github.io/UnityOSC/)


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

The triggerPressed boolean in the [SendPositionOnUpdate script](!!), is used to know when a user is making a gesture. When the trigger is pressed, distanceX, distanceY, and distanceZ are calculated, added to the OSC message, and sent to Wekinator. Below, you can see this process. 

**Note:** distanceX is x position of the controller the script is attached to calculated in relation to the headset, that way no matter where the user is in 3D space, the gesture is the same. The equivalent calculations are done for distanceY and distanceZ as well. 

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



## Expectations
This project is one of the 4 major projects due during the semester and should be much more developed and polished than the weekly exercises. Having said that, it doesn't have to be an enterprise solution, it can certainly be at a proof of concept stage. As mentioned above, you have to use some sort of machine learning as a processing step between your input and your ouput. However, that doesn't have to be limited to Wekinator strictly. You are free to use other platforms if you prefer. If you have questions about what I will consider as "Machine Learning", please ask. 

The documentation for this project should be more much detailed as well. Take videos/pictures every step of development, so you can tell a story later. You're documentation should include whatever other informative information you can provide as well, like sketches, text, diagrams, etc. We will do user testing in class. Your documentation should address your user testing feedback. You don't have to accept all user testing feedback, but you should explain why you did not. Your original proposal should be included in your documentation as well.

Push all code and assets needed to run your project to your repo. You should also include detailed instructions on how to set up and operate your project.

## Deadlines
* Idea Proposal - Due Feb. 5th
* Working Prototype - Due Feb. 14th
* ***Finished Version - Due Feb. 19th*** 

## Grading Rubric
* 20% Idea Proposal - Due Feb. 5th
* 20% Working Prototype - Due Feb. 14th
* 20% Creativity
* 20% Clear Interaction - it's very clear to a user what they are doing, and how they're doing it
* 20% Consistent - the system works consistently
