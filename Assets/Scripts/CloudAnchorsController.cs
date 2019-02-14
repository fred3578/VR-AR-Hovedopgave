using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.CloudAnchors;
using UnityEngine;
using UnityEngine.Networking;
using LocalPlayerController = Assets.Scripts.LocalPlayerController;

public class CloudAnchorsController : MonoBehaviour
{
    [Header("ARCore")] public NetworkManagerUIController UIController;

    /// <summary>
    /// ARCore-specifikke GameObjects i scenen.
    /// </summary>
    public GameObject ARCoreRoot;

    private bool IsOriginPlaced = false;

    private bool AnchorAlreadyInstantiated = false;

    /// <summary>
    /// indicates cloud anchor er færdig med at host
    /// </summary>
    private bool AnchorFinishedHosting = false;

    /// <summary>
    /// til hvis der er forbindelse fejl (sand eller falsk)
    /// </summary>
    private bool IsQuitting = false;

    ///<summary>
    /// Den sidste placeret anchor
    /// </summary>
    private Component LastPlacedAnchor = null;

    /// <summary>
    /// det rigtig cloud anchor mode
    /// </summary>
    private ApplicationMode CurrentMode = ApplicationMode.Ready;

    public enum ApplicationMode
    {
        Ready,
        Hosting,
        Resolving,
    }


    void Start()
    {
        gameObject.name = "CloudAnchorsController";
        ARCoreRoot.SetActive(false);
        ResetStatus();
    }


    void Update()
    {
        UpdateApplicationLifecycle();

        if (CurrentMode != ApplicationMode.Hosting && CurrentMode != ApplicationMode.Resolving)
        {
            return;
        }

        if (CurrentMode == ApplicationMode.Resolving && !IsOriginPlaced)
        {
            return;
        }

        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        if (LastPlacedAnchor != null)
        {
            if (CanPlaceBrick())
            {
                InstantiateBrick();
            }
            else if (!IsOriginPlaced && CurrentMode == ApplicationMode.Hosting)
            {
                SetWorldOrigin(LastPlacedAnchor.transform);
                InstantiateAnchor();
                OnAnchorInstantiated(true);
            }
        }

    }

    public void SetWorldOrigin(Transform anchorTransform)
    {
        if (IsOriginPlaced)
        {
            return;
        }

        IsOriginPlaced = true;
    }

    public void OnEnterHostingModeClick()
    {
        if (CurrentMode == ApplicationMode.Hosting)
        {
            CurrentMode = ApplicationMode.Ready;
            ResetStatus();
            return;
        }

        CurrentMode = ApplicationMode.Hosting;
        SetPlatformActive();
    }

    public void OnEnterResolvingModeClick()
    {
        if (CurrentMode == ApplicationMode.Resolving)
        {
            CurrentMode = ApplicationMode.Ready;
            ResetStatus();
            return;
        }
        CurrentMode = ApplicationMode.Resolving;
        SetPlatformActive();
    }

    public void OnAnchorInstantiated(bool isHost)
    {
        if (AnchorAlreadyInstantiated)
        {
            return;
        }

        AnchorAlreadyInstantiated = true;
        UIController.OnAnchorInstantiated(isHost);
    }

    public void OnAnchorHosted(bool success, string response)
    {
        AnchorFinishedHosting = success;
        UIController.OnAnchorHosted(success, response);
    }

    public void OnAnchorResolved(bool success, string response)
    {
        UIController.OnAnchorResolved(success, response);
    }

    private void InstantiateAnchor()
    {
        GameObject.Find("LocalPlayer").GetComponent<LocalPlayerController>()
            .SpawnAnchor(Vector3.zero, Quaternion.identity, LastPlacedAnchor);
    }

    private void InstantiateBrick()
    {
        GameObject.Find("LocalPlayer").GetComponent<LocalPlayerController>()
            .CmdSpawnBrick(LastPlacedAnchor.transform.position, LastPlacedAnchor.transform.rotation);
    }

    private void SetPlatformActive()
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            ARCoreRoot.SetActive(true);
        }
        else
        {
            ARCoreRoot.SetActive(false);
        }
    }

    private bool CanPlaceBrick()
    {
        if (CurrentMode == ApplicationMode.Resolving)
        {
            return IsOriginPlaced;
        }

        if (CurrentMode == ApplicationMode.Hosting)
        {
            return IsOriginPlaced && AnchorFinishedHosting;
        }

        return false;
    }

    private void ResetStatus()
    {
        CurrentMode = ApplicationMode.Ready;
        if (LastPlacedAnchor != null)
        {
            Destroy(LastPlacedAnchor.gameObject);
        }

        LastPlacedAnchor = null;
    }

    public void UpdateApplicationLifecycle()
    {
        // luk app ved klik på tilbage knappen på mobilen
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        //får screen til ikke at skrue ned for lyset på mobilen
        var sleepTimeout = SleepTimeout.NeverSleep;

        Screen.sleepTimeout = sleepTimeout;

        if (IsQuitting)
        {
            return;
        }

        //besked for at give tilladelse til brug at kameraet 
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            ShowAndroidToastMessage("Kamera tilladelse skal bruges for at køre denne app ");
            IsQuitting = true;
            Invoke("DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            ShowAndroidToastMessage("ARCore havde et problem med connecting. Prøv at starte appen igen.");
            IsQuitting = true;
            Invoke("DoQuit", 0.5f);
        }
    }

    ///<summary>
    /// Funktion til at lukke appen
    /// </summary>
    private void DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// vise en android toast besked
    /// </summary>
    /// <param name="message"></param>
    public void ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }

}
