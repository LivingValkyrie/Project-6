using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Script_PlayerHealth : NetworkBehaviour
{
    [SyncVar]
    public int syncHealth;           // Synced variable to hold the players health

    public string playerName;        // Holds the player name
    public int playerHealth = 100;   // Holds the players health
    public int respawnHealth = 75;   // Holds the health the player will respawn with

    public Text playerNameText;      // Text field that displays the players name.
    public Text playerHealthText;    // Text field that displays the players health.
    public Text respawnTimeText;     // Text field that displays respawn countdown.

    private GameObject respawnUI;    // Holds the respawn UI
    public int respawnTime = 10;     // How long until player respawns
    private int countdownTime;       // Variable to hold the current time the player has been dead
    private bool isDead;             // Boolean for if the player is dead

    private GameObject target;       // Holds the target in center of screen

    // @author: Nathan Boehning
    // @summary: Displays the player name and debugs the players life every two seconds.
    //           Keeps the players life synced inside of the server.
    public override void OnStartClient()
    {
        // Finds the various UI elements within the scene when its created
        playerNameText = GameObject.Find("TextPlayerName").GetComponent<Text>();
        playerHealthText = GameObject.Find("TextPlayerHealth").GetComponent<Text>();
        respawnUI = GameObject.Find("Canvas").transform.GetChild(3).gameObject;
        respawnTimeText = respawnUI.transform.GetChild(1).GetComponent<Text>();
        target = GameObject.Find("Image");

        // Sets the text of the text field to the player name
        playerNameText.text = playerName;

        // Sets the player name to host if it doesn't have a name
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Host";
        }

        string iD = "Player " + GetComponent<NetworkIdentity>().netId;
        transform.name = iD;

        // Set the variable that will be counted down to the designer defined variable
        countdownTime = respawnTime;

        // Sync the players initial health
        CmdSendHealthToServer(playerHealth);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            // Change the color of the player health text based on how much is left
            if (playerHealth > 60)
                playerHealthText.color = Color.green;
            else if (playerHealth > 25)
                playerHealthText.color = Color.yellow;
            else
                playerHealthText.color = Color.red;
                
            // Set the text to the players health
            playerHealthText.text = "Health: " + playerHealth;
            
            // If the players health is less than or equal to zero
            if (playerHealth <= 0)
            {
                playerHealth = 0;
                // If player hasn't already been set to dead
                if (!isDead)
                {
                    // enable the respawn UI
                    respawnUI.SetActive(true);

                    // Set the player to dead
                    isDead = true;

                    // Remove the target reticule
                    target.SetActive(false);

                    // Set the respawn countdown text
                    respawnTimeText.text = respawnTime.ToString();

                    // Disable the renderer and controller
                    gameObject.GetComponent<RigidbodyFirstPersonController>().enabled = false;
                    transform.GetChild(2).GetComponent<MeshRenderer>().enabled = false;

                    // Invoke Countdown timer to decrement time til respawn
                    InvokeRepeating("CountdownTimer", 0.0f, 1.0f);
                }
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

    // Decrements the time til player can respawn
    // Whenever it hits zero, it sets the position of the player to one of the spawn positions and resets all variables
    // To the state where player can play the game, and cancels the invoke on countdown so it doesn't
    // Continue to count down
    void CountdownTimer()
    {
        countdownTime -= 1;
        respawnTimeText.text = countdownTime.ToString();
        if (countdownTime <= 0)
        {
            gameObject.transform.position =
                GameObject.Find("Spawn Positions").transform.GetChild(Random.Range(0, 3)).transform.position;
            gameObject.GetComponent<RigidbodyFirstPersonController>().enabled = true;
            transform.GetChild(2).GetComponent<MeshRenderer>().enabled = false;
            isDead = false;
            countdownTime = respawnTime;
            respawnUI.SetActive(false);
            target.SetActive(true);
            playerHealth = respawnHealth;
            CancelInvoke("CountdownTimer");
        }
    }
}
