using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private GameObject[] planets;

    public List<Ships> shipTypes;

    public string displayName, type;
    public int fuelCap;
    public int hullMax;
    public float maxSpeed;
    public float size;

    public float currentFuel;
    public float currentCredit;

    private bool isUnitEnabled;
    private bool isUnitMoving;
    private bool isUnitDrifting;
    private bool isUnitDocked;
    private bool isTradeComplete;
    private bool isFuelingComplete;

    int tickWhenLanded;
    int tickCurrent;

    private Vector3 scale;

    void Awake()
    {
        GetShipClass();

        scale = new Vector3(transform.localScale.x * size, transform.localScale.y * size, transform.localScale.z * size);
        transform.localScale = scale;

        planets = GameObject.FindGameObjectsWithTag("Planet");

        isUnitEnabled = false;
        isUnitDocked = false;
        isUnitDrifting = false;
        isTradeComplete = false;
        isFuelingComplete = true;

        Renderer rend = GetComponent<Renderer>();
        Color color = new Color(Random.value, Random.value, Random.value, 1.0f);

        rend.material.SetColor("_Color", color);

        TimeTickSystem.OnTick += delegate (object sender, TimeTickSystem.OnTickEventArgs e)
        {
            tickCurrent = e.tick;
            ConsumeFuel();
            DecisionMaking(tickCurrent);
        };
    }

    void DecisionMaking(int tick)
    {
        if (isUnitDocked && currentFuel <= fuelCap * 0.3f)
        {
            StartCoroutine(PurchaseFuel());
        }

        if (isUnitDocked && currentFuel > fuelCap * 0.3f)
        {
            isFuelingComplete = true;
        }

        if (!isUnitMoving && isFuelingComplete)
        {
            Vector3 destination = planets[Random.Range(0, planets.Length)].transform.position;
            Vector3 origin = transform.position;

            isUnitMoving = true;
            isUnitDocked = false;

            StartCoroutine(TravelTo(origin, destination));
        }
    }

    void ConsumeFuel()
    {
        if (isUnitMoving)
        {
            currentFuel -= 1;
        }
    }

    IEnumerator TravelTo(Vector3 origin, Vector3 destination)
    {
        while (transform.position != destination)
        {
            if (currentFuel > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, maxSpeed / 100);
                yield return null;
            }
            else if (currentFuel <= 0)
            {
                isUnitDrifting = true;
                isUnitMoving = false;
                yield return null;
            }
        }

        if (transform.position == destination)
        {
            isUnitMoving = false;
            isUnitDocked = true;
            isFuelingComplete = false;
            tickWhenLanded = tickCurrent;
        }
    }

    IEnumerator PurchaseFuel()
    {
        while (currentCredit > 0 && currentFuel < fuelCap)
        {
            currentCredit -= 1;
            currentFuel += 1;

            yield return null;
        }

        isFuelingComplete = true;
    }

    private void GetShipClass()
    {
        int index = Random.Range(0,shipTypes.Count);

        displayName = shipTypes[index].displayName;
        type = shipTypes[index].type;
        fuelCap = shipTypes[index].fuelCap;
        hullMax = shipTypes[index].hullMax;
        maxSpeed = shipTypes[index].maxSpeed;
        size = shipTypes[index].size / 5;

        currentFuel = fuelCap;
        currentCredit = Random.Range(10, 100);
    }
}
