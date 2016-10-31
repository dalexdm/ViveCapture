using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PrimitiveTimeline : MonoBehaviour {

	public List<Actor> actors = new List<Actor> (); //Data structure for storing keys, 
													//displayed during their own creation

	public Actor mainActor; //Actor used to display interpolated points

	public Actor tempActor;

	public GameObject actorPrefab;

	public int key;

	public Slider slider;

	public float[] boneLengths = new float[15];
	/*
	// Use this for initialization
	void Start () {
		setKey ();
		mainActor.gameObject.SetActive (false);
	}

	//uses existing poses to pose the mainactor
	public void pose () {
		for (int i = 0; i < 15; i++) {
			mainActor.joints [i].orientation = new Quaternion ();
		}
	}
	/*
	//necessary changes when time is switched
	public void setKey() {
		

		this.key = (int) slider.value;

		//make mainActor assume a pose if we have anything
		if (actors.Count > 0)
			key = key;
			pose ();

		//instantiate a new actor awaiting input
		if (tempActor) Destroy(tempActor.gameObject);
		tempActor = ((GameObject) Instantiate(actorPrefab)).GetComponent<Actor>();
		tempActor.tl = this;
		tempActor.key = key;
	}

	public void addActor(Actor toAdd) {

		Actor a2 = new Actor (toAdd);
		mainActor.gameObject.SetActive (false);

		Actor toReplace = null;
		foreach (Actor a in actors) {
			if (a.key == a2.key)
				toReplace = a;
		}
		if (toReplace)
			toReplace = a2;
		else
			actors.Add (a2);

		a2.isAdded = true;
		//sort actors by time?
	}

	public void interpolateSimple(float time) {
		
	}

	public void hideActor () {
		mainActor.gameObject.SetActive(false);
	}

	/*
	void computeControlPoints(Quaternion startQuat, Quaternion endQuat)
	{
		// startQuat is a phantom point at the left-most side of the spline
		// endQuat is a phantom point at the left-most side of the spline

		mCtrlPoints.clear();
		int nuactors = actors.Length ();
		if (nuactors <= 1) return;

		Quaternion b0, b1, b2, b3;
		Quate q0p, q0a, q1p, q1a;

		for (int segment = 0; segment < nuactors-1; segment++)
		{
			// DONE: student implementation goes here
			//  Given the quaternion keys q_1, q0, q1 and q2 associated with a curve segment, compute b0, b1, b2, b3 
			//  for each cubic quaternion curve, then store the results in mCntrlPoints in same the same way 
			//  as was used with the SplineVec implementation
			//  Hint: use the SDouble, SBisect and Slerp to compute b1 and b2

			b0 = actors[segment].joints[].orientation;
			b3 = actors[segment + 1].joints[].orientation;
			//clamp end
			if (segment == nuactors - 2) q0p = quat::SDouble(actors[segment + 1].orientation, actors[segment + 1].orientation);
			else q0p = quat::SDouble(actors[segment+2].orientation, actors[segment+1].orientation);
			q0a = quat::SBisect(actors[segment].joints[].orientation, q0p);
			//clamp beginning
			if (segment == 0) q1p = quat::SDouble(actors[segment].joints[].orientation, actors[segment].joints[].orientation);
			else q1p = quat::SDouble(actors[segment - 1].orientation, actors[segment].joints[].orientation);
			q1a = quat::SBisect(q1p, actors[segment+1].orientation);
			//calculate control points
			b1 = quat::Slerp(b0, q1a, 1.0/3.0);
			b2 = quat::Slerp(b3, q0a, 1.0/3.0);

			mCtrlPoints.push_back(b0);
			mCtrlPoints.push_back(b1);
			mCtrlPoints.push_back(b2);
			mCtrlPoints.push_back(b3);
		}
	}

	quat quat::Slerp(const quat& q0, const quat& q1, double u)
	{
		//DONE: student implemetation of Slerp goes here
		return (q0 + u * (q1 - q0)).Normalize();
	}
	quat quat::SDouble(const quat& a, const quat& b)
	{
		//DONE: student implementation ofSDouble goes here
		quat q = 2 * quat::Dot(a, b) * b - a;
		return q.Normalize();
	}

	quat quat::SBisect(const quat& a, const quat& b)
	{
		//DONE: student implementation of SBisect goes here
		quat q = (a + b);
		return q.Normalize();
	}


	quat quat::Scubic(const quat& b0, const quat& b1, const quat& b2, const quat& b3, double u)
	{
		quat result = b0;
		quat b01, b11, b21, b02, b12, b03;
		// TODO: Return the result of Scubic based on the cubic quaternion curve control points b0, b1, b2 and b3

		b01 = Slerp(b0,b1,u);
		b11 = Slerp(b1, b2, u);
		b21 = Slerp(b2, b3, u);
		b02 = Slerp(b01, b11, u);
		b12 = Slerp(b11, b21, u);
		result = Slerp(b02,b12,u);

		return result.Normalize(); // result should be of unit length
	}
	
	// Update is called once per frame
	void Update () {
	
	}*/
}
