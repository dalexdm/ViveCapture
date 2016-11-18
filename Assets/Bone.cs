using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bone {

	public Quaternion orientation;
	public Vector3 pos;
	public List<Bone> children = new List<Bone>();
	public float boneLength;
	public Bone parent;
	public int index;


	public Bone (Vector3 pos, int index) {
		this.pos = pos;
		this.index = index;
	}

	public Bone (Bone toCopy, Skeleton owner) {
		this.orientation = toCopy.orientation;
		this.pos = toCopy.pos;
		this.index = toCopy.index;
		owner.bones [index] = this;
		foreach (Bone b in toCopy.children) {
			this.children.Add (new Bone (b, owner));
		}
		foreach (Bone b in children) {
			b.parent = this;
		}
	}

	public void addChild(Bone b) {
		
		Vector3 newPosition = b.pos;

		//compute a custom bonelength for this frame, to be averaged later
		b.boneLength = Vector3.Magnitude (newPosition - pos);

		//Compute orientation quat
		Vector3 towardChild = Vector3.Normalize (b.pos - pos);
		b.orientation = Quaternion.FromToRotation (Vector3.forward, towardChild);

		//bone position initialization
		b.pos = b.orientation * Vector3.forward * b.boneLength + pos; //assumes bone length 1 for now

		//set heirarchial bounds
		children.Add (b);
		b.parent = this;
	}

    public void changeBone (Vector3 newPosition)
    {
        pos = newPosition;

        //compute a custom bonelength for this frame, to be averaged later
        if (parent == null) return;

        boneLength = Vector3.Magnitude(pos - parent.pos);
        if (true) Timeline.Instance.boneLengths[index] = boneLength;

        //Compute orientation quat
        Vector3 towardChild = Vector3.Normalize(pos - parent.pos);
        orientation = Quaternion.FromToRotation(Vector3.forward, towardChild);
        foreach (Bone b in children)
        {
            //b.changeBone(orientation);
        }

        //bone position initialization
        pos = orientation * Vector3.forward * boneLength + parent.pos;
    }

    public void changeBone(Quaternion q)
    {
        orientation = q * orientation;
        foreach (Bone b in children)
        {
            b.changeBone(q);
        }
    }

	public void updatePos () {
		if (parent != null)
			pos = orientation * Vector3.forward * boneLength + parent.pos;

		foreach (Bone b in children) {
			b.updatePos ();
		}
	}
}
