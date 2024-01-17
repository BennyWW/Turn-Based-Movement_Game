using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathFinder : MonoBehaviour
{
    //Wavefront Pathfinding algorithm --------------------------------------------------------------------------------------------------------------------------------------
    public GameObject GridRef;
    public Transform StartPosition;
    public Transform TargetPosition;

    public List<GameObject> Path;
    public List<GameObject> cleanPath;
    public List<GameObject> NearbyTileList;
    public List<GameObject> LabeledTiles = new(); //Tiles used to label the other tiles in the loop 
    public bool findPath;
    public List<GameObject> gridTiles;

    public bool pathFound;
    public bool move;
    public bool moved;
    public bool targetFound;
    public Transform PlayerRef;
    public float movementSpeed = 5f;

    public GameObject nearestTile;

    private void Start()
    {
        GridRef = GameObject.FindGameObjectWithTag("Map");
        StartPosition = transform;

        //Get the grid tiles that are not blocked        
        List<GameObject> tilesRef = new List<GameObject>(GameObject.FindGameObjectsWithTag("Map Tile"));
        foreach(GameObject tile in tilesRef)
        {
            if (tile.GetComponent<TileDetails>().isBlocked == false)
            {
                gridTiles.Add(tile);
            }
            ResetLabelNos(true);
        }
        PlayerRef = GameObject.FindGameObjectWithTag("Player").transform;
    }


    private void Update()
    {        
        if (findPath)
        {
            LabelTiles();
            if (targetFound)
            {
                Debug.Log("All tiles Labeled");
                Path.Add(TargetPosition.gameObject);
                FindPath();
                findPath = false;
                targetFound = false;
            }
        }
    }
    public void ResetLabelNos(bool x)
    {
        foreach (GameObject tile in gridTiles)
        {
            if (x)
            {
                if (tile.transform.position.x == (int)StartPosition.position.x && tile.transform.position.z == (int)StartPosition.position.z)
                {
                    if (!LabeledTiles.Contains(tile)) LabeledTiles.Add(tile);
                    tile.GetComponent<TileDetails>().lblNo = 0;
                }
                else
                {
                    tile.GetComponent<TileDetails>().lblNo = -1;
                }
            }
            else
            {
                if (tile.transform.position.x == (int)StartPosition.position.x && tile.transform.position.z == (int)StartPosition.position.z)
                {
                    if (!LabeledTiles.Contains(tile)) LabeledTiles.Add(tile);
                    tile.GetComponent<TileDetails>().lblNo = -1;
                }
                else
                {
                    tile.GetComponent<TileDetails>().lblNo = -1;
                }
            }
            
        }
    }
    void LabelTiles()
    {
        foreach (GameObject lblTile in LabeledTiles)
        {            
            int currentLabelNo = lblTile.GetComponent<TileDetails>().lblNo;
            foreach (GameObject tile in gridTiles)
            {
                int labelNo = tile.GetComponent<TileDetails>().lblNo;
                if (tile.transform.position.x == lblTile.transform.position.x && tile.transform.position.z == lblTile.transform.position.z + 1 && !tile.GetComponent<TileDetails>().isBlocked && labelNo < currentLabelNo && !LabeledTiles.Contains(tile)) //Tile z +1 (up)
                {
                    tile.GetComponent<TileDetails>().lblNo = currentLabelNo + 1;
                    LabeledTiles.Add(tile);
                }
                if (tile.transform.position.x == lblTile.transform.position.x + 1 && tile.transform.position.z == lblTile.transform.position.z && !tile.GetComponent<TileDetails>().isBlocked && labelNo < currentLabelNo && !LabeledTiles.Contains(tile)) //Tile x +1 (right)
                {
                    tile.GetComponent<TileDetails>().lblNo = currentLabelNo + 1;
                    LabeledTiles.Add(tile);
                }
                if (tile.transform.position.x == lblTile.transform.position.x && tile.transform.position.z == lblTile.transform.position.z - 1 && !tile.GetComponent<TileDetails>().isBlocked && labelNo < currentLabelNo && !LabeledTiles.Contains(tile)) //Tile z -1 (down)
                {
                    tile.GetComponent<TileDetails>().lblNo = currentLabelNo + 1;
                    LabeledTiles.Add(tile);
                }
                if (tile.transform.position.x == lblTile.transform.position.x - 1 && tile.transform.position.z == lblTile.transform.position.z && !tile.GetComponent<TileDetails>().isBlocked && labelNo < currentLabelNo && !LabeledTiles.Contains(tile)) //Tile x -1 (left)
                {
                    tile.GetComponent<TileDetails>().lblNo = currentLabelNo + 1;
                    LabeledTiles.Add(tile);
                }

                if (LabeledTiles.Contains(TargetPosition.gameObject))
                {
                    targetFound = true;
                    break;
                }
            }
        }
    }
    void FindPath()
    {
        foreach (GameObject tile in gridTiles)
        {
            if (tile.transform.position.x == transform.position.x && tile.transform.position.z == transform.position.z)
            {
                StartPosition = tile.transform;
            }
        }
        foreach (GameObject tile in LabeledTiles)
        {
            int targetLabelNo = TargetPosition.gameObject.GetComponent<TileDetails>().lblNo;
            int labelNo = tile.GetComponent<TileDetails>().lblNo;
            if (tile.transform.position.x == TargetPosition.position.x && tile.transform.position.z == TargetPosition.position.z + 1 && !tile.GetComponent<TileDetails>().isBlocked && labelNo < targetLabelNo && !NearbyTileList.Contains(tile)) //Tile z + 1 (up)
                NearbyTileList.Add(tile);
            if (tile.transform.position.x == TargetPosition.position.x + 1 && tile.transform.position.z == TargetPosition.position.z && !tile.GetComponent<TileDetails>().isBlocked && labelNo < targetLabelNo && !NearbyTileList.Contains(tile)) //Tile x + 1 (right)
                NearbyTileList.Add(tile);
            if (tile.transform.position.x == TargetPosition.position.x && tile.transform.position.z == TargetPosition.position.z - 1 && !tile.GetComponent<TileDetails>().isBlocked && labelNo < targetLabelNo && !NearbyTileList.Contains(tile)) //Tile z - 1 (down)
                NearbyTileList.Add(tile);
            if (tile.transform.position.x == TargetPosition.position.x - 1 && tile.transform.position.z == TargetPosition.position.z && !tile.GetComponent<TileDetails>().isBlocked && labelNo < targetLabelNo && !NearbyTileList.Contains(tile)) //Tile x - 1 (left)
                NearbyTileList.Add(tile);
        }
        //Find the tile closest to the target
        float ShortestDistance = 100f;
        foreach (GameObject tile in NearbyTileList)
        {
            tile.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            if (Vector2.Distance(tile.transform.position,StartPosition.position) < ShortestDistance)
            {
                ShortestDistance = Vector2.Distance(tile.transform.position, StartPosition.position);
                nearestTile = tile;
            }
        }
        if(!Path.Contains(nearestTile)) Path.Add(nearestTile);
        TargetPosition = nearestTile.transform;
        NearbyTileList.Clear();

        if (!Path.Contains(StartPosition.gameObject) && Path.Count - 1 <= LabeledTiles[LabeledTiles.Count-1].GetComponent<TileDetails>().lblNo)
        {
            FindPath();
        }
        else
        {
            Debug.Log("Path added");
        }
        
        move = true;
        findPath = false;
    }   
}
