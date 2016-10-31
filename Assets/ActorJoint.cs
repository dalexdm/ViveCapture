using UnityEngine;
using System.Collections.Generic;

public class ActorJoint : MonoBehaviour
{

	public Actor actor;

	public ActorJoint parent;
	//if null, neck node
	public float boneLength;
	//distance to parent joint
	public Quaternion orientation;
	//This * forward-vector * bonelength + parent position = this.position

	public List<ActorJoint> children;
	//child joints

	public GameObject AJPrfab;
	//used for instantiating new joints
	public LineRenderer LR;
	//feature of the instantiated thing

	// Use this for initialization
	void Start ()
	{
		
	}


	public void addChild (GameObject newAJ, int index)
	{
		/*
		Vector3 newPosition = newAJ.transform.position;
		ActorJoint newAJScript = newAJ.GetComponent<ActorJoint>();

		//consult master actor to conform bone length
		//boneLengths[i] refers to the bone between joints[i-1] and joints[i]
//		newAJScript.boneLength = actor.tl.boneLengths [index];

		//if bonelength == 0, this is the first frame we've recorded
		if (newAJScript.boneLength == 0) {
			newAJScript.boneLength = Vector3.Magnitude (newPosition - this.transform.position);
			actor.tl.boneLengths [index] = newAJScript.boneLength;
		}

		//Set the initial orientation and hard position of the new joint
		this.transform.LookAt (newPosition);
		newAJScript.orientation = this.transform.localRotation;
		this.transform.localRotation = new Quaternion ();
		newAJ.transform.position = newAJScript.orientation * Vector3.forward * newAJScript.boneLength + this.transform.position;

		//set heirarchial bounds
		children.Add (newAJScript);
		newAJScript.parent = this;
		*/
	}
	
	// Update is called once per frame
	void Update ()
	{
		//update linerenderer
		if (parent) this.transform.position = orientation * Vector3.forward * boneLength + parent.transform.position;
		//check for changes to rotation and apply them to children?
	}
}
