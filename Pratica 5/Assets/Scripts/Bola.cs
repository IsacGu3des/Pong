using UnityEngine;
using TMPro;
using System.Net.Sockets;
using System.Globalization;

public class Bola : MonoBehaviour
{
    public int PontoA = 0;
    public int PontoB = 0;
    public float velocidade = 7f;
    private Rigidbody2D rb;

    public TextMeshProUGUI textoPontoA;
    public TextMeshProUGUI textoPontoB;

    public UdpClientTwoClients netClient; // referência ao cliente

    private bool iniciou = false; // bola começou a se mover?

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        transform.position = Vector3.zero;
    }

    void Update()
    {
        if (!iniciou && netClient.myId == 2)
        {
            // Agora o ID 2 entrou, começamos o jogo
            IniciarBola();
        }

        // Só envia posição se o ID 2 (host) está jogando
        if (iniciou && netClient.myId == 2)
        {
            string msg = $"BALL:{transform.position.x.ToString("F2", CultureInfo.InvariantCulture)};" +
                         $"{transform.position.y.ToString("F2", CultureInfo.InvariantCulture)}";
            netClient.SendMessage(msg);
        }
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        if (!iniciou || netClient.myId != 2) return;

        if (colisao.gameObject.CompareTag("Raquete"))
        {
            float posicaoBolaY = transform.position.y;
            float posicaoRaqueteY = colisao.transform.position.y;
            float alturaRaquete = colisao.collider.bounds.size.y;

            float diferenca = (posicaoBolaY - posicaoRaqueteY) / (alturaRaquete / 2);
            float direcaoX = rb.linearVelocity.x > 0 ? 1 : -1;
            Vector2 novaDirecao = new Vector2(direcaoX, diferenca).normalized;

            rb.linearVelocity = novaDirecao * velocidade;
        }

        if (colisao.gameObject.tag == "Gol1")
        {
            PontoA++;
            AtualizarTexto();
            ReiniciarBola();
            netClient.SendMessage($"SCORE:{PontoA};{PontoB}");
        }

        if (colisao.gameObject.tag == "Gol2")
        {
            PontoB++;
            AtualizarTexto();
            ReiniciarBola();
            netClient.SendMessage($"SCORE:{PontoA};{PontoB}");
        }
    }

    void AtualizarTexto()
    {
        textoPontoA.text = "Pontos: " + PontoA;
        textoPontoB.text = "Pontos: " + PontoB;
    }

    void ReiniciarBola()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = Vector3.zero;

        if (netClient.myId == 2)
        {
            Invoke(nameof(ArremessarBola), 1f);
        }
    }

    void IniciarBola()
    {
        iniciou = true;
        ArremessarBola();
    }

    void ArremessarBola()
    {
        float direcaoX = Random.value < 0.5f ? -1f : 1f;
        float direcaoY = Random.Range(-0.5f, 0.5f);
        rb.linearVelocity = new Vector2(direcaoX, direcaoY).normalized * velocidade;
    }
}
