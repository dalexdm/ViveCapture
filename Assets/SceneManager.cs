﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour {

	public Actor mainActor;

	//skeleton construction
	public Skeleton inProgress;
	int leftIndex;
	int rightIndex;
	int centerIndex;

	public Slider slider;
	public Text sliderText;
	public GameObject sliderKeyPrefab;
	public float currentKey;
	public float maxKey;

	public Image statusImage;

	// Use this for initialization
	void Start () {
		ChangeFrame ();
		maxKey = slider.maxValue;
	}
	
	// Update is called once per frame
	void Update () {
		//accept key input to create a skeleton on the currently selected frame
		//Get left controller
		if (Input.GetMouseButtonUp (0) && Input.GetKey(KeyCode.LeftShift)) {
			Plane plane = new Plane (new Vector3 (0, 0, 1), new Vector3 ());
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float distance;
			Vector3 hitpoint;
			if (plane.Raycast (ray, out distance)) {
				hitpoint = ray.GetPoint (distance);
				if (leftIndex < 6)
					addBone (hitpoint, 1);
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
					addBone (hitpoint, 2);
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
					addBone (hitpoint, 0);
			}
		}
	}

	public void ChangeFrame () {

		currentKey = slider.value;
		sliderText.text = currentKey.ToString ();
		if (Timeline.Instance.containsKey (currentKey)) {
			sliderText.transform.parent.gameObject.GetComponent<Image> ().color = Color.red;
			sliderText.color = Color.white;
		} else {
			sliderText.transform.parent.gameObject.GetComponent<Image> ().color = Color.white;
			sliderText.color = Color.black;
		}

		inProgress = null;
		leftIndex = 0;
		rightIndex = 0;
		centerIndex = 0;
		statusImage.gameObject.SetActive (false);

		//if we have any skeletons so far, display the main actor in the correct pose
		Skeleton touse = Timeline.Instance.getFramePose(currentKey);
		if (touse == null) {
			mainActor.gameObject.SetActive (false);
			return;
		}
		touse.update ();
		mainActor.pose(touse);

	}

	public void addBone (Vector3 pos, int clr) {

		mainActor.gameObject.SetActive (true);
		
		//one first input of the frame, we should begin constructing a new skeleton
		if (inProgress == null) {
			inProgress = new Skeleton (currentKey);
			statusImage.gameObject.SetActive (true);
			for (int i = 0; i < 15; i++) {
				statusImage.transform.GetChild (i).gameObject.GetComponent<Image> ().color = Color.red;
			} //construct the necessary components for the actor business
		}

		switch (clr) {
		case 0:
			inProgress.bones [centerIndex + 12] = new Bone (pos, centerIndex + 12);
			statusImage.transform.GetChild (centerIndex+12).gameObject.GetComponent<Image> ().color = Color.blue;
			inProgress.addJointCenter (centerIndex);
			centerIndex++;
			break;
		case 1:
			inProgress.bones [leftIndex] = new Bone (pos, leftIndex);
			statusImage.transform.GetChild (leftIndex).gameObject.GetComponent<Image> ().color = Color.blue;
			inProgress.addJointLeft (leftIndex);
			leftIndex++;
			break;
		case 2:
			inProgress.bones [rightIndex + 6] = new Bone (pos, rightIndex + 6);
			statusImage.transform.GetChild (rightIndex+6).gameObject.GetComponent<Image> ().color = Color.blue;
			inProgress.addJointRight (rightIndex);
			rightIndex++;
			break;
		}
		
		
		//subsequent input requires us to rebuild actor
		mainActor.pose(inProgress);

		//once we have 15 skeleton.bones we can package the actor and send it to the timeline
		for (int i = 0; i < 15; i++) {
			if (inProgress.bones [i] == null)
				return;
		}

		Timeline.Instance.addKeyframe (inProgress);
		GameObject newKeyIcon = (GameObject)Instantiate (sliderKeyPrefab, sliderText.transform.position, new Quaternion());
		newKeyIcon.transform.SetParent (slider.transform.parent);
		newKeyIcon.transform.localScale = new Vector3 (1, 1, 1);
		newKeyIcon.transform.position -= new Vector3 (0, 10, 0);
		slider.value += 5;
	
	}

	public void play () {
		StartCoroutine (playRoutine());
	}

	IEnumerator playRoutine() {
		while (slider.value < slider.maxValue) {
			slider.value += 1f;
			yield return null;
		}
	}
}
