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
    [SyncVar]
    Vector3 curObstacle;
    
    public Vector2 xBounds, yBounds, zBounds;

    public GameObject obstaclePrefab;
    
    GameObject[] obstacles;

    #endregion

    //@author Marshall Mason
    void Start() {
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
        else
        {
            for (int i = 0; i < objectCount; i++)
            {
                CmdFetchMap(i);
                data.InstantiateObstacle(curObstacle, obstaclePrefab);
            }
        }
    }

    //@author Marshall Mason
    [Command]
    void CmdFetchMap(int index)
    {
        Vector3 toReturn = obstacles[index].transform.position;
        curObstacle = toReturn;
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
}