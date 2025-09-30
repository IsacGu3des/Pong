using UnityEngine;

public class Bola : MonoBehaviour
{
    private Rigidbody2D rb;
    private UdpClientTwoClients udpClient;

    public int PontoA = 0;
    public int PontoB = 0;
    public UnityEngine.UI.Text textoPontoA;
    public UnityEngine.UI.Text textoPontoB;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        udpClient = FindObjectOfType<UdpClientTwoClients>();

        // Só o jogador 2 controla o movimento inicial da bola
        if (udpClient != null && udpClient.myId == 2)
        {
            Invoke("LancarBola", 1f);
        }
    }

    void LancarBola()
    {
        float dirX = Random.Range(0, 2) == 0 ? -1 : 1;
        float dirY = Random.Range(-1f, 1f);
        rb.linearVelocity = new Vector2(dirX, dirY).normalized * 5f;
    }

    void Update()
    {
        if (udpClient == null) return;

        // Só o player 2 envia a posição da bola
        if (udpClient.myId == 2)
        {
            string msg = "BALL:" +
                         transform.position.x.ToString(System.Globalization.CultureInfo.InvariantCulture) + ";" +
                         transform.position.y.ToString(System.Globalization.CultureInfo.InvariantCulture);

            udpClient.SendUdpMessage(msg);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (udpClient == null) return;

        if (col.gameObject.CompareTag("Gol1"))
        {
            PontoB++;
            ResetBola();
        }
        else if (col.gameObject.CompareTag("Gol2"))
        {
            PontoA++;
            ResetBola();
        }
    }

    void ResetBola()
    {
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;

        if (udpClient != null && udpClient.myId == 2)
        {
            Invoke("LancarBola", 1f);

            // envia placar atualizado
            string msg = "SCORE:" + PontoA + ";" + PontoB;
            udpClient.SendUdpMessage(msg);
        }
    }
}