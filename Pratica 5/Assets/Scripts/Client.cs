using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Globalization;

public class UdpClientTwoClients : MonoBehaviour {
    UdpClient client;
    Thread receiveThread;
    IPEndPoint serverEP;
    int myId = -1;
    Vector3 remotePos = Vector3.zero;

    public GameObject player1Cube;
    public GameObject player2Cube;

    GameObject localCube;
    GameObject remoteCube;

    void Start() {
        client = new UdpClient();
        serverEP = new IPEndPoint(IPAddress.Parse("10.57.1.11"), 5001);
        client.Connect(serverEP);
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
        client.Send(Encoding.UTF8.GetBytes("HELLO"), 5);
    }

    void Update() {
        if (myId == -1) return; // ainda não recebeu ID do servidor

        // Movimento local
        float v = Input.GetAxis("Vertical");
        localCube.transform.Translate(new Vector3(0, v, 0) * Time.deltaTime * 5);

        // Limitar movimento no eixo Y (exemplo: entre -3 e 3)
        Vector3 pos = localCube.transform.position;
        pos.y = Mathf.Clamp(pos.y, -3f, 3f);
        localCube.transform.position = pos;

        // Envia posição junto com ID
        string msg = "POS:" + myId + ";" +
                     localCube.transform.position.x.ToString("F2", CultureInfo.InvariantCulture) + ";" +
                     localCube.transform.position.y.ToString("F2", CultureInfo.InvariantCulture);

        client.Send(Encoding.UTF8.GetBytes(msg), msg.Length);

        // Atualiza posição do outro jogador
        remoteCube.transform.position =
            Vector3.Lerp(remoteCube.transform.position, remotePos, Time.deltaTime * 10f);
    }

    void ReceiveData() {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        while (true) {
            byte[] data = client.Receive(ref remoteEP);
            string msg = Encoding.UTF8.GetString(data);

            if (msg.StartsWith("ASSIGN:")) {
                myId = int.Parse(msg.Substring(7));
                Debug.Log("[Cliente] Meu ID = " + myId);

                // Define quem é quem
                if (myId == 1) {
                    localCube = player1Cube;
                    remoteCube = player2Cube;
                } else {
                    localCube = player2Cube;
                    remoteCube = player1Cube;
                }
            }
            else if (msg.StartsWith("POS:")) {
                string[] parts = msg.Substring(4).Split(';');

                if (parts.Length == 3) {
                    int id = int.Parse(parts[0]);

                    if (id != myId) {
                        float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float y = float.Parse(parts[2], CultureInfo.InvariantCulture);

                        remotePos = new Vector3(x, y, 0);
                    }
                }
            }
        }
    }

    void OnApplicationQuit() {
        if (receiveThread != null) receiveThread.Abort();
        if (client != null) client.Close();
    }
}
