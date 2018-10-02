using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

	public GameObject confetti;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.name == "PlaneObject") //Checking for a tag would be faster
		{
			GameObject con = Instantiate (confetti, transform.position, transform.rotation);

			Destroy (con, 10f);

		}

	}
}
