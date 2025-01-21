using UnityEngine;
using Unity.Netcode;
using System.IO;
#if UNITY_SERVER
using Unity.Services.Multiplay;
#endif
using UnityEngine.Rendering;
using Unity.Netcode.Transports.UTP;

// Place this script on an empty GameObject in your main server scene
public class ServerManager : MonoBehaviour
{

    public static ServerManager Instance { get; private set; }
    public bool IsServer => Application.isBatchMode || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    private ServerConfig config;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

#if UNITY_SERVER
    private void Start()
    {
        if (IsServer)
        {
            InitializeServer();
        }
    }

    private void InitializeServer()
    {
        LoadServerConfig();
        SetupServerNetwork();
        DisableUnneededComponents();
        ServerLogger.Log("Server initialized");
    }

    private void LoadServerConfig()
    {
        string configPath = Path.Combine(Application.dataPath, "config.json");
        if (File.Exists(configPath))
        {
            string jsonContent = File.ReadAllText(configPath);
            config = JsonUtility.FromJson<ServerConfig>(jsonContent);
            ServerLogger.Log("Server config loaded");
        }
        else
        {
            config = new ServerConfig();
            ServerLogger.Log("Using default server config");
        }
    }

    private void SetupServerNetwork()
    {
        // Parse command line arguments for port override
        string[] args = System.Environment.GetCommandLineArgs();
        int port = config.port;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-port" && args.Length > i + 1)
            {
                int.TryParse(args[i + 1], out port);
            }
        }

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData("0.0.0.0", (ushort)port);

        // Set up network callbacks
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        // Start the server
        NetworkManager.Singleton.StartServer();

        // Apply server-specific settings
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = config.tickRate;
        Physics.autoSimulation = true;
        Time.fixedDeltaTime = 1f / config.tickRate;
        MultiplayService.Instance.ReadyServerForPlayersAsync();
        Loader.LoadNetworkServerRpc("Menu");
    }

    private void OnServerStarted()
    {
        ServerLogger.Log($"Server started on port {config.port}");

    }

    private void OnClientConnected(ulong clientId)
    {
        ServerLogger.Log($"Client {clientId} connected");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        ServerLogger.Log($"Client {clientId} disconnected");
    }

    public void DisableUnneededComponents()
    {
        if (!IsServer) return;

        // Disable cameras
        foreach (Camera cam in FindObjectsOfType<Camera>())
        {
            cam.enabled = false;
        }

        // Disable audio
        foreach (AudioListener listener in FindObjectsOfType<AudioListener>())
        {
            listener.enabled = false;
        }

        // Disable UI
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            canvas.enabled = false;
        }

        ServerLogger.Log("Disabled unnecessary components");
    }
#endif
}

public class ServerConfig
{
    public int port = 7777;
    public int maxPlayers = 10;
    public int tickRate = 30;
    public string logLevel = "Info";
    public bool enableAntiCheat = true;
    public float serverTimeout = 60f;
    public float clientTimeout = 30f;
}