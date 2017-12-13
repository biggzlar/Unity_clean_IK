# Unity_clean_IK
This is a barebones implementation of an IK solver, based on a Unity Example for Blendtrees. It includes repositioning and rerotation of the feet, as well as a collider displacing method, that pushes the character controller's collider upwards when there is no other way to ground both feet.

**The repo is rather unorganized, so focus on the centerpiece ```Assets/Scripts/CleanIK.cs```!**

### Setup:
  1. Set up your scene and characters (make sure the characters possess a character controller)
  2. Add the CleanIK script to your characters
  3. Assign the feet transforms respectively
  4. Test the scene and adjust foot offset
  5. Enable ik and rotation
  


![Alt text](gitPreview.png?raw=true "Clean IK Preview")
