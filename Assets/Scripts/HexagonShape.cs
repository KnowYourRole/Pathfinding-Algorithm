using UnityEngine;
/// <summary>
/// This class gets the hexagon directions, making sure they are well positioned
/// </summary>
public static class HexagonAxis
{
    public static int totalDirections { get { return transLocation.Length; } }

    private static float[] cost =
    {
        1.0f, 1.0f, 1.0f,
        1.0f, 1.0f, 1.0f
    }; 

    private static HexagonShape[] transLocation =
	{
		new HexagonShape( 0,-1), new HexagonShape( 1,-1), new HexagonShape( 1, 0),
		new HexagonShape( 0, 1), new HexagonShape(-1, 1), new HexagonShape(-1, 0)
	};

    public static HexagonShape GetDirection(int dir)
    {
        HexagonShape h = new HexagonShape();
        h.a = transLocation[RoundToDir(dir)].a;
        h.b = transLocation[RoundToDir(dir)].b;
        return h;
    }

    public static int RoundToDir(int dir)
	{
		if (dir < 0) return totalDirections + (dir % totalDirections);
		return dir % totalDirections;
	}

	public static float GetCost(int dir)
	{
		float c = cost[RoundToDir(dir)];
		return c;
	}

    public static HexagonShape[] GetDirections()
    {
        HexagonShape[] h = new HexagonShape[totalDirections];
        for (int i = 0; i < totalDirections; i++)
        {
            h[i] = new HexagonShape();
            h[i].a = transLocation[i].a;
            h[i].b = transLocation[i].b;
        }
        return h;
    }

    public static float GetHeuristic(HexagonShape current, HexagonShape goal)
	{
		int distX = Mathf.Abs(current.a - goal.a);
		int distY = Mathf.Abs(current.b - goal.b);

		//x + y + z = 0
		int current_z = current.a + current.b;
		int goal_z = goal.a + goal.b;

		int distZ = Mathf.Abs(current_z - goal_z);
		return (distX + distY + distZ) / 2.0f;
	}
}

[System.Serializable]
public class HexagonShape : GridPiece
{
	//producing the shapes
	public HexagonShape(int x, int y) : base(x, y) { }
	public HexagonShape() : base(0, 0) { }

	//Setting the functions
	public HexagonShape GetDirection(int dir)
	{
		return HexagonAxis.GetDirection(dir);
	}

    public override int guidance
    {
        get { return HexagonAxis.totalDirections; }
    }

    public override int ApproximateDirection(int dir)
    {
        return HexagonAxis.RoundToDir(dir);
    }

	public float GetCost(int dir)
	{
		return HexagonAxis.GetCost(dir);
	}

    public HexagonShape[] GetDirections()
    {
        return HexagonAxis.GetDirections();
    }

    public float GetHeuristic(HexagonShape goal)
	{
		return HexagonAxis.GetHeuristic(this, goal);
	}

	public HexagonShape GetNeighbour(int dir)
	{
        HexagonShape neighDir = GetDirection(dir);
		return this + neighDir; 
	}

    public float GetHeuristic(int x, int y)
    {
        HexagonShape goal = new HexagonShape(x, y);
        return GetHeuristic(goal);
    }

    public HexagonShape[] GetNeighbours()
	{
        HexagonShape[] neighDirs = GetDirections();
		for(int i = 0; i < neighDirs.Length; i++)
		{
			neighDirs[i] += this;
		}
		return neighDirs;
	}

	//Functions that are operators
	public static HexagonShape operator -(HexagonShape self, HexagonShape other)
	{
		return new HexagonShape(self.a - other.a, self.b - other.b);
	}

    public static HexagonShape operator /(HexagonShape self, HexagonShape other)
    {
        return new HexagonShape(self.a / other.a, self.b / other.b);
    }

    public static HexagonShape operator *(HexagonShape self, HexagonShape other)
	{
		return new HexagonShape(self.a * other.a, self.b * other.b);
	}

    public static HexagonShape operator +(HexagonShape self, HexagonShape other)
    {
        return new HexagonShape(self.a + other.a, self.b + other.b);
    }

    public static HexagonShape operator /(HexagonShape self, int other)
    {
        return new HexagonShape(self.a / other, self.b / other);
    }

    public static HexagonShape operator *(HexagonShape self, int other)
	{
		return new HexagonShape(self.a * other, self.b * other);
	}

    public static HexagonShape operator /(int other, HexagonShape self)
    {
        return self / other;
    }

    public static HexagonShape operator *(int other, HexagonShape self)
	{
		return self * other;
	}
}

public static class AxialHexExtension
{
	public static HexagonShape ToArrayPos(this HexagonShape self, int radius)
	{
		if(self.a == 0 && self.b == 0) return self;
		return new HexagonShape(self.a + radius - 1, self.b + radius - 1);
	}
}