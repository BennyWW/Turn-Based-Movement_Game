using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IAI
{
    public int EnemyIndex = 0;
    public List<GameObject> Path = new();
    public Transform PlayerRef;
    public GameObject GridMapRef;
    public Transform TargetPosition;
    public List<GameObject> EnemyList;
    public float lerpDuration;
    public List<GameObject> gridTiles;
    public GameObject avoidTile;
    public List<GameObject> tileList;
    public List<GameObject> enemyTiles;

    void Start()
    {
        FindPlayer();
        GridMapRef = this.GetComponent<PathFinder>().GridRef;
    }

    // Update is called once per frame
    void Update()
    {
        GetPath();
        if (GridMapRef.GetComponent<GridGenerator>().enemiesSorted)
        {
            //If player moved and this enemy is index 0, move to target
            MoveToPlayer();
        }
        
    }

    #region AI Interface Functions
    public void GetPath()
    {
        Path = this.GetComponent<PathFinder>().Path;        
    }

    public void MoveToPlayer()
    {
        if (PlayerRef.GetComponent<PathFinder>().moved && !this.GetComponent<PathFinder>().moved)
        {
            print("Changing target");
            TargetPosition = PlayerRef.gameObject.GetComponent<Player>().currentTile.transform;
            FindOtherEnemies();
            if (EnemyIndex == 0 && !this.GetComponent<PathFinder>().findPath)
            {
                GridMapRef.GetComponent<GridGenerator>().enemiesMoving = true;
                this.GetComponent<PathFinder>().LabeledTiles[0].GetComponent<TileDetails>().lblNo = 0;
                this.GetComponent<PathFinder>().findPath = true;
                this.GetComponent<PathFinder>().TargetPosition = TargetPosition;
                if (this.GetComponent<PathFinder>().move)
                {
                    foreach (GameObject tile in Path)
                    {
                        if (tile != TargetPosition.gameObject)
                            tile.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
                    }
                    StartCoroutine(MoveToTarget());
                }
            }
            else
            {
                if (this.GetComponent<PathFinder>().move)
                {
                    if (GridMapRef.GetComponent<GridGenerator>().EnemyList[EnemyIndex - 1].GetComponent<PathFinder>().moved)
                    {
                        foreach (GameObject tile in Path)
                        {
                            if (tile != TargetPosition.gameObject)
                                tile.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
                        }
                        StartCoroutine(MoveToTarget());
                    }
                }
            }
        }
           
    }
    public void FindPlayer()
    {
        PlayerRef = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void FindOtherEnemies()
    {
        EnemyList = GridMapRef.GetComponent<GridGenerator>().EnemyList;
    }
    #endregion
    public IEnumerator MoveToTarget()
    {
        if (Path.Capacity == 0) Debug.Log("Path list Empty");
        Path.Remove(TargetPosition.gameObject);
        foreach (GameObject tile in GridMapRef.GetComponent<GridGenerator>().unblockedTiles)
        {
            if (tile.transform.position.x == transform.position.x && tile.transform.position.z == transform.position.z + 1 && !tileList.Contains(tile)) //Tile z +1 (up)
            {
                tileList.Add(tile);
            }
            if (tile.transform.position.x == transform.position.x + 1 && tile.transform.position.z == transform.position.z && !tileList.Contains(tile)) //Tile x +1 (right)
            {
                tileList.Add(tile);
            }
            if (tile.transform.position.x == transform.position.x && tile.transform.position.z == transform.position.z - 1 && !tileList.Contains(tile)) //Tile z -1 (down)
            {
                tileList.Add(tile);
            }
            if (tile.transform.position.x == transform.position.x - 1 && tile.transform.position.z == transform.position.z && !tileList.Contains(tile)) //Tile x -1 (left)
            {
                tileList.Add(tile);
            }
        }
        foreach(GameObject tile in Path)
        {
            foreach(GameObject enemy in EnemyList)
            {
                if(enemy!=gameObject)
                    if(tile.transform.position.x == enemy.transform.position.x && tile.transform.position.z == enemy.transform.position.z && tile == Path[Path.Count-1])
                        if (!enemyTiles.Contains(tile)) enemyTiles.Add(tile);
            }
        }

        if (!tileList.Contains(PlayerRef.GetComponent<Player>().currentTile) && !enemyTiles.Contains(Path[Path.Count-1]))
        {            
            foreach (GameObject tile in Path)
            {
                if (tile == PlayerRef.GetComponent<Player>().currentTile)
                {
                    avoidTile = tile;
                    break;
                }
                foreach (GameObject enemy in EnemyList)
                {
                    if (enemy != this.gameObject)
                        if (tile.transform.position.x == enemy.transform.position.x && tile.transform.position.z == enemy.transform.position.z)
                        {
                            avoidTile = tile;
                        }
                }
            }

            for (int i = Path.Count - 1; i >= 0; i--)
            {
                Vector3 newPos = new(Path[i].transform.position.x, 1f, Path[i].transform.position.z);                
                yield return StartCoroutine(Move(transform.position, newPos));
                if (i > 0 && Path[i - 1] && Path[i - 1] == avoidTile)
                {
                    transform.position = new Vector3(avoidTile.transform.position.x, 1f, avoidTile.transform.position.z);
                    break;
                }
            }
            if (!avoidTile)
                transform.position = new Vector3(Path[0].transform.position.x, 1f, Path[0].transform.position.z);
            

            if (!avoidTile) this.GetComponent<PathFinder>().StartPosition = this.GetComponent<PathFinder>().Path[1].transform;
            else this.GetComponent<PathFinder>().StartPosition = avoidTile.transform;
            avoidTile = null;

            FinishMovement();
        }
        else
        {
            FinishMovement();
        }
    }
    private void FinishMovement()
    {
        foreach (GameObject tile in Path)
        {
            tile.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
            TargetPosition.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        }

        enemyTiles.Clear();
        tileList.Clear();
        this.GetComponent<PathFinder>().move = false;
        PlayerRef.GetComponent<PathFinder>().LabeledTiles[0].GetComponent<TileDetails>().lblNo = 0;

        this.GetComponent<PathFinder>().LabeledTiles.Clear();
        this.GetComponent<PathFinder>().ResetLabelNos(false);
        transform.LookAt(PlayerRef.transform);
        this.GetComponent<PathFinder>().moved = true;
        this.GetComponent<PathFinder>().findPath = false;
        this.GetComponent<PathFinder>().Path.Clear();

        if (EnemyIndex == GridMapRef.GetComponent<GridGenerator>().EnemyList.Count - 1)
        {
            PlayerRef.GetComponent<Player>().currentTile.GetComponent<TileDetails>().lblNo = 0;
            PlayerRef.GetComponent<PathFinder>().moved = false;
            GridMapRef.GetComponent<GridGenerator>().enemiesMoving = false;
            GridMapRef.GetComponent<GridGenerator>().enemiesSorted = false;
            foreach (GameObject enemy in EnemyList)
            {
                enemy.GetComponent<PathFinder>().moved = false;
            }
        }

        foreach (GameObject enemy in EnemyList)
        {
            if (enemy.GetComponent<Enemy>().EnemyIndex == this.GetComponent<Enemy>().EnemyIndex + 1)
            {
                enemy.GetComponent<PathFinder>().LabeledTiles[0].GetComponent<TileDetails>().lblNo = 0;
                PlayerRef.GetComponent<PathFinder>().LabeledTiles[0].GetComponent<TileDetails>().lblNo = -1;
                enemy.GetComponent<PathFinder>().findPath = true;
                enemy.GetComponent<PathFinder>().TargetPosition = TargetPosition;
            }
        }
    }

    private IEnumerator Move(Vector3 startPos, Vector3 endPos)
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            transform.LookAt(endPos);
            transform.position = Vector3.Lerp(startPos, endPos, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
    }
}