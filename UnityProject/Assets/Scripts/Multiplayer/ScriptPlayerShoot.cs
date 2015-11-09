using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ScriptPlayerShoot : NetworkBehaviour
{
    public ScriptPlayerWeapon weapon;

    [SerializeField]
    private Camera camera;

    private bool canFire = true;

    void Start()
    {
        if (camera == null)
        {
            Debug.Log("ScriptPlayerShoot: No camera referenced");
            this.enabled = false;
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (canFire)
                {
                    Shoot();
                    canFire = false;
                    StartCoroutine(RecycleWeapon());
                }
            }
        }
    }

    [Client]
    void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, weapon.range))
        {
                
            if (hit.collider.tag == "Player")
            {
                CmdPlayerShot(hit.collider.name);
            }
            if (hit.collider.tag == "Obstacle")
            {
                transform.position = hit.transform.GetChild(0).position;
            }
            else
            {
                Debug.Log("We hit: " + hit.collider.name);
            }
        }
    }

    [Command]
    void CmdPlayerShot(string inputName)
    {
        Debug.Log(inputName + " has been shot.");

        GameObject.Find(inputName).GetComponent<Script_PlayerHealth>().RemoveHealth(weapon.damage);
    }

    IEnumerator RecycleWeapon()
    {
        yield return new WaitForSeconds(TimeSpan.FromMilliseconds(weapon.rateOfFire).Seconds);
        canFire = true;
    }
}
