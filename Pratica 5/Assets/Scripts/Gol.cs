using Unity.VisualScripting;
using UnityEngine;

public class Gol : MonoBehaviour
{
    public GameObject GolA;
    public GameObject GolB;
    public int PontoA = 0;
    public int PontoB = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D (Collision2D collision)
    {
        if (collision.gameObject.tag == "Gol1" )
        {
            PontoA++;
        }
        if (collision.gameObject.tag == "Gol2")
        {
            PontoB++;
        }
    }
}
