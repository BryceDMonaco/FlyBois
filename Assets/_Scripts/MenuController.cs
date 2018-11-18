using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	public InputField nameField;
	public Text nameNoticeText;
	private int resetCount = 0;

	// Use this for initialization
	void Start () {
		Time.timeScale = 1f; 
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateName (string name)
	{
		PlayerPrefs.SetString ("PlayerName", nameField.text);

		nameNoticeText.gameObject.SetActive (true);

		nameNoticeText.text = "Player name set to " + PlayerPrefs.GetString ("PlayerName", "Player")
								+ "\n(This will not change existing scores)";

		return;

	}

	public void OpenLevel (int levelIndex)
	{
		if (levelIndex == 0) //TargetScore
		{
			SceneManager.LoadScene ("TargetScore");

		} else if (levelIndex == 1) //TargetTime
		{
			SceneManager.LoadScene ("TargetTime");

		} else if (levelIndex == 2) //GoalScore
		{
			SceneManager.LoadScene("GoalScore");

		} else if (levelIndex == 3) //GoalTime
		{
			SceneManager.LoadScene("GoalTime");

		}

	}

	public void ResetStats ()
	{
		resetCount++;

		if (resetCount == 1)
		{
			nameNoticeText.gameObject.SetActive(true);
			nameNoticeText.text = "WARNING: Press again to reset stats.";

		} else if (resetCount == 2)
		{
			string name = PlayerPrefs.GetString ("PlayerName", "Player"); 

			resetCount = 0;

			PlayerPrefs.DeleteAll ();

			nameNoticeText.text = "All stats reset.";

			PlayerPrefs.SetString ("PlayerName", name);

		}

	}

}
