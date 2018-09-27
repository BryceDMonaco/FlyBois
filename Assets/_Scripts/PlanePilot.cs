using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PigeonCoopToolkit.Effects.Trails;
using InControl;
using UnityEngine.UI;

public class PlanePilot : MonoBehaviour {
    //public PlayerActions Actions { get; set; }

    public int playerNumber = 1;        //Eventually this number will be assigned 

    public bool usingKeyboard = false;  //Should be under control variables but it's a commonly used var

    public bool usingOnScreen = false;

    public int playerHealth = 100;

    private bool isAlive = true;
    //--------------------------------------------------------------------------------------------------------------//
    [Header("Speed Values")]
    public float speed = 50f;

    [Range(0f, 2f)]
    public float pitchDamp = 1f;

    [Range(0f, 2f)]
    public float rollDamp = .7f;

    [Range(0f, 1f)]
    public float yawDamp = .7f;

    [Range(0f, 1f)]
    public float speedDamp = 1f;

    [Range(0f, 1f)]
    public float cameraSpring = 0.96f;

    //--------------------------------------------------------------------------------------------------------------//
    [Header("Weapon Variables")]
    public float shotGap = .5f; //Time between shots fire
    public float shotLifetime = .1f;

    public LineRenderer[] shotRenders;

    public int numberOfGuns = 4;
    public int gunDamage = 10;

    public Rigidbody bullet;
    public float bulletForce = 1000;

    private bool canShoot = true;
    private int shotCycle = 0;

    //--------------------------------------------------------------------------------------------------------------//
    [Header("Effects Variables")]
    public SmokeTrail[] wingTrails;

    public Transform aimPoint;
    public Transform myCameraTransform;

    //public Camera myCamera;           //Deprecated

    public GameObject explosion;
    public GameObject planeArt;

    //--------------------------------------------------------------------------------------------------------------//
    [Header("Control Variables")]
    public KeyCode yawLeft = KeyCode.Q;
    public KeyCode yawRight = KeyCode.E;

    public KeyCode rollLeft = KeyCode.A;
    public KeyCode rollRight = KeyCode.D;

    public KeyCode pitchUp = KeyCode.S;     //Nose up
    public KeyCode pitchDown = KeyCode.W;   //Nose down

    public float maxCamLookX = 30f;
    public float maxCamLookY = 30f;

    public float stickYSensitivity = 1f;
    public float stickXSensitivity = 1f;

    public SingleJoystick stick;
    public Button greenButton;
    public Button redButton;
    public Button leftButton;
    public Button rightButton;

    private InputDevice myInDevice;

    //--------------------------------------------------------------------------------------------------------------//
    [Header("Flap Variables")]
	public Transform leftAileron;
	public Transform rightAileron;

	public Transform leftElevator;
    public Transform rightElevator;

    public Transform rudder;

    public float maxFlapAngle = 15f;

    private Quaternion leftAileronRestingRotation;
    private Quaternion rightAileronRestingRotation;
    private Quaternion leftElevatorRestingRotation;
    private Quaternion rightElevatorRestingRotation;

    //--------------------------------------------------------------------------------------------------------------//
    //Private Vars
    //Note these do not really have a good spot to sort them to
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private float screenW;
    private float screenH;
    

