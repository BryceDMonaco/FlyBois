using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
    public int health = 100;

    public GameObject explosion;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DoDamage (int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            GameObject exp = Instantiate(explosion, transform.position, transform.rotation);

            Destroy(exp, 5f);

            Destroy(gameObject);

        }

    }
}
