using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// this class is a container for the hexagon shapes 
/// It holds the position information of the shape
/// </summary>

public class HexagonShapeScript : GridGeneration
{
	protected HexagonShape _transformValueHexagon = new HexagonShape();

    //Initialise
    void Awake()
    {
        a = transformValueHexagon.a;
        b = transformValueHexagon.b;
    }

    public HexagonShape transformValueHexagon
    {
		get { return _transformValueHexagon; }
		set
		{
            _transformValueHexagon = value;

            AssociateLocation();

			a = _transformValueHexagon.a;
			b = _transformValueHexagon.b;
		}
	}

	protected override void AssociateLocation()     //this function is allocating the  position of the next spawned block for the hexagon type shapes
	{
		Vector3 offsetY = new Vector3(-0.0f, -GridManager.type.distanceHeight, 0.01f) * _transformValueHexagon.b;

        Vector3 offsetX = new Vector3(GridManager.type.distanceWidth, -GridManager.type.distanceHeight / 2.0f, 0.0f) * _transformValueHexagon.a;

        transform.localPosition = offsetX + offsetY;
	}
}
