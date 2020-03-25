using UnityEngine;

// This class sets the directions of the square shape for the grid and shape of tiles

public static class SquareAxis
{
    public static int guidance { get { return directions.Length; } }
    
    private static float[] cost =
	{
		1.0f, Mathf.Sqrt(2.0f),
		1.0f, Mathf.Sqrt(2.0f),
		1.0f, Mathf.Sqrt(2.0f),
		1.0f, Mathf.Sqrt(2.0f)
    };

    private static SquareShape[] directions =
    {
        new SquareShape( 0, 1), new SquareShape( 1, 1),
        new SquareShape( 1, 0), new SquareShape( 1,-1),
        new SquareShape( 0,-1), new SquareShape(-1,-1),
        new SquareShape(-1, 0), new SquareShape(-1, 1)
    };

	public static SquareShape GetDirection(int dir)
	{
        SquareShape s = new SquareShape();
		s.a = directions [ApproximateDirection(dir)].a;
		s.b = directions [ApproximateDirection(dir)].b;
		return s;
	}

	public static SquareShape[] GetDirections()
	{
        SquareShape[] s = new SquareShape[guidance];
		for(int i = 0; i < guidance; i++)
		{
			s[i] = new SquareShape();
			s[i].a = directions[i].a;
			s[i].b = directions[i].b;
		}
		return s;
	}

    public static int ApproximateDirection(int track)
    {
        if (track < 0) return guidance + (track % guidance);
        return track % guidance;
    }

    public static float GetCost(int dir)
	{
		float c = cost[ApproximateDirection(dir)];
		return c;
	}

	//creates a diagonal heuristic with the square shape
	public static float GetHeuristic(SquareShape current, SquareShape goal)
	{
		int distX = Mathf.Abs(current.a - goal.a);
		int distY = Mathf.Abs(current.b - goal.b);
		return (distX >= distY ? distX : distY);
	}
}

public static class NSquareAxis
{
    public static int guidance { get { return directions.Length; } }

    private static float[] cost =
    {
        1.0f,
        1.0f,
        1.0f,
        1.0f
    };

    private static SquareShape[] directions =
	{
		new SquareShape( 0, 1),
		new SquareShape( 1, 0),
		new SquareShape( 0,-1),
		new SquareShape(-1, 0)
	};

    public static SquareShape GetDirection(int dir)
    {
        SquareShape s = new SquareShape();
        s.a = directions[ApproximateDirection(dir)].a;
        s.b = directions[ApproximateDirection(dir)].b;
        return s;
    }

    public static SquareShape[] GetDirections()
    {
        SquareShape[] s = new SquareShape[guidance];
        for (int i = 0; i < guidance; i++)
        {
            s[i] = new SquareShape();
            s[i].a = directions[i].a;
            s[i].b = directions[i].b;
        }
        return s;
    }

    public static int ApproximateDirection(int dir)
	{
		if (dir < 0) return guidance - (dir % guidance);
		return dir % guidance;
	}

	public static float GetCost(int dir)
	{
		float c = cost[ApproximateDirection(dir)];
		return c;
	}

	//This is a Manhattan heuristic style
	public static float GetHeuristic(SquareShape current, SquareShape goal)
	{
		int distX = Mathf.Abs(current.a - goal.a);
		int distY = Mathf.Abs(current.b - goal.b);
		return distX + distY;
	}
}

[System.Serializable]
public class SquareShape : GridPiece
{
	//Boolean for diagonal grid piece
	public bool isDiagonal
	{
		get { return PathfinderManager.type.blockSidewayWall; }
	}

	//Two constructor variables
	public SquareShape(int a, int b) : base(a, b) { }
	public SquareShape() : base(0, 0) { }

	//These are functions for the direction of the square
	public override int guidance
    {
		get { return (isDiagonal ? SquareAxis.guidance : NSquareAxis.guidance); }
	}

	public SquareShape[] GetDirections()
	{
		return (isDiagonal ? SquareAxis.GetDirections() : NSquareAxis.GetDirections());
	}

    public SquareShape GetDirection(int track)
    {
        return (isDiagonal ? SquareAxis.GetDirection(track) : NSquareAxis.GetDirection(track));
    }

    public float GetHeuristic(SquareShape goal)
    {
        return (isDiagonal ? SquareAxis.GetHeuristic(this, goal) : NSquareAxis.GetHeuristic(this, goal));
    }

    public float GetCost(int track)
	{
		return (isDiagonal ? SquareAxis.GetCost(track) : NSquareAxis.GetCost(track));
	}

    public override int ApproximateDirection(int track)
    {
        return (isDiagonal ? SquareAxis.ApproximateDirection(track) : NSquareAxis.ApproximateDirection(track));
    }

	public float GetHeuristic(int a, int b)
	{
        SquareShape goal = new SquareShape(a, b);
		return GetHeuristic(goal);
	}

	public SquareShape getClosestBlock(int track)
	{
        SquareShape neighDir = GetDirection(track);
		return this + neighDir; 
	}

	public SquareShape[] getClosestBlock()
	{
        SquareShape[] closestBlock = GetDirections();
		for(int i = 0; i < closestBlock.Length; i++)
		{
            closestBlock[i] += this; 
		}
		return closestBlock;
	}

	//These are operators of the square shape
	

	public static SquareShape operator /(SquareShape self, SquareShape other)
	{
		return new SquareShape(self.a / other.a, self.b / other.b);
	}

	public static SquareShape operator *(SquareShape self, int other)
	{
		return new SquareShape(self.a * other, self.b * other);
	}

	public static SquareShape operator /(SquareShape self, int other)
	{
		return new SquareShape(self.a / other, self.b / other);
	}

    public static SquareShape operator +(SquareShape self, SquareShape other)
    {
        return new SquareShape(self.a + other.a, self.b + other.b);
    }

    public static SquareShape operator -(SquareShape self, SquareShape other)
    {
        return new SquareShape(self.a - other.a, self.b - other.b);
    }

    public static SquareShape operator *(SquareShape self, SquareShape other)
    {
        return new SquareShape(self.a * other.a, self.b * other.b);
    }

    public static SquareShape operator *(int other, SquareShape self)
	{
		return self * other;
	}

	public static SquareShape operator /(int other, SquareShape self)
	{
		return self / other;
	}
}