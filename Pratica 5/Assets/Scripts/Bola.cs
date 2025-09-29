using UnityEngine;
using TMPro;

public class Bola : MonoBehaviour
{
    public int PontoA = 0;
    public int PontoB = 0;
    public float velocidade = 7f;
    private Rigidbody2D rb;
    public TextMeshProUGUI textoPontoA;
    public TextMeshProUGUI textoPontoB;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Começa indo para um lado aleatório
        float direcaoX = Random.value < 0.5f ? -1f : 1f;
        float direcaoY = Random.Range(-0.5f, 0.5f);

        rb.linearVelocity = new Vector2(direcaoX, direcaoY).normalized * velocidade;
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        if (colisao.gameObject.CompareTag("Raquete"))
        {
            // Pega a posição da bola e da raquete
            float posicaoBolaY = transform.position.y;
            float posicaoRaqueteY = colisao.transform.position.y;
            float alturaRaquete = colisao.collider.bounds.size.y;

            // Calcula onde bateu (valor entre -1 e 1)
            float diferenca = (posicaoBolaY - posicaoRaqueteY) / (alturaRaquete / 2);

            // Define a nova direção da bola
            float direcaoX = rb.linearVelocity.x > 0 ? 1 : -1; // mantém indo pro lado certo
            Vector2 novaDirecao = new Vector2(direcaoX, diferenca).normalized;

            rb.linearVelocity = novaDirecao * velocidade;
        }
        
                if (colisao.gameObject.tag == "Gol1" )
                {
                    PontoA++;
                    AtualizarTexto();
                    ReiniciarBola();
                }

                if (colisao.gameObject.tag == "Gol2")
                {
                    PontoB++;
                    AtualizarTexto();
                    ReiniciarBola();
                }

    }
    void AtualizarTexto()
    {
        textoPontoA.text = "Pontos: " + PontoA.ToString();
        textoPontoB.text = "Pontos: " + PontoB.ToString();
    }
    void ReiniciarBola()
    {
        // Para a bola
        rb.linearVelocity = Vector2.zero;

        // Coloca no centro
        transform.position = Vector2.zero;

        // Espera um curto tempo e arremessa
        Invoke(nameof(ArremessarBola), 1f); // 1 segundo de espera
    }

    void ArremessarBola()
    {
        float direcaoX = Random.value < 0.5f ? -1f : 1f;
        float direcaoY = Random.Range(-0.5f, 0.5f);
        Vector2 direcao = new Vector2(direcaoX, direcaoY).normalized;

        rb.linearVelocity = direcao * velocidade;
    }
}