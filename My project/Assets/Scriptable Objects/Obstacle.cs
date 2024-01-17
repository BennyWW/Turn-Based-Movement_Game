using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Obstacle",menuName = "Obstacle")]
[System.Serializable]
public class Obstacle : ScriptableObject
{
    [SerializeField]
    
    public List<bool> obstacleArray = new List<bool>(100);
}
