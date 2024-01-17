using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDetails : MonoBehaviour
{
    public int gridX;
    public int gridY;

    public bool isBlocked;
    public Vector3 Position;

    public TileDetails Parent;

    public MouseHover mouseHoverRef;
    public int lblNo = -1;
    public List<Material> tileMaterial = new(2);

    private void Start()
    {
        mouseHoverRef = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MouseHover>();
    }
    private void Update()
    {
        Position = transform.position;
        if(mouseHoverRef.tilePos != transform.position)
        {
            this.gameObject.GetComponent<Renderer>().material = tileMaterial[1];
            transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z); 
        }
    }  
}
