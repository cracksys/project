using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTank
{
    private Ship ship;

    public float _fuelLevel;
    public float _maxFuel;
    
    public FuelTank(Ship _ship)
    {
        ship = _ship;

        _fuelLevel = ship._shipFuelLevel;
        _maxFuel = ship._shipMaxFuel;
    }


    public bool AddFuel(float fuel)
    {
        if (IsFull())
        {
            return false;
        }
        else
        {
            _fuelLevel += fuel;
            ship._shipFuelLevel = _fuelLevel;
            return true;
        }
    }

    public bool RemoveFuel(float fuel)
    {
        if (_fuelLevel - fuel > 0)
        {
            _fuelLevel -= fuel;
            ship._shipFuelLevel = _fuelLevel;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsFull()
    {
        if (_fuelLevel >= _maxFuel)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