	// Use this for initialization
	void Start ()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            usingKeyboard = false;
            usingOnScreen = true;

        }

        GameObject.Find("_ScreenManager").GetComponent<ScreenManager>().ChangePlayerCount(1);

        if (usingKeyboard && usingOnScreen)
        {
            Debug.LogError("ERROR: USING KEYBOARD AND ONSCREEN");

        }

        if (!usingKeyboard && !usingOnScreen)
        {
            myInDevice = InputManager.Devices[playerNumber - 1];

        } else
        {
            myInDevice = null;

        }

        

        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        numberOfGuns = shotRenders.Length;

        screenH = Screen.height / 2;
        screenW = Screen.width / 2;

        GetFlapRestingRotation();



    }
	
	// Update is called once per frame
	void Update ()
    {
        //myInDevice = InputManager.ActiveDevice;

        Vector3 moveCamTo = transform.position - transform.forward * 10f + Vector3.up * 5f;

        myCameraTransform.position =myCameraTransform.position * cameraSpring + moveCamTo * (1f - cameraSpring);
        myCameraTransform.LookAt(aimPoint);

        if (isAlive)
        {
            transform.position += transform.forward * Time.deltaTime * speed;

            speed -= transform.forward.y * speedDamp;

            if (speed < 35f)
            {
                speed = 35f;

            }
            else if (speed > 60f)
            {
                foreach (SmokeTrail smoke in wingTrails)
                {
                    smoke.Emit = true;

                }

            }
            else
            {
                foreach (SmokeTrail smoke in wingTrails)
                {
                    smoke.Emit = false;

                }

            }

            /*
             * Calculating Input, doing it this way will help keep the code less cluttered when multiple inputs are possible
             * i.e When using a controller for one player and a KB for another.
             * 
             */

            Vector3 InputDirections = CalculateInputsDamped();

            transform.Rotate(InputDirections);

            UpdateFlaps ();

            //Legacy versions of the above function below:
            //transform.Rotate(myInDevice.LeftStickY * pitchDamp, yawDirection * yawDamp, -myInDevice.LeftStickX * rollDamp);
            //transform.Rotate(Input.GetAxis("Vertical") * pitchDamp, 0f, -Input.GetAxis("Horizontal") * rollDamp);

            //Check if a button is held down, uses the color, but I can't find a better way
            bool greenButtonHeld = greenButton.GetComponent<CanvasRenderer>().GetColor() == greenButton.GetComponent<Button>().colors.pressedColor * greenButton.GetComponent<Button>().colors.colorMultiplier;

            //Firing Controls
            if (canShoot && isAlive && ((!usingKeyboard && !usingOnScreen && myInDevice.RightTrigger.IsPressed) || greenButtonHeld || (usingKeyboard && Input.GetKey(KeyCode.Space))))
            {
                StartCoroutine (ShotTimer());

                //StartCoroutine (ShotLifetime(shotRenders[shotCycle]));

                Rigidbody bul = Instantiate (bullet, shotRenders[shotCycle].transform.position, Quaternion.identity);

                //bul.transform.LookAt(aimPoint);

                GameObject.Destroy(bul.gameObject, 5f);

                RaycastHit hit;

                if (Physics.Raycast(myCameraTransform.position, myCameraTransform.forward, out hit, 500, ~LayerMask.GetMask("Plane")))
                {
                    Debug.Log("Hit " + hit.collider.name);

                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        hit.collider.transform.GetComponent<Target>().DoDamage(gunDamage);

                    }

                    bul.transform.LookAt(hit.point);

                    //Debug code which creates a sphere at the hit position of the raycast
                    //GameObject dbSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //dbSphere.transform.position = hit.point;

                } else
                {
                    Ray ray = new Ray(myCameraTransform.position, myCameraTransform.forward);
                    Physics.Raycast(ray, 500, ~LayerMask.GetMask("Plane"));
                    bul.transform.LookAt(ray.GetPoint(500f));

                }

                

                bul.AddForce(bul.transform.forward * bulletForce);

                shotCycle++;

                if (shotCycle == shotRenders.Length)
                {
                    shotCycle = 0;

                }
            }
        }
    }

    IEnumerator ShotTimer ()
    {
        canShoot = false;

        yield return new WaitForSeconds(shotGap);

        canShoot = true;

    }

    IEnumerator ShotLifetime (LineRenderer thisShot)
    {
        thisShot.enabled = true;

        yield return new WaitForSeconds(shotLifetime);

        thisShot.enabled = false;

    }

    void OnTriggerEnter (Collider col)
    {
        if (col.CompareTag("Ring"))
        {
            Debug.Log("Score!");

        } else if (col.CompareTag("Bullet"))
        {
            //Do nothing, ignore the collsion
            
        } else
        {
            Respawn ();


        }
        

    }

    void Respawn ()
    {
        isAlive = false;

        foreach (SmokeTrail smoke in wingTrails)
        {
            smoke.Emit = false;

        }

        speed = 0f;

        StartCoroutine(SpawnTimer());

        GameObject exp = Instantiate(explosion, transform.position, transform.rotation);

        Destroy(exp, 5f);

    }

    IEnumerator SpawnTimer ()
    {
        planeArt.SetActive(false);

        yield return new WaitForSeconds(5f);

        planeArt.SetActive(true);

        isAlive = true;

        transform.position = spawnPosition;

        speed = 50f;

        transform.rotation = spawnRotation;

    }

    public bool IsAlive ()
    {
        return isAlive;

    }

    private Vector3 CalculateInputsRaw ()
    {
        Vector3 inputs = new Vector3(0f, 0f, 0f); //Pitch = x, Yaw = y, Roll = z

        //Pitch
        if (!usingKeyboard) //Controller Pitch Up and Down
        {
            inputs.x = myInDevice.LeftStickY;

        } else if (usingKeyboard && Input.GetKey(pitchUp) && !Input.GetKey(pitchDown))  //KB Pitch Up
        {
            inputs.x = 1f;

        } else if (usingKeyboard && Input.GetKey(pitchDown) && !Input.GetKey(pitchUp))  //KB Pitch Down
        {
            inputs.x = -1f;

        }

        //Yaw
        if (!usingKeyboard && myInDevice.LeftBumper.IsPressed && !myInDevice.RightBumper.IsPressed)         //Controller Yaw Left
        {
            inputs.y = -1f;

        } else if (!usingKeyboard && myInDevice.RightBumper.IsPressed && !myInDevice.LeftBumper.IsPressed)  //Controller Yaw Right
        {
            inputs.y = 1f;

        } else if (usingKeyboard && Input.GetKey(yawLeft) && !Input.GetKey(yawRight))   //KB Yaw Left
        {
            inputs.y = -1f;

        }
        else if (usingKeyboard && Input.GetKey(yawRight) && !Input.GetKey(yawLeft))     //KB Yaw Right
        {
            inputs.y = 1f;

        }

        //Roll
        if (!usingKeyboard) //Controller Roll Left and Right
        {
            inputs.x = -myInDevice.LeftStickX;

        }
        else if (usingKeyboard && Input.GetKey(rollLeft) && !Input.GetKey(rollRight))  //KB Roll left
        {
            inputs.z = 1f;

        }
        else if (usingKeyboard && Input.GetKey(rollRight) && !Input.GetKey(rollLeft))  //KB Roll Right
        {
            inputs.z = -1f;

        }

        return inputs;

    }

    private Vector3 CalculateInputsDamped()
    {
        Vector3 inputs = new Vector3(0f, 0f, 0f); //Pitch = x, Yaw = y, Roll = z

        //Pitch
        if (!usingKeyboard && !usingOnScreen) //Controller Pitch Up and Down
        {
            inputs.x = myInDevice.LeftStickY * stickYSensitivity;

        }
        else if (usingKeyboard && Input.GetKey(pitchUp) && !Input.GetKey(pitchDown))  //KB Pitch Up
        {
            inputs.x = -1f;

        }
        else if (usingKeyboard && Input.GetKey(pitchDown) && !Input.GetKey(pitchUp))  //KB Pitch Down
        {
            inputs.x = 1f;

        } else if (usingOnScreen)
        {
            inputs.x = stick.GetInputDirection().y;

        }

        //Yaw
        if (!usingKeyboard  && !usingOnScreen && myInDevice.LeftBumper.IsPressed && !myInDevice.RightBumper.IsPressed)         //Controller Yaw Left
        {
            inputs.y = -1f;

        }
        else if (!usingKeyboard  && !usingOnScreen && myInDevice.RightBumper.IsPressed && !myInDevice.LeftBumper.IsPressed)  //Controller Yaw Right
        {
            inputs.y = 1f;

        }
        else if (usingKeyboard && Input.GetKey(yawLeft) && !Input.GetKey(yawRight))   //KB Yaw Left
        {
            inputs.y = -1f;

        }
        else if (usingKeyboard && Input.GetKey(yawRight) && !Input.GetKey(yawLeft))     //KB Yaw Right
        {
            inputs.y = 1f;

        } else if (usingOnScreen)
        {
            //Check if a button is held down, uses the color, but I can't find a better way
            bool leftButtonHeld = leftButton.GetComponent<CanvasRenderer>().GetColor() == leftButton.GetComponent<Button>().colors.pressedColor * leftButton.GetComponent<Button>().colors.colorMultiplier;
            bool rightButtonHeld = rightButton.GetComponent<CanvasRenderer>().GetColor() == rightButton.GetComponent<Button>().colors.pressedColor * rightButton.GetComponent<Button>().colors.colorMultiplier;

            if (leftButtonHeld && rightButtonHeld)
            {
                inputs.y = 0f;

            } else if (leftButtonHeld)
            {
                inputs.y = -1f;

            } else if (rightButtonHeld)
            {
                inputs.y = 1f;

            }

        }

        //Roll
        if (!usingKeyboard && !usingOnScreen) //Controller Roll Left and Right
        {
            inputs.z = -myInDevice.LeftStickX * stickXSensitivity;

        }
        else if (usingKeyboard && Input.GetKey(rollLeft) && !Input.GetKey(rollRight))  //KB Roll left
        {
            inputs.z = 1f;

        }
        else if (usingKeyboard && Input.GetKey(rollRight) && !Input.GetKey(rollLeft))  //KB Roll Right
        {
            inputs.z = -1f;

        } else if (usingOnScreen)
        {
            inputs.z = -stick.GetInputDirection().x;

        }

        //Note: there's probably some vector multiplication I can do to make these operations happen in one line
        inputs.x *= pitchDamp;
        inputs.y *= yawDamp;
        inputs.z *= rollDamp;

        return inputs;

    }
    
    //This is more vector math than it really should be, this might need to be changed to fixed update to increase performance/
    //TODO: Get Free Look Down to behave more consistently and point more downwards
	//This function might not actually be used, it is easy to cause motion sickness/very disorienting
    void DoCameraFreeLook ()
    {
        Vector3 startingPos = myCameraTransform.localRotation.eulerAngles;

        Vector3 stickIn =  new Vector3 (myInDevice.RightStickX, myInDevice.RightStickY, 0f);

        Vector3 newPos = new Vector3(Mathf.Lerp(startingPos.x, startingPos.x + maxCamLookX, Mathf.Abs(stickIn.y)), Mathf.Lerp(startingPos.y, startingPos.y + maxCamLookY, Mathf.Abs(stickIn.x)), 0f);

        if (stickIn.x < 0f)
        {
            newPos.y *= -1f;

        }

        if (stickIn.y < 0f)
        {
            newPos.x *= 2f;

        }

        myCameraTransform.localRotation = Quaternion.Euler(newPos);
       
    }

	void UpdateFlaps ()
	{
        float rudderDirection = 0f;

        //Rudder
        if (!usingKeyboard && !usingOnScreen && myInDevice.LeftBumper.IsPressed && !myInDevice.RightBumper.IsPressed)         //Controller Yaw Left
        {
            rudderDirection = 1f;

        }
        else if (!usingKeyboard  && !usingOnScreen && myInDevice.RightBumper.IsPressed && !myInDevice.LeftBumper.IsPressed)  //Controller Yaw Right
        {
            rudderDirection = -1f;

        }
        else if (usingKeyboard && Input.GetKey(yawLeft) && !Input.GetKey(yawRight))   //KB Yaw Left
        {
            rudderDirection = 1f;

        }
        else if (usingKeyboard && Input.GetKey(yawRight) && !Input.GetKey(yawLeft))     //KB Yaw Right
        {
            rudderDirection = -1f;

        } else if (usingOnScreen)
        {
            bool leftButtonHeld = leftButton.GetComponent<CanvasRenderer>().GetColor() == leftButton.GetComponent<Button>().colors.pressedColor * leftButton.GetComponent<Button>().colors.colorMultiplier;
            bool rightButtonHeld = rightButton.GetComponent<CanvasRenderer>().GetColor() == rightButton.GetComponent<Button>().colors.pressedColor * rightButton.GetComponent<Button>().colors.colorMultiplier;

            if (leftButtonHeld && rightButtonHeld)
            {
                rudderDirection = 0f;

            } else if (leftButtonHeld)
            {
                rudderDirection = 1f;

            } else if (rightButtonHeld)
            {
                rudderDirection = -1f;

            }

        }

        rudder.localRotation = Quaternion.Euler(new Vector3(0f, rudderDirection * maxFlapAngle, 0f));

        float aileronDirection = 0f;

        //Ailerons
        if (!usingKeyboard && !usingOnScreen) //Controller Roll Left and Right
        {
            aileronDirection = -myInDevice.LeftStickX;

        }
        else if (usingKeyboard && Input.GetKey(rollLeft) && !Input.GetKey(rollRight))  //KB Roll left
        {
            aileronDirection = 1f;

        }
        else if (usingKeyboard && Input.GetKey(rollRight) && !Input.GetKey(rollLeft))  //KB Roll Right
        {
            aileronDirection = -1f;

        } else if (usingOnScreen)
        {
            aileronDirection = stick.GetInputDirection().x;

        }

        leftAileron.localRotation = Quaternion.Euler(new Vector3(maxFlapAngle, 0f, 0f) * -aileronDirection + leftAileronRestingRotation.eulerAngles);
        rightAileron.localRotation = Quaternion.Euler(new Vector3(maxFlapAngle, 0f, 0f) * aileronDirection + rightAileronRestingRotation.eulerAngles);

        float elevatorDirection = 0f;

        //Elevators
        if (!usingKeyboard && !usingOnScreen ) //Controller Pitch Up and Down
        {
            elevatorDirection = myInDevice.LeftStickY;

        }
        else if (usingKeyboard && Input.GetKey(pitchUp) && !Input.GetKey(pitchDown))  //KB Pitch Up
        {
            elevatorDirection = -1f;

        }
        else if (usingKeyboard && Input.GetKey(pitchDown) && !Input.GetKey(pitchUp))  //KB Pitch Down
        {
            elevatorDirection = 1f;

        } else if (usingOnScreen)
        {
            elevatorDirection = stick.GetInputDirection().y;

        }

        leftElevator.localRotation = Quaternion.Euler(new Vector3(maxFlapAngle, 0f, 0f) * -elevatorDirection + leftElevatorRestingRotation.eulerAngles);
        rightElevator.localRotation = Quaternion.Euler(new Vector3(maxFlapAngle, 0f, 0f) * -elevatorDirection + rightElevatorRestingRotation.eulerAngles);

    }

    void GetFlapRestingRotation ()
    {
        leftAileronRestingRotation = leftAileron.localRotation;
        rightAileronRestingRotation = rightAileron.localRotation;

        leftElevatorRestingRotation = leftElevator.localRotation;
        rightElevatorRestingRotation = rightElevator.localRotation;

    }

    float GetGameCubeStickY (Vector2 sentCoords)
    {
        float n = Mathf.Cos(Mathf.Deg2Rad * 22.5f) / Mathf.Sin(Mathf.Deg2Rad * 22.5f);
        float m = 1 / n;

        float a = 2.4f;
        float b = 1f;

        float x = sentCoords.x;
        float y = 0;

        if (sentCoords.y >= 0)
        {
            if (-1 <= x && x <= -0.7f)
            {
                y = (n * x) + a;

            } else if (-0.7f < x && x <= 0) 
            {
                y = (m * x) + b;

            } else if (0 < x && x <= 0.7f) 
            {
                y = (-m * x) + b;

            } else if (0.7f < x && x <= 1)
            {
	            y = (-n * x) + a;

            }

        } else if (sentCoords.y < 0) 
        {
            if (-1 <= x && x <= -0.7f)
            {
                y = (-n * x) - a;

            } else if (-0.7f < x && x <= 0) 
            {
                y = (-m * x) - b;

            } else if (0 < x && x <= 0.7f) 
            {
                y = (m * x) - b;

            } else if (0.7f < x && x <= 1)
            {
                y = (n * x) - a;

            }

        }

        return y;

    }

}
