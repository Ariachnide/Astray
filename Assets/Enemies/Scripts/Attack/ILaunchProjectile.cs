using System;
using UnityEngine;

public interface ILaunchProjectile {
    public void HandleLaunch(
        Vector2 argDirection,
        GameObject argLauncher,
        float thrust,
        float flyDuration,
        Int16 argDmg,
        bool isAlly
    );
}
