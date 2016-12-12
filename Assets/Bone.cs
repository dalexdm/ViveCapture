using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bone {

	public Bone parent;
	public int index;
    public GameObject mesh;


	public Bone (Vector3 pos, int index) {
		
	}

	public Bone (Bone toCopy, Skeleton owner) {
		
	}

	public void addChild(Bone b) {
        
	}

    public void changeBone (Vector3 newPosition)
    {
        
    }

    public void changeBone(Quaternion q)
    {
       
    }

	public void updatePos () {
		
	}
}
