using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: EnableOnDisable 
/// </summary>
public class EnableOnDisable : NetworkBehaviour {
    #region Fields

    #endregion

    void Start() {
        transform.SetParent(null);
    }

    void Update() { }

    void OnDisable() {
        //this.gameObject.SetActive(true);
    }

}