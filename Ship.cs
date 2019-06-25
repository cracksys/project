using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public List<GameObject> listOfPlanets;
    public List<ShipType> shipTypes;

    public string _shipType, _shipDescription;
    public int _shipMaxFuel, _shipMaxHull;
    public float _shipMaxSpeed, _shipSize;

    public float _shipFuelLevel, _shipCreditLevel;

    [SerializeField]
    private bool _isShipDocked;
    [SerializeField]
    private bool _isShipFlying;
    [SerializeField]
    private bool _isShipBusy;
    [SerializeField]
    private bool _isShipRefueling;

    private int _tickWhenLastDocked;
    private int _tickLast;
    private int _tickCurrent;

    private const float FUEL_CONSUMPTION_RATE = 1.0f;
    private const float FUEL_REFUEL_RATE = 1.0f;
    private const float CREDIT_PER_FUEL = 1.0f;

    private FuelTank fuelTank;
    private Agent agent;

    private void Awake()
    {
        InitListOfPlanets();
        InitShipStat();
        InitShipStatus();
        InitShipModel();
        InitTimeTick();

        fuelTank = new FuelTank(this);
        agent = new Agent(this);
    }

    private void InitTimeTick()
    {
        TimeTickSystem.OnTick += delegate (object sender, TimeTickSystem.OnTickEventArgs e)
        {
            _tickLast = _tickCurrent;
            _tickCurrent = e.tick;

            if (_isShipFlying)
            {
                ConsumeFuel();
            }

            if (_isShipRefueling)
            {
                Refuel();
            }
            
            MakeDecision();
        };
    }

    private void MakeDecision()
    {
        const float CRITICAL_FUEL_PERCENTAGE = 0.5f;

        if (_isShipDocked && !_isShipBusy)
        {
            if (_shipFuelLevel < _shipMaxFuel * CRITICAL_FUEL_PERCENTAGE)
            {
                _isShipRefueling = true;
                _isShipBusy = true;
                
            }
        }

        if (_isShipRefueling)
        {
            float _lower = Mathf.Min(_shipMaxFuel - _shipFuelLevel, _shipCreditLevel);

            if (_lower <= 0f)
            {
                _isShipRefueling = false;
                _isShipBusy = false;
            }
        }

        if (!_isShipFlying && !_isShipBusy)
        {
            Vector3 destination = listOfPlanets[Random.Range(0, listOfPlanets.Count)].transform.position;
            Vector3 origin = transform.position;

            StartCoroutine(TravelToPlanet(origin, destination));
        }
    }

    private void ConsumeFuel()
    {
        fuelTank.RemoveFuel(FUEL_CONSUMPTION_RATE);
    }

    private void Refuel()
    {
        fuelTank.AddFuel(FUEL_REFUEL_RATE);
        _shipCreditLevel -= CREDIT_PER_FUEL;
    }

    IEnumerator TravelToPlanet(Vector3 originPlanet, Vector3 destinationPlanet)
    {
        const float SHIP_SPEED_NERF = 0.01f;

        while (transform.position != destinationPlanet)
        {
            if (_shipFuelLevel > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, destinationPlanet, _shipMaxSpeed * SHIP_SPEED_NERF);
                _isShipFlying = true;
                _isShipDocked = false;
            }

            else if (_shipFuelLevel <= 0)
            {
                _isShipFlying = false;
                _isShipDocked = false;
            }

            yield return null;
        }

        if (transform.position == destinationPlanet)
        {
            _isShipDocked = true;
            _isShipFlying = false;
        }
    }

    private void InitListOfPlanets()
    {
        listOfPlanets.AddRange(GameObject.FindGameObjectsWithTag("Planet"));
    }

    private void InitShipStat()
    {
        int _index = Random.Range(0, shipTypes.Count);

        _shipType = shipTypes[_index].displayName;
        _shipDescription = shipTypes[_index].type;
        _shipMaxFuel = shipTypes[_index].fuelCap;
        _shipMaxHull = shipTypes[_index].hullMax;
        _shipMaxSpeed = shipTypes[_index].maxSpeed;
        _shipSize = shipTypes[_index].size;
    }

    private void InitShipStatus()
    {
        const float FUEL_START_PERCENTAGE = 0.5f;
        const float CREDIT_START_MIN = 10f;
        const float CREDIT_START_MAX = 50f;

        _shipFuelLevel = (int)Random.Range(_shipMaxFuel * FUEL_START_PERCENTAGE, _shipMaxFuel);
        _shipCreditLevel = (int)Random.Range(CREDIT_START_MIN, CREDIT_START_MAX);

        _isShipFlying = false;
        _isShipDocked = false;
        _isShipBusy = false;
        _isShipRefueling = false;
    }

    private void InitShipModel()
    {
        const float SHIP_SIZE_NERF = 0.1f;

        Renderer rend = GetComponent<Renderer>();
        Color color = new Color(Random.value, Random.value, Random.value, 1.0f);
        rend.material.SetColor("_Color", color);

        Vector3 scale = new Vector3(transform.localScale.x * _shipSize, transform.localScale.y * _shipSize, transform.localScale.z * _shipSize);
        transform.localScale = scale * SHIP_SIZE_NERF;
    }

}
