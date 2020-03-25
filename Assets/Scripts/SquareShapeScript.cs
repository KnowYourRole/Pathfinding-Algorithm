using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class acts as a container for the square shapes
/// And it holds the position information of the shape
/// </summary>
public class SquareShapeScript : GridGeneration
{
	protected SquareShape _transformValue = new SquareShape();

    //initialise 
    void Awake()
    {
        a = transformValue.a;
        b = transformValue.b;
    }

    public SquareShape transformValue
	{
		get { return _transformValue; }
		set
		{
            _transformValue = value;

            AssociateLocation();

			a = _transformValue.a;
			b = _transformValue.b;
		}
	}

	protected override void AssociateLocation()
	{
		float offsetX = (_transformValue.a + 0.5f - (GridManager.type.blockNumber / 2.0f)) * GridManager.type.distanceWidth;
		float offsetY = (_transformValue.b + 0.5f - (GridManager.type.blockNumber / 2.0f)) * GridManager.type.distanceHeight;
		transform.localPosition = new Vector3(offsetX, offsetY, 0.0f);
	}
}
