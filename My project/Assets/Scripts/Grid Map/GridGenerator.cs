using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public Obstacle obstacle;
    public Transform parent;
    //public Transform EnemyParent;
    public GameObject tile;
    public GameObject[,] mapGridArray;
    public List<GameObject> unblockedTiles;
    public int columns=10;
    public int rows=10;
    public Camera mainCamera;
    public GameObject player;
    private bool spawnedPlayer=false;
    public bool enemiesMoving;
    public bool enemiesSorted;
    public GameObject EnemyAI;
    public int EnemyCount=5;
    public List<GameObject> EnemyList;
    private int playerTileIndex;
    private int enemyTileIndex;
    public bool[] tileBools;
    void Start()
    {
        mainCamera.transform.position = new Vector3(columns/2,mainCamera.transform.position.y+columns/5,rows+3.5f);
        mapGridArray = new GameObject[columns,rows];
        GenerateGridMap();
        SpawnEnemyAI();
        SpawnPlayer();
        print(mapGridArray[8,5].GetComponent<TileDetails>().gridX+", "+mapGridArray[8,5].GetComponent<TileDetails>().gridY);   
    }
    private void Update()
    {
        if(!enemiesSorted)
            if (player.GetComponent<PathFinder>().moved)
            {
                SortAndIndexEnemies();
                enemiesSorted = true;
            }
    }
    void GenerateGridMap(){
        for(int i=0;i<columns;i++){
            for(int j=0;j<rows;j++){
                GameObject mapTile = Instantiate(tile,new Vector3(i,0,j),Quaternion.identity);
                mapTile.GetComponent<TileDetails>().isBlocked = obstacle.obstacleArray[(i*10)+j];
                if (!obstacle.obstacleArray[(i * 10) + j]) unblockedTiles.Add(mapTile);
                mapGridArray[i,j] = mapTile;
                mapTile.transform.SetParent(parent);
                mapTile.GetComponent<TileDetails>().gridX = i;
                mapTile.GetComponent<TileDetails>().gridY = j;
                mapTile.GetComponent<TileDetails>().lblNo = -1;
            }
        }
        tileBools = new bool[unblockedTiles.Count];
    }

    void SpawnPlayer(){
        if(!spawnedPlayer){
            //Instantiate(player,new Vector3(9,1,8),Quaternion.identity);    
            int tileIndex = Random.Range(0, unblockedTiles.Count - 1);
            while (tileBools[tileIndex])
            {
                tileIndex = Random.Range(0, unblockedTiles.Count - 1);
            }
            playerTileIndex = tileIndex;
            player = Instantiate(player, new Vector3(unblockedTiles[tileIndex].transform.position.x, 1f, unblockedTiles[tileIndex].transform.position.z), Quaternion.identity);
            spawnedPlayer = true;
        }
    }
    void SpawnEnemyAI(){
        for(int i=0;i<EnemyCount;i++){
            int tileIndex = Random.Range(0, unblockedTiles.Count - 1);
            while (tileIndex == playerTileIndex || tileBools[tileIndex])
            {
                tileIndex = Random.Range(0, unblockedTiles.Count - 1);
            }
            //EnemyAI.GetComponent<Enemy>().EnemyIndex = i;
            enemyTileIndex = tileIndex;
            GameObject newEnemyAI = Instantiate(EnemyAI, new Vector3(unblockedTiles[tileIndex].transform.position.x, 1f, unblockedTiles[tileIndex].transform.position.z), Quaternion.identity);
            newEnemyAI.GetComponent<Enemy>().EnemyIndex = i;
            EnemyList.Add(newEnemyAI);
            tileBools[tileIndex] = true;
        }
    }
    public void SortAndIndexEnemies()
    {
        List<GameObject> enemyList = new();
        //Sort enemies
        enemyList = EnemyList.OrderBy(x => Vector3.Distance(player.transform.position, x.transform.position)).ToList();
        if (enemyList.Count == EnemyList.Count)
        {
            EnemyList = enemyList;
        }

        //index enemies
        for(int i = 0; i <= EnemyList.Count-1; i++)
        {
            EnemyList[i].GetComponent<Enemy>().EnemyIndex = i;
        }
    }

}
