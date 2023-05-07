using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interacturable : MonoBehaviour

{
    private bool puedeInteractuar;
    private BoxCollider2D bc;
    private SpriteRenderer sp;
    private GameObject indicadorInteractuable;

    public UnityEvent evento;
    public bool esSelector;



    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        sp = GetComponent<SpriteRenderer>();
        //anim = GetComponent<Animator>();
        if (transform.GetChild(0) != null)
            indicadorInteractuable = transform.GetChild(0).gameObject;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            puedeInteractuar = true;
            indicadorInteractuable.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            puedeInteractuar = false;
            indicadorInteractuable.SetActive(false);
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if (puedeInteractuar && Input.GetKeyDown(KeyCode.C))
        {
            
            SeleccionarNivel();
        }
        
    }

    private void SeleccionarNivel()
    {
        if (esSelector)
        {
            evento.Invoke();
        }
    }


}
