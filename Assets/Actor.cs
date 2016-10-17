using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Actor : MonoBehaviour {

	public GameObject[] joints = new GameObject[15];
	public GameObject jointPrefab;

	public PrimitiveTimeline tl;

	private int leftIndex;
	private int rightIndex;
	private int centerIndex;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp (0)) {
			Plane plane = new Plane (new Vector3 (0, 0, 1), new Vector3 ());
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float distance;
			Vector3 hitpoint;
			if (plane.Raycast(ray, out distance)) {
				hitpoint = ray.GetPoint(distance);
				addJointLeft(hitpoint);
			}
		}

		if (Input.GetMouseButtonUp (1)) {
			Plane plane = new Plane (new Vector3 (0, 0, 1), new Vector3 ());
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float distance;
			Vector3 hitpoint;
			if (plane.Raycast(ray, out distance)) {
				hitpoint = ray.GetPoint(distance);
				addJointRight(hitpoint);
			}
		}

		if (Input.GetMouseButtonUp (2)) {
			Plane plane = new Plane (new Vector3 (0, 0, 1), new Vector3 ());
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float distance;
			Vector3 hitpoint;
			if (plane.Raycast(ray, out distance)) {
				hitpoint = ray.GetPoint(distance);
				addJointCenter(hitpoint);
			}
		}
	}

	void addJointLeft(Vector3 pos) {
		//arm
		if (leftIndex < 3) {
			joints [leftIndex] = (GameObject)Instantiate (jointPrefab, pos, new Quaternion ());
			joints [leftIndex].GetComponent<ActorJoint> ().parent = this;
			if (leftIndex != 0)
				joints [leftIndex - 1].GetComponent<ActorJoint> ().addChild (joints [leftIndex], leftIndex - 1);
			else if (joints[12]) joints [12].GetComponent<ActorJoint> ().addChild (joints [leftIndex], 12);
			GameObject.Find("Canvas").transform.GetChild(0).GetChild (leftIndex).GetComponent<Image> ().color = Color.green;
			leftIndex++;
		//leg
		} else if (leftIndex < 6) {
			joints [leftIndex] = (GameObject) Instantiate (jointPrefab, pos, new Quaternion());
			joints [leftIndex].GetComponent<ActorJoint> ().parent = this;
			if (leftIndex != 3)
				joints [leftIndex - 1].GetComponent<ActorJoint> ().addChild (joints[leftIndex],leftIndex-1);
					else if (joints[14]) joints [14].GetComponent<ActorJoint> ().addChild (joints [leftIndex], 14);
			GameObject.Find("Canvas").transform.GetChild(0).GetChild (leftIndex).GetComponent<Image> ().color = Color.green;
			leftIndex++;
		}
	}

	void addJointRight(Vector3 pos) {
		//arm
		if (rightIndex < 3) {
			joints [rightIndex + 6] = (GameObject) Instantiate (jointPrefab, pos, new Quaternion());
			joints [rightIndex + 6].GetComponent<ActorJoint> ().parent = this;
			if (rightIndex != 0)
				joints [rightIndex + 5].GetComponent<ActorJoint> ().addChild (joints[rightIndex + 6], rightIndex+5);
			else if (joints[12]) joints [12].GetComponent<ActorJoint> ().addChild (joints [rightIndex + 6], 12);
			GameObject.Find("Canvas").transform.GetChild(0).GetChild (rightIndex + 6).GetComponent<Image> ().color = Color.green;
			rightIndex++;
		//leg
		} else if (rightIndex < 6) {
			joints [rightIndex + 6] = (GameObject) Instantiate (jointPrefab, pos, new Quaternion());
			joints [rightIndex + 6].GetComponent<ActorJoint> ().parent = this;
			if (rightIndex != 3)
				joints [rightIndex + 5].GetComponent<ActorJoint> ().addChild (joints[rightIndex + 6],rightIndex + 5);
			else if (joints[14]) joints [14].GetComponent<ActorJoint> ().addChild (joints [rightIndex + 6], 14);
			GameObject.Find("Canvas").transform.GetChild(0).GetChild (rightIndex + 6).GetComponent<Image> ().color = Color.green;
			rightIndex++;
		}
	}

	void addJointCenter(Vector3 pos) {
		if (centerIndex < 3) {
			GameObject.Find("Canvas").transform.GetChild(0).GetChild (centerIndex + 12).GetComponent<Image> ().color = Color.green;
			joints [centerIndex + 12] = (GameObject)Instantiate (jointPrefab, pos, new Quaternion ());
			joints [centerIndex + 12].GetComponent<ActorJoint> ().parent = this;
			//neck
			if (centerIndex == 0) {
				joints [12].transform.SetParent (transform);
				if (joints [0])
					joints [12].GetComponent<ActorJoint> ().addChild (joints [0], 12);
				if (joints [6])
					joints [12].GetComponent<ActorJoint> ().addChild (joints [6], 12);
			}
			if (centerIndex == 1)
				joints [12].GetComponent<ActorJoint> ().addChild (joints [13], 12);

			if (centerIndex == 2) {
				joints [13].GetComponent<ActorJoint> ().addChild (joints [14], 13);
				if (joints [3])
					joints [14].GetComponent<ActorJoint> ().addChild (joints [3],14);
				if (joints [9])
					joints [14].GetComponent<ActorJoint> ().addChild (joints [9],14);
			}
			centerIndex++;
		}
	}
}
