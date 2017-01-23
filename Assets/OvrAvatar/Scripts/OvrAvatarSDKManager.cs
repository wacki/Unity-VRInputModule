using UnityEngine;
using System.Collections;
using Oculus.Avatar;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public delegate void specificationCallback(IntPtr specification);
public delegate void assetLoadedCallback(OvrAvatarAsset asset);

public class OvrAvatarSDKManager : MonoBehaviour {
    private static OvrAvatarSDKManager _instance;
    private Dictionary<UInt64, HashSet<specificationCallback>> specificationCallbacks;
    private Dictionary<UInt64, HashSet<assetLoadedCallback>> assetLoadedCallbacks;
    private Dictionary<UInt64, OvrAvatarAsset> assetCache;

    public static OvrAvatarSDKManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<OvrAvatarSDKManager>();
                if (_instance == null)
                {
                    GameObject manager = new GameObject("OvrAvatarSDKManager");
                    _instance = manager.AddComponent<OvrAvatarSDKManager>();
                    _instance.Initialize();
                }
            }
            return _instance;
        }
    }

    private void Initialize()
    {
        string appId = OvrAvatarSettings.AppID;
        if (appId == "")
        {
            Debug.LogError("No Oculus Rift App ID has been provided. Go to OvrAvatar > Edit Configuration to supply one", OvrAvatarSettings.Instance);
            appId = "0";
        }
        CAPI.ovrAvatar_Initialize(appId);
        specificationCallbacks = new Dictionary<UInt64, HashSet<specificationCallback>>();
        assetLoadedCallbacks = new Dictionary<UInt64, HashSet<assetLoadedCallback>>();
        assetCache = new Dictionary<ulong, OvrAvatarAsset>();
    }

    void OnDestroy()
    {
        CAPI.ovrAvatar_Shutdown();
    }

	// Update is called once per frame
	void Update () {
        IntPtr message = CAPI.ovrAvatarMessage_Pop();
        if (message == IntPtr.Zero)
        {
            return;
        }

        ovrAvatarMessageType messageType = CAPI.ovrAvatarMessage_GetType(message);
        switch (messageType)
        {
            case ovrAvatarMessageType.AssetLoaded:
                {
                    ovrAvatarMessage_AssetLoaded assetMessage = CAPI.ovrAvatarMessage_GetAssetLoaded(message);
                    IntPtr asset = assetMessage.asset;
                    UInt64 assetID = assetMessage.assetID;
                    ovrAvatarAssetType assetType = CAPI.ovrAvatarAsset_GetType(asset);
                    OvrAvatarAsset assetData;
                    switch (assetType)
                    {
                        case ovrAvatarAssetType.Mesh:
                            assetData = new OvrAvatarAssetMesh(assetID, asset);
                            break;
                        case ovrAvatarAssetType.Texture:
                            assetData = new OvrAvatarAssetTexture(assetID, asset);
                            break;
                        default:
                            throw new NotImplementedException(
                                string.Format("Unsupported asset type format {0}",
                                              assetType.ToString()));
                    }
                    assetCache.Add(assetID, assetData);
                    HashSet<assetLoadedCallback> callbackSet;
                    if (!assetLoadedCallbacks.TryGetValue(assetMessage.assetID, out callbackSet))
                    {
                        throw new Exception("Error, got an avatar specification callback from a user id we don't have a record for.");
                    }
                    foreach (var callback in callbackSet)
                    {
                        callback(assetData);
                    }
                    specificationCallbacks.Remove(assetMessage.assetID);
                    break;
                }
            case ovrAvatarMessageType.AvatarSpecification:
                {
                    ovrAvatarMessage_AvatarSpecification spec = CAPI.ovrAvatarMessage_GetAvatarSpecification(message);
                    HashSet<specificationCallback> callbackSet;
                    if (!specificationCallbacks.TryGetValue(spec.oculusUserID, out callbackSet))
                    {
                        throw new Exception("Error, got an avatar specification callback from a user id we don't have a record for.");
                    }
                    foreach (var callback in callbackSet)
                    {
                        callback(spec.avatarSpec);
                    }
                    specificationCallbacks.Remove(spec.oculusUserID);
                    break;
                }
            default:
                throw new NotImplementedException(
                    "Unhandled ovrAvatarMessageType: " + messageType);
        }
        CAPI.ovrAvatarMessage_Free(message);
	}

    public void RequestAvatarSpecification(UInt64 userId, specificationCallback callback)
    {
        HashSet<specificationCallback> callbackSet;
        if (!specificationCallbacks.TryGetValue(userId, out callbackSet))
        {
            callbackSet = new HashSet<specificationCallback>();
            specificationCallbacks.Add(userId, callbackSet);
            //Only request the spec if we don't already have one in flight.
            CAPI.ovrAvatar_RequestAvatarSpecification(userId);
        }
        //callbackSet is now in the callbacks dictionary ready to be added to
        callbackSet.Add(callback);
    }

    public void BeginLoadingAsset(UInt64 assetId, assetLoadedCallback callback)
    {
        HashSet<assetLoadedCallback> callbackSet;
        if (!assetLoadedCallbacks.TryGetValue(assetId, out callbackSet))
        {
            callbackSet = new HashSet<assetLoadedCallback>();
            assetLoadedCallbacks.Add(assetId, callbackSet);
            //Only request the asset if we don't already have one in flight.
            CAPI.ovrAvatarAsset_BeginLoading(assetId);
        }
        if (callbackSet.Add(callback))
        {
            callbackSet.Add(callback);
        }
    }

    public OvrAvatarAsset GetAsset(UInt64 assetId) {
        OvrAvatarAsset asset;
        if (assetCache.TryGetValue(assetId, out asset))
        {
            return asset;
        }
        else
        {
            return null;
        }
    }
}
