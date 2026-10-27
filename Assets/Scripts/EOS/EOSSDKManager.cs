using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Epic.OnlineServices;
using Epic.OnlineServices.Platform;


public class EOSSDKManager : MonoBehaviour
{
    [DllImport("Kernel32.dll")] 
    static extern IntPtr LoadLibrary(string fileName);

    [DllImport("Kernel32.dll")] 
    static extern IntPtr GetProcAddress(IntPtr module, string procName);

    [DllImport("Kernel32.dll")] 
    static extern int FreeLibrary(IntPtr module);

    private static IntPtr eosLib = IntPtr.Zero;

    private static void LoadEOSBindings()
    {
        string dllPath = Path.Combine(
            Application.dataPath,
            "Plugins",
            "x86_64",
            "EOSSDK-Win64-Shipping.dll"
        );
        eosLib = LoadLibrary(dllPath);
        if (eosLib == IntPtr.Zero) 
        {
            throw new Exception("Failed to load the EOS SDK DLL");
        } 
        Bindings.Hook(eosLib, GetProcAddress);
        WindowsBindings.Hook(eosLib, GetProcAddress);
    }
    
    private static PlatformInterface s_eosPlatformInterface;

    private static EOSSDKManager instance;
    public static PlatformInterface GetEOSPlatformInterface()
    {
        return s_eosPlatformInterface;
    }
    private Result InitializePlatformInterface()
    {
        InitializeOptions initOptions = new()
        {
            ProductName = "UberUnityClient",
            ProductVersion = "1.0",
            OverrideThreadAffinity = new InitializeThreadAffinity
            {
                NetworkWork = 0,
                StorageIo = 0,
                WebSocketIo = 0,
                P2PIo = 0,
                HttpRequestIo = 0,
                RTCIo = 0
            }
        };

        var result = PlatformInterface.Initialize(ref initOptions);

        return result;
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

        LoadEOSBindings();
        var result = InitializePlatformInterface();

        if (result != Result.Success)
        {
            Debug.LogError($"EOS failed to initialize: {result}");
            return;
        }
    
        CreatePlatformInterface();

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
        s_eosPlatformInterface = null;
        return;
    }
    public void CreatePlatformInterface()
    {
        var options = new WindowsOptions()
        {
            ProductId = "9dd583a08ed840369e551e42fba24e0c",
            SandboxId = "f5bb034d4b08458bb0d790b0164d05be",
            ClientCredentials = new ClientCredentials()
            {
                ClientId = "xyza78910Rgra1tsBytv30D9yKUSyxfH",
                ClientSecret = "9CyhuxRdp+4lRgDkEE9U6r9YwppkFhS0YXLEkJqvxv0",
            },
            DeploymentId = "1404b10b7b004f0a927ad623464017e7",
            EncryptionKey = null,
            Flags = PlatformFlags.DisableOverlay,
            IsServer = false,
        };
        s_eosPlatformInterface = PlatformInterface.Create(ref options);

        if (s_eosPlatformInterface != null)
        {
            Debug.Log("PlatformInterface::CreatePlatformInstace has created successfully.");
        }
        else
        {
            Debug.LogError("PlatformInterface::CreatePlatformInstace creation has failed.");
            return;
        }
    }
}

