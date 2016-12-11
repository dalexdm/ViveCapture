using UnityEngine;
using System.Collections;

public class looker : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.transform.LookAt(new Vector3(0, 100, 0));
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.LookAt(new Vector3(0, 100, 0));
    }
}
