using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    public SpriteRenderer mouseMarker;
    public GameObject[] placedObjectPrefabs;
    public Sprite[] mouseMarkerSprites;
    public GameObject[,] placedObjects;
    int GRID_SIZE = 7;
    float GRID_LEFT = -4.2f;
    float GRID_RIGHT = 4.2f;
    float GRID_TOP = 4.2f;
    float GRID_BOTTOM = -4.2f;
    int SPRITE_PIXEL_SIDE = 128;
    float SPRITE_WIDTH;
    Vector3 mouseWorldPos;
    int selectedIndex = -1; //-1 means no tile selected
    public Transform shipTransform;
    public bool SHOULD_SMART_ROTATE = true;
    float FACING_UP_ANGLE = 0f;
    float FACING_DOWN_ANGLE = 180f;
    float FACING_RIGHT_ANGLE = 270f;
    float FACING_LEFT_ANGLE = 90f;
    int prevGridX = -1;
    int prevGridY = -1;

    // Start is called before the first frame update
    void Start()
    {
        SPRITE_WIDTH = SPRITE_PIXEL_SIDE / 100f;
        GRID_LEFT = -(GRID_SIZE / 2) * SPRITE_WIDTH;
        GRID_RIGHT = (GRID_SIZE / 2) * SPRITE_WIDTH;
        GRID_TOP = (GRID_SIZE / 2) * SPRITE_WIDTH;
        GRID_BOTTOM = -(GRID_SIZE / 2) * SPRITE_WIDTH;
        
        placedObjects = new GameObject[GRID_SIZE,GRID_SIZE];
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                placedObjects[i,j] = null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float posX = Mathf.Floor(mouseWorldPos.x / SPRITE_WIDTH + 0.5f) * SPRITE_WIDTH;
        float posY = Mathf.Floor(mouseWorldPos.y / SPRITE_WIDTH + 0.5f) * SPRITE_WIDTH;
        int gridX = Mathf.RoundToInt((posX + (GRID_SIZE / 2) * SPRITE_WIDTH) / SPRITE_WIDTH);
        int gridY = Mathf.RoundToInt((-posY + (GRID_SIZE / 2) * SPRITE_WIDTH) / SPRITE_WIDTH);
        bool mouseInGrid = (gridX > -1 && gridX < GRID_SIZE && gridY > -1 && gridY < GRID_SIZE);
        if (gridX != prevGridX || gridY != prevGridY)
        {
            mouseMarker.transform.position = new Vector3(posX, posY);
            if (mouseInGrid)
            {
                if (!isMouseMarkerRotationValid(gridX, gridY))
                {
                    attemptToRotateMouseMarkerToValidPosition(gridX, gridY);
                }
            }
            prevGridX = gridX;
            prevGridY = gridY;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (isMouseMarkerRotationValid(gridX, gridY))
            {
                attemptToRotateMouseMarkerToValidPosition(gridX, gridY);
            }
            else
            {
                mouseMarker.transform.eulerAngles = new Vector3(0, 0, mouseMarker.transform.rotation.eulerAngles.z - 90);
            }
        }
        
        if (Input.GetMouseButton(1))
        {
            if (mouseInGrid)
            {
                Destroy(placedObjects[gridX, gridY]);
                placedObjects[gridX, gridY] = null;
            }
            ///It is confusing when you erase with a brush of the same component you placed because your brush makes it look like it is still there
            ///To fix this, we activate "eraser mode" when you erase, hiding your brush until you release right click
            ///It's good to do this even when the mouse is outside of the grid so it looks like they enter erase mode before entering the grid
            mouseMarker.sprite = null;
        }
        ///The else here means that if you press both right and left click at the same time, we default to right click and do erasing
        ///This can be changed easily, but note that if it is changed, we will need to bring the brush back if we were in eraser mode as it currently stays hidden until right click is released
        else if (Input.GetMouseButton(0))
        {
            if (mouseInGrid)
            {
                if (selectedIndex != -1)
                {
                    if (placedObjects[gridX, gridY]) //Should optimize so that we do not do anything if current tile is identical to one we are placing (rotation and index) -- currently we can needlessly delete and replace many times per second
                    {
                        Destroy(placedObjects[gridX, gridY]);
                    }
                    placedObjects[gridX, gridY] = Instantiate(placedObjectPrefabs[selectedIndex], new Vector3(posX, posY), mouseMarker.transform.rotation, shipTransform);
                }
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            mouseMarker.sprite = mouseMarkerSprites[selectedIndex];
        }
    }

    public void onSelectBuildItem(int index)
    {
        selectedIndex = index;
        mouseMarker.sprite = mouseMarkerSprites[index];
        mouseMarker.gameObject.transform.rotation = Quaternion.identity;
    }
    /// <summary>
    /// Returns true if mouse marker rotation is valid, false otherwise
    /// </summary>
    /// <param name="x"></param>
    /// mouse grid x
    /// <param name="y"></param>
    /// mouse grid y
    private bool isMouseMarkerRotationValid(int x, int y)
    {
        if (mouseMarker.transform.rotation.eulerAngles.z == FACING_UP_ANGLE) //Facing up
        {
            return isBlockBelow(x, y);
        }
        if (mouseMarker.transform.rotation.eulerAngles.z == FACING_DOWN_ANGLE) //Facing down
        {
            return isBlockAbove(x, y); 
        }
        if (mouseMarker.transform.rotation.eulerAngles.z == FACING_RIGHT_ANGLE) //Facing right
        {
            return isBlockToLeft(x, y);
        }
        if (mouseMarker.transform.rotation.eulerAngles.z == FACING_LEFT_ANGLE) //Facing left
        {
            return isBlockToRight(x, y);
        }
        return false;
    }
    /// <summary>
    /// Attempts to rotate mouse marker to valid position
    /// </summary>
    /// <param name="x"></param>
    /// mouse grid x
    /// <param name="y"></param>
    /// mouse grid y
    private void attemptToRotateMouseMarkerToValidPosition(int x, int y, bool clockwise = true)
    {
        //Directions will be prioritized in clockwise/counterclockwise order starting from current direction
        //TODO: Counterclockwise option
        
        if (mouseMarker.transform.rotation.eulerAngles.z == FACING_UP_ANGLE) //Facing up
        {
            //Priority: Right, Down, Left
            if (isBlockToLeft(x, y))
            {
                //There is a block to the left, so point right
                mouseMarker.transform.eulerAngles = new Vector3(0, 0, FACING_RIGHT_ANGLE);
            }
            else if (isBlockAbove(x, y))
            {
                //There is a block above, so point down
                mouseMarker.transform.eulerAngles = new Vector3(0, 0, FACING_DOWN_ANGLE);
            }
            else if (isBlockToRight(x, y))
            {
                //There is a block to the right, so point left
                mouseMarker.transform.eulerAngles = new Vector3(0, 0, FACING_LEFT_ANGLE);
            }
        }
        else if (mouseMarker.transform.rotation.eulerAngles.z == FACING_DOWN_ANGLE) //Facing down
        {
            //Priority: Left, Up, Right
            if (isBlockToRight(x, y))
            {
                //There is a block to the right, so point left
                mouseMarker.transform.eulerAngles = new Vector3(0, 0, FACING_LEFT_ANGLE);
            }
            else if (isBlockBelow(x, y))
            {
                //There is a block below, so point up
                mouseMarker.transform.eulerAngles = new Vector3(0, 0, FACING_UP_ANGLE);
            }
            else if (isBlockToLeft(x, y))
            {
                //There is a block to the left, so point right
                mouseMarker.transform.eulerAngles = new Vector3(0, 0, FACING_RIGHT_ANGLE);
            }
        }
        else if (mouseMarker.transform.rotation.eulerAngles.z == FACING_RIGHT_ANGLE) //Facing right
        {
            //Priority: Down, Left, Up
            if (isBlockAbove(x, y))
            {
                //There is a block above, so point down
                mouseMarker.transform.eulerAngles = new Vector3(0, 0, FACING_DOWN_ANGLE);
            }
            else if (isBlockToRight(x, y))
            {
                //There is a block to the right, so point left
                mouseMarker.transform.eulerAngles = new Vector3(0, 0, FACING_LEFT_ANGLE);
            }
            else if (isBlockBelow(x, y))
            {
                //There is a block below, so point up
                mouseMarker.transform.eulerAngles = new Vector3(0, 0, FACING_UP_ANGLE);
            }
        }
       else if (mouseMarker.transform.rotation.eulerAngles.z == FACING_LEFT_ANGLE) //Facing left
       {
            //Priority: Up, Right, Down
            if (isBlockBelow(x, y))
            {
                //There is a block below, so point up
                mouseMarker.transform.eulerAngles = new Vector3(0, 0, FACING_UP_ANGLE);
            }
            else if (isBlockToLeft(x, y))
            {
                //There is a block to the left, so point right
                mouseMarker.transform.eulerAngles = new Vector3(0, 0, FACING_RIGHT_ANGLE);
            }
            else if (isBlockAbove(x, y))
            {
                //There is a block above, so point down
                mouseMarker.transform.eulerAngles = new Vector3(0, 0, FACING_DOWN_ANGLE);
            }
       }
    }
    private bool isBlockAbove(int x, int y)
    {
        return (y > 0 && placedObjects[x, y - 1] != null && placedObjects[x, y - 1].tag == "Block");
    }
    private bool isBlockBelow(int x, int y)
    {
        return (y < GRID_SIZE - 1 && placedObjects[x, y + 1] != null && placedObjects[x, y + 1].tag == "Block");
    }
    private bool isBlockToLeft(int x, int y)
    {
        return (x > 0 && placedObjects[x - 1, y] != null && placedObjects[x - 1, y].tag == "Block");
    }
    private bool isBlockToRight(int x, int y)
    {
        return (x < GRID_SIZE - 1 && placedObjects[x + 1, y] != null && placedObjects[x + 1, y].tag == "Block");
    }
}
