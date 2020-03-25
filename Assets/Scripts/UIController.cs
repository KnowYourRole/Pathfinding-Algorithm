using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    //types of UI fileds in the game (player navigation)
    [Header("Options Panel")]
    public Slider tempo;                  //tempo at whcih draws line
    public Slider areaValue;              //how large is the grid going to be (slider)
    public Dropdown selectSize;           //choose between square and hexagon type grid (dropdown)
    public Toggle avoidObstaclesSideway;  //enable obstacle avoidance
    public Dropdown pathfinderType;       //choose between A* and Dijkstra
    public InputField textValue;          //text value for the previous field
    public Slider lineThickness;          //needs to be high to detech it faster
    public Toggle sidewayMovement;        //allow sideway movement           
    public InputField thicknessTextBox;   //just the text box for the same variable
    public InputField tempoText;          //text box for the tempo
    
    [Header("Control Panel")]
    public Button moveForward;            //move only once so you can see the results
    public Text createPathText;           //tedxt for the same button
    public Button createPath;             //creates path between the two points
   public Text resetButtonText;          //text of previus button
    public Button resetButton;            //reset current path and start over
   

    [Header("Mouse")]
    [HideInInspector]
    public bool isAddingObstacle;           //boolean on true adds obstacles (activated on left mouse click)

    [HideInInspector]
    public bool isUndoingObstacle;          //boolean on true undo obstacles (activated on left mouse click if obsitcles in the area)

    [HideInInspector]
    public GridGeneration selectedBlockType;//current selected block type being Hexagon and Square

    [HideInInspector]
    public bool isStartBlock;               //boolean that checks if the start block is clicked on

    [HideInInspector]
    public bool isEndBlock;                 //boolean that checks if the end block is clicked on

    //use single pattern
    private static UIController _instance;

	public static UIController instance
	{
		get { return _instance; }
	}
    
    void Awake()
    {                                   //implement on awake
                                        //allows only one object to execute actions in the current system //not sure about this oNE!!!!!!!!!!!!!!!!!!!!
        if (_instance == null)
			_instance = this;
		else
			Destroy(this.gameObject);
	}

	// when play, instantiate this list of variables
	void Start ()
	{
        selectSize.value 	= (int)GridManager.type.shapeOfBlock;
        areaValue.value 			= GridManager.type.blockNumber;
        textValue.text 		= areaValue.value.ToString();
        pathfinderType.value 	= (int)PathfinderManager.type.algorithmType;
        thicknessTextBox.text 	= lineThickness.value.ToString();
        sidewayMovement.isOn 		= PathfinderManager.type.blockSidewayWall;
        avoidObstaclesSideway.isOn		= PathfinderManager.type.ignoreSidewayWall;
        tempo.value 		= 1.0f / PathfinderManager.type.Delay;
        tempoText.text 		= tempo.value.ToString("##.000");
	}

	// runs interface menu and checks if buttons are clicked
	void Update ()
	{
        createPath.interactable = !(PathfinderManager.type.isSearching && PathfinderManager.type.PathLocatingStyle == pathLocatingProp.completeSpawn);
        moveForward.interactable = PathfinderManager.type.isSearching && PathfinderManager.type.isPause && PathfinderManager.type.PathLocatingStyle != pathLocatingProp.completeSpawn;
        resetButton.interactable = true;

        selectSize.interactable = !PathfinderManager.type.isSearching;
        areaValue.interactable = !PathfinderManager.type.isSearching;
        pathfinderType.interactable = !PathfinderManager.type.isSearching;
        lineThickness.interactable = !PathfinderManager.type.isSearching;
        
        sidewayMovement.interactable = !PathfinderManager.type.isSearching;
        avoidObstaclesSideway.interactable = !PathfinderManager.type.isSearching;

        lineThickness.gameObject.SetActive(PathfinderManager.type.algorithmType == PathDecisionAlgorithm.AStarStyleAlgorithm);
        
        sidewayMovement.gameObject.SetActive(GridManager.type.shapeOfBlock == BlockGrid.SquareShape);
        avoidObstaclesSideway.gameObject.SetActive(GridManager.type.shapeOfBlock == BlockGrid.SquareShape && PathfinderManager.type.blockSidewayWall);

		//checks if mouse one is pressed and generates obstacles
		if(Input.GetMouseButtonUp(0))
		{
            isAddingObstacle = false;
            isUndoingObstacle = false;

            selectedBlockType = null;
            isStartBlock = false;
            isEndBlock = false;
		}
	}

    //checks if type of board is changed (switched between Square and Hexagon) and refreshes it
    public void OnBoardTypeChange()
    {
        if (!PathfinderManager.type.isSearching)
        {
            GridManager.type.shapeOfBlock = (BlockGrid)selectSize.value;
            GridManager.type.resetGrid();
        }
    }

    void OnGUI()
	{
        createPathText.text = (!PathfinderManager.type.isSearching ? "Begin" : (!PathfinderManager.type.isPause ? "Next" : "Resume"));
        resetButtonText.text = (PathfinderManager.type.isSearching && PathfinderManager.type.PathLocatingStyle == pathLocatingProp.completeSpawn ? "Clear" : (!PathfinderManager.type.isSearching ? "Clear" : "Stop"));
	}

	public void OnAlgorithmChange()
	{
		if(!PathfinderManager.type.isSearching)
            PathfinderManager.type.algorithmType = (PathDecisionAlgorithm)pathfinderType.value;
	}

	public void OnWeightChange(bool isText)
	{
		int result;

		if(!isText)
		{
			result = (int)lineThickness.value;
            thicknessTextBox.text = result.ToString();
		}
		else
		{
			if(int.TryParse(thicknessTextBox.text, out result))
                lineThickness.value = result;
			else
				result = (int)lineThickness.value;
		}
        if (!PathfinderManager.type.isSearching)
            PathfinderManager.type.weight = result;

		
	}

    //Checks if size of board is changed and refreshes it
    public void OnBoardSizeChange(bool isText)
    {
        int result;

        if (!isText)
        {
            result = (int)areaValue.value;
            textValue.text = result.ToString();
        }
        else
        {
            if (int.TryParse(textValue.text, out result))
                areaValue.value = result;
            else
                result = (int)areaValue.value;
        }

        if (!PathfinderManager.type.isSearching)
        {
            GridManager.type.blockNumber = result;
            GridManager.type.blockSize = 10.0f / GridManager.type._blockNumber;
            PathfinderManager.type.locatePath.startWidth = 2.0f / GridManager.type._blockNumber;
            PathfinderManager.type.locatePath.endWidth = 2.0f / GridManager.type._blockNumber;
            GridManager.type.resetGrid();
        }
    }

	public void OnCrossGapsToggle()
	{
		if(!PathfinderManager.type.isSearching)
            PathfinderManager.type.ignoreSidewayWall = avoidObstaclesSideway.isOn;
	}

    public void OnDiagonalToggle()
    {
        if (!PathfinderManager.type.isSearching)
            PathfinderManager.type.blockSidewayWall = sidewayMovement.isOn;
    }

	public void OnStartBtnPress()
	{
		if(!PathfinderManager.type.isSearching)
            PathfinderManager.type.GeneratePathfinding();
		else
            PathfinderManager.type.isPause = !PathfinderManager.type.isPause;
	}

    public void OnStopBtnPress()
    {
        if (PathfinderManager.type.isSearching)
        {
            PathfinderManager.type.isSearching = false;
            PathfinderManager.type.CleanBoard();
        }
        else
        {
            GridManager.type.resetGrid(true);

            PathfinderManager.type.StartPosition.currentSit = blockEnum.startBlock;
            PathfinderManager.type.EndPosition.currentSit = blockEnum.endBlock;
        }
    }

    public void OnStepBtnPress()
	{
		if(PathfinderManager.type.isSearching && PathfinderManager.type.isPause)
            PathfinderManager.type.MoveOnce();
	}

    public void OnSpeedChange(bool isText)
    {
        int result;

        if (!isText)
        {
            result = (int)tempo.value;
            tempoText.text = result.ToString("##.000");
        }
        else
        {
            if (int.TryParse(tempoText.text, out result))
                tempo.value = result;
            else
                result = (int)tempo.value;
        }

        PathfinderManager.type.Delay = 1.0f / result;
    }

}
