using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Server : MonoBehaviour
{
    public TextMeshProUGUI player1Text;
    public TextMeshProUGUI player2Text;
    public TextMeshProUGUI playerText;
    public GameObject multiplayerPanel;
    public GameObject singleplayerPanel;
    public GameObject errorPanelGetter;
    public static GameObject errorPanel;
    public TextMeshProUGUI errorPanelTextGetter;
    public static TextMeshProUGUI errorPanelText;
    public static bool reconnectingFailed;

    public static TcpClient player1;
    public static TcpClient player2;
    public static NetworkStream player1Stream;
    public static NetworkStream player2Stream;

    private const int TcpPort = 8080;
    private const int DiscoveryPort = 9000;
    private const string DiscoveryRequest = "ENGINEERS_ARENA_DISCOVER_V1";
    private const string DiscoveryResponse = "ENGINEERS_ARENA_SERVER_V1";

    private TcpListener tcpListener;
    private UdpClient discoverySocket;
    private bool closingClients;

    private void Start()
    {
        errorPanel = errorPanelGetter;
        errorPanelText = errorPanelTextGetter;
        reconnectingFailed = false;
        closingClients = false;

        if (Menu.isMultiplayer)
        {
            multiplayerPanel.SetActive(true);
            singleplayerPanel.SetActive(false);
        }

        ClosePlayerSockets();

        try
        {
            tcpListener = new TcpListener(IPAddress.Any, TcpPort);
            tcpListener.Start();
            Debug.Log("[LAN] TCP server listening on port " + TcpPort);
        }
        catch (Exception e)
        {
            Debug.LogError("[LAN] Cannot start TCP port 8080: " + e);
        }

        try
        {
            discoverySocket = new UdpClient(new IPEndPoint(IPAddress.Any, DiscoveryPort));
            discoverySocket.EnableBroadcast = true;
            Debug.Log("[LAN] UDP discovery listening on port " + DiscoveryPort);
        }
        catch (Exception e)
        {
            Debug.LogError("[LAN] Cannot start UDP discovery port 9000: " + e);
        }
    }

    private void Update()
    {
        if (reconnectingFailed)
        {
            reconnectingFailed = false;
            if (!closingClients) StartCoroutine(CloseClients());
        }

        // The PC replies to the Android's UDP broadcast. The phone uses
        // the source IP of this reply, so multi-adapter PCs work correctly.
        if (discoverySocket != null)
        {
            try
            {
                int packets = 0;
                while (discoverySocket.Available > 0 && packets++ < 20)
                {
                    IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                    byte[] request = discoverySocket.Receive(ref remote);
                    string message = Encoding.UTF8.GetString(request).Trim();
                    if (message != DiscoveryRequest) continue;
                    byte[] response = Encoding.UTF8.GetBytes(DiscoveryResponse);
                    discoverySocket.Send(response, response.Length, remote);
                    Debug.Log("[LAN] Discovery reply sent to " + remote.Address);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[LAN] UDP discovery error: " + e.Message);
            }
        }

        if (tcpListener == null) return;
        try
        {
            while (tcpListener.Pending())
            {
                TcpClient incoming = tcpListener.AcceptTcpClient();
                incoming.NoDelay = true;
                string remoteAddress = incoming.Client.RemoteEndPoint.ToString();

                if (player1 == null)
                {
                    player1 = incoming;
                    player1Stream = incoming.GetStream();
                    if (Menu.isMultiplayer) player1Text.text = "Player 1 connected";
                    else playerText.text = "Player connected";
                    Debug.Log("[LAN] Player 1 connected: " + remoteAddress);
                }
                else if (Menu.isMultiplayer && player2 == null)
                {
                    player2 = incoming;
                    player2Stream = incoming.GetStream();
                    player2Text.text = "Player 2 connected";
                    Debug.Log("[LAN] Player 2 connected: " + remoteAddress);
                }
                else
                {
                    Debug.LogWarning("[LAN] Rejected extra client: " + remoteAddress);
                    incoming.Close();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[LAN] TCP accept error: " + e.Message);
        }
    }

    public static IEnumerator TryReconnect(int clientNumber)
    {
        yield return CloseClients();
    }

    public static IEnumerator CloseClients()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2);
        ClosePlayerSockets();
        Menu.errorMessage = "Connection lost. Return to the lobby and reconnect.";
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    private static void ClosePlayerSockets()
    {
        try { if (player1Stream != null) player1Stream.Close(); } catch { }
        try { if (player2Stream != null) player2Stream.Close(); } catch { }
        try { if (player1 != null) player1.Close(); } catch { }
        try { if (player2 != null) player2.Close(); } catch { }
        player1Stream = null;
        player2Stream = null;
        player1 = null;
        player2 = null;
    }

    private void OnDestroy()
    {
        PreventSleep.RestoreSleepSettings();
        try { if (tcpListener != null) tcpListener.Stop(); } catch { }
        try { if (discoverySocket != null) discoverySocket.Close(); } catch { }
        ClosePlayerSockets();
    }

    private void OnApplicationQuit()
    {
        PreventSleep.RestoreSleepSettings();
        try { if (tcpListener != null) tcpListener.Stop(); } catch { }
        try { if (discoverySocket != null) discoverySocket.Close(); } catch { }
        ClosePlayerSockets();
    }
}
