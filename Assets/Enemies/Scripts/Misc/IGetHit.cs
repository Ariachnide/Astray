using System;
using UnityEngine;

public interface IGetHit {
    public void GetHit(Int16 damage, GameObject blowGiver, Vector3 enemyPosition, float kbThrust = 25f, float kbDuration = 0.1f, string weaponName = "");
}
