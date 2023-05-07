using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private int direccionX;

    private Rigidbody2D rb;
    private Animator anim;
    public Vector2 direccion;
    private CinemachineVirtualCamera cm;
    private Vector2 direccionMovimiento;
    private Vector2 direccionDano;
    private bool bloqueado;
    private GrayCamera gc;
    private SpriteRenderer sprite;

    [Header("Estadisticas")]
    public float velocidadDeMovimiento = 10;
    public float fuerzaDeSalto = 5;
    public float velocidadDash = 20;
    public int vidas = 3;
    public float tiempoInmortalidad;


    [Header("Colisiones")]
    public LayerMask layerPiso;
    public float radioDeColision;
    public Vector2 abajo;

    [Header("Booleanos")]
    public bool puedeMover = true;
    public bool enSuelo = true;
    public bool puedeDash;
    public bool haciendoDash;
    public bool tocadoPiso;
    public bool haciendoShake = false;
    public bool estaAtacando;
    public bool esInmortal;
    public bool aplicarFuerza;
    public bool terminandoMapa;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        gc = Camera.main.GetComponent<GrayCamera>();
        sprite = GetComponent<SpriteRenderer>();

    }
    public void SetBloqueadoTrue()
    {
        bloqueado = true;
    }
    public void Morir()
    {
        if (vidas > 0)
            return;

        GameManager.instance.GameOver();
        this.enabled = false;
    }

    public void RecibirDano()
    {
        StartCoroutine(ImpactoDano(Vector2.zero));
    }

    public void RecibirDano(Vector2 direccionDano)
    {
        StartCoroutine(ImpactoDano(direccionDano));
    }

    private IEnumerator ImpactoDano(Vector2 direccionDano)
    {
        if (!esInmortal)
        {
            StartCoroutine(Inmortalidad());
            vidas--;
            gc.enabled = true;
            float velocidadAuxiliar = velocidadDeMovimiento;
            this.direccionDano = direccionDano;
            aplicarFuerza = true;
            Time.timeScale = 0.4f;
            FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
            StartCoroutine(AgitarCamara());
            yield return new WaitForSeconds(0.2f);
            Time.timeScale = 1;
            gc.enabled = false;

            for (int i = GameManager.instance.vidasUI.transform.childCount - 1; i >= 0; i--)
            {
                if (GameManager.instance.vidasUI.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    GameManager.instance.vidasUI.transform.GetChild(i).gameObject.SetActive(false);
                    break;
                }
               
            }

            

            velocidadDeMovimiento = velocidadAuxiliar;
            Morir();
        }
    }
 
    private void FixedUpdate()
    {
        if (aplicarFuerza)
        {
            velocidadDeMovimiento = 0;
            rb.velocity = Vector2.zero;
            rb.AddForce(-direccionDano * 25, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }
    public void DarInmortalidad()
    {
        StartCoroutine(Inmortalidad());
    }

    private IEnumerator Inmortalidad()
    {
        esInmortal = true;

        float tiempoTranscurrido = 0;

        while (tiempoTranscurrido < tiempoInmortalidad)
        {
            sprite.color = new Color(1, 1, 1, .5f);
            yield return new WaitForSeconds(tiempoInmortalidad / 20);
            sprite.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(tiempoInmortalidad / 20);
            tiempoTranscurrido += tiempoInmortalidad / 10;
        }

        esInmortal = false;
    }
    public void MovimientoFinalMapa(int direccionX)
    {
        terminandoMapa = true;
        this.direccionX = direccionX;
        anim.SetBool("caminar", true);
        if (this.direccionX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        }
        else if (this.direccionX > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

    }




    void Start()
    {

    }

    // Update is called one per frame
    void Update()
    {
        if (!terminandoMapa)
        {
            Movimiento();

        }
        else
        {
            rb.velocity = (new Vector2(direccionX * velocidadDeMovimiento, rb.velocity.y));
        }

        /*   if (!esInmortal && ultimoEnemigo != null)
        {
            Physics2D.IgnoreCollision(ultimoEnemigo.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
               ultimoEnemigo = null;
        }*/

    }

    private void Atacar(Vector2 direccion)
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            if (!estaAtacando && !haciendoDash)
            {
                estaAtacando = true;

                anim.SetFloat("ataqueX",direccion.x);
                anim.SetFloat("ataqueY",direccion.y);

                anim.SetBool("atacar", true);


            }
        }
    }

    public void FinalizarAtaque()
    {
        anim.SetBool("atacar", false);
        bloqueado = false;
        estaAtacando = false;
    }

    private Vector2 DireccionAtaque(Vector2 direccionMovimiento, Vector2 direccion)
    {
        if(rb.velocity.x == 0 && direccion.y != 0)
            return new Vector2(0, direccion.y);
        
        return new Vector2(direccionMovimiento.x, direccion.y);

    }
    private IEnumerator AgitarCamara()
    {
        haciendoShake = true;

        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(0.3f);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        haciendoShake = false;
    }    
    
    private IEnumerator AgitarCamara(float tiempo)
    {
        haciendoShake = true;

        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(tiempo);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        haciendoShake = false;
    }



    private void Dash(float x, float y)
    {
        anim.SetBool("dash", true);
        Vector3 posicionJugador = Camera.main.WorldToViewportPoint(transform.position);
        Camera.main.GetComponent<RippleEffect>().Emit(posicionJugador);
        StartCoroutine(AgitarCamara());

        puedeDash = true;
        rb.velocity = Vector2.zero;
        rb.velocity += new Vector2(x, y).normalized * velocidadDash;
        StartCoroutine(PrepararDash());
    }

    private IEnumerator PrepararDash()
    {
        StartCoroutine(DashSuelo());


        rb.gravityScale = 0;
        haciendoDash = true;

        yield return new WaitForSeconds(0.3f);

        rb.gravityScale = 3;
        haciendoDash = false;
        FinalizarDash();


    }


    private IEnumerator DashSuelo()
    {
        yield return new WaitForSeconds(0.15f);
        if (enSuelo)
        {
            puedeDash = false;

        }
    }

    public void FinalizarDash()
    {
        anim.SetBool("dash", false);
    }

    private void TocarPiso()
    {
        puedeDash = false;
        haciendoDash = false;
        anim.SetBool("saltar", false);
    }

    private void Movimiento()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        
        direccion = new Vector2(x, y);
        Vector2 direccionRaw = new Vector2(xRaw, yRaw);

       
        Caminar();
        Atacar(DireccionAtaque(direccionMovimiento, direccionRaw));

        MejorarSalto();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (enSuelo)
            {
                anim.SetBool("saltar", true);
                Saltar();
            }

        }
      /*  if (Input.GetKeyDown(KeyCode.X) && !haciendoDash && !puedeDash)
        {
            if (xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }
        if (enSuelo && !tocadoPiso)*/
        {
            TocarPiso();
            tocadoPiso = true;
        }
        if (!enSuelo && tocadoPiso)
        {
            tocadoPiso = false;
        }

        float velocidad;
        if (rb.velocity.y > 0)
            velocidad = 1;
        else
            velocidad = -1;
        if (!enSuelo)
        {

            anim.SetFloat("velocidadVertical", velocidad);
        }
        else
        {
            if (velocidad == -1)
                FinalizarSalto();
        }
    }



    public void FinalizarSalto()
    {
        anim.SetBool("saltar", false);
        
    }
    private void MejorarSalto()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.5f - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.0f - 1) * Time.deltaTime;
        }
    }

    private void Agarres()
    {
        enSuelo = Physics2D.OverlapCircle((Vector2)transform.position+ abajo, radioDeColision, layerPiso);
    }

    private void Saltar()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        
        rb.velocity += Vector2.up * fuerzaDeSalto;
    }

    //private void Caminar(Vector2 direccion)
    private void Caminar()
    {
        if(puedeMover && !haciendoDash && !estaAtacando)
        {
        rb.velocity = new Vector2(direccion.x * velocidadDeMovimiento, rb.velocity.y);

            if(direccion != Vector2.zero)
            {
                if (!enSuelo)
                {
                    anim.SetBool("saltar", true);
                }
                else
                {
                    anim.SetBool("caminar", true);
                }
                
                if(direccion.x <0 && transform.localScale.x >0 )
                {
                    direccionMovimiento = DireccionAtaque(Vector2.left, direccion);
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y,transform.localScale.z);

                } else if(direccion.x > 0 && transform.localScale.x < 0)
                {
                    direccionMovimiento = DireccionAtaque(Vector2.right, direccion);
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
            }
            else
            {
                if(direccion.y > 0 && direccion.x == 0)
                {
                    direccionMovimiento = DireccionAtaque(direccion, Vector2.up);
                }
                anim.SetBool("caminar", false);
            }
        }else
        {
            if (bloqueado)
            {
                FinalizarAtaque();
            }

        }

    }
}