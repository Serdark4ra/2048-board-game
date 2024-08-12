using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] rows { get; private set; }

    public TileCell[] cells { get; private set; }

    public int size => cells.Length;
    public int height => rows.Length;
    public int width =>  size / height;


    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
    }
    
    private void Start()
    {
        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[y].cells.Length; x++)
            {
                rows[y].cells[x].coordinates = new Vector2Int(x, y);
            }
        }
        Debug.Log("grid started");

    }

    public TileCell getRandomEmptyCell()
    {
        int index = Random.Range(0, cells.Length);
        int startIndex = index;

        while (cells[index].isOccupied)
        {
            index++;
            if (index > cells.Length)
            {
                index = 0;
            }

            if (index > startIndex)
            {
                return null;
            }

        }

        print("here is the index of the random cell : " + index);

        return cells[index];


    }

    public TileCell getCell (int x, int y){
        if (x < width && x >= 0 && y < height && y >= 0 )
        {
        return rows[y].cells[x];
        }else{
            return null;
        }
    }

    public TileCell getCell (Vector2Int coordinates){
        return getCell(coordinates.x, coordinates.y);
    }

    public TileCell getNextCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        Debug.Log("here is new x and y" + coordinates.x  + coordinates.y);
        return getCell(coordinates);
    }
}
