using System;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.CrossPlatform;
using UnityEngine;
using UnityEngine.Networking;

public class AnchorController : NetworkBehaviour
{
    /// <summary>
    /// Cloud Anchor ID, der vil blive brugt til at være vært for at host Cloud Anchor. Denne variabel vil være
    /// syncrhonized over alle klienter.
    /// </summary>
    [SyncVar(hook = "OnChangeId")]
    private string CloudAnchorId = string.Empty;

    private bool IsHost = false;

    private bool ShouldResolve = false;

    private CloudAnchorsController CloudAnchorsController;

    void Start()
    {
        CloudAnchorsController = GameObject.Find("CloudAnchorsController").GetComponent<CloudAnchorsController>();
    }

    public override void OnStartClient()
    {
        if (CloudAnchorId != string.Empty)
        {
            ShouldResolve = true;
        }
    }


    void Update()
    {
        if (ShouldResolve)
        {
            ResolveAnchorFromId(CloudAnchorId);
        }
    }

    /// <summary>
    /// Kommandoen kører på serveren for at indstille Cloud Anchor ID.
    /// </summary>
    [Command]
    private void CmdSetCloudAnchorId(string cloudAnchorId)
    {
        CloudAnchorId = cloudAnchorId;
    }
    
    public string GetCloudAnchorId()
    {
        return CloudAnchorId;
    }

    /// <summary>
    /// Hosts brugeren anbragte cloud anker og associerer det resulterende id med dette objekt.
    /// </summary>
    public void HostLastPlacedAnchor(Component lastPlacedAnchor)
    {
        IsHost = true;
    }

    private void ResolveAnchorFromId(string cloudAnchorId)
    {
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
            ;
        }

        ShouldResolve = false;

        XPSession.ResolveCloudAnchor(cloudAnchorId).ThenAction((System.Action<CloudAnchorResult>)(result =>
        {
            if (result.Response != CloudServiceResponse.Success)
            {
        
                CloudAnchorsController.OnAnchorResolved(false, result.Response.ToString());
                ShouldResolve = true;
                return;
            }
           CloudAnchorsController.OnAnchorResolved(true, result.Response.ToString());
            OnResolved(result.Anchor.transform);
        }));
    }

    private void OnResolved(Transform anchorTransform)
    {
        var cloudAnchorsController = GameObject.Find("CloudAnchorsController").GetComponent<CloudAnchorsController>();
        cloudAnchorsController.SetWorldOrigin(anchorTransform);
    }

    private void OnChangeId(string newId)
    {
        if (!IsHost && newId != string.Empty)
        {
            CloudAnchorId = newId;
            ShouldResolve = true;
        }
    }
}
