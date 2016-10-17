using UnityEngine;
using System.Collections;

public class ActorJoint : MonoBehaviour {

	public Actor parent;
	public ArrayList bones = new ArrayList();
	public GameObject bonePrefab;
	public Quaternion orientation;

	// Use this for initialization
	void Start () {
		
	}

	public void addChild (GameObject child, int index) {

		//create bone
		float boneLength = parent.tl.boneLengths[index];
		//if this is the first frame we've recorded
		if (boneLength == 0) {
			boneLength = Vector3.Magnitude(child.transform.position - this.transform.position);
			parent.tl.boneLengths [index] = boneLength;
		}
		GameObject bone = (GameObject) Instantiate (bonePrefab,transform.position, new Quaternion());
		this.transform.LookAt (child.transform.position);
		this.orientation = this.transform.rotation;
		this.transform.rotation = new Quaternion();
		child.transform.position = this.transform.position + this.orientation * Vector3.forward * boneLength;
		bone.transform.SetParent (transform);
		child.transform.SetParent (bone.transform);
		bone.GetComponent<LineRenderer>().SetPosition (0, transform.position);
		bones.Add (bone);
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < bones.Count; i++) {
			GameObject bone = (GameObject)bones [i];
			bone.transform.position = this.transform.position;
			bone.GetComponent<LineRenderer> ().SetPosition (0, transform.position);
			bone.GetComponent<LineRenderer> ().SetPosition (1, bone.transform.GetChild(0).position);
		}
	}
}
