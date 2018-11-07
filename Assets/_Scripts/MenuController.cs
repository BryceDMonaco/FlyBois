using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	public InputField nameField;
	public Text nameNoticeText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateName (string name)
	{
		PlayerPrefs.SetString ("PlayerName", nameField.text);

		nameNoticeText.gameObject.SetActive (true);

		nameNoticeText.text = "Player name set to " + PlayerPrefs.GetString ("PlayerName", "Player");

		return;

	}

	public void OpenLevel (int levelIndex)
	{
		if (levelIndex == 0) //TargetScore
		{
			SceneManager.LoadScene ("TargetScore");

		} else if (levelIndex == 1) //TargetTime
		{


		} else if (levelIndex == 2) //GoalScore
		{


		} else if (levelIndex == 3) //GoalTime
		{


		}

	}
}
