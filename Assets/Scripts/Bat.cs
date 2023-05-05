using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Bat : MonoBehaviour
{
    private CinemachineVirtualCamera cm;
    private SpriteRenderer sp;
    private PlayerController player;
    private Rigidbody2D rb;
    private bool aplicarFuerza;
    private bool agitando;

    public float velocidadDeMovimiento = 3;
    public float radioDeDeteccion = 15;
    public LayerMask layerJugador;

    public Vector2 posicionCabeza;

    public int vidas = 3;
    public string nombre;

    private void Awake()
    {
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = nombre;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioDeDeteccion);
        Gizmos.color = Color.green;
        Gizmos.DrawCube((Vector2)transform.position + posicionCabeza, new Vector2(1, 0.5f) * 0.7f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direccion = player.transform.position - transform.position;
        float distancia = Vector2.Distance(transform.position, player.transform.position);

        if (distancia <= radioDeDeteccion)
        {
            rb.velocity = direccion.normalized * velocidadDeMovimiento;
            CambiarVista(direccion.normalized.x);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void CambiarVista(float direccionX)
    {
        if (direccionX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (direccionX > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (transform.position.y + posicionCabeza.y < player.transform.position.y - 0.7f)
            {
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up * player.fuerzaDeSalto;
                StartCoroutine(AgitarCamara(0.1f));
                Destroy(gameObject, 0.2f);
            }
            else
            {
                //player.RecibirDano((transform.position - player.transform.position).normalized);
            }
        }
    }

    private void FixedUpdate()
    {
        if (aplicarFuerza)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * 100, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }

    public void RecibirDano()
    {
        if (vidas > 0)
        {
            StartCoroutine(EfectoDano());
            StartCoroutine(AgitarCamara(0.1f));
            aplicarFuerza = true;
            vidas--;
        }
        else
        {
            StartCoroutine(UltimoAgitarCamara(0.3f));
        }
    }

    private void Morir()
    {
        if (vidas <= 0)
        {
            Destroy(gameObject, 0.2f);
        }
    }

    private IEnumerator AgitarCamara(float tiempo)
    {
        if (!agitando)
        {
            agitando = true;
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
            yield return new WaitForSeconds(tiempo);
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            //Morir();
            agitando = false;
        }

    }

    private IEnumerator UltimoAgitarCamara(float tiempo)
    {
        if (!agitando)
        {
            transform.localScale = Vector3.zero;
            agitando = true;
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
            yield return new WaitForSeconds(tiempo);
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            Morir();
            agitando = false;
        }
    }

    private IEnumerator EfectoDano()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }
}
