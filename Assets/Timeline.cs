using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Timeline : MonoBehaviour {

	// Static singleton property
	public static Timeline Instance { get; private set; }

	void Awake()
	{
		// Save a reference to the AudioHandler component as our singleton instance
		Instance = this;
	}

	//Members
	public List<Skeleton> keyframes = new List<Skeleton>();
	public List<Skeleton> controlPoints = new List<Skeleton>();
	public float[] boneLengths = new float[16]; 

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 16; i++) {
			boneLengths [i] = 0;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void addKeyframe (Skeleton newKeyframe) {

		//check if a keyframe exists at this key
		Skeleton keyToReplace = null;
		foreach (Skeleton s in keyframes) {
			if (s.key == newKeyframe.key) {
				keyToReplace = s;
			}
		}

		//if so, replace it
		if (keyToReplace != null)
			keyToReplace = newKeyframe;

		//otherwise, create one
		else
			keyframes.Add(newKeyframe);

		// sort keyframes and reconstruct control points for each quat
		keyframes.Sort ();

		//converge boneLengths with more and more measurements
		for (int i = 0; i < 16; i++) {
			if (boneLengths [i] == 0)
				boneLengths [i] = newKeyframe.bones [i].boneLength;
			else
				boneLengths [i] = (boneLengths [i] + newKeyframe.bones [i].boneLength) / 2;
		}

	}

	public Skeleton getFramePose(float key) {
		if (keyframes.Count > 1) {
			int segment = getSegment (key);
			float u = (key - keyframes [segment].key) / (keyframes [segment + 1].key - keyframes [segment].key);
			u = Mathf.Max (0, Mathf.Min (1, u));
			return lerp (segment, u);
		} else if (keyframes.Count == 1) {
			return keyframes [0];
		} else {
			Debug.Log ("No frames to use");
			return null;
		}
	}

	public bool containsKey(float key) {
		foreach (Skeleton s in keyframes) {
			if (s.key == key)
				return true;
		}
		return false;
	}

	int getSegment(float key) {
		if (key <= keyframes [0].key)
			return 0;
		if (key > keyframes [keyframes.Count - 1].key)
			return keyframes.Count - 2;

		for (int i = 0; i < keyframes.Count; i++) {
			if (key > keyframes [i].key && key <= keyframes[i + 1].key)
					return i;
		}

		return 666;
	}

	Skeleton lerp (int seg, float u) {
		Skeleton output = new Skeleton (keyframes[seg]);
		for (int i = 0; i < 16; i++) {
			if (i == 12)
				output.bones [i].pos = Vector3.Lerp(keyframes[seg].bones[12].pos,keyframes[seg+1].bones[12].pos,u);
			Quaternion v1 = keyframes [seg].bones [i].orientation;
			Quaternion v2 = keyframes [seg + 1].bones [i].orientation;
			output.bones [i].orientation = Quaternion.Slerp (v1, v2, u);
			output.bones [i].boneLength = Instance.boneLengths [i];
			output.update ();
		}
		return output;
	}
}
