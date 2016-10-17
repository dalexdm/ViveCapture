using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrimitiveTimeline : MonoBehaviour {

	public Actor[] actors = new Actor[3];
	public Actor mainActor;
	public GameObject actorPrefab;

	public float[] boneLengths = new float[15];

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 3; i++) {
			actors [i] = ((GameObject) Instantiate (actorPrefab)).GetComponent<Actor>();
			actors [i].tl = this;
		}

		for (int i = 0; i < 15; i++)
			boneLengths [i] = 0;
		setActor (0);
	}

	public void setActor(int actorIndex) {
		//mainActor.gameObject.SetActive (false);
		GameObject canvas = GameObject.Find ("Canvas").transform.GetChild(0).gameObject;
		for (int i = 0; i < 3; i++) {
			if (i != actorIndex)
				actors [i].gameObject.SetActive (false);
			else
				actors [i].gameObject.SetActive (true);
		}

		for (int i = 0; i < 15; i++) {
			if (actors [(int) actorIndex].joints [i])
				canvas.transform.GetChild (i).GetComponent<Image> ().color = Color.green;
			else canvas.transform.GetChild (i).GetComponent<Image> ().color = Color.red;
		}
	}

	public void interpolateSimple(float time) {
		if (time < 0 || time > actors.Length - 1)
			return;
		if (checkValidactor ((int) Mathf.Floor (time)) && checkValidactor ((int) Mathf.Ceil (time))) {
			mainActor.gameObject.SetActive (true);
			for (int i = 0; i < 15; i++) {
				mainActor.joints [i].transform.rotation = Quaternion.Slerp (actors [(int)Mathf.Floor (time)].joints [i].GetComponent<ActorJoint> ().orientation, 
					actors [(int)Mathf.Ceil (time)].joints [i].GetComponent<ActorJoint> ().orientation,
					time - Mathf.Floor (time));
			}
		}
	}

	bool checkValidactor (int index) {
		Actor a = actors[index];
		for (int i = 0; i < 15; i++) {
			if (!a.joints [i])
				return false;
		}
		return true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
