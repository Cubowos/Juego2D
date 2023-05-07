using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject vidasUI;

    public PlayerController player;
    public Text textoMonedas;
    public int monedas;
   

    public GameObject panelPausa;
    public GameObject panelGameOver;
    public GameObject panelCarga;

    public bool avanzandoNivel;
    public int nivelActual;
    public List<Transform> posicionesAvance = new List<Transform>();
    public List<Transform> posicionesRetroceder = new List<Transform>();
    public GameObject panelTransicion;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        /*if(PlayerPrefs.GetInt("vidas") != 0)
            CargarPartida();*/
    }

    public void ActivarPanelTransicion()
    {
        panelTransicion.GetComponent<Animator>().SetTrigger("ocultar");
    }

    public void CambiarPosicionJugador()
    {
        if (avanzandoNivel)
        {
            if (nivelActual + 1 < posicionesAvance.Count)
            {
                player.transform.position = posicionesAvance[nivelActual + 1].transform.position;
             //   cinemachineConfiner.m_BoundingShape2D = areasCamara[nivelActual + 1];
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.GetComponent<Animator>().SetBool("caminar", false);
                player.terminandoMapa = false;
            }

        }
        else
        {
            if (posicionesRetroceder.Count > nivelActual - 1)
            {
                player.transform.position = posicionesRetroceder[nivelActual - 1].transform.position;
          //      cinemachineConfiner.m_BoundingShape2D = areasCamara[nivelActual - 1];
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.GetComponent<Animator>().SetBool("caminar", false);
                player.terminandoMapa = false;

            }

        }
    }
    public void ActualizarContadorMonedas()
    {
        monedas++;
        textoMonedas.text = monedas.ToString();
    }

    public void PausarJuego()
    {
        Time.timeScale = 0;
        panelPausa.SetActive(true);
    }

    public void DespausarJuego()
    {
        Time.timeScale = 1;
        panelPausa.SetActive(false);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
    public void CargarSelector()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SampleScene");
    }
    public void GameOver()
    {
        panelGameOver.SetActive(true);

    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }

    public void CargarEscenaSelector()
    {
        StartCoroutine(CargarEscena());
    }

    private IEnumerator CargarEscena()
    {
        panelCarga.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SampleScene");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return new WaitForSeconds(3);
        }
    }

    public void CargarNivel(string nombreNivel)
    {
        SceneManager.LoadScene(nombreNivel);
    }

}
