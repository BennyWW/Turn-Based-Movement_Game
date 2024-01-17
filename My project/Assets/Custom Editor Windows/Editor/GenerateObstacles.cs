using UnityEngine;
using UnityEditor;
using System;

[System.Serializable]
public class GenerateObstacles : EditorWindow
{
    [SerializeField]
    public Obstacle obstacle;
    public bool reset;

    [MenuItem("Tools/ObstaclesGenerator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(GenerateObstacles));     //Get Window is a method inherited from EditorWindow class        
        
    }
    
    private void OnGUI() {
        EditorUtility.SetDirty(obstacle);
        obstacle = EditorGUILayout.ObjectField("Obstacle Manager",obstacle,typeof(Obstacle),false) as Obstacle;
        if(GUILayout.Button("Reset Obstacles"))
        {
            obstacle.obstacleArray.Clear();
        }
        if (GUILayout.Button("Create Empty Obstacles"))
        {
            obstacle.obstacleArray.Clear();
            for (int i = 0; i < 100; i++)
            {
                obstacle.obstacleArray.Add(false);
            }
        }

        GUI.BeginGroup(new Rect(0.5f,70f,1000,1000));
        GUI.Box(new Rect(10,0,320,330), "Tile Grid");
        if (obstacle == null) Debug.Log("Obstacle empty. Insert obstacle in Obstacle Generator window");
        else if (obstacle.obstacleArray.Capacity < 100)
        {
            Debug.Log("Please create empty obstacles first");
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {                    
                    Rect toggleBtn = new Rect(30 + 30 * i, 30 * (j + 1), 30, 30);
                    if (GUI.Toggle(toggleBtn, obstacle.obstacleArray[i * 10 + j], ""))
                    {     //if pressed                     
                        if (obstacle.obstacleArray.Capacity < 100)
                        {
                            Debug.Log("Please create empty obstacles first");
                        }
                        else
                        {
                            obstacle.obstacleArray[i * 10 + j] = true;
                        }
                    }
                    else
                    {
                        obstacle.obstacleArray[i * 10 + j] = false;
                    }
                }
            }
        }
        GUI.EndGroup();
    }
}
