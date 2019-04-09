using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour {

	private BoardManager b;

	// Use this for initialization
	void Start () {
		b = BoardManager.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		if (b == null) {
			b = BoardManager.Instance;
		}
		if (b.isWhiteTurn) {
			this.gameObject.transform.localRotation = Quaternion.Lerp (this.transform.rotation, Quaternion.Euler (0, 0, 0), Time.deltaTime);
			//this.gameObject.transform.localRotation = Quaternion.Euler (0, 0, 0);
		} else {
			this.gameObject.transform.localRotation = Quaternion.Lerp (this.transform.rotation, Quaternion.Euler (0, 180, 0), Time.deltaTime);
			//this.gameObject.transform.localRotation = Quaternion.Euler (0, 180, 0);

		}
	}
}
