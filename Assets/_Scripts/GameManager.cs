﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public bool isGameOver = false;

	public int score = 0;

	[Header("UI Variables")]

	public Text primaryText;
	public Text secondaryText;

	[Header("Game Mode Selection")]

	public GameModes thisGameMode = GameModes.Free; //Default to free flight gamemode
	private bool isGameTimeBased = false;
	
	public bool shouldCountDown = false;

	//Optional Param, only needed to be not null if the game mode requires it
	public Target[] targets; //Array of all the targets in the map, in the order they should be destroyed (if any order)

	public Transform targetPositionHolder; //The _TargetHolder GameObject
	public Transform[] targetPositions; //Array of all the potential positions to spawn targets at
	public GameObject targetObject; //Stores the prefab of the target to spawn
	public int numberOfTargetsToSpawn = 10; //How many targets should be spawned? (0 < numberOfTargetsToSpawn <= targetPositions.length())

	//Optional Param, only needed to be not null if the game mode requires it
	public Goal[] goals; //Array of all the goals in the map, in the order they should be flown through
	private Goal currentGoal; //The next goal that the player should fly through
	private int currentGoalIndex = 0;

	public float timeAllowed = 60f; //The time to count down from, in seconds
	public string timerString; //The timer converted to a string value in the format MM:SS.MS
	private float startTime; //The time when the timer has started
	private float currentTime; //The time currently

	public AlwaysPoint arrow; //The arrow which the plan uses to point to the next goal

	// Use this for initialization
	void Start () {
		score = 0;

		isGameTimeBased = (thisGameMode == GameModes.GoalTime) || (thisGameMode == GameModes.TargetTime);

		startTime = Time.time;

		if ((thisGameMode == GameModes.TargetScore || thisGameMode == GameModes.TargetTime) && targets.Length == 0)
		{

			SpawnTargetsAtRandomPositions ();

		} else 
		{
			currentGoal = goals[currentGoalIndex];

			arrow.gameObject.SetActive(true);
			arrow.target = currentGoal.transform;

		}

		//Debug.Log ("4628.5s to timer = " + GetTimeFormattedAsTimer(4628.5f)); //Used to test for output of 01:17:08.500

	}
	
	// Update is called once per frame
	void Update () {
		if (isGameTimeBased)
		{
			primaryText.text = GetCurrentTimeFormattedAsTimer ();
			secondaryText.text = "Score: " + score.ToString ();

		} else
		{
			primaryText.text = "Score: " + score.ToString ();
			secondaryText.text = GetCurrentTimeFormattedAsTimer ();


		}

		CheckIfGameOver (thisGameMode);

		if (isGameOver)
		{
			Time.timeScale = 0f;

			FindObjectOfType<PlanePilot>().enabled = false;

		}
		
	}

	public void CheckIfGameOver (GameModes mode)
	{
		if (mode == GameModes.TargetScore && Time.time >= (startTime + timeAllowed))
		{
			isGameOver = true;
			primaryText.text = "Game Over!";
			secondaryText.text = "Score: " + score.ToString () + " | ";

			int remainingCount = numberOfTargetsToSpawn - (score / 10);

			if (remainingCount > 0)
			{
				secondaryText.text += "Targets Remaining: " + remainingCount.ToString();

			} else
			{
				secondaryText.text += "All Targets Destroyed";

			}

		} else if (mode ==GameModes.GoalTime && currentGoalIndex >= goals.Length)
		{
			isGameOver = true;
			primaryText.text = "Lap Complete!";
			secondaryText.text = "Time: " + GetCurrentTimeFormattedAsTimer ();

		}

	}

	public void UpdateScore (int value)
	{
		score += value;

		return;

	}

	string GetCurrentTimeFormattedAsTimer ()
	{
		if (shouldCountDown)
		{
			return GetTimeFormattedAsTimer (timeAllowed - (Time.time - startTime));

		} else
		{
			return GetTimeFormattedAsTimer (Time.time - startTime);

		}

	}

	string GetTimeFormattedAsTimer (float sentTime)
	{
		int hourCount = 0;
		int minuteCount = 0;
		float secondCount = 0f;

		string finalString = "";

		if (sentTime >= 3600f)
		{
			float hourCountFl = sentTime / 3600; //Get the number of hours, 1 hr = 3600 s
			hourCount = (int) hourCountFl;

			sentTime = (hourCountFl - hourCount) * 3600;

			if (hourCount > 99)
			{
				hourCount = 99;

			} else if (hourCount < 10)
			{
				finalString += "0";

			}

			finalString += hourCount.ToString() + ":";

		} else
		{
			//If no hours, do not add it to string

		}

		if (sentTime >= 60)
		{
			float minuteCountFl = sentTime / 60; //Get the number of minutes, 1 min = 60 s
			minuteCount = (int) minuteCountFl;

			sentTime = (minuteCountFl - minuteCount) * 60;

			if (minuteCount < 10)
			{
				finalString += "0";

			}

			finalString += minuteCount.ToString() + ":";

		} else
		{
			finalString += "00:";

		}

		if (sentTime > 0f)
		{
			secondCount = sentTime;

			if (secondCount < 10)
			{
				finalString += "0";

			}

			finalString += secondCount.ToString("F3");

		} else
		{
			finalString += "00.0";

		}

		return finalString;

	}

	void SpawnTargetsAtRandomPositions ()
	{
		targets = new Target[numberOfTargetsToSpawn];

		bool[] pointUsed = new bool[targetPositions.Length]; //Used to make sure multiple targets do not spawn in the same location

		for (int i = 0; i < numberOfTargetsToSpawn; i++)
		{
			bool pointFound = false;
			int index = 0;


			while (!pointFound)
			{
				index = Random.Range (0, targetPositions.Length); //Pick an index

				pointFound = !pointUsed[index]; //If that index isn't already used then a point is found

			}

			pointUsed [index] = true; //Mark that newly found point as used

			Vector3 randRotation = new Vector3 (90, Random.Range (0, 180), 0);

			GameObject trg = Instantiate (targetObject, targetPositions [index].position, Quaternion.Euler(randRotation));

			targets [i] = trg.GetComponent<Target> ();

		}

		return;

	}

	void EnableNextGoal ()
	{
		currentGoalIndex++;

		if (currentGoalIndex < goals.Length) //Check to make sure not going out of bounds
		{
			currentGoal = goals [currentGoalIndex];

		} 

		//The else condition, where all goals have been reached, means the game is over, should be
		//...detected by CheckIfGameOver () when the game mode is GoalTime

		return;

	}
}
