using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the grid
/// it allows hexagon and square block type shapes 
/// depending on the grid shape
/// </summary>

public enum BlockGrid
{
	SquareShape = 0,
    HexagonShape,

	Total
}

public class GridManager : MonoBehaviour
{
	//Single instance
	private static GridManager _type;

	public static GridManager type
    {
		get { return _type; }
	}


	[ContextMenuItem("Refresh Grid During Play", "RefreshGrid")]
	public bool refreshDuringPlay = false;

	[Header("Prefabs")]
	public GameObject[] savedBlocks;        //takes the stored prefabs of square and hexagon

	[Range(0.0f, 3.0f)]
	public float blockSize = 1.0f;

    [Header("Block Options")]
    public BlockGrid shapeOfBlock;

    [Range(3, 100)]
	public int _blockNumber = 20;         //value oof blocks (with added range)

	public int blockNumber              //number of blocks spawn (generating the grid)
	{
		get
		{
			if(shapeOfBlock == BlockGrid.HexagonShape) return Mathf.FloorToInt(_blockNumber / 2.0f) + 1;
			else return _blockNumber;
		}
		set
		{
            _blockNumber = value;
		}
	}

    //both variables can be flattened to 1D for easier display
    public GridGeneration[,] squareBlockToGrid;             //generate the gird by using square blocks
    public GridGeneration[,] HexagonShapeBoard;

    [Header("Editor")]
    public Transform squareBoardParent;
    public Transform hexagonShapeBoardParent;

    public GridGeneration[,] blockToGrid            //generate the grid by spawning blocks
	{
		get
		{
			if(shapeOfBlock == BlockGrid.SquareShape) return squareBlockToGrid;
			else if(shapeOfBlock == BlockGrid.HexagonShape) return HexagonShapeBoard;
			return null;
		}
		set
		{
			if(shapeOfBlock == BlockGrid.SquareShape) squareBlockToGrid = value;
			else if(shapeOfBlock == BlockGrid.HexagonShape) HexagonShapeBoard = value;
		}
	}

    public float distanceHeight     //distance between blocks in the square shape grid (Height)
    {
        get
        {
            switch (shapeOfBlock)
            {
                case BlockGrid.SquareShape:
                    return gridHeight;
                case BlockGrid.HexagonShape:
                    return gridHeight;
            }
            return gridHeight / 2.0f;
        }
    }

    //get variables
    public float  gridWidth         //total grid width by using sqaure shape
	{
		get
		{
			switch(shapeOfBlock)
			{
				case BlockGrid.SquareShape:
					return blockSize;
				case BlockGrid.HexagonShape:
					return blockSize;
			}
			return blockSize;
		}
	}

	public float  distanceWidth     //distance between blocks in the square shape grid (Width) 
	{
		get
		{
			switch(shapeOfBlock)
			{
				case BlockGrid.SquareShape:
					return gridWidth;
				case BlockGrid.HexagonShape:
					return gridWidth * 3.0f / 4.0f;
			}
			return gridWidth / 2.0f;
		}
	}

    public float gridHeight         //total grid hight by using square shape
    {
        get
        {
            switch (shapeOfBlock)
            {
                case BlockGrid.SquareShape:
                    return blockSize;
                case BlockGrid.HexagonShape:
                    return Mathf.Sqrt(3.0f) / 2.0f * gridWidth;
            }
            return blockSize;
        }
    }
  	
	void Awake()
	{
		//Single instance
		if(_type == null)
            _type = this;
		else
			Destroy(this.gameObject);
	}

	void Start ()
	{
        resetGrid();        //It's reseting the grid on a number of occastions (On start)
	}

	void Update ()
	{
		if(refreshDuringPlay)
            resetGrid();        //It's reseting the grid on a number of occastions (On button)
    }

