using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GoogleARCore;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

//skal bruge networkManager 
[RequireComponent(typeof(NetworkManager))]
public class NetworkManagerUIController : MonoBehaviour
{
    /// <summary>
    /// lobby screen er til at lave og finde et nyt rum
    /// </summary>
    public Canvas LobbyScreen;
    public Text SnackbarText;

    /// <summary>
    /// label til at vise ative room
    /// </summary>
    public GameObject CurrentRoomLabel;

    public CloudAnchorsController CloudAnchorsController;

    /// <summary>
    /// et panel til at vise en lise af rum som kan join
    /// </summary>
    public GameObject RoomListPanel;

    public Text NoPreviousRoomsText;
    public GameObject JoinRoomListRowPrefab;

    /// <summary>
    /// antalt af kampe som kan se i rum listen
    /// </summary>
    public const int MatchPageSize = 5;

    private NetworkManager Manager;

    private string CurrentRoomNumber;

    private List<GameObject> JoinRoomButtonsPool = new List<GameObject>();

    public void Awake()
    {
        //tilføjer nye knapper til JoinRoomButtonsPool listen
        for (int i = 0; i < MatchPageSize; i++)
        {
            GameObject button = Instantiate(JoinRoomListRowPrefab);
            button.transform.SetParent(RoomListPanel.transform, false);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100 -(200 * i));
            button.SetActive(true);
            button.GetComponentInChildren<Text>().text = string.Empty;
            JoinRoomButtonsPool.Add(button);
        }

        Manager = GetComponent<NetworkManager>();
        Manager.StartMatchMaker();
        Manager.matchMaker.ListMatches(
            startPageNumber: 0, resultPageSize: MatchPageSize, matchNameFilter: string.Empty,
            filterOutPrivateMatchesFromResults: false, eloScoreTarget: 0, requestDomain: 0, callback: OnMatchList);
        ChangeLobbyUIVisibility(true);
    }

    public void OnCreateRoomClicked()
    {
        Manager.matchMaker.CreateMatch(Manager.matchName, Manager.matchSize, true, string.Empty, string.Empty,
            string.Empty, 0, 0, OnMatchCreate);
    }

    //funtion til at refresh rum til check af nye rum
    public void OnRefhreshRoomListClicked()
    {
        Manager.matchMaker.ListMatches(startPageNumber: 0,
            resultPageSize: MatchPageSize,
            matchNameFilter: string.Empty,
            filterOutPrivateMatchesFromResults: false,
            eloScoreTarget: 0,
            requestDomain: 0,
            callback: OnMatchList);
    }

    public void OnAnchorInstantiated(bool isHost)
    {
        if (isHost)
        {
            SnackbarText.text = "Hosting board";
        }
        else
        {
            SnackbarText.text = "forsøger på at løse anchor";
        }
    }

    public void OnAnchorHosted(bool success, string response)
    {
        if (success)
        {
            SnackbarText.text = "Forbindelse til Cloud Anchor successfuldt";
        }
        else
        {
            SnackbarText.text = "kan ikke oprette forbindelse til Cloud Anchor " + response;
        }
    }

    public void OnAnchorResolved(bool success, string response)
    {
        if (success)
        {
            SnackbarText.text = "Cloud Anchor successfully";
        }
        else
        {
            SnackbarText.text = "Cloud Anchor kan ikke blive løst";
        }
    }

    private void OnJoinRoomClicked(MatchInfoSnapshot match)
    {
        Manager.matchName = match.name;
        Manager.matchMaker.JoinMatch(match.networkId, string.Empty, string.Empty,
            string.Empty, 0, 0, OnMatchJoined);
        
        CloudAnchorsController.OnEnterResolvingModeClick();
    }

    private void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        Manager.OnMatchList(success, extendedInfo, matches);
        if (!success)
        {
            SnackbarText.text = "Kunne ikke oprette en kamp liste: " + extendedInfo;
            return;
        }
        if (Manager.matches != null)
        {
            // Reset all buttons in the pool.
            foreach (GameObject button in JoinRoomButtonsPool)
            {
                button.SetActive(false);
                button.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                button.GetComponentInChildren<Text>().text = string.Empty;
            }

            NoPreviousRoomsText.gameObject.SetActive(Manager.matches.Count == 0);

            // Add buttons for each existing match.
            int i = 0;
            foreach (var match in Manager.matches)
            {
                if (i >= MatchPageSize)
                {
                    break;
                }

                var text = "Room " + GeetRoomNumberFromNetworkId(match.networkId);
                GameObject button = JoinRoomButtonsPool[i++];
                button.GetComponentInChildren<Text>().text = text;
                button.GetComponentInChildren<Button>().onClick.AddListener(() => OnJoinRoomClicked(match));
                button.SetActive(true);
            }
        }
    }

    private void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        Manager.OnMatchCreate(success, extendedInfo, matchInfo);
        if (!success)
        {
            SnackbarText.text = "Kunne ikke starte kamp: " + extendedInfo;
            return;
        }

        CurrentRoomNumber = GeetRoomNumberFromNetworkId(matchInfo.networkId);
        ChangeLobbyUIVisibility(false);
        SnackbarText.text = "Find et grid til at oprette bræt";
        CurrentRoomLabel.GetComponentInChildren<Text>().text = "Rum: " + CurrentRoomNumber;
    }

    private void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        Manager.OnMatchJoined(success, extendedInfo, matchInfo);
        if (!success)
        {
            SnackbarText.text = "Kan ikke forbinde til kampen:" + extendedInfo;
            return;
        }

        CurrentRoomNumber = GeetRoomNumberFromNetworkId(matchInfo.networkId);
        ChangeLobbyUIVisibility(false);
        SnackbarText.text = "Fejl for cloud anchor til at lave en host:";
        CurrentRoomLabel.GetComponentInChildren<Text>().text = "Rum: " + CurrentRoomNumber;
    }

    private void ChangeLobbyUIVisibility(bool visible)
    {
        LobbyScreen.gameObject.SetActive(visible);
        CurrentRoomLabel.gameObject.SetActive(!visible);
        foreach (GameObject button in JoinRoomButtonsPool)
        {
            bool active = visible && button.GetComponentInChildren<Text>().text != string.Empty;
            button.SetActive(active);
        }
    }

    private string GeetRoomNumberFromNetworkId(UnityEngine.Networking.Types.NetworkID networkID)
    {
        return (System.Convert.ToInt64(networkID.ToString()) % 10000).ToString();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

