using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ObstacleManager : MonoBehaviour
{   
    public GameObject redSphere;
    private GameObject obstaclesParent;     
    public Obstacle obstacle;
    private bool[,] noInstantiate = new bool [1000,1000];
    public GameObject[] gridTiles;

    private void Start() {
        obstaclesParent = new GameObject("Obstacles");
        obstaclesParent.transform.SetParent(this.transform);

        //Get the grid 
        gridTiles = GameObject.FindGameObjectsWithTag("Map Tile");
        
    }
    private void Update() {
        gridTiles = GameObject.FindGameObjectsWithTag("Map Tile");
        for (int i=0;i<10;i++){
            for(int j=0;j<10;j++){
                if(obstacle.obstacleArray[i*10+j] && noInstantiate[i,j]==false){
                    GameObject obstacleObj = Instantiate(redSphere,new Vector3(i,0.75f,j),Quaternion.Euler(0,0,0));
                    obstacleObj.transform.SetParent(obstaclesParent.transform);
                    noInstantiate[i,j]=true;
                    foreach(GameObject tile in gridTiles)
                    {
                        //Vector3 obstaclePos = new Vector3(i, 0f, j);
                        if (tile.transform.position.x == i && tile.transform.position.z == j)
                        {
                            tile.GetComponent<TileDetails>().isBlocked = true;
                            print("Tile" + i + ", " + j + " blocked");
                        }                            
                    }
                }
            }
        }
    }
    
}
