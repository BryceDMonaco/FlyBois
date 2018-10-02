using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public enum GameModes {TargetScore, TargetTime, GoalScore, GoalTime, Free};

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
	
	//Optional Param, only needed to be not null if the game mode requires it
	public Goal[] goals; //Array of all the goals in the map, in the order they should be flown through
	private Goal currentGoal; //The next goal that the player should fly through

	public float timeAllowed = 60f; //The time to count down from, in seconds
	public string timerString; //The timer converted to a string value in the format MM:SS.MS
	private float startTime; //The time when the timer has started
	private float currentTime; //The time currently

	// Use this for initialization
	void Start () {
		score = 0;

		isGameTimeBased = (thisGameMode == GameModes.GoalTime) || (thisGameMode == GameModes.TargetTime);

		startTime = Time.time;

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
			Debug.Log("Timer is " + sentTime.ToString());

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
}
