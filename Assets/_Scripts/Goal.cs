using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

	public ParticleSystem confetti;

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
			confetti.Emit (30);

		}

	}
}
