using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<GameObject> Path = new();
    public Transform TargetPosition;
    public float lerpDuration = 1f;
    public List<GameObject> DuplicateLbl = new();
    public GameObject currentTile;
    // Update is called once per frame
    void Update()
    {
        Path = this.GetComponent<PathFinder>().Path;
        TargetPosition = this.GetComponent<PathFinder>().TargetPosition;
        if (this.GetComponent<PathFinder>().move)
        {
            foreach (GameObject tile in Path)
            {
                tile.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
            }
            StartCoroutine(MoveToTarget());
        }
    }
    public IEnumerator MoveToTarget()
    {
        if (Path.Capacity == 0) Debug.Log("Path list Empty");

        for (int i = Path.Count - 1; i >= 0;i--)
        {
            Vector3 newPos = new(Path[i].transform.position.x, 1f, Path[i].transform.position.z);
            transform.LookAt(newPos);
            yield return StartCoroutine(Move(transform.position, newPos));
        }
        transform.position = new Vector3(Path[0].transform.position.x, 1f, Path[0].transform.position.z);


        foreach (GameObject tile in Path)
        {
            tile.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
            TargetPosition.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        }

        this.GetComponent<PathFinder>().move = false;
        this.GetComponent<PathFinder>().LabeledTiles.Clear();
        this.GetComponent<PathFinder>().StartPosition = this.GetComponent<PathFinder>().Path[0].transform;
        this.GetComponent<PathFinder>().ResetLabelNos(true);
        currentTile = Path[0];
        this.GetComponent<PathFinder>().Path.Clear();
        transform.LookAt(new Vector3(this.GetComponent<PathFinder>().StartPosition.transform.position.x, 1f, this.GetComponent<PathFinder>().StartPosition.transform.position.z));      
        this.GetComponent<PathFinder>().moved = true;
    }


    private IEnumerator Move(Vector3 startPos, Vector3 endPos)
    {
        float timeElapsed = 0;
        while(timeElapsed < lerpDuration)
        {
            transform.LookAt(endPos);
            transform.position = Vector3.Lerp(startPos, endPos, timeElapsed/lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
    }
}
