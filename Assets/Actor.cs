using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour {

	// 0 - 5  : left shoulder, elbow, wrist, hip, knee, foot
	// 6 - 11 : right ^
	// 12 - 14: neck, spine, pelvis

	private ActorJoint[] skeleton = new ActorJoint[15];
	private int lIndex = 0; //Joints captured by left controller
	private int rIndex = 6; //Joints captured by right controller
	private int cIndex = 12; //central joints

	SteamVR_TrackedController lvr;
	SteamVR_TrackedController rvr;

	// Use this for initialization
	void Start () {

		//identify right and left via device index

		//attach button input to method calls
		lvr.OnTriggerClicked += addLeft ();
		rvr.OnTriggerClicked += addRight ();
		lvr.OnPadClicked += addCenter ();
		rvr.OnPadClicked += addCenter ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void addLeft () {
		if (lIndex < 5) {
			ActorJoint [lIndex] = new ActorJoint (lvr.transform.position);
			lIndex++;
		}
	}

	void addRight () {
		if (lIndex < 11) {
			ActorJoint [rIndex] = new ActorJoint (lvr.transform.position);
			rIndex++;
		}
	}

	void addCenter () {
		if (lIndex < 14) {
			ActorJoint [cIndex] = new ActorJoint (lvr.transform.position);
			cIndex++;
		}
	}
}
