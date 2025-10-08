using System;
using UnityEngine;

public interface ICast {
    public void Cast(Vector2 direction, AnimosityState hostility, GameObject argCaster, float thrust = 10f, float duration = 1.25f, Int16 argDmg = 0);
}
