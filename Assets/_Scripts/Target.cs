using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
    public int health = 100;
    public int pointsWorth = 10;

    public GameObject explosion;

    private GameManager gameManager;

    private int initialHealth;

	// Use this for initialization
	void Start () {
        initialHealth = health;

		gameManager = FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DoDamage (int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            gameManager.UpdateScore(pointsWorth);
            gameManager.MarkTargetDestroyed ();

            if (gameManager.thisGameMode == GameModes.TargetScore)
            {
                Kill ();

            } else {
                KillAndDestroy ();


            }


        }

    }

    void Kill ()
    {
        GameObject exp = Instantiate(explosion, transform.position, transform.rotation);

        Destroy(exp, 5f);

        gameObject.SetActive(false);

        bool[] positionUsed = new bool[gameManager.targetPositions.Length];
        Transform[] targetPositions = gameManager.targetPositions;

        int thisIndex = -1;

        foreach (Target trg in gameManager.targets)
        {
            Vector3 trgPosition = trg.transform.position;

            for (int i = 0; i < gameManager.targetPositions.Length; i++)
            {
                if (trgPosition == targetPositions [i].position)
                {
                    positionUsed [i] = true;

                    if (trg == this)
                    {
                        thisIndex = i;

                    }

                }

            }

        }

        bool pointFound = false;
        int index = 0;

        while (!pointFound)
        {
            index = Random.Range (0, targetPositions.Length); //Pick an index

            pointFound = !positionUsed [index]; //If that index isn't already used then a point is found

        }

        transform.position = targetPositions [index].position;

        health = initialHealth;

        gameObject.SetActive (true);

    }

    void KillAndDestroy ()
    {
        GameObject exp = Instantiate(explosion, transform.position, transform.rotation);

        Destroy(exp, 5f);

        Destroy(gameObject);

    }
}
