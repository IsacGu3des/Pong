using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Globalization;

public class UdpClientTwoClients : MonoBehaviour
{
    public int myId = -1; // Agora público para a Bola acessar
    UdpClient client;
    Thread receiveThread;
    IPEndPoint serverEP;

    Vector3 remotePos = Vector3.zero;
    public GameObject localCube;
    public GameObject remoteCube;

    public GameObject bola; // referência à bola no Inspector

    void Start()
    {
        client = new UdpClient();
        serverEP = new IPEndPoint(IPAddress.Parse("10.57.1.11"), 5001);
        client.Connect(serverEP);

        receiveThread = new Thread(ReceiveData);
        receiveThread.Start();

        client.Send(Encoding.UTF8.GetBytes("HELLO"), 5);

        // ⚡ Garante que cada jogador comece no lado certo
        if (myId == 1)
        {
            localCube = GameObject.Find("Player 1");
            remoteCube = GameObject.Find("Player 2");

            localCube.transform.position = new Vector3(-7f, 0f, 0f); // Esquerda
            remoteCube.transform.position = new Vector3(7f, 0f, 0f);  // Direita
        }
        else if (myId == 2)
        {
            localCube = GameObject.Find("Player 2");
            remoteCube = GameObject.Find("Player 1");

            localCube.transform.position = new Vector3(7f, 0f, 0f);   // Direita
            remoteCube.transform.position = new Vector3(-7f, 0f, 0f); // Esquerda
        }

        // ⚡ Bola sempre começa no centro
        if (bola != null)
        {
            bola.transform.position = Vector3.zero;
            var rb = bola.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }
    }

    void Update()
    {
        if (myId == -1) return;

        // Movimento vertical da raquete
        float v = Input.GetAxis("Vertical");
        localCube.transform.Translate(new Vector3(0, v, 0) * Time.deltaTime * 5);

        // Limite no eixo Y
        Vector3 pos = localCube.transform.position;
        pos.y = Mathf.Clamp(pos.y, -3f, 3f);
        localCube.transform.position = pos;

        // Envia posição da raquete
        string msg = "POS:" +
                     myId + ";" + // ✅ envia junto o ID
                     localCube.transform.position.x.ToString("F2", CultureInfo.InvariantCulture) + ";" +
                     localCube.transform.position.y.ToString("F2", CultureInfo.InvariantCulture);

        SendMessage(msg);

        // Atualiza posição do outro jogador suavemente
        remoteCube.transform.position =
            Vector3.Lerp(remoteCube.transform.position, remotePos, Time.deltaTime * 10f);
    }

    void ReceiveData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            byte[] data = client.Receive(ref remoteEP);
            string msg = Encoding.UTF8.GetString(data);

            if (msg.StartsWith("ASSIGN:"))
            {
                myId = int.Parse(msg.Substring(7));
                Debug.Log("[Cliente] Meu ID = " + myId);
            }
            else if (msg.StartsWith("POS:"))
            {
                string[] parts = msg.Substring(4).Split(';');
                if (parts.Length == 3)
                {
                    int id = int.Parse(parts[0]);
                    if (id != myId)
                    {
                        float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        remotePos = new Vector3(x, y, 0);
                    }
                }
            }
            else if (msg.StartsWith("BALL:"))
            {
                // Só atualiza se não for o host da bola (ID 2)
                if (myId != 2)
                {
                    string[] parts = msg.Substring(5).Split(';');
                    if (parts.Length == 2)
                    {
                        float x = float.Parse(parts[0], CultureInfo.InvariantCulture);
                        float y = float.Parse(parts[1], CultureInfo.InvariantCulture);

                        if (bola != null)
                            bola.transform.position = new Vector3(x, y, 0);
                    }
                }
            }
            else if (msg.StartsWith("SCORE:"))
            {
                string[] parts = msg.Substring(6).Split(';');
                if (parts.Length == 2)
                {
                    int scoreA = int.Parse(parts[0]);
                    int scoreB = int.Parse(parts[1]);

                    if (bola != null)
                    {
                        Bola bolaScript = bola.GetComponent<Bola>();
                        bolaScript.PontoA = scoreA;
                        bolaScript.PontoB = scoreB;
                        bolaScript.textoPontoA.text = "Pontos: " + scoreA;
                        bolaScript.textoPontoB.text = "Pontos: " + scoreB;
                    }
                }
            }
        }
    }

    public void SendMessage(string msg)
    {
        client.Send(Encoding.UTF8.GetBytes(msg), msg.Length);
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        client.Close();
    }
}
