using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public bool isGameOver = false;
	private bool hasHandledGameOver = false;

	public int score = 0;

	[Header("UI Variables")]

	public Text primaryText;
	public Text secondaryText;
	public Text leaderText;
	public Button menuButton;

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
	private int numberOfTargetsRemaining;

	//Optional Param, only needed to be not null if the game mode requires it
	public Goal[] goals; //Array of all the goals in the map, in the order they should be flown through
	private Goal currentGoal; //The next goal that the player should fly through
	private int currentGoalIndex = 0;
	private int lapNumber = 0;

	public float timeAllowed = 60f; //The time to count down from, in seconds
	public string timerString; //The timer converted to a string value in the format MM:SS.MS
	private float startTime; //The time when the timer has started
	private float currentTime; //The time currently
	private float endTime; //The time at gameover (for timed gamemodes)

	public AlwaysPoint arrow; //The arrow which the plan uses to point to the next goal

	// Use this for initialization
	void Start () {
		score = 0;

		Time.timeScale = 1f;

		isGameTimeBased = (thisGameMode == GameModes.GoalTime) || (thisGameMode == GameModes.TargetTime);

		startTime = Time.time;

		if ((thisGameMode == GameModes.TargetScore || thisGameMode == GameModes.TargetTime) && targets.Length == 0)
		{

			SpawnTargetsAtRandomPositions ();

			numberOfTargetsRemaining = numberOfTargetsToSpawn; //Only used by TargetTime

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

		if (!hasHandledGameOver && isGameOver)
		{
			Time.timeScale = 0f;

			FindObjectOfType<PlanePilot>().enabled = false;

			GenerateLeaderboard ();

			leaderText.gameObject.SetActive (true);
			menuButton.gameObject.SetActive (true);

			hasHandledGameOver = true;

		}
		
	}

	public void CheckIfGameOver (GameModes mode)
	{
		if (mode == GameModes.TargetScore && Time.time >= (startTime + timeAllowed)) //Timer over
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

		} if (mode == GameModes.TargetTime && numberOfTargetsRemaining <= 0) //All targets destroyed
		{
			isGameOver = true;
			endTime = Time.time;
			primaryText.text = "Time: " + GetCurrentTimeFormattedAsTimer ();
			secondaryText.text = "All Targets Destroyed!";

		} else if (mode ==GameModes.GoalTime && currentGoalIndex >= goals.Length) //One lap completed
		{
			isGameOver = true;
			primaryText.text = "Lap Complete!";
			endTime = Time.time;
			secondaryText.text = "Time: " + GetCurrentTimeFormattedAsTimer ();

		} else if (mode ==GameModes.GoalScore && Time.time >= (startTime + timeAllowed)) //Timer over
		{
			isGameOver = true;
			primaryText.text = "Game Over!";
			secondaryText.text = "Score: " + score.ToString ();

		}

	}

	public void UpdateScore (int value)
	{
		score += value;

		return;

	}

	public void MarkTargetDestroyed ()
	{
		numberOfTargetsRemaining--;

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

		if (currentGoalIndex < goals.Length && thisGameMode == GameModes.GoalTime) //Check to make sure not going out of bounds
		{
			currentGoal = goals [currentGoalIndex];

		}  else
		{
			currentGoalIndex = 0;
			lapNumber++;
			currentGoal = goals [currentGoalIndex];


		}

		return;

	}

	void GenerateLeaderboard ()
	{
		if (thisGameMode == GameModes.TargetScore)
		{
			int[] topScores = new int[3];
			string[] topNames = new string[3];

			topScores [0] = PlayerPrefs.GetInt ("TS_1S", 50);
			topScores [1] = PlayerPrefs.GetInt ("TS_2S", 40);
			topScores [2] = PlayerPrefs.GetInt ("TS_3S", 30);

			topNames [0] = PlayerPrefs.GetString ("TS_1N", "Maverick");
			topNames [1] = PlayerPrefs.GetString ("TS_2N", "Goose");
			topNames [2] = PlayerPrefs.GetString ("TS_3N", "Iceman");

			int thisScore = score;
			string thisName = PlayerPrefs.GetString ("PlayerName", "Player");

			if (thisScore > topScores[0]) //Score is better than first place
			{
				//Move Second place down to third
				topNames[2] = string.Copy(topNames[1]);
				topScores[2] = topScores[1];

				//Move first place down to second
				topNames[1] = string.Copy(topNames[0]);
				topScores[1] = topScores[0];

				//Replace first place with the new first
				topScores [0] = thisScore;
				topNames [0] = thisName;

			} else if (thisScore > topScores[1]) //Score is better than second but not first
			{
				//Move Second place down to third
				topNames[2] = string.Copy(topNames[1]);
				topScores[2] = topScores[1];

				//Replace second place with the new second
				topScores [1] = thisScore;
				topNames [1] = thisName;

			} else if (thisScore > topScores[2]) //Score is better than third but not second or first
			{
				//Replace third place with the new third
				topScores [2] = thisScore;
				topNames [2] = thisName;

			}

			//Store the new highs
			PlayerPrefs.SetInt ("TS_1S", topScores [0]);
			PlayerPrefs.SetInt ("TS_2S", topScores [1]);
			PlayerPrefs.SetInt ("TS_3S", topScores [2]);

			PlayerPrefs.SetString ("TS_1N", topNames [0]);
			PlayerPrefs.SetString ("TS_2N", topNames [1]);
			PlayerPrefs.SetString ("TS_3N", topNames [2]);

			leaderText.text = "Top Scores:\n"
								+ topNames [0] + " - " + topScores [0].ToString() + "\n"
								+ topNames [1] + " - " + topScores [1].ToString() + "\n"
								+ topNames [2] + " - " + topScores [2].ToString();

		} else if (thisGameMode == GameModes.TargetTime)
		{
			float[] topScores = new float[3];
			string[] topNames = new string[3];

			topScores [0] = PlayerPrefs.GetFloat ("TT_1S", 30f);
			topScores [1] = PlayerPrefs.GetFloat ("TT_2S", 60f);
			topScores [2] = PlayerPrefs.GetFloat ("TT_3S", 90f);

			topNames [0] = PlayerPrefs.GetString ("TT_1N", "Fox");
			topNames [1] = PlayerPrefs.GetString ("TT_2N", "Falco");
			topNames [2] = PlayerPrefs.GetString ("TT_3N", "Slippy");

			string thisName = PlayerPrefs.GetString ("PlayerName", "Player");

			if (endTime < topScores[0]) //Time is faster than best time
			{
				//Move Second place down to third
				topNames[2] = string.Copy(topNames[1]);
				topScores[2] = topScores[1];

				//Move first place down to second
				topNames[1] = string.Copy(topNames[0]);
				topScores[1] = topScores[0];

				//Replace first place with the new first
				topScores [0] = endTime;
				topNames [0] = thisName;

			} else if (endTime < topScores[1]) //Time is faster than second but not first
			{
				//Move Second place down to third
				topNames[2] = string.Copy(topNames[1]);
				topScores[2] = topScores[1];

				//Replace second place with the new second
				topScores [1] = endTime;
				topNames [1] = thisName;

			} else if (endTime < topScores[2]) //Time is faster than third but not second or first
			{
				//Replace third place with the new third
				topScores [2] = endTime;
				topNames [2] = thisName;

			}

			//Store the new highs
			PlayerPrefs.SetFloat ("TT_1S", topScores [0]);
			PlayerPrefs.SetFloat ("TT_2S", topScores [1]);
			PlayerPrefs.SetFloat ("TT_3S", topScores [2]);

			PlayerPrefs.SetString ("TT_1N", topNames [0]);
			PlayerPrefs.SetString ("TT_2N", topNames [1]);
			PlayerPrefs.SetString ("TT_3N", topNames [2]);

			leaderText.text = "Top Times:\n"
								+ topNames [0] + " - " + GetTimeFormattedAsTimer (topScores [0]) + "\n"
								+ topNames [1] + " - " + GetTimeFormattedAsTimer (topScores [1]) + "\n"
								+ topNames [2] + " - " + GetTimeFormattedAsTimer (topScores [2]);

		} else if (thisGameMode == GameModes.GoalScore)
		{
			int[] topScores = new int[3];
			string[] topNames = new string[3];

			topScores [0] = PlayerPrefs.GetInt ("GS_1S", 10);
			topScores [1] = PlayerPrefs.GetInt ("GS_2S", 8);
			topScores [2] = PlayerPrefs.GetInt ("GS_3S", 6);

			topNames [0] = PlayerPrefs.GetString ("GS_1N", "Dwight");
			topNames [1] = PlayerPrefs.GetString ("GS_2N", "Jim");
			topNames [2] = PlayerPrefs.GetString ("GS_3N", "Pam");

			int thisScore = score;
			string thisName = PlayerPrefs.GetString ("PlayerName", "Player");

			if (thisScore > topScores[0]) //Score is better than first place
			{
				//Move Second place down to third
				topNames[2] = string.Copy(topNames[1]);
				topScores[2] = topScores[1];

				//Move first place down to second
				topNames[1] = string.Copy(topNames[0]);
				topScores[1] = topScores[0];

				//Replace first place with the new first
				topScores [0] = thisScore;
				topNames [0] = thisName;

			} else if (thisScore > topScores[1]) //Score is better than second but not first
			{
				//Move Second place down to third
				topNames[2] = string.Copy(topNames[1]);
				topScores[2] = topScores[1];

				//Replace second place with the new second
				topScores [1] = thisScore;
				topNames [1] = thisName;

			} else if (thisScore > topScores[2]) //Score is better than third but not second or first
			{
				//Replace third place with the new third
				topScores [2] = thisScore;
				topNames [2] = thisName;

			}

			//Store the new highs
			PlayerPrefs.SetInt ("GS_1S", topScores [0]);
			PlayerPrefs.SetInt ("GS_2S", topScores [1]);
			PlayerPrefs.SetInt ("GS_3S", topScores [2]);

			PlayerPrefs.SetString ("GS_1N", topNames [0]);
			PlayerPrefs.SetString ("GS_2N", topNames [1]);
			PlayerPrefs.SetString ("GS_3N", topNames [2]);

			leaderText.text = "Top Scores:\n"
								+ topNames [0] + " - " + topScores [0].ToString() + "\n"
								+ topNames [1] + " - " + topScores [1].ToString() + "\n"
								+ topNames [2] + " - " + topScores [2].ToString();

		} else if (thisGameMode == GameModes.GoalTime)
		{
			float[] topScores = new float[3];
			string[] topNames = new string[3];

			topScores [0] = PlayerPrefs.GetFloat ("GT_1S", 30f);
			topScores [1] = PlayerPrefs.GetFloat ("GT_2S", 60f);
			topScores [2] = PlayerPrefs.GetFloat ("GT_3S", 90f);

			topNames [0] = PlayerPrefs.GetString ("GT_1N", "Ash");
			topNames [1] = PlayerPrefs.GetString ("GT_2N", "Brock");
			topNames [2] = PlayerPrefs.GetString ("GT_3N", "Misty");

			string thisName = PlayerPrefs.GetString ("PlayerName", "Player");

			if (endTime < topScores[0]) //Time is faster than best time
			{
				//Move Second place down to third
				topNames[2] = string.Copy(topNames[1]);
				topScores[2] = topScores[1];

				//Move first place down to second
				topNames[1] = string.Copy(topNames[0]);
				topScores[1] = topScores[0];

				//Replace first place with the new first
				topScores [0] = endTime;
				topNames [0] = thisName;

			} else if (endTime < topScores[1]) //Time is faster than second but not first
			{
				//Move Second place down to third
				topNames[2] = string.Copy(topNames[1]);
				topScores[2] = topScores[1];

				//Replace second place with the new second
				topScores [1] = endTime;
				topNames [1] = thisName;

			} else if (endTime < topScores[2]) //Time is faster than third but not second or first
			{
				//Replace third place with the new third
				topScores [2] = endTime;
				topNames [2] = thisName;

			}

			//Store the new highs
			PlayerPrefs.SetFloat ("GT_1S", topScores [0]);
			PlayerPrefs.SetFloat ("GT_2S", topScores [1]);
			PlayerPrefs.SetFloat ("GT_3S", topScores [2]);

			PlayerPrefs.SetString ("GT_1N", topNames [0]);
			PlayerPrefs.SetString ("GT_2N", topNames [1]);
			PlayerPrefs.SetString ("GT_3N", topNames [2]);

			leaderText.text = "Top Times:\n"
								+ topNames [0] + " - " + GetTimeFormattedAsTimer (topScores [0]) + "\n"
								+ topNames [1] + " - " + GetTimeFormattedAsTimer (topScores [1]) + "\n"
								+ topNames [2] + " - " + GetTimeFormattedAsTimer (topScores [2]);

		}

	}

	public void ReturnToMenu ()
	{
		SceneManager.LoadScene ("MainMenu");

	}
}
