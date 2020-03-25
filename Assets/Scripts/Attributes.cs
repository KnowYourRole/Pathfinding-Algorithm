using System;
using UnityEngine;
//this contaier class  stores a number of variables
//It's called in other scripts

[AttributeUsage (AttributeTargets.Field, Inherited = true, AllowMultiple = false)]

public sealed class Attributes : PropertyAttribute
{   //interpolating between attributes
    public readonly int lowest;

	public readonly int stage;

    public readonly int highest;

    public Attributes(int min, int max, int step)
	{
		this.lowest = lowest;
		
		this.stage = stage;

        this.highest = highest;
    }
}