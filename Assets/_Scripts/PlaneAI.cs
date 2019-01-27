using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneAI : MonoBehaviour {

	public Transform target;
	public float degreesPerSecond = 720;
	public float speed = 50f;
	public float speedDamp = 0.25f;

	public int state = 0; //0 = straight line, 1 = targeting, 2 = banking (turn in some direction)
	public float minStateTime = 3f;
	public float maxStateTime = 7f;

	private bool timerRunning = false;

	void FixedUpdate ()
	{
		transform.position += transform.forward * Time.deltaTime * speed;

        speed -= transform.forward.y * speedDamp;

		if (speed <= 35f)
		{
			speed = 35f;

		}

		if (state == 0) //Straight line
		{
			//For now, just orient and then fly towards an arbitrary point
			Vector3 arbitraryTarget = new Vector3(0, 200, 0);

			Vector3 dirFromMeToTarget = arbitraryTarget - transform.position;
			Quaternion lookRotation = Quaternion.LookRotation(dirFromMeToTarget);
			transform.rotation = Quaternion.Lerp (transform.rotation, lookRotation, Time.deltaTime * (degreesPerSecond / 360.0f));

		} else if (state == 1) //Targeting
		{
			Vector3 dirFromMeToTarget = target.position - transform.position;
			Quaternion lookRotation = Quaternion.LookRotation(dirFromMeToTarget);
			transform.rotation = Quaternion.Lerp (transform.rotation, lookRotation, Time.deltaTime * (degreesPerSecond / 360.0f));

		}

		if (!timerRunning)
		{
			StartCoroutine(stateTimer(Random.Range(0, 2)));

		}


	}

	IEnumerator stateTimer (int newState)
	{
		timerRunning = true;

		int oldState = state;

		state = newState;

		yield return new WaitForSeconds (Random.Range(minStateTime, maxStateTime));

		state = oldState;

		timerRunning = false;

	}
}
