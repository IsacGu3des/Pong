using UnityEngine;

public class Bola : MonoBehaviour
{
    public int PontoA = 0;
    public int PontoB = 0;
    public float velocidade = 7f;
    private Rigidbody2D rb;

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
        
                if (collisao.gameObject.tag == "Gol1" )
                {
                    PontoA++;
                }
                if (collisao.gameObject.tag == "Gol2")
                {
                    PontoB++;
                }
            
    }
}