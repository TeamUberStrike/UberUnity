using Epic.OnlineServices;
using Epic.OnlineServices.Platform;
using System;
using UnityEngine;

public class EOSSDKManager : MonoBehaviour
{
    private static PlatformInterface s_eosPlatformInterface;

    private static EOSSDKManager instance;
    public PlatformInterface GetEOSPlatformInterface()
    {
        return s_eosPlatformInterface;
    }
    private Epic.OnlineServices.Result InitializePlatformInterface()
    {
        InitializeOptions initOptions = new InitializeOptions
        {
            ProductName = "UberUnityClient",
            ProductVersion = "1.0",

            //AllocateMemoryFunction = IntPtr.Zero,
            //ReallocateMemoryFunction = IntPtr.Zero,
            //ReleaseMemoryFunction = IntPtr.Zero
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
        Debug.Log($"initOptions: {initOptions}");
        return PlatformInterface.Initialize(ref initOptions);
    }

    public void Awake()
    {
        
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        var result = InitializePlatformInterface();
        Debug.Log($"EOS Initialize: {result}");
        if (result != Result.Success)
        {
            Debug.LogError($"EOS failed to initialize: {result}");
            return;
        }
    
        s_eosPlatformInterface = CreatePlatformInterface();

        if (s_eosPlatformInterface == null)
        {
            Debug.LogError("EOS Platform creation failed.");
            return;
        }

        Debug.Log("EOS Platform created.");        
    }

    public void Update()
    {
        s_eosPlatformInterface?.Tick();    
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
            ClientId = "",
            ClientSecret = ""
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

