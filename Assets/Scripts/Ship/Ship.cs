using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Ship : Entity
{
    [HideInInspector] public int ShipSize = 0; 
    private ShipComponent[] _childrenComponents = new ShipComponent[] { };
    private ShipComponent[,] _componentGraph;
    public int pilotRow;
    public int pilotCol;
    
    private const int dimensions = 7;

    public BuildingManager buildingManager;

    private void Start()
    {
        Initialize(new ShipComponent[dimensions,dimensions]);
        if (!Frozen)
        {
            Unfreeze();
        }
    }

    public void Initialize(ShipComponent[,] componentGraph)
    {
        _componentGraph = componentGraph;
        FindPilot();
        InitializeComponents();
        CalculateMass();
    }

    private void InitializeComponents()
    {
        _childrenComponents = GetComponentsInChildren<ShipComponent>();
        foreach (var component in _childrenComponents)
        {
            component.Initialize(this);
        }
    }
    public override void Freeze()
    {
        base.Freeze();
        foreach (var component in _childrenComponents)
        {
            component.Frozen = true;
        }
    }
    public override void Unfreeze()
    {
        base.Unfreeze();
        foreach (var component in _childrenComponents)
        {
            component.Frozen = false;
        }
    }

    public void Turn(float amount)
    {
        Rb2d.AddTorque(-amount);
    }

    public void Thrust(float amount, Vector3 direction, Vector3 position)
    {
        Rb2d.AddForceAtPosition( direction * amount, position);
    }

    private void CalculateMass()
    {
        ShipSize = 0;
        foreach (var component in _childrenComponents)
        {
            if (component.CompareTag("Block"))
            {
                ShipSize++;
            }
        }

        Rb2d.mass = Mathf.Sqrt(ShipSize);
    }

    public void IntegrityCheck(bool inBuildingMode = false)
    {
        int n = dimensions;
        bool[,] visited = new bool[n, n];

        BFSVisit(new Tuple<int,int>(pilotRow, pilotCol), visited, n-1);
        
        List<ShipComponent> allComponents = new List<ShipComponent>();
        List<ShipComponent> invalidComponents = new List<ShipComponent>();

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (!visited[i, j])
                {
                    ShipComponent block = _componentGraph[i, j];
                    allComponents.Add(block);
                    if (block != null)
                    {
                        if (buildingManager)
                        {
                            if (block.GetComponent<PilotComponent>() != null)
                            {
                                buildingManager.inventories[0] += 1;
                            }
                            else if (block.GetComponent<GunComponent>() != null)
                            {
                                buildingManager.inventories[3] += 1;
                            }
                            else if (block.GetComponent<ThrusterComponent>() != null)
                            {
                                buildingManager.inventories[2] += 1;
                            }
                            else //There is no other way to check for hull component! :(
                            {
                                buildingManager.inventories[1] += 1;
                            }
                        }
                        invalidComponents.Add(block);
                    }
                }
            }
        }

        if (inBuildingMode && invalidComponents.Count == allComponents.Count) return;
        
        foreach (var component in invalidComponents)
        {
            RemoveComponent(component);
            Destroy(component.gameObject);
        }
    }

    private void BFSVisit(Tuple<int,int> loc, bool[,] visited, int maxIndex)
    {
        Stack < Tuple<int, int> > stack = new Stack<Tuple<int,int>>();
        stack.Push(loc);

        while (stack.Count > 0)
        {
            Tuple<int, int> current = stack.Pop();
            int i = current.Item1;
            int j = current.Item2;
            visited[i, j] = true;
            print(visited[i, j]);

            ShipComponent currentBlock = _componentGraph[i, j];
            if (currentBlock == null)
            {
                continue;
            }

            if (i > 0)
            {
                // check left
                ShipComponent leftBlock = _componentGraph[i - 1, j];
                if (visited[i-1, j] == false && leftBlock != null && currentBlock.CanConnectLeft() && leftBlock.CanConnectRight())
                {
                    stack.Push(new Tuple<int, int>(i - 1, j));
                    visited[i - 1, j] = true;
                }
            }

            if (i < maxIndex)
            {
                // check right
                ShipComponent rightBlock = _componentGraph[i + 1, j];
                if (visited[i + 1, j] == false && rightBlock != null && currentBlock.CanConnectRight() && rightBlock.CanConnectLeft())
                {
                    stack.Push(new Tuple<int, int>(i + 1, j));
                    visited[i + 1, j] = true;
                }
            }

            if (j > 0)
            {
                // check up
                ShipComponent upBlock = _componentGraph[i, j-1];
                if (visited[i, j-1] == false &&  upBlock != null && currentBlock.CanConnectUp() && upBlock.CanConnectDown())
                {
                    stack.Push(new Tuple<int, int>(i, j-1));
                    visited[i, j-1] = true;
                }
            }

            if (j < maxIndex)
            {
                // check down
                ShipComponent downBlock = _componentGraph[i, j+1];
                if (visited[i, j + 1] == false && downBlock != null && currentBlock.CanConnectDown() && downBlock.CanConnectUp())
                {
                    stack.Push(new Tuple<int, int>(i, j + 1));
                    visited[i, j + 1] = true;
                }
            }
        }
    }

    private void FindPilot()
    {
        int n = dimensions;

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                ShipComponent block = _componentGraph[i, j];
                if (block != null)
                {
                    if (block.GetComponent<PilotComponent>() != null)
                    {
                        pilotRow = i;
                        pilotCol = j;
                        return;
                    }
                }
            }
        }
    }

    public void RemoveComponent(ShipComponent component)
    {
        int n = dimensions;

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                ShipComponent block = _componentGraph[i, j];
                if (block != null)
                {
                    if (block == component)
                    {
                        _componentGraph[i, j] = null;
                        return;
                    }
                }
            }
        }
    }
}