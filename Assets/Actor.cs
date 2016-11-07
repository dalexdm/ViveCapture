using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Constructs a gameobject representation of a skeleton
public class Actor : MonoBehaviour
{
	public Skeleton skeleton;
	public GameObject jointPrefab;

    public GameObject[] boneObjects = new GameObject[16];


	void Start () {
		for (int i = 0; i < 16; i++) {
			boneObjects [i] = (GameObject)Instantiate (jointPrefab);
			boneObjects [i].transform.SetParent (transform);
		}
	}


	//repositions joint positions based on skeleton
	public void pose (Skeleton s) {

		if (s == null) {
			return;
		}

		for (int i = 0; i < 16; i++) {
			if (s.bones [i] != null) {
				boneObjects [i].SetActive (true);
				boneObjects [i].transform.position = s.bones [i].pos;
				LineRenderer lr = boneObjects [i].GetComponent<LineRenderer> ();
				lr.SetPosition (0, lr.transform.position);
				if (s.bones [i].parent == null)
					lr.SetPosition(1,lr.transform.position);
				else 
					lr.SetPosition(1,s.bones[i].parent.pos);

			} else {
				boneObjects [i].SetActive (false);
			}
		}
	}

	/*
	public ActorJoint[] joints = new ActorJoint[16];
	public GameObject jointPrefab;

	public int key;
	public bool isAdded = false;
	public bool isMain = false;

	public PrimitiveTimeline tl;

	private int leftIndex;
	private int rightIndex;
	private int centerIndex;


	// Use this for initialization
	void Start ()
	{
	
	}

	public Actor (Actor copied) {
		//this = copied;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isMain)
			return;

		//Get left controller
		if (Input.GetMouseButtonUp (0) && Input.GetKey(KeyCode.LeftShift)) {
			Plane plane = new Plane (new Vector3 (0, 0, 1), new Vector3 ());
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float distance;
			Vector3 hitpoint;
			if (plane.Raycast (ray, out distance)) {
				hitpoint = ray.GetPoint (distance);
				if (leftIndex < 6)
					addJoint (hitpoint, 1);
			}


		}

		//Get right controller
		if (Input.GetMouseButtonUp (1) && Input.GetKey(KeyCode.LeftShift)) {
			Plane plane = new Plane (new Vector3 (0, 0, 1), new Vector3 ());
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float distance;
			Vector3 hitpoint;
			if (plane.Raycast (ray, out distance)) {
				hitpoint = ray.GetPoint (distance);
				if (rightIndex < 6)
					addJoint (hitpoint, 2);
			}

			if (!isAdded) {
				isAdded = true;
				tl.addActor(this);
			}
		}

		//Get center button tapped on either controller
		if (Input.GetMouseButtonUp (2) && Input.GetKey(KeyCode.LeftShift)) {
			Plane plane = new Plane (new Vector3 (0, 0, 1), new Vector3 ());
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float distance;
			Vector3 hitpoint;
			if (plane.Raycast (ray, out distance)) {
				hitpoint = ray.GetPoint (distance);
				if (centerIndex < 3)
					addJoint (hitpoint, 0);
			}

			if (!isAdded) {
				isAdded = true;
				tl.addActor(this);
			}
		}
	}

	//intantiates a joint and sends it to the correct limb
	void addJoint (Vector3 pos, int clr)
	{

		//new AJ
		GameObject newAJ = (GameObject)Instantiate (jointPrefab, pos, new Quaternion ());
		newAJ.transform.parent = transform;
		newAJ.GetComponent<ActorJoint> ().actor = this;

		//kickoff assignment
		switch (clr) {
		case 0:
			joints [centerIndex + 12] = newAJ.GetComponent<ActorJoint> ();
			addJointCenter (newAJ);
			break;
		case 1:
			joints [leftIndex] = newAJ.GetComponent<ActorJoint> ();
			addJointLeft (newAJ);
			break;
		case 2:
			joints [rightIndex + 6] = newAJ.GetComponent<ActorJoint> ();
			addJointRight (newAJ);
			break;
		}
	}


	void addJointLeft (GameObject newAJ)
	{
		Debug.Log (leftIndex);
		//arm
		if (leftIndex < 3) {
			//elbow, wrist
			if (leftIndex != 0)
				joints [leftIndex - 1].addChild (newAJ, leftIndex);
			//shoulder
			else if (joints [12])
				joints [12].addChild (newAJ, leftIndex);
			GameObject.Find ("Canvas").transform.GetChild (0).GetChild (leftIndex).GetComponent<Image> ().color = Color.green;
			leftIndex++;

			//leg
		} else {
			//elbow, wrist
			if (leftIndex != 3)
				joints [leftIndex - 1].addChild (newAJ, leftIndex);
			//shoulder
			else if (joints [14])
				joints [14].addChild (newAJ, leftIndex);
			GameObject.Find ("Canvas").transform.GetChild (0).GetChild (leftIndex).GetComponent<Image> ().color = Color.green;
			leftIndex++;
		}
	}

	void addJointRight (GameObject newAJ)
	{
		//arm
		if (rightIndex < 3) {
			//elbow, wrist
			if (rightIndex != 0)
				joints [rightIndex + 5].addChild (newAJ, rightIndex);
			//shoulder
			else if (joints [12])
				joints [12].addChild (newAJ, rightIndex);
			GameObject.Find ("Canvas").transform.GetChild (0).GetChild (rightIndex).GetComponent<Image> ().color = Color.green;
			rightIndex++;

			//leg
		} else {
			//elbow, wrist
			if (rightIndex != 3)
				joints [rightIndex + 5].addChild (newAJ, rightIndex);
			//shoulder
			else if (joints [14])
				joints [14].addChild (newAJ, rightIndex);
			GameObject.Find ("Canvas").transform.GetChild (0).GetChild (rightIndex).GetComponent<Image> ().color = Color.green;
			rightIndex++;
		}
	}

	void addJointCenter (GameObject newAJ)
	{
			
		//neck
		if (centerIndex == 0) {
			if (joints [0])
				joints [12].addChild (joints [0].gameObject, 0);
			if (joints [6])
				joints [12].addChild (joints [6].gameObject, 6);
		}
		if (centerIndex == 1)
			joints [12].addChild (joints [13].gameObject, 13);

		if (centerIndex == 2) {
			joints [13].addChild (joints [14].gameObject, 14);
			if (joints [3])
				joints [14].addChild (joints [3].gameObject, 3);
			if (joints [9])
				joints [14].addChild (joints [9].gameObject, 9);
		}

		GameObject.Find ("Canvas").transform.GetChild (0).GetChild (centerIndex + 12).GetComponent<Image> ().color = Color.green;
		centerIndex++;
	}

	void checkForAdd() {
		for (int i = 0; i < 16; i++) {
			if (joints [i] == null) {
				return;
			} else {
				isAdded = true;
				tl.addActor (this);
			}
		}
	}
*/
}
