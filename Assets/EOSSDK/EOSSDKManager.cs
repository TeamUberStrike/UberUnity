using Epic.OnlineServices;
using Epic.OnlineServices.Platform;
using System;
using UnityEngine;

class EOSSDKManager : MonoBehaviour
{
    static private PlatformInterface s_eosPlatformInterface;

    public PlatformInterface GetEOSPlatformInterface()
    {
        #if UNITY_EDITOR
            return s_eosPlatformInterface;
        #else
            if (s_eosPlatformInterface == null) 
            {
                if (EOS_GetPlatformInterface() == IntPtr.Zero)
                {
                    throw new Exception("The native code returned a NULL EOS Platform. The issue is likely in the GFX plugin.");
                }
                SetEOSPlatformInterface(new Epic.OnlineServices.Platform.PlatformInterface(EOS_GetPlatformInterface()));
            }
        #endif
    }
    private Epic.OnlineServices.Result InitializePlatformInterface()
    {
        InitializeOptions initOptions = new InitializeOptions
        {
            ProductName = "UberUnityClient",
            ProductVersion = "1.0",

            AllocateMemoryFunction = IntPtr.Zero,
            ReallocateMemoryFunction = IntPtr.Zero,
            ReleaseMemoryFunction = IntPtr.Zero
        };

        var overrideThreadAffinity = new InitializeThreadAffinity
        {
            NetworkWork = 0,
            StorageIo = 0,
            WebSocketIo = 0,
            P2PIo = 0,
            HttpRequestIo = 0,
            RTCIo = 0
        };

        initOptions.OverrideThreadAffinity = overrideThreadAffinity;

        return PlatformInterface.Initialize(ref initOptions);
    }

    public void Awake()
    {
        InitializePlatformInterface();
        CreatePlatformInterface();
    }

    public void Update()
    {
        if (GetEOSPlatformInterface() != null) 
        {
            s_eosPlatformInterface.Tick();
        }
    }

    private void OnApplicationQuit()
    {
        s_eosPlatformInterface?.Release();
        
        return;
    }
    private PlatformInterface CreatePlatformInterface()
    {
        var platformOptions = new Epic.OnlineServices.Platform.WindowsOptions();
        platformOptions.CacheDirectory = Application.temporaryCachePath;
        platformOptions.IsServer = false;

        platformOptions.EncryptionKey = null;
        platformOptions.OverrideCountryCode = null;
        platformOptions.OverrideLocaleCode = null;
        platformOptions.ProductId = "9dd583a08ed840369e551e42fba24e0c";
        platformOptions.SandboxId = "f5bb034d4b08458bb0d790b0164d05be";
        platformOptions.DeploymentId = "1404b10b7b004f0a927ad623464017e7";

        platformOptions.TickBudgetInMilliseconds = 0;

        var clientCredentials = new Epic.OnlineServices.Platform.ClientCredentials
        {
            ClientId = "xyza7891eoyT5X50GzpCEPe33aoFwjiZ",
            ClientSecret = "FJ778IE2OSW7/xTfoUEKSUtqIvynytcBELk9IMl+uig"
        };
        platformOptions.ClientCredentials = clientCredentials;

        platformOptions.Flags =
        #if UNITY_EDITOR
            PlatformFlags.LoadingInEditor;
        #else
            PlatformFlags.None;
        #endif
        return Epic.OnlineServices.Platform.PlatformInterface.Create(ref platformOptions);
    }
}

