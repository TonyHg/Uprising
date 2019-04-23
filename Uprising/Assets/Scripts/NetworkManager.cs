﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Uprising.Players;
using Photon.Pun;
using Photon.Realtime;

using System.Runtime.InteropServices;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public bool inMatchMaking = false;
    public bool inMainMenu = false;
    public Text matchMakingText;
    public int MaxPlayer = 2;
    private bool isInGame = false;
    public Text StartingText;
    public MainMenu mainMenu;
    PlayerStats localPlayerGameStats;
    Dictionary<Player, PlayerStats> players;
    Stack<Player> scoreboard;
    
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern string GetUsername();
#endif


    // Start is called before the first frame update
    void Start()
    {
        StartingText.text = "Connection...";
        
#if UNITY_WEBGL
        Debug.Log("Trying to get username (WEBGL).");
        Debug.Log(GetUsername());
        PhotonNetwork.LocalPlayer.NickName = GetUsername();
#endif

        PhotonNetwork.ConnectUsingSettings();
        // PhotonNetwork.ConnectToRegion("eu");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected!");
        PhotonNetwork.JoinLobby();
        StartingText.text = "";
        if(!inMainMenu)
            JoinMainMenu();
    }

    public void JoinMainMenu()
    {
        inMainMenu = true;
        Debug.Log("Loading Main Menu");
        // Load the main menu scene. SetMainMenu() will then be called to get the Main Menu
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }

    public void SetMainMenu(MainMenu mainMenu)
    {
        Debug.Log("Main menu loaded!");
        this.mainMenu = mainMenu;
        mainMenu.matchMakingText.text = "";
    }

    public void PlayRandom()
    {
        if(PhotonNetwork.InLobby)
        {
            // Try to join a random room..
            PhotonNetwork.JoinRandomRoom();
            mainMenu.matchMakingText.text = "Joinning...";
        }
        else
        {
            mainMenu.matchMakingText.text = "Connecting to server...";
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No available room found. Creating a new one...");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)MaxPlayer;
        roomOptions.PublishUserId = true;
        bool success = PhotonNetwork.CreateRoom(null, roomOptions);
        if(!success)
        {
            PhotonNetwork.Disconnect();
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex(1));
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause == DisconnectCause.DisconnectByClientLogic)
        {
            StartingText.text = "Connection...";
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }

    public override void OnJoinedRoom()
    {
        inMatchMaking = true;
        Debug.Log("Room joined");
    }

    public void CancelPlay()
    {
        inMatchMaking = false;
        mainMenu.matchMakingText.text = "";
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.JoinLobby();
        Debug.Log("Room left");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby joined");
        mainMenu.matchMakingText.text = "";
        isInGame = false;
    }

    private void Update()
    {
        // Check if all players are ready to start the game
        if (inMatchMaking)
        {
            Photon.Realtime.Room currentRoom = PhotonNetwork.CurrentRoom;
            if (currentRoom == null) return;
            mainMenu.matchMakingText.text = "Waiting for players... " + currentRoom.PlayerCount + "/"+ MaxPlayer;

            if(currentRoom.PlayerCount == MaxPlayer && !isInGame)
            {
                // All players ready, start the game
                StartGame();
            }
        }
    }

    private void StartGame()
    {
        // Lock the room
         //if (PhotonNetwork.LocalPlayer.IsMasterClient)
            //PhotonNetwork.CurrentRoom.IsOpen = false;


        // Unload main menu, display loading time
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Main Menu"));
        StartingText.text = "Loading world...";

        // Add map 1 to loaded scene
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
        Debug.Log("Load map 1 scene");
        inMatchMaking = false;
        isInGame = true;
        inMainMenu = false;
    }

    public void OnGameLoaded()
    {
        StartingText.text = "";
    }

    public void LeaveToScoreBoard(Stack<Player> scoreboard, Dictionary<Player, PlayerStats> players)
    {
        Debug.Log("Leaving to scoreBoard...");
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex(2));
        this.scoreboard = scoreboard;
        // this.localPlayerGameStats = stats;
        this.players = players;
        StartCoroutine("LoadScoreBoardScene");
    }

    IEnumerator LoadScoreBoardScene()
    {
        AsyncOperation loadScene = SceneManager.LoadSceneAsync("Scoreboard", LoadSceneMode.Additive);
        while(!loadScene.isDone)
        {
            // Wait until loading is completed
            yield return null;
        }

        ScoreBoardManager scoreBoardManager = GameObject.Find("ScoreBoard").GetComponent<ScoreBoardManager>();
        scoreBoardManager.SetStats(this.scoreboard, this.players);
    }

    public void QuitGame(bool isLastInRoom = false, bool isInScoreBoardScene = true)
    {
        if (isLastInRoom)
            PhotonNetwork.CurrentRoom.IsOpen = true;
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
            PhotonNetwork.DestroyAll();
        isInGame = false;
        PhotonNetwork.LeaveRoom(); // Leaving the room will automatically re-join the server, and then call OnConnected()

        if (!isInScoreBoardScene)
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex(2));
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex(3));
    }
}
