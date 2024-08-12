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

    public GameManager gameManager;




    private void Awake()
    {
        TileGrid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
    }

    

    public  void CreateTile()
    {

        Tile tile = Instantiate(tilePrefab, TileGrid.transform);
        tile.SetState(tileStates[0], 2);

        tile.spawn(TileGrid.getRandomEmptyCell());
        tiles.Add(tile);
        Debug.Log("tiles created started");
    }

    public void ClearBoard(){

        foreach (var cell in TileGrid.cells)
        {   
            cell.tile = null;
        }

        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
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

        gameManager.IncreaseScore(number);

    }

    private bool CanMerge(Tile a, Tile b){
       return  a.number == b.number && !b.locked;
    }

    private IEnumerator waitForChanges(){
        waiting = true;
        yield return new WaitForSeconds(0.2f);

        waiting = false;

        foreach (var tile in tiles)
        {
            tile.locked = false;
        }

        if (tiles.Count != TileGrid.size) {
            CreateTile();
        }

        if (CheckGameOver())
        {
            gameManager.GameOver();
        }


    }

    private bool CheckGameOver(){
        if(tiles.Count != TileGrid.size){
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = TileGrid.getNextCell(tile.cell, Vector2Int.up);
            TileCell down = TileGrid.getNextCell(tile.cell, Vector2Int.down);
            TileCell left = TileGrid.getNextCell(tile.cell, Vector2Int.left);
            TileCell right = TileGrid.getNextCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile))
            {
                return false;
            }
            if (down != null && CanMerge(tile, down.tile))
            {
                return false;
            }
            if (left != null && CanMerge(tile, left.tile))
            {
                return false;
            }
            if (right != null && CanMerge(tile, right.tile))
            {
                return false;
            }
        }

        return true;
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

