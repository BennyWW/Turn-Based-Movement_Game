using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAI
{
    void GetPath();
    void MoveToPlayer();
    void FindPlayer();
    void FindOtherEnemies();
}