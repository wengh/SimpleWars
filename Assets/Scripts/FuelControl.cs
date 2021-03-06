﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FuelControl : MonoBehaviour
{
    public Text force;
    public Text altitude;
    public Text velocity;
    public Slider slider100;
    public Slider slider10;
    public Image currentFuel;
    public short fuelHeight;
    public GameObject vehicle;
    public float currentForce = 0;
    [HideInInspector]
    public bool turnLeft;
    [HideInInspector]
    public bool turnRight;

    Animator mask;
    float totalFuel = 0;
    float fuel = 0;
    Tank[] tanks;
    float slider100last = 0;
    float slider10last = 0;
    Transform pod;

    void Awake ()
    {
        mask = GameObject.Find("SceneTransitionMask").GetComponent<Animator>();
        StartCoroutine(VehicleLoader.Wait(0.1f, delegate
        {
            mask.SetBool("open", true);
        }));
    }

    void Start ()
    {
        vehicle = GameObject.Find("Vehicle");
        pod = vehicle.transform.GetChild(0);
        tanks = vehicle.GetComponentsInChildren<Tank>();
        foreach (Tank tank in tanks)
        {
            totalFuel += tank.capacity;
        }
    }

    void FixedUpdate ()
    {
        fuel = 0;
        if (tanks != null)
        {
            foreach (Tank tank in tanks)
            {
                fuel += tank.fuel;
            }
        }
    }

    void Update ()
    {
        if (hardInput.GetKeyUp("Quit"))
        {
            mask.SetBool("open", false);
            StartCoroutine(SceneLoader.LoadSceneAnim(mask, delegate
            {
                SceneManager.LoadScene("Builder");
                Destroy(GameObject.Find("Vehicle"));
            }));
        }
        if (pod != null && pod.GetComponent<Rigidbody2D>() != null)
        {
            altitude.text = (pod.position.y * (5f / 3)).ToString("N0") + " m";
            velocity.text = (pod.GetComponent<Rigidbody2D>().velocity.magnitude * (5f / 3)).ToString("N0") + " m/s";
        }
        if (hardInput.GetKey("Turn Left"))
        {
            turnLeft = true;
        }
        else if (hardInput.GetKeyUp("Turn Left"))
        {
            turnLeft = false;
        }
        if (hardInput.GetKey("Turn Right"))
        {
            turnRight = true;
        }
        else if (hardInput.GetKeyUp("Turn Right"))
        {
            turnRight = false;
        }
        if (slider100last != slider100.value)
        {
            currentForce = slider100.value / 100;
            slider10.value = Mathf.RoundToInt(currentForce * 10);
        }
        else if (slider10last != slider10.value)
        {
            currentForce = slider10.value / 10;
            slider100.value = Mathf.RoundToInt(currentForce * 100);
        }
        else if (hardInput.GetKeyDown("Increase Throttle") && currentForce < 0.99)
        {
            currentForce += 0.1f;
            slider10.value = Mathf.RoundToInt(currentForce * 10);
            slider100.value = Mathf.RoundToInt(currentForce * 100);
        }
        else if (hardInput.GetKeyDown("Decrease Throttle") && currentForce > 0.01)
        {
            currentForce -= 0.1f;
            slider10.value = Mathf.RoundToInt(currentForce * 10);
            slider100.value = Mathf.RoundToInt(currentForce * 100);
        }
        force.text = (Mathf.RoundToInt(currentForce * 100)).ToString() + "%";
        currentFuel.rectTransform.offsetMax = new Vector2(currentFuel.rectTransform.offsetMax.x, (fuel / totalFuel) * fuelHeight - fuelHeight);
        slider100last = slider100.value;
        slider10last = slider10.value;
    }

    public void TurnLeft ()
    {
        turnLeft = true;
    }
    public void StopTurnLeft ()
    {
        turnLeft = false;
    }
    public void TurnRight ()
    {
        turnRight = true;
    }
    public void StopTurnRight ()
    {
        turnRight = false;
    }
}
