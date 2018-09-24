using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour {

    public int playerCount = 0;

    void Awake () 
    {
        Application.targetFrameRate = 60;

    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangePlayerCount (int amount)
    {
        playerCount += amount;

        UpdateCameras ();

    }

    void UpdateCameras ()
    {
        if (playerCount == 1)
        {
            Camera.allCameras[0].rect.Set(0f, 0f, 1f, 1f);

        } else if (playerCount == 2)
        {
            Camera.allCameras[0].rect = new Rect(0f, 0.5f, 1f, 0.5f);

            Camera.allCameras[1].rect = new Rect(0f, 0, 1f, 0.5f);

        }

    }
}
