using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class manages pathfinder
/// It checks wheather the pathfinder has started, and finds the closest way to the end block
/// If the pathfinder is paused, it allows the 'Next Step' button to be pressed 
/// The script makes sure thatpathfinder will find the closest way to the end
/// This class also ensures that isAddingObstacle is deleted on pressed
/// </summary>
public enum PathDecisionAlgorithm
{
	DijkstraStyleAlgorithm, 
	AStarStyleAlgorithm
}

public enum pathLocatingProp
{
    setting = 0,
    GetAccess,
    recreatePath,
    completeSpawn
}

public class PathfinderManager : MonoBehaviour
{

    [Header("Current State")]
    [HideInInspector]
    public bool isPause = false;

    public pathLocatingProp PathLocatingStyle;  //allows the user to change the pathfidning style

    [HideInInspector]
    public bool isSearching = false;            //is the pathfinding search activated
   

    [Header("Options")]
    [HideInInspector]
    public PathDecisionAlgorithm algorithmType;
    public bool ignoreSidewayWall;              //ignore diagonals (allow crossing diagonals while pathfinding)
    public bool blockSidewayWall;               //doesn't allow diagonal crosing
    [Range(1, 10)]
    public int weight = 1;
    public float generationTime = 0.0f;         //shows total time that took to generate

    public float Delay = 0.01f;                 //shows the pathfinding delay (It allows users to see the pathfidning generation process piece by piece. It also allows delay during ai finder)
   
    public LineRenderer locatePath;

    //Pathfinder
    GridGeneration[,] blockGrid;
    [HideInInspector]
    public GridGeneration StartPosition;
    [HideInInspector]
    public GridGeneration EndPosition;
    BlockGrid BlockStyle;
    [HideInInspector]
    List<GridGeneration> generationList;

    //Single instance
    private static PathfinderManager _type;
	public static PathfinderManager type
	{
		get { return _type; }
	}

	void Awake()
	{
		//Single instance
		if(_type == null)
            _type = this;
		else
			Destroy(this.gameObject);
	}

	void Start()
	{
        locatePath.gameObject.SetActive(false);
	}

    public void MoveOnce()  //Move only once (when the game is paused)
    {
        switch (PathLocatingStyle)
        {
            case pathLocatingProp.setting:
                setting();
                break;
            case pathLocatingProp.GetAccess:
                GetAccess();
                break;
            case pathLocatingProp.recreatePath:
                recreatePath();
                break;
            case pathLocatingProp.completeSpawn:
                break;
        }
    }

    void Update()
	{
		if(isSearching)
		{
			if(!isPause)
			{
                generationTime += Time.deltaTime;
				if(generationTime >= Delay)
				{
                    MoveOnce();
                    generationTime = 0.0f;
				}
			}
		}
	}

	
	public void StartingPosition()
	{
		if(GridManager.type.shapeOfBlock == BlockGrid.SquareShape)
		{
            StartPosition = GridManager.type.getBlocks(Mathf.RoundToInt(GridManager.type.blockNumber / 4.0f) - 1, Mathf.FloorToInt(GridManager.type.blockNumber / 2.0f));
            StartPosition.currentSit = blockEnum.startBlock;

            EndPosition = GridManager.type.getBlocks(Mathf.RoundToInt(GridManager.type.blockNumber / 4.0f * 3.0f), Mathf.FloorToInt(GridManager.type.blockNumber / 2.0f));
            EndPosition.currentSit = blockEnum.endBlock;
		}
		else if(GridManager.type.shapeOfBlock == BlockGrid.HexagonShape)
		{
            StartPosition = GridManager.type.getBlocks(-Mathf.RoundToInt(GridManager.type.blockNumber / 2.0f), 0);
            StartPosition.currentSit = blockEnum.startBlock;

            EndPosition = GridManager.type.getBlocks(Mathf.RoundToInt(GridManager.type.blockNumber / 2.0f), 0);
            EndPosition.currentSit = blockEnum.endBlock;
		}
	}

