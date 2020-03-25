// This class contains information about grid blocks that are later used in other scripts

public abstract class GridPiece
{
    //variables
    public abstract int guidance { get; }   

    public abstract int ApproximateDirection(int track);   

    public int a;
    
    public int b;

	public GridPiece(int a, int b)
	{
		this.a = a;
		this.b = b;
	}
}