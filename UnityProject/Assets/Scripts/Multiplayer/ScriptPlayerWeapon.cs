using UnityEngine;
using System.Collections;

[System.Serializable]
public class ScriptPlayerWeapon
{
    public string name = "Pistol";

    public int damage = 10;

    public float range = 50f;

    [Tooltip("Rate of fire of the weapon (ms)")]
    public float rateOfFire = 500;
}