	public void CleanBoard()
	{
        //Removes the retrace line from the grid 
        locatePath.gameObject.SetActive(false);

        //Everything on the grid is reset including the obstacles
        blockGrid = GridManager.type.blockToGrid;

		for(int i = 0; i < blockGrid.GetLength(0); i++)
		{
			for(int j = 0; j < blockGrid.GetLength(1); j++)
			{
				if(blockGrid[i, j] == null) continue;
                blockGrid[i, j]._isScannedArea = false;
                blockGrid[i, j].trackPos = Mathf.Infinity;
                blockGrid[i, j].groupBlock = null;
				if(blockGrid[i, j].isWall) continue;
                blockGrid[i, j].currentSit = blockEnum.baseBlock;
			}
		}

        //Origin and destination states are set
        StartPosition.currentSit = blockEnum.startBlock;
        EndPosition.currentSit = blockEnum.endBlock;
	}

    public void FindPath(GridGeneration StartPosition, GridGeneration EndPosition)
    {
        //checks the origin and destination of the tiles and findes the path
        this.StartPosition = StartPosition;
        this.EndPosition = EndPosition;
        GeneratePathfinding();
    }

    public void GeneratePathfinding()
    {
        PathLocatingStyle = pathLocatingProp.setting;
        isSearching = true;
        isPause = false;
    }

    void setting()
	{
		//It calls the clear the board function
		CleanBoard();

        //Getting the used tile type
        BlockStyle = GridManager.type.shapeOfBlock;

        //A new list is set up
        generationList = new List<GridGeneration>();

        //The origin movement cost is set to 0 and it's added to open list
        StartPosition.trackPos = 0.0f;
        generationList.Add(StartPosition);

        //If stage function is on, this line helps it to find the next stage
        PathLocatingStyle = (pathLocatingProp)((int)PathLocatingStyle + 1);
	}

