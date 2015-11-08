using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: GenerateObstacles 
/// </summary>
public class GenerateObstacles : NetworkBehaviour {
    #region Fields

    [SyncVar]
    public int objectCount;
    
    public Vector2 xBounds, yBounds, zBounds;

    public GameObject obstaclePrefab;
    
    GameObject[] obstacles;

    #endregion

    //@author Marshall Mason
    void Start()
	{
        MenuDataHolder data = GameObject.Find("Network Manager").GetComponent<MenuDataHolder>();
        
        //pull in data from MenuDataHolder
        if (data.isServer)
        {
            objectCount = data.numObstacles;
            xBounds = new Vector2(data.minX, data.maxX);
            yBounds = new Vector2(data.minY, data.maxY);
            zBounds = new Vector2(data.minZ, data.maxZ);
            Generate();
        }
    }

    void Generate() {
        obstacles = new GameObject[objectCount];

        for (int i = 0; i < objectCount; i++) {
            //generate random xyz within ranges.
            Vector3 rndPos = new Vector3(Random.Range(xBounds.x, xBounds.y),
                                         Random.Range(yBounds.x, yBounds.y),
                                         Random.Range(zBounds.x, zBounds.y));
            obstacles[i] = Instantiate(obstaclePrefab);
            obstacles[i].transform.position = rndPos;
        }
    }

	//@author Marshall Mason
	void OnPlayerConnect(NetworkPlayer client)
	{
		NetworkViewID[] networkViewIDs = new NetworkViewID[obstacles.Length];
		Vector3[] positions = new Vector3[obstacles.Length];

		for (int i = 0; i < obstacles.Length; i++)
		{
			networkViewIDs[i] = obstacles[i].GetComponent<NetworkView>().viewID;
			positions[i] = obstacles[i].transform.position;
		}

		GetComponent<NetworkView> ().RPC ("SpawnObstacles", client, networkViewIDs, positions);
	}

	[RPC] //@author Marshall Mason
	void SpawnObstacles(NetworkViewID[] networkViewIDs, Vector3[] positions)
	{
		for (int i = 0; i < networkViewIDs.Length; i++)
		{
			if(!networkViewIDs[i].isMine)
			{
				GameObject temp = (GameObject)Instantiate (obstaclePrefab, positions[i], Quaternion.identity);
				temp.GetComponent<NetworkView>().viewID = networkViewIDs[i];
			}
		}
	}
}