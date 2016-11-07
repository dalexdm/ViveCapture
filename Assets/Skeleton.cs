using UnityEngine;
using System.Collections;
using System;

public class Skeleton : IComparable<Skeleton> {

	public Bone[] bones = new Bone[16];
	public float key;

	public Skeleton () {
	
	}

	public Skeleton(float key) {
		this.key = key;
	}
		
	//deep copy
	public Skeleton(Skeleton toCopy) {
		this.bones [12] = new Bone (toCopy.bones [12], this);
		this.key = toCopy.key;
	}

	public int CompareTo(Skeleton compareSkeleton) {
		return key.CompareTo (compareSkeleton.key);
	}

	public void addJointLeft (int leftIndex)
	{
		Bone b2 = bones [leftIndex];

		//arm
		if (leftIndex < 3) {
			//elbow, wrist
			if (leftIndex != 0)
				bones [leftIndex - 1].addChild (b2);
			//shoulder
			else if (bones [12] != null)
				bones [12].addChild (b2);

			//leg
		} else {
			//elbow, wrist
			if (leftIndex != 3)
				bones [leftIndex - 1].addChild (b2);
			//shoulder
			else if (bones [14] != null)
				bones [14].addChild (b2);
		}
	}

	public void addJointRight (int rightIndex)
	{
		Bone b2 = bones [rightIndex + 6];

		//arm
		if (rightIndex < 3) {
			//elbow, wrist
			if (rightIndex != 0)
				bones [rightIndex + 5].addChild (b2);
			//shoulder
			else if (bones [12] != null)
				bones [12].addChild (b2);

			//leg
		} else {
			//elbow, wrist
			if (rightIndex != 3)
				bones [rightIndex + 5].addChild (b2);
			//shoulder
			else if (bones [14] != null)
				bones [14].addChild (b2);
		}
	}

	public void addJointCenter (int centerIndex)
	{

		//neck
		if (centerIndex == 0) {
			if (bones [0] != null)
				bones [12].addChild (bones [0]);
			if (bones [6] != null)
				bones [12].addChild (bones [6]);
		}
		if (centerIndex == 1)
			bones [12].addChild (bones [13]);

		if (centerIndex == 2) {
			bones [13].addChild (bones [14]);
			if (bones [3] != null)
				bones [14].addChild (bones [3]);
			if (bones [9] != null)
				bones [14].addChild (bones [9]);
		}
	}

	public void update () {
		for (int i = 0; i < 16; i++) {
			bones[i].boneLength = Timeline.Instance.boneLengths[i];
		}
		bones [12].updatePos ();
	}
}
