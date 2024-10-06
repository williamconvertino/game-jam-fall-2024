using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    [Header("UI")]
    public SpriteRenderer mouseMarker;
    public Sprite[] mouseMarkerSprites;
    
    [Header("Controls")]
    public bool SHOULD_SMART_ROTATE = true;
    
    [Header("Prefabs")]
    public Ship parentShip;
    public GameObject[] placedObjectPrefabs;
    public ShipComponent[,] placedObjects;

    public GameObject buildingUI;
    public GameObject gameplayUI;
    public GameObject grid;
    public GameObject mouseMarkerProxy;
    private CameraFollow2D follow;
    public TextMeshProUGUI [] inventoryLabels;

    int GRID_SIZE = 7;
    float GRID_LEFT = -4.2f;
    float GRID_RIGHT = 4.2f;
    float GRID_TOP = 4.2f;
    float GRID_BOTTOM = -4.2f;
    int SPRITE_PIXEL_SIDE = 128;
    float SPRITE_WIDTH;
    Vector3 mouseWorldPos;
    int selectedIndex = -1; //-1 means no tile selected
    float FACING_UP_ANGLE = 0f;
    float FACING_DOWN_ANGLE = 180f;
    float FACING_RIGHT_ANGLE = 270f;
    float FACING_LEFT_ANGLE = 90f;
    int prevGridX = -1;
    int prevGridY = -1;
    bool isBuilding = true;
    bool selectedComponentRotatable = false;
    public int [] inventories;

    // Start is called before the first frame update
    void Start()
    {
        SPRITE_WIDTH = SPRITE_PIXEL_SIDE / 100f;
        GRID_LEFT = -(GRID_SIZE / 2) * SPRITE_WIDTH;
        GRID_RIGHT = (GRID_SIZE / 2) * SPRITE_WIDTH;
        GRID_TOP = (GRID_SIZE / 2) * SPRITE_WIDTH;
        GRID_BOTTOM = -(GRID_SIZE / 2) * SPRITE_WIDTH;
        
        placedObjects = new ShipComponent[GRID_SIZE,GRID_SIZE];
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                placedObjects[i,j] = null;
            }
        }
        follow = Camera.main.gameObject.GetComponent<CameraFollow2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isBuilding)
        {
            selectedComponentRotatable = (selectedIndex > 1);
            for (int i = 0; i < 4; i++)
            {
                inventoryLabels[i].text = "x" + inventories[i].ToString();
            }
            mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            mouseMarkerProxy.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 2);
            
            float mouseMarkerLocalX = mouseMarkerProxy.transform.localPosition.x;
            float mouseMarkerLocalY = mouseMarkerProxy.transform.localPosition.y;

            float posX = Mathf.Floor(mouseMarkerLocalX / SPRITE_WIDTH + 0.5f) * SPRITE_WIDTH;
            float posY = Mathf.Floor(mouseMarkerLocalY / SPRITE_WIDTH + 0.5f) * SPRITE_WIDTH;
            int gridX = Mathf.RoundToInt((posX + (GRID_SIZE / 2) * SPRITE_WIDTH) / SPRITE_WIDTH);
            int gridY = Mathf.RoundToInt((-posY + (GRID_SIZE / 2) * SPRITE_WIDTH) / SPRITE_WIDTH);
            bool mouseInGrid = (gridX > -1 && gridX < GRID_SIZE && gridY > -1 && gridY < GRID_SIZE);
            if (gridX != prevGridX || gridY != prevGridY)
            {
                mouseMarker.transform.localPosition = new Vector3(posX, posY, 2); //+ gridOffset;
                if (mouseInGrid)
                {
                    if (selectedComponentRotatable && !isMouseMarkerRotationValid(gridX, gridY))
                    {
                        attemptToRotateMouseMarkerToValidPosition(gridX, gridY);
                    }
                }
                prevGridX = gridX;
                prevGridY = gridY;
            }

            if (selectedComponentRotatable && Input.GetKeyDown(KeyCode.RightArrow))
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
                if (mouseInGrid && placedObjects[gridX, gridY])
                {
                    if (placedObjects[gridX, gridY].GetComponent<PilotComponent>() != null)
                    {
                        inventories[0] += 1;
                    }
                    else if (placedObjects[gridX, gridY].GetComponent<GunComponent>() != null)
                    {
                        inventories[3] += 1;
                    }
                    else if (placedObjects[gridX, gridY].GetComponent<ThrusterComponent>() != null)
                    {
                        inventories[2] += 1;
                    }
                    else //There is no other way to check for hull component! :(
                    {
                        inventories[1] += 1;
                    }
                    //Destroy(placedObjects[gridX, gridY].gameObject);
                    placedObjects[gridX, gridY].DestroyBlock();
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
                            if (placedObjects[gridX, gridY].GetComponent<PilotComponent>() != null)
                            {
                                inventories[0] += 1;
                            }
                            else if (placedObjects[gridX, gridY].GetComponent<GunComponent>() != null)
                            {
                                inventories[3] += 1;
                            }
                            else if (placedObjects[gridX, gridY].GetComponent<ThrusterComponent>() != null)
                            {
                                inventories[2] += 1;
                            }
                            else //There is no other way to check for hull component! :(
                            {
                                inventories[1] += 1;
                            }
                            //Destroy(placedObjects[gridX, gridY].gameObject);
                            placedObjects[gridX, gridY].DestroyBlock();
                        }

                        placedObjects[gridX, gridY] =
                            Instantiate(placedObjectPrefabs[selectedIndex], mouseMarker.transform.position,
                                mouseMarker.transform.rotation, parentShip.transform).GetComponent<ShipComponent>();

                        inventories[selectedIndex] -= 1;
                        if (inventories[selectedIndex] == 0)
                        {
                            selectedIndex = -1;
                            mouseMarker.sprite = null;
                        }
                    }
                }
            }
            if (Input.GetMouseButtonUp(1) && selectedIndex != -1)
            {
                mouseMarker.sprite = mouseMarkerSprites[selectedIndex];
            }
        }
    }
    public void exitBuildMode()
    {
        parentShip.Initialize(placedObjects);
        bool validShip = parentShip.ConstructionIsValid();
        if (!validShip) return;
            
        buildingUI.gameObject.SetActive(false);
        mouseMarker.gameObject.SetActive(false);
        grid.SetActive(false);
        gameplayUI.gameObject.SetActive(true);
        isBuilding = false;
        selectedIndex = -1;
        mouseMarker.sprite = null;
        follow.trackRotation = false;
        follow.zoomScale = 1;
        
        GameManager.Instance.Unpause();
        parentShip.IntegrityCheck();
    }
    
    public void enterBuildMode() //fix rotation
    {
        GameManager.Instance.Pause();
        buildingUI.gameObject.SetActive(true);
        mouseMarker.gameObject.SetActive(true);
        grid.SetActive(true);
        gameplayUI.gameObject.SetActive(false);
        isBuilding = true;
        follow.trackRotation = true;
        follow.zoomScale = .5f;
    }

    public void onSelectBuildItem(int index)
    {
        if (inventories[index] > 0)
        {
            selectedIndex = index;
            mouseMarker.sprite = mouseMarkerSprites[index];
            mouseMarker.gameObject.transform.localEulerAngles = new Vector3(0, 0, FACING_UP_ANGLE);
        }
        else
        {
            selectedIndex = -1;
            mouseMarker.sprite = null;
        }
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
        if (mouseMarker.transform.localEulerAngles.z == FACING_UP_ANGLE) //Facing up
        {
            return isBlockBelow(x, y);
        }
        if (mouseMarker.transform.localEulerAngles.z == FACING_DOWN_ANGLE) //Facing down
        {
            return isBlockAbove(x, y); 
        }
        if (mouseMarker.transform.localEulerAngles.z == FACING_RIGHT_ANGLE) //Facing right
        {
            return isBlockToLeft(x, y);
        }
        if (mouseMarker.transform.localEulerAngles.z == FACING_LEFT_ANGLE) //Facing left
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
        
        if (mouseMarker.transform.localEulerAngles.z == FACING_UP_ANGLE) //Facing up
        {
            //Priority: Right, Down, Left
            if (isBlockToLeft(x, y))
            {
                //There is a block to the left, so point right
                mouseMarker.transform.localEulerAngles = new Vector3(0, 0, FACING_RIGHT_ANGLE);
            }
            else if (isBlockAbove(x, y))
            {
                //There is a block above, so point down
                mouseMarker.transform.localEulerAngles = new Vector3(0, 0, FACING_DOWN_ANGLE);
            }
            else if (isBlockToRight(x, y))
            {
                //There is a block to the right, so point left
                mouseMarker.transform.localEulerAngles = new Vector3(0, 0, FACING_LEFT_ANGLE);
            }
        }
        else if (mouseMarker.transform.localEulerAngles.z == FACING_DOWN_ANGLE) //Facing down
        {
            //Priority: Left, Up, Right
            if (isBlockToRight(x, y))
            {
                //There is a block to the right, so point left
                mouseMarker.transform.localEulerAngles = new Vector3(0, 0, FACING_LEFT_ANGLE);
            }
            else if (isBlockBelow(x, y))
            {
                //There is a block below, so point up
                mouseMarker.transform.localEulerAngles = new Vector3(0, 0, FACING_UP_ANGLE);
            }
            else if (isBlockToLeft(x, y))
            {
                //There is a block to the left, so point right
                mouseMarker.transform.localEulerAngles = new Vector3(0, 0, FACING_RIGHT_ANGLE);
            }
        }
        else if (mouseMarker.transform.localEulerAngles.z == FACING_RIGHT_ANGLE) //Facing right
        {
            //Priority: Down, Left, Up
            if (isBlockAbove(x, y))
            {
                //There is a block above, so point down
                mouseMarker.transform.localEulerAngles = new Vector3(0, 0, FACING_DOWN_ANGLE);
            }
            else if (isBlockToRight(x, y))
            {
                //There is a block to the right, so point left
                mouseMarker.transform.localEulerAngles = new Vector3(0, 0, FACING_LEFT_ANGLE);
            }
            else if (isBlockBelow(x, y))
            {
                //There is a block below, so point up
                mouseMarker.transform.localEulerAngles = new Vector3(0, 0, FACING_UP_ANGLE);
            }
        }
       else if (mouseMarker.transform.localEulerAngles.z == FACING_LEFT_ANGLE) //Facing left
       {
            //Priority: Up, Right, Down
            if (isBlockBelow(x, y))
            {
                //There is a block below, so point up
                mouseMarker.transform.localEulerAngles = new Vector3(0, 0, FACING_UP_ANGLE);
            }
            else if (isBlockToLeft(x, y))
            {
                //There is a block to the left, so point right
                mouseMarker.transform.localEulerAngles = new Vector3(0, 0, FACING_RIGHT_ANGLE);
            }
            else if (isBlockAbove(x, y))
            {
                //There is a block above, so point down
                mouseMarker.transform.localEulerAngles = new Vector3(0, 0, FACING_DOWN_ANGLE);
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
