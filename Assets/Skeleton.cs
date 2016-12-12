using UnityEngine;
using System.Collections;
using System;

public class Skeleton : IComparable<Skeleton> {

    public Quaternion[] rotations = new Quaternion[16];
	public float key;
    public Vector3 rootPos;

	public Skeleton () : this(0) {
	    
	}

	public Skeleton(float key) {
        this.key = key;
        this.rootPos = new Vector3(0, 9.62f, 0);
        for (int i = 0; i < 16; i++)
        {
            rotations[i] = Quaternion.Euler(0, 0, 0);
        }
        rotations[0] = Quaternion.Euler(0, 180, 0);
	}
		
	//deep copy
	public Skeleton(Skeleton toCopy) {
        this.rootPos = toCopy.rootPos;
        for (int i = 0; i < 16; i++)
        {
            rotations[i] = toCopy.rotations[i];
        }
    }

	public int CompareTo(Skeleton compareSkeleton) {
		return key.CompareTo (compareSkeleton.key);
	}
}
