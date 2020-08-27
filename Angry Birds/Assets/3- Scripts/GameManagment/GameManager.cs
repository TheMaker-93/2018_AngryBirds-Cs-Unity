using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// esta fuera de la clase para poder acceder a el mas facilmente
public enum GameStatus
{
    onPlay, onEndSucces, onEndDefeat,
}

public class GameManager : MonoBehaviour
{
    public GameObject birdsContainer;           // contenedor de los pajaros
    public GameObject[] birds;
    private BirdController activeBirdController;
    private BirdController[] birdsController;
    private ScoreController scController;
    private LevelController levelController;

    // boleansaa que guardaran si quedan o no dichos elementos
    public bool noMoreBirds;
    public bool noMorePigs;

    public GameObject blocksContainer;
    public GameObject pigsContainer;        // contenedor de los cerdos
                                            //private GameObject[] pigs;
    [Header("Canvases")]
    public GameObject winCanvas;
    public GameObject loseCanvas;
    public GameObject starsCanvas;
    public GameObject leftStar, midleStar, rightStar;


    [Header("birds HUD")]
    public int remainningBirds;                 // pajaros restantes 
    public Text remainningBirdsText;            // texto que se muestra


    public GameStatus gameStatus;

    [Header("Sound SFX")]
    public GameObject speakerPrefab;
    public AudioClip succesSFX;
    public AudioClip defeatSFX;

    // Use this for initialization
    void Start()
    {
        scController = GetComponent<ScoreController>();
        levelController = GetComponent<LevelController>();

        // activamos el estado de play
        gameStatus = GameStatus.onPlay;

        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        starsCanvas.SetActive(false);

        // damos un tamaño al array de pajaros y lo rellenamos 
        birds = new GameObject[birdsContainer.transform.childCount];
        birdsController = new BirdController[birds.Length];
        for (int i = 0; i < birdsContainer.transform.childCount; i++)
        {
            birds[i] = birdsContainer.transform.GetChild(i).gameObject;
            birdsController[i] = birds[i].GetComponent<BirdController>();
        }

        // seteamos el numero de pajaros inicial en la interfaz
        remainningBirds = birds.Length;
        remainningBirdsText.text = remainningBirds.ToString();


    }

    // Update is called once per frame
    void Update()
    {
        // si hemso perdido permitimos el volver a realizar el nivel
        if (Input.GetKeyDown(KeyCode.R) && gameStatus == GameStatus.onEndDefeat)
        {
            ReloadLevel();
        } 
        if (Input.GetKeyDown(KeyCode.Return) && gameStatus == GameStatus.onEndSucces)
        {
            levelController.NextScene();
        }



        // avanzamos la cola de los pajaros
        if (Input.GetKeyDown(KeyCode.Space))
        {          
            // avanzamos el resto de pajaros y destruimos el ultimo
            MoveBirdsOnePosition(true);
        }


    }

    // este funcion hara que el resto de pajaros se mueva una posicion adelante
    public void MoveBirdsOnePosition(bool cheatDestruction = false)
    {

        // restamos un bird a los que nos quedan en el hud
        remainningBirds -= 1;
        remainningBirdsText.text = remainningBirds.ToString();


        // antes de avanzar en la cola comprobamos si quedan pajaros (para mostrar el game Over)
        // tambien si quedan cerdos (esto primero para que sea mas determinadnte que el numero de pajaros)

        if (CheckForPigs())
        {
            noMorePigs = true;
            winCanvas.SetActive(true);
            gameStatus = GameStatus.onEndSucces;        // estado de victoria

            Debug.LogWarning("Has ganado");

            // creamos el altavoz de victoria
            GameObject victorySpeaker = Instantiate(speakerPrefab);
            victorySpeaker.GetComponent<AudioEffectController>().PlayAudioclip(succesSFX);

            ShowStarsCanvas();

            return;
        }

        if (CheckForBirds())
        {
            noMoreBirds = true;
            loseCanvas.SetActive(true);
            gameStatus = GameStatus.onEndDefeat;        // estado de derrota

            ShowStarsCanvas();
            return;
        }


        for (int i = 0; i < birds.Length; i++)
        {
        
            // guardamos el pajaro que estamos controlando solo para guardar el que borramos
            if (birdsController[i].underPlayerControl)
            {
                activeBirdController = birdsController[i];          
            }

            // avanzamos en la cola
            birdsController[i].AdvanceOnQueue();

        }
                
        if (cheatDestruction)
        {
            Destroy(activeBirdController.gameObject);
        }

    }


    // comporvamos que no queden cerdos
    public bool CheckForPigs()
    {
        if (pigsContainer.transform.childCount == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // comporbamos si nos quedan pajaros
    public bool CheckForBirds()
    {
        // usamos 1 por el orden de ejecucion
        if (birdsContainer.transform.childCount <= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // funcionar para recargar el nivel
    public void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public GameStatus GetGameStatus ()
    {
        return gameStatus;
    }

    public void ShowStarsCanvas()
    {

        int starsToShow = scController.StarsCalculator(3);

        // mostramos las estrellas
        starsCanvas.SetActive(true);

        switch (starsToShow)
            {
                case (1):
                    leftStar.SetActive(true);
                    // mostramos la estrella de la izqueirda

                    break;
                case (2):
                    leftStar.SetActive(true);
                    midleStar.SetActive(true);

                    // mostramos la izquierda y la central

                    break;
                case (3):
                    leftStar.SetActive(true);
                    midleStar.SetActive(true);
                    rightStar.SetActive(true);
                    // mostramos todas las estrellas

                    break;
            default:
                Debug.Log("No mostramos ninguna estrella");

                break;
            }

    }

}





