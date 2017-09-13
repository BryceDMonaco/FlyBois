using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpray : MonoBehaviour {

    public float sprayHeight = 5f;

    public Transform heightTarget; //This spray should always be at the height (y) of this object

    public Transform followTarget; //The object that this spray should follow on the XZ Plane

    private float yConstraint = 0f;
    private ParticleSystem mySpray;
    private bool isSpraying = false;
    private PlanePilot myPlane;

    void Start ()
    {
        yConstraint = heightTarget.position.y;

        mySpray = GetComponent<ParticleSystem>();

        myPlane = followTarget.GetComponentInParent<PlanePilot>();

    }

    void FixedUpdate ()
    {
        //Set the spray's position
        Vector3 posVector;

        posVector = followTarget.position;
        posVector.y = yConstraint;

        transform.position = posVector;

        //Check if the spray should be spraying
        posVector = transform.position - followTarget.position;

        if (!isSpraying && posVector.magnitude <= sprayHeight)
        {
            mySpray.Play();

            isSpraying = true;

        } else if (isSpraying && posVector.magnitude > sprayHeight)
        {
            mySpray.Stop();

            isSpraying = false;

        }

        if (!myPlane.IsAlive())
        {
            mySpray.Stop();

        }

    }

}
