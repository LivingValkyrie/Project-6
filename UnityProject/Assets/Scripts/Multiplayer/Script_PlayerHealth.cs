using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Script_PlayerHealth : NetworkBehaviour
{
    [SyncVar (hook = "UpdatePlayerHealth")]
    public int syncHealth;           // Synced variable to hold the players health

    public string playerName;        // Holds the player name
    public int playerHealth = 100;   // Holds the players health
    public int respawnHealth = 75;   // Holds the health the player will respawn with

    public Text playerNameText;      // Text field that displays the players name.
    public Text playerHealthText;    // Text field that displays the players health.
    public Text respawnTimeText;     // Text field that displays respawn countdown.

    private GameObject respawnUI;
    public int respawnTime = 10;
    private int countdownTime;
    private bool isDead;

    // @author: Nathan Boehning
    // @summary: Displays the player name and debugs the players life every two seconds.
    //           Keeps the players life synced inside of the server.
    private void Start()
    {
        // Finds the various UI elements within the scene when its created
        playerNameText = GameObject.Find("TextPlayerName").GetComponent<Text>();
        playerHealthText = GameObject.Find("TextPlayerHealth").GetComponent<Text>();
        respawnUI = GameObject.Find("Canvas").transform.GetChild(3).gameObject;
        respawnTimeText = respawnUI.transform.GetChild(1).GetComponent<Text>();


        // Sets the player name to host if it doesn't have a name
        if (playerName == "") {
            playerName = "Host";
            }

        // Sets the text of the text field to the player name
        playerNameText.text = playerName;

        gameObject.name = playerName;

        // Set the variable that will be counted down to the designer defined variable
        countdownTime = respawnTime;

        // Sync the players initial health
        CmdSendHealthToServer(playerHealth);
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (playerHealth > 60)
                playerHealthText.color = Color.green;
            else if (playerHealth > 25)
                playerHealthText.color = Color.yellow;
            else
                playerHealthText.color = Color.red;
                
            playerHealthText.text = "Health: " + playerHealth;
            if (playerHealth <= 0)
            {
                if (!isDead)
                {
                    respawnUI.SetActive(true);
                    isDead = true;
                    respawnTimeText.text = respawnTime.ToString();
                    this.gameObject.GetComponent<RigidbodyFirstPersonController>().enabled = false;
                    InvokeRepeating("CountdownTimer", 0.0f, 1.0f);
                }
            }

            if (Input.GetKey(KeyCode.H))
            {
                RemoveHealth(5);
            }
        }
    }

    [Client]
    public void AddHealth(int healthToAdd)
    {
        // Adds health to the local player, and sends a command to update the players health 
        // within the server.
        if (isLocalPlayer)
        {
            playerHealth += healthToAdd;
            CmdSendHealthToServer(playerHealth);
        }
    }

    [Client]
    public void RemoveHealth(int healthToRemove)
    {
        // Removes health from the local player, and sends a command to update the players heath
        // within the server
        if (isLocalPlayer)
        {
            playerHealth -= healthToRemove;
            CmdSendHealthToServer(playerHealth);
        }
    }

    // Sets the synced variable to the updated player health
    [Command]
    private void CmdSendHealthToServer(int healthToSend)
    {
        syncHealth = healthToSend;
    }

    void UpdatePlayerHealth(int input)
    {
        Debug.Log("input: " + input);
        syncHealth = input;
        playerHealth = syncHealth;
    }

    void CountdownTimer()
    {
        countdownTime -= 1;
        respawnTimeText.text = countdownTime.ToString();
        if (countdownTime <= 0)
        {
            this.gameObject.transform.position =
                GameObject.Find("Spawn Positions").transform.GetChild(Random.Range(0, 3)).transform.position;
            this.gameObject.GetComponent<RigidbodyFirstPersonController>().enabled = true;
            isDead = false;
            countdownTime = respawnTime;
            respawnUI.SetActive(false);
            playerHealth = respawnHealth;
            CancelInvoke("CountdownTimer");
        }
    }
}
