using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

	public GameManager gameManager;
	public ParticleSystem confetti;

	// Use this for initialization
	void Start () 
	{
		gameManager = FindObjectOfType <GameManager> ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.name == "PlaneObject") //Checking for a tag would be faster
		{
			gameManager.UpdateScore (1);
			gameManager.EnableNextGoal ();
			confetti.Emit (30);

		}

	}
}
