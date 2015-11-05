using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ScriptPlayerShoot : NetworkBehaviour
{
    public ScriptPlayerWeapon weapon;

    [SerializeField]
    private Camera camera;

    [SerializeField]
    private LayerMask mask;

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
                Shoot();
            }
        }
    }

    [Client]
    void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, weapon.range, mask))
        {
            if (hit.collider.tag == "Untagged")
            {
                CmdPlayerShot(hit.collider.name);
            }
            
        }
    }

    [Command]
    void CmdPlayerShot(string name)
    {
        Debug.Log(name + " has been shot.");

        GameObject.Find(name).GetComponent<Script_PlayerHealth>().RemoveHealth(5);
    }
}
