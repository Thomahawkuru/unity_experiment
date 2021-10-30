# unity_experiment
Unity project for gesture based teloepration of the panda_experiment package:  
https://github.com/Thomahawkuru/panda_experiment

# Contents
The unity project consists of the following scenes with their respective objects and scripts::  
  
Startup: 
- Experiment Manager 	(Experiment Manager)
- Varjo Leap Rig 		(Varjo Manager, Varjo Mixed Reality) 
  - VarjoCamera 	(Camera, Leap XR Service Provider)
- Directional Light 		(Light)
Calibration
- Varjo Leap Rig		(Varjo Manager, Varjo Mixed Reality)
 - VarjoCamera 	(Camera, Leap XR Service Provider)
 - HandModels	(Hand Model Manager)
- Data Logger		(Request Calibration, Logger)
- Directional Light 		(Light)
Scene1 (separated condition)
- RosConnector		(RosConnector, 6 Data subscribers, Velcmd Publisher) 		
- Varjo Leap Rig		(Varjo Manager, Varjo Mixed Reality)
 - VarjoCamera	(Camera, Leap XR Service Provider)
- Data Logger		(Request Calibration, Logger)
- Screen			(Mesh renderer)
- Directional Light 		(Light)
Scene2 (situated condition)
- RosConnector		(RosConnector, 6 Data subscribers, Velcmd Publisher)
- Varjo Leap Rig		(Varjo Manager, Varjo Mixed Reality)
 - VarjoCamera	(Camera, Leap XR Service Provider)
- Data Logger		(Request Calibration, Logger)
- Screen			(Mesh renderer)
- Directional Light 		(Light)
The end
- VarjoCamera		(Camera, Leap XR Service Provider)
- Directional Light 		(Light)

### Experiment Manager Script
A script that was developed for this study. Assigns participant number and creates respective data folder. It loads the next/previous scene when the right/left arrow key is pressed by the experimenter. The experiment manager itself is not destroyed on load but kept alive with every new scene. When the participant number is odd the scene order is:

Startup > Calibration > Scene 1 > The End > Calibration > Scene 2 > The End

When the participant number is even, the scene order is

Startup > Calibration > Scene 2 > The End > Calibration > Scene 1 > The End

### Varjo Manager Script
A script that was developed by Varjo and included in their plugin for the Varjo XR-1. Default configuration

### Varjo Mixed Reality Script
A script that was developed by Varjo and included in their plugin for the Varjo XR-1. Default configuration, with ‘Video See Through’ Enabled.

### Camera Script
A script that was developed by Varjo and included in their plugin for the Varjo XR-1. Default configuration

### Leap XR Service Provider Script
A script developed by Ultra leap and included in their plugin for the Leap Motion Controller. The device is configured as ‘Head-mounted A’, connected to the VarjoCamera with “manual head offset’:
-	Device offset y-axis 	=	0.05m
-	Device offset z-axis 	= 	0.14m
-	Device tilt		= 	24deg

### Light Script
A standard script for a directional overhead light source, taken from a Unity Sample scene. Default configuration.

### Hand Model Manager Script
A script developed by Ultra leap and included in their plugin for the Leap Motion Controller Default Configuration. The script is only added to the calibration scene. It shows virtual hands based on the motion tracking data of the leap motion controller. The experimenter then visually determined if motion tracking was accurate compared to the actual hands of the participant. 

### Request Calibration Script
A script that was developed by Varjo and included as an example in their plugin for the Varjo XR-1. It runs the eye-tracking calibration. The spacebar was configured as a calibration request key. The application button on the headset was disabled. ‘Use calibration parameters’ was turned with ‘Calibration Type’ = LEGACY and ‘Output Filter Type’ = STANDARD.

### Ros Connector Script
A script that was developed by Siemens in their ROS-sharp project. The script opens the ROS ridge client over LAN at ‘Ros Bridge Server Url’ = ws://172.0.0.1:9090, with ‘Serializer’ = Microsoft, ‘Protocol’ = Web Socket Sharp and ‘Seconds Timeout’ = 10.

### Logger Script 
A script that was developed for this study, adapted from an example provided by Varjo in their plugin. “One Gaze data per Frame’ was turned on as well as auto-logging after successful calibration. The key “L” on the keyboard was configured to manually stop/start the logging. The script logs hand position data, HMD position and rotation data, eye-tracking data, and data from the six data subscribers.

### 6 Data Subscribers Script
Six scripts were developed for this study, based on examples from the ROS_sharp project by Siemens. The RosConnector object has six attached scripts that subscribe through the ROS bridge client to specific data topics published from the experiment package on the Linux PC.  

•	Image Subscriber: Subscribes to the topic /panda/camera1/image_raw/compressed and receives the camera capture from the simulated camera. The script shows the image on the virtual screen using a mesh renderer.

•	Transform Subscriber: Subscribes to the topic /panda/handTransform to receive the transform of the robot’s end-effector. The script is called by the Logger to log the received data.

•	Mole Position Subscriber: Subscribes to the topic /experiment/mole_position to receive the position at which the current target has spawned. The script is called by the Logger to log the received data.

•	Trial Number Subscriber: Subscribes to the topic /experiment/trial_number to receive the current spawn number. The script is called by the Logger to log the received data.

•	Level Number Subscriber: Subscribes the topic to the topic /experiment/level_number to receive the number of the current set of spawns. The script is called by the Logger to log the received data.

•	Whack Check Subscriber: Subscribes to /experiment/whacked to receive the boolean that indicates if the current target is hit or not. The script is called by the Logger to log the received data.

### Velcmd Publisher script
A script that was developed for this study. The full Velcmd Publisher script is given below. The script publishes velocity command messages to the servo_server topic. The script receives the transforms for both left-handed and right-handed input from Unity. The specific transform is the index_b transform, corresponding with the distal interphalangeal joint of the index finger. If a hand is detected, the script checks whether the transforms fall within the control bounding box. For the separated condition, the bounding box is placed in front of the screen with a y-offset of 0.4. For the Situated condition, the bounding box is placed at the same location as the screen. If a hand transform falls within the bounding box, the script calculates velocity commands and publishes them to /servo_server/delta_twist_cmds. If no hand transform falls within the bounding box the script publishes empty messages. The velocity command messages consist of six values representing six degrees of freedom. All three rotations are constrained, as well as one translation that corresponds with the x-axis of the robot. Command values for the remaining two translations are calculated by calculating the velocity of the hand input in those directions.  A PID value on the difference between input position and robot position is added to the calculated input velocity. The PID is also implemented as the previously mentioned constraints, using the difference to a fixed value.