	public void generateSquareSizeGrid(int size)        //keeping the same size
	{
		if (size <= 0) return;

		int prevSize = 0; //Square shaped board is null when 'prevRadius' <= 0

		if(blockToGrid != null)
			prevSize = blockToGrid.GetLength(0);

		if(size < prevSize)
		{
			for (int i = 0; i < prevSize; i++)
			{
				for (int j = 0; j < prevSize; j++)
				{
					if(i >= size || j >= size)
					{
						Destroy(blockToGrid[i, j].gameObject);
					}
				}
			}
		}

        SquareShapeScript[,] newSquareBoard = new SquareShapeScript[size, size];

		//Search for arrays
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				if(i < prevSize && j < prevSize)
				{
					//itinerate
					newSquareBoard[i, j] = (SquareShapeScript)blockToGrid[i, j];
				}
				else
				{
					//Instantiation
					newSquareBoard[i, j] = Instantiate(savedBlocks[(int)shapeOfBlock]).GetComponent<SquareShapeScript>();
					newSquareBoard[i, j].transform.parent = squareBoardParent;
					
				}

			
				newSquareBoard[i, j].transform.localScale = Vector3.one * blockSize;
				newSquareBoard[i, j].transformValue = new SquareShape(j, i);

				newSquareBoard[i, j]._isScannedArea = false;
				newSquareBoard[i, j].isWall = false;
				newSquareBoard[i, j].trackPos = Mathf.Infinity;
				newSquareBoard[i, j].groupBlock = null;
				newSquareBoard[i, j].currentSit = blockEnum.baseBlock;
			}
		}

        //the new square board is saved
        blockToGrid = newSquareBoard;
		return;
	}

    //Functions
    public void generateGridBlocs(int size) //generates the type of blocks by switching between square and hexagon type shapes
    {
        switch (shapeOfBlock)
        {
            case BlockGrid.SquareShape:
                generateSquareSizeGrid(size);
                break;
            case BlockGrid.HexagonShape:
                GenerateHexagonRadiusGrid(size);
                break;
        }
    }

    public void GenerateHexagonRadiusGrid(int radius)  //keeping the radius
	{
		if (radius <= 0) return;

		int prevRadius = 0; //hexagon shape board is null when 'prevRadius' <= 0

		if(blockToGrid != null)
			prevRadius = (blockToGrid.GetLength(0) - 1) / 2 + 1;

		if(radius < prevRadius)
		{
			for (int i = 0; i < prevRadius; i++)
			{
                HexagonShape nextHexagonShape = HexagonAxis.GetDirection(4) * i;
				for(int dir = 0; dir < 6; dir++)
				{
					for (int j = 0; j < i; j++)
					{
						if(i >= radius)
						{
						
							
							Destroy(blockToGrid[nextHexagonShape.ToArrayPos(prevRadius).b, nextHexagonShape.ToArrayPos(prevRadius).a].gameObject);

                            nextHexagonShape = nextHexagonShape.GetNeighbour(dir);
						}
					}
				}
			}
		}

        HexagonShapeScript[,] newHexagonShapeBoard = new HexagonShapeScript[(radius - 1) * 2 + 1, (radius - 1) * 2 + 1];

		//generate blocks in the center
		if(0 < prevRadius)
		{
            //itinerate
            newHexagonShapeBoard[0, 0] = (HexagonShapeScript)blockToGrid[0, 0];
		}
		else
		{
            //Instantiate
            newHexagonShapeBoard[0, 0] = Instantiate(savedBlocks[(int)shapeOfBlock]).GetComponent<HexagonShapeScript>();
            newHexagonShapeBoard[0, 0].transform.parent = hexagonShapeBoardParent;
		}

        newHexagonShapeBoard[0, 0].transform.localScale = Vector3.one * blockSize;
        newHexagonShapeBoard[0, 0].transformValueHexagon = new HexagonShape(0, 0);

        newHexagonShapeBoard[0, 0]._isScannedArea = false;
        newHexagonShapeBoard[0, 0].isWall = false;
        newHexagonShapeBoard[0, 0].trackPos = Mathf.Infinity;
        newHexagonShapeBoard[0, 0].groupBlock = null;
        newHexagonShapeBoard[0, 0].currentSit = blockEnum.baseBlock;

		
		for (int i = 1; i < radius; i++)
		{
            HexagonShape nextHexagonShape = HexagonAxis.GetDirection(4) * i;
			for(int dir = 0; dir < 6; dir++)
			{
				for (int j = 0; j < i; j++)
				{
                    HexagonShape arrayPos = nextHexagonShape.ToArrayPos(radius);
                    HexagonShape oldArrayPos = nextHexagonShape.ToArrayPos(prevRadius);

					if(i < prevRadius)
					{
                        //Itinerate
                        newHexagonShapeBoard[arrayPos.b, arrayPos.a] = (HexagonShapeScript)blockToGrid[oldArrayPos.b, oldArrayPos.a];
						
					}
					else
					{
                        //Instantiating
                        newHexagonShapeBoard[arrayPos.b, arrayPos.a] = Instantiate(savedBlocks[(int)shapeOfBlock]).GetComponent<HexagonShapeScript>();
                        newHexagonShapeBoard[arrayPos.b, arrayPos.a].transform.parent = hexagonShapeBoardParent;
						
					}

                   
                    newHexagonShapeBoard[arrayPos.b, arrayPos.a].transform.localScale = Vector3.one * blockSize;
                    newHexagonShapeBoard[arrayPos.b, arrayPos.a].transformValueHexagon = new HexagonShape(nextHexagonShape.a, nextHexagonShape.b);

                    newHexagonShapeBoard[arrayPos.b, arrayPos.a]._isScannedArea = false;
                    newHexagonShapeBoard[arrayPos.b, arrayPos.a].isWall = false;
                    newHexagonShapeBoard[arrayPos.b, arrayPos.a].trackPos = Mathf.Infinity;
                    newHexagonShapeBoard[arrayPos.b, arrayPos.a].groupBlock = null;
                    newHexagonShapeBoard[arrayPos.b, arrayPos.a].currentSit = blockEnum.baseBlock;

                    nextHexagonShape = nextHexagonShape.GetNeighbour(dir);
				}
			}
		}

        //Save the new hexagon shape board
        blockToGrid = newHexagonShapeBoard;
		return;
	}

	public GridGeneration getBlocks(int x, int y)
	{
		if(shapeOfBlock == BlockGrid.SquareShape)
		{
			return blockToGrid[y, x];
		}
		else if(shapeOfBlock == BlockGrid.HexagonShape)
		{
            HexagonShape h = new HexagonShape(x, y);
			h = h.ToArrayPos(blockNumber); 
			return blockToGrid[h.b, h.a];
		}

		return null;
	}

    public void resetGrid(bool isStartingPointSet = false)       //reseting the grid function for both hexagon and square shape grid checkes if the starting point is set 
    {
        generateGridBlocs(blockNumber);

        switch (shapeOfBlock)
        {
            case BlockGrid.SquareShape:
                squareBoardParent.gameObject.SetActive(true);
                hexagonShapeBoardParent.gameObject.SetActive(false);
                break;
            case BlockGrid.HexagonShape:
                hexagonShapeBoardParent.gameObject.SetActive(true);
                squareBoardParent.gameObject.SetActive(false);
                break;
        }

        PathfinderManager.type.locatePath.gameObject.SetActive(false);

        if (!isStartingPointSet)
            PathfinderManager.type.StartingPosition();
    }

}
