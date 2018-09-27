using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
    public int health = 100;
    public int pointsWorth = 10;

    public GameObject explosion;

    private GameManager gameManager;

	// Use this for initialization
	void Start () {
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

            GameObject exp = Instantiate(explosion, transform.position, transform.rotation);

            Destroy(exp, 5f);

            Destroy(gameObject);

        }

    }
}
