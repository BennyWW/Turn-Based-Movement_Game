using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseHover : MonoBehaviour
{
    Camera cam;
    public Vector3 tilePos;
    public TextMeshProUGUI tilePostxt;
    public GameObject Player;
    public GameObject gridRef;
    public GameObject InputDisabledUI;

    void Start()
    {
        cam = Camera.main;
        InputDisabledUI.SetActive(false);
    }

    void Update()
    {
        Player = GameObject.FindWithTag("Player");
        gridRef = GameObject.FindGameObjectWithTag("Map");
        //Draw ray
        Vector3 mousePos =Input.mousePosition;
        mousePos.z = 10f;
        mousePos = cam.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(transform.position, mousePos-transform.position,Color.red);

        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Player.GetComponent<PathFinder>().move && !gridRef.GetComponent<GridGenerator>().enemiesMoving)
        {
            InputDisabledUI.SetActive(false);
            if (Physics.Raycast(ray, out hit, 100))
            {
                tilePos = hit.transform.GetComponent<TileDetails>().Position;
                tilePostxt.text = "X: " + tilePos.x + ", Z: " + tilePos.z;
                if (hit.transform.gameObject.GetComponent<TileDetails>().isBlocked == false && !(hit.transform.position.x == Player.transform.position.x && hit.transform.position.z == Player.transform.position.z))
                {
                    hit.transform.gameObject.GetComponent<Renderer>().material = hit.transform.gameObject.GetComponent<TileDetails>().tileMaterial[0];
                    hit.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                    hit.transform.position = new Vector3(hit.transform.position.x, .3f, hit.transform.position.z);
                }

                if (Input.GetMouseButtonDown(0) && hit.transform.gameObject.GetComponent<TileDetails>().isBlocked == false && !(hit.transform.position.x == Player.transform.position.x && hit.transform.position.z == Player.transform.position.z))
                {
                    Player.GetComponent<PathFinder>().findPath = true;
                    Player.GetComponent<PathFinder>().TargetPosition = hit.transform;
                }
            }
        }
        else InputDisabledUI.SetActive(true);
        
    }

    
}
