using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Find the PC automatically; optional manual IPv4 entry is a fallback.
public class Client : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    [Header("Optional manual connection UI")]
    public TMP_InputField manualIpInput;

    public static TcpClient client;
    public static NetworkStream stream;
    public static bool connected;
    public static bool notReconnected;

    private const int TcpPort = 8080;
    private const int DiscoveryPort = 9000;
    private const string DiscoveryRequest = "ENGINEERS_ARENA_DISCOVER_V1";
    private const string DiscoveryResponse = "ENGINEERS_ARENA_SERVER_V1";
    private const float DiscoveryIntervalSeconds = 2f;
    private const float TcpTimeoutSeconds = 5f;

    private UdpClient discoverySocket;
    private string serverIp = "";
    private bool isConnecting;
    private bool manualConnection;
    private float nextDiscoveryTime;
    private float nextConnectionTime;
    private bool requestingLocalPermission;
    private bool disconnecting;

    private void Start()
    {
        CloseConnections();
        connected = false;
        notReconnected = false;
        isConnecting = false;
        manualConnection = false;
        disconnecting = false;
        serverIp = "";
        nextConnectionTime = 0;
        Debug.Log("[LAN] Starting automatic server discovery");
        ShowStatus("Searching for PC on the same Wi-Fi...");
        StartDiscovery();
    }

    private void Update()
    {
        if (notReconnected && !disconnecting)
        {
            notReconnected = false;
            StartCoroutine(Disconnect());
            disconnecting = true;
            return;
        }

        if (connected) return;

        if (!LocalNetworkPermissionReady()) return;

        if (discoverySocket == null) StartDiscovery();

        try
        {
            // Receives unicast answers on the same ephemeral port used by our
            // broadcast request. No MulticastLock is needed for this reply.
            int received = 0;
            while (discoverySocket != null && discoverySocket.Available > 0 && received++ < 20)
            {
                IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = discoverySocket.Receive(ref remote);
                string message = Encoding.UTF8.GetString(data).Trim();
                if (message == DiscoveryResponse && !manualConnection && !isConnecting)
                {
                    serverIp = remote.Address.ToString();
                    Debug.Log("[LAN] Found PC at " + serverIp);
                    ShowStatus("PC found. Connecting...");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[LAN] Discovery receive failed: " + e.Message);
            CloseDiscovery();
        }

        if (!manualConnection && Time.unscaledTime >= nextDiscoveryTime && !isConnecting)
        {
            SendDiscovery();
            nextDiscoveryTime = Time.unscaledTime + DiscoveryIntervalSeconds;
        }

        if (!isConnecting && !string.IsNullOrEmpty(serverIp) && Time.unscaledTime >= nextConnectionTime)
            StartCoroutine(ConnectToServer(serverIp));
    }

    private void StartDiscovery()
    {
        if (discoverySocket != null) return;
        try
        {
            discoverySocket = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            discoverySocket.EnableBroadcast = true;
            nextDiscoveryTime = 0;
            Debug.Log("[LAN] UDP socket opened on local port " +
                      ((IPEndPoint)discoverySocket.Client.LocalEndPoint).Port);
        }
        catch (Exception e)
        {
            Debug.LogError("[LAN] Cannot create UDP discovery socket: " + e.Message);
            ShowStatus("Cannot start discovery. Check app network permission.");
        }
    }

    private void SendDiscovery()
    {
        if (discoverySocket == null) return;
        try
        {
            byte[] request = Encoding.UTF8.GetBytes(DiscoveryRequest);
            discoverySocket.Send(request, request.Length,
                new IPEndPoint(IPAddress.Broadcast, DiscoveryPort));
            Debug.Log("[LAN] Searching for PC...");
        }
        catch (Exception e)
        {
            Debug.LogWarning("[LAN] Discovery broadcast failed: " + e.Message);
            ShowStatus("Discovery failed. Try manual PC IP.");
        }
    }

    // Optional
    public void ConnectUsingManualIp()
    {
        if (manualIpInput != null) ConnectToManualIp(manualIpInput.text);
        else ShowStatus("Manual IP field is not assigned in the Inspector.");
    }

    public void ConnectToManualIp(string address)
    {
        IPAddress parsed;
        if (string.IsNullOrWhiteSpace(address) ||
            !IPAddress.TryParse(address.Trim(), out parsed) ||
            parsed.AddressFamily != AddressFamily.InterNetwork)
        {
            ShowStatus("Enter a valid PC IPv4 address, e.g. 192.168.1.20");
            return;
        }
        manualConnection = true;
        serverIp = parsed.ToString();
        nextConnectionTime = 0;
        Debug.Log("[LAN] Manual IP: " + serverIp);
        ShowStatus("Connecting to " + serverIp + "...");
    }

    public void ReturnToAutomaticDiscovery()
    {
        manualConnection = false;
        serverIp = "";
        nextDiscoveryTime = 0;
        ShowStatus("Searching for PC on the same Wi-Fi...");
    }

    private IEnumerator ConnectToServer(string address)
    {
        isConnecting = true;
        TcpClient candidate = null;
        IAsyncResult pending = null;
        bool started = false;

        try
        {
            candidate = new TcpClient();
            candidate.NoDelay = true;
            pending = candidate.BeginConnect(address, TcpPort, null, null);
            started = true;
        }
        catch (Exception e)
        {
            Debug.LogWarning("[LAN] TCP connect start failed: " + e.Message);
        }

        if (started)
        {
            float deadline = Time.realtimeSinceStartup + TcpTimeoutSeconds;
            while (!pending.IsCompleted && Time.realtimeSinceStartup < deadline)
                yield return null;

            if (!pending.IsCompleted)
            {
                Debug.LogWarning("[LAN] TCP timeout to " + address + ":8080");
            }
            else
            {
                try
                {
                    candidate.EndConnect(pending);
                    client = candidate;
                    stream = client.GetStream();
                    connected = true;
                    candidate = null; // ownership moved to static client
                    Debug.Log("[LAN] TCP connected to " + address + ":8080");
                    ShowStatus("Connected to PC. Waiting for game...");
                    CloseDiscovery();
                }
                catch (Exception e)
                {
                    Debug.LogWarning("[LAN] TCP connection failed: " + e.Message);
                }
            }
        }

        if (candidate != null) { try { candidate.Close(); } catch { } }
        if (!connected)
        {
            if (!manualConnection) serverIp = "";
            nextConnectionTime = Time.unscaledTime + 3f;
            ShowStatus("PC not reachable. Check Windows Firewall / Wi-Fi.");
        }
        isConnecting = false;
    }

    private bool LocalNetworkPermissionReady()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Android 17 / target SDK 37+ requires an explicit runtime permission.
        // This permission MUST also be in the merged AndroidManifest.xml.
        try
        {
            using (AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION"))
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject info = activity.Call<AndroidJavaObject>("getApplicationInfo"))
            {
                int deviceSdk = version.GetStatic<int>("SDK_INT");
                int targetSdk = info.Get<int>("targetSdkVersion");
                if (deviceSdk >= 37 && targetSdk >= 37)
                {
                    const string permission = "android.permission.ACCESS_LOCAL_NETWORK";
                    if (UnityEngine.Android.Permission.HasUserAuthorizedPermission(permission))
                        return true;
                    if (!requestingLocalPermission)
                    {
                        requestingLocalPermission = true;
                        ShowStatus("Allow nearby devices/local network access.");
                        UnityEngine.Android.Permission.RequestUserPermission(permission);
                    }
                    return false;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[LAN] Permission status check failed: " + e.Message);
        }
#endif
        return true;
    }

    private void ShowStatus(string message)
    {
        if (textMeshPro != null) textMeshPro.text = message;
        Debug.Log("[LAN] " + message);
    }

    private void CloseDiscovery()
    {
        if (discoverySocket != null)
        {
            try { discoverySocket.Close(); } catch { }
            discoverySocket = null;
        }
    }

    private void CloseConnections()
    {
        try { if (stream != null) stream.Close(); } catch { }
        try { if (client != null) client.Close(); } catch { }
        stream = null;
        client = null;
        CloseDiscovery();
    }

    public static IEnumerator Disconnect()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2);
        try { if (stream != null) stream.Close(); } catch { }
        try { if (client != null) client.Close(); } catch { }
        stream = null;
        client = null;
        connected = false;
        Menu.errorMessage = "Connection lost. Reconnect from the lobby.";
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public static IEnumerator TryReconnect()
    {
        yield return Disconnect();
    }

    private void OnDestroy()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        CloseConnections();
        connected = false;
    }

    private void OnApplicationQuit()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        CloseConnections();
        connected = false;
    }
}
