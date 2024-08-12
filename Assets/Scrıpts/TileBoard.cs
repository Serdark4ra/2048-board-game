using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public Tile tilePrefab;
    private TileGrid TileGrid;
    private List<Tile> tiles;

    public TileState[] tileStates;
    private bool waiting;




    private void Awake()
    {
        TileGrid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
    }

    private void Start()
    {
        CreateTile();
        CreateTile();
        Debug.Log("board started");
    }

    private void CreateTile()
    {

        Tile tile = Instantiate(tilePrefab, TileGrid.transform);
        tile.SetState(tileStates[0], 2);

        tile.spawn(TileGrid.getRandomEmptyCell());
        tiles.Add(tile);
        Debug.Log("tiles created started");
    }

    private void Update()
    {
       if(!waiting)
       {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            Move(Vector2Int.up, 0, 1, 1, 1);
        } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            Move(Vector2Int.left, 1, 1, 0, 1);
        } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            Move(Vector2Int.down, 0, 1, TileGrid.height - 2, -1);
        } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
            Move(Vector2Int.right, TileGrid.width - 2, -1, 0, 1);
        }
       }

       

    }

    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY  ){

        bool changed = false;
        for (int x = startX; x>= 0 && x < TileGrid.width; x += incrementX)
        {
            for (int y = startY; y>= 0 && y < TileGrid.height; y += incrementY)
            {
                TileCell cell = TileGrid.getCell(x,y);

                if (cell.isOccupied)
                {
                    changed |= MoveTiles(cell.tile, direction);
                }
            }
        }

        if (changed)
        {
            StartCoroutine(waitForChanges());
        }
    }

    private bool MoveTiles(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = TileGrid.getNextCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.isOccupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    Merge(tile, adjacent.tile);
                    return true;
                }

                break;
            }

            newCell = adjacent;
            adjacent = TileGrid.getNextCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;

        }

        return false;
    }

    private void Merge(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);
        
        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        int number = b.number * 2;

        b.SetState(tileStates[index], number);

    }

    private bool CanMerge(Tile a, Tile b){
       return  a.number == b.number;
    }

    private IEnumerator waitForChanges(){
        waiting = true;
        yield return new WaitForSeconds(0.2f);

        waiting = false;

        if (tiles.Count != TileGrid.size) {
            CreateTile();
        }
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i]) {
                return i;
            }
        }

        return -1;
    }
}