    void GetAccess()
    {
        //If open list count is empty
        if (generationList.Count > 0 && !EndPosition._isScannedArea)
        {
            //It sets the current block as first on the list
            GridGeneration currentTile = generationList[0];

            //The Block with the smallest movement cost is found
            for (int i = 1; i < generationList.Count; i++)
            {
                float newHeuristic = 0.0f;
                float curHeuristic = 0.0f;

                if (algorithmType == PathDecisionAlgorithm.AStarStyleAlgorithm)
                {
                    if (BlockStyle == BlockGrid.SquareShape)
                    {
                        SquareShape newSquareShape = ((SquareShapeScript)generationList[i]).transformValue;
                        SquareShape curSquareShape = ((SquareShapeScript)currentTile).transformValue;

                        newHeuristic = newSquareShape.GetHeuristic(EndPosition.a, EndPosition.b);
                        curHeuristic = curSquareShape.GetHeuristic(EndPosition.a, EndPosition.b);
                    }
                    else if (BlockStyle == BlockGrid.HexagonShape)
                    {
                        HexagonShape newHexagonShape = ((HexagonShapeScript)generationList[i]).transformValueHexagon;
                        HexagonShape curHexagonShape = ((HexagonShapeScript)currentTile).transformValueHexagon;

                        newHeuristic = newHexagonShape.GetHeuristic(EndPosition.a, EndPosition.b);
                        curHeuristic = curHexagonShape.GetHeuristic(EndPosition.a, EndPosition.b);
                    }
                    newHeuristic *= weight;
                    curHeuristic *= weight;



                }

                if (generationList[i].trackPos + newHeuristic < currentTile.trackPos + curHeuristic)
                {
                    currentTile = generationList[i];
                }
            }

            //It removes the current Block from open list
            currentTile._isScannedArea = true;
            generationList.Remove(currentTile);

            if (BlockStyle == BlockGrid.SquareShape)
            {
                //Neighbours of current block are taken
                SquareShape s = ((SquareShapeScript)currentTile).transformValue;
                SquareShape[] neighbours = s.getClosestBlock();

                for (int i = 0; i < neighbours.Length; i++)
                {
                    //Coordinate valid or not
                    if
                    (
                        neighbours[i].b < 0 || neighbours[i].b >= blockGrid.GetLength(0) ||
                        neighbours[i].a < 0 || neighbours[i].a >= blockGrid.GetLength(1)
                    )
                    {
                        continue;
                    }

                    //The coordinaes are transformed to grid piece script
                    GridGeneration closestBlock = GridManager.type.getBlocks(neighbours[i].a, neighbours[i].b);

                    //If neighbour does not exist, then skip
                    if (closestBlock == null) continue;
                    //If neighbour is obstacle, then skip
                    if (closestBlock.isWall) continue;
                    //If crossing diagonal gaps exist, then skip
                    if (blockSidewayWall && !ignoreSidewayWall && i % 2 != 0)
                    {
                        GridGeneration prevNeighbour = GridManager.type.getBlocks(s.getClosestBlock(i - 1).a, s.getClosestBlock(i - 1).b);
                        GridGeneration nextNeighbour = GridManager.type.getBlocks(s.getClosestBlock(i + 1).a, s.getClosestBlock(i + 1).b);
                        if (prevNeighbour.isWall && nextNeighbour.isWall) //If diagonal gap, then do not cross
                            continue;
                    }
                    //If neighbour is checked, then skip
                    if (closestBlock._isScannedArea) continue;

                    //Setting the new cost to the neighbour's mevement cost
                    float newCost = currentTile.trackPos + s.GetCost(i);

                    //If the new cost is smaller than the neighbour movement cost, then replace it and set it as parent tile
                    if (newCost < closestBlock.trackPos)
                    {
                        closestBlock.trackPos = newCost;
                        closestBlock.groupBlock = currentTile;

                        if (!generationList.Contains(closestBlock))
                        {
                            generationList.Add(closestBlock);
                        }
                    }
                }
            }
            else if(BlockStyle == BlockGrid.HexagonShape)
			{
                //Getting the neighbours of the current tile for the hexagon shape
                HexagonShape Hexagon = ((HexagonShapeScript)currentTile).transformValueHexagon;
                HexagonShape[] neighbours = Hexagon.GetNeighbours();

				for(int i = 0; i < neighbours.Length; i++)
				{
					//Coordinates valid or not
					if
					(
						neighbours[i].ToArrayPos(GridManager.type.blockNumber).b < 0 || neighbours[i].ToArrayPos(GridManager.type.blockNumber).b >= blockGrid.GetLength(0) ||
						neighbours[i].ToArrayPos(GridManager.type.blockNumber).a < 0 || neighbours[i].ToArrayPos(GridManager.type.blockNumber).a >= blockGrid.GetLength(1)
					)
					{
						continue;
					}

                    //Coordinates a transfromed to grid piece script
                    GridGeneration neighbour = GridManager.type.getBlocks(neighbours[i].a, neighbours[i].b);

                    //If the neighbour does not exist, then skip
                    if (neighbour == null) continue;
                    //If the neighbour is an obstacle, then skip
                    if (neighbour.isWall) continue;
					//also if is checked
					if(neighbour._isScannedArea) continue;

                    //Setting the new cost to neighbour's movement cost
                    float newCost = currentTile.trackPos + Hexagon.GetCost(i);

					// If the new cost is smaller, then replace it and set it as the parent tile
					if(newCost < neighbour.trackPos)
					{
						neighbour.trackPos = newCost;
						neighbour.groupBlock = currentTile;

						if(!generationList.Contains(neighbour))
						{
                            generationList.Add(neighbour);
						}
					}
				}
			}
		}
		else
		{
            //else, pathfinder is finding the next stage
            PathLocatingStyle = (pathLocatingProp)((int)PathLocatingStyle + 1);
		}
	}

            void recreatePath()
            {

                List<GridGeneration> locateInitialList = new List<GridGeneration>();
                List<Vector3> LocateList = new List<Vector3>();

                GridGeneration LocatePrevoiusBlock = EndPosition;

                while (LocatePrevoiusBlock != null)
                {
                    if (LocatePrevoiusBlock.currentSit != blockEnum.startBlock && LocatePrevoiusBlock.currentSit != blockEnum.endBlock) //Colour of the Tiles are not replaced
                        LocatePrevoiusBlock.currentSit = blockEnum.pathTrack;

                    locateInitialList.Add(LocatePrevoiusBlock);
                    LocateList.Add(LocatePrevoiusBlock.transform.position);

                    if (LocatePrevoiusBlock == StartPosition) break;

                    LocatePrevoiusBlock = LocatePrevoiusBlock.groupBlock;
                }
                //retrace line is drawn
                if (LocateList.Count <= 1) //if there is no solution
                {
                    locatePath.gameObject.SetActive(false);
                }
                else
                {
                    locatePath.positionCount = LocateList.Count;
                    locatePath.SetPositions(LocateList.ToArray());
                    locatePath.gameObject.SetActive(true);
                }

                //Pathfinder is set to go to next stage
                PathLocatingStyle = (pathLocatingProp)((int)PathLocatingStyle + 1);
            }
}
    
