using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this class sets the different tile colours depending on their state
/// It also prevents the pathfinder from going over obstacles
/// Makes sure when pathfinder is on, players can not edit the grid
/// </summary>
public abstract class GridGeneration : MonoBehaviour
{
    [HideInInspector]
    public int b;       //the b integer value is used only in the scripts

     [HideInInspector]   //used to store position
    public int a;       //the a integer value is used only in the scripts

    [HideInInspector]
    public bool _isScanned;     //the scanned area

    [HideInInspector]
    public bool _isScannedArea
	{
		get
		{
			if(currentSit != blockEnum.startBlock && currentSit != blockEnum.endBlock && !_isWall)
			{
				if(_isScanned)
                    currentSit = blockEnum.scannedArea;
				else
                    currentSit = blockEnum.toBeScanned;
			}
			return _isScanned; 
		}
		set
		{
            _isScanned = value;
			if(currentSit != blockEnum.startBlock && currentSit != blockEnum.endBlock && !_isWall)
			{
				if(_isScanned)
                    currentSit = blockEnum.scannedArea;
				else
                    currentSit = blockEnum.toBeScanned;
			}
		}
	}
    [HideInInspector]
    public bool _isWall;

    [HideInInspector]
    public bool isWall
	{
		get
		{
			return _isWall; 
		}
		set
		{
            _isWall = value;
			if(currentSit != blockEnum.startBlock && currentSit != blockEnum.endBlock)
			{
				if(_isWall)
                    currentSit = blockEnum.addWall;
				else
                    currentSit = blockEnum.baseBlock;
			}
		}
	}
    public float trackPos;                  //the position of the track

    protected abstract void AssociateLocation(); 

    [HideInInspector]
    public blockEnum currentSit;    //current situation of the block

    [HideInInspector]
    public GridGeneration groupBlock;       //grouping together the blocks during pathfinding
    
    public SpriteRenderer spriteGeneration;   //uses the sprite file to render the blocks on the grid

    void OnMouseOver()
    {
        //Not able to edit during pathfinding
        if (PathfinderManager.type.isSearching) return;

        if (Input.GetMouseButton(0))
        {
            if (UIController.instance.isStartBlock)
            {
                if (this.isWall) return;
                UIController.instance.selectedBlockType.currentSit = blockEnum.baseBlock;
                this.currentSit = blockEnum.startBlock;
                PathfinderManager.type.StartPosition = this;
                UIController.instance.selectedBlockType = PathfinderManager.type.StartPosition;
            }
            else if (UIController.instance.isEndBlock)
            {
                if (this.isWall) return;
                UIController.instance.selectedBlockType.currentSit = blockEnum.baseBlock;
                this.currentSit = blockEnum.endBlock;
                PathfinderManager.type.EndPosition = this;
                UIController.instance.selectedBlockType = PathfinderManager.type.EndPosition;
            }
            else if (UIController.instance.isAddingObstacle)
            {
                isWall = true;
            }
            else if (UIController.instance.isUndoingObstacle)
            {
                isWall = false;
            }
            else
            {
                if (currentSit == blockEnum.startBlock)
                {
                    UIController.instance.isStartBlock = true;
                    UIController.instance.selectedBlockType = this;
                }
                else if (currentSit == blockEnum.endBlock)
                {
                    UIController.instance.isEndBlock = true;
                    UIController.instance.selectedBlockType = this;
                }
                else
                {
                    UIController.instance.isAddingObstacle = !isWall;
                    UIController.instance.isUndoingObstacle = isWall;
                }
            }
        }
    }

    protected virtual void OnGUI()
	{
		switch(currentSit)                          
		{
			case blockEnum.baseBlock:
                spriteGeneration.color = Color.grey;    //sets the color of the grid
				break;
			case blockEnum.toBeScanned:
                spriteGeneration.color = Color.Lerp(Color.Lerp(Color.blue, Color.yellow, 0.25f), Color.yellow, 0.5f);
				break;
			case blockEnum.scannedArea:
                spriteGeneration.color = Color.Lerp(Color.Lerp(Color.red, Color.yellow, 0.5f), Color.yellow, 0.5f);
				break;
			case blockEnum.pathTrack:
                spriteGeneration.color = Color.Lerp(Color.yellow, Color.yellow, 0.5f);
				break;
			case blockEnum.startBlock:
                spriteGeneration.color = Color.red;   //start point colour
				break;
			case blockEnum.endBlock:     //end point colour
                spriteGeneration.color = Color.green;
				break;
			case blockEnum.addWall:
                spriteGeneration.color = Color.black;   //obstacle colour
				break;
		}
	}

	
}

public enum blockEnum   //the blockEnum holds the base pathfinding variables
{                       //the enum also stores the current block position

    baseBlock = 0,  //The basic block on the grid (without anything added to it, only it's original colour). This value is set in the editor
    startBlock,     //The initial block from which the path is drawn (currently set to RED)
    endBlock,       //This is the final block towards which the path is drawn (currently set to GREEN)
    addWall,		//Adding wall, so that the pathfinder cannot go through it (currently set to Black)
    toBeScanned,    //Area that's yet to be scanned for pathfinding (currently set to Light Blue)
    scannedArea,    //The area that was scanned (currently set to Orange)
    pathTrack,      //The actual path track between start block and end block (currently set to )
}