using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

    [Header("Referencias e inputs")]
    private GameManager gameManager;
    public Text inGameScore;                // puntuacion que mostraremos durante la partida
    public int currentScore;
    public GameObject punctuableItemsContainer;         // los hijos de este objeto son los objetos que generan puntuacion
    private GameObject pigsContainer;
    private GameObject blocksContainer;
    public int totalPosibleScore;          // puntuacion maxima posible en el nivel (se basara en la suma de las puntuaciones que dan todos los objetos puntuables)



	// Use this for initialization
	void Start () {

        gameManager = GetComponent<GameManager>();

        // score inicial 
        inGameScore.text = "0";

        // rellenamos guardamos la puntuacion total que nos pueden dar los hijos al destruirlos todos
        for (int i = 0; i < punctuableItemsContainer.transform.childCount; i ++)
        {
            Debug.Log("cargando score del elemento " + i + " de la lita de puntuable items con nombre " + punctuableItemsContainer.transform.GetChild(i).transform.name );

            // si se trata de la coleccion de cerdos 
            if (punctuableItemsContainer.transform.GetChild(i).name == "PigsCollection")
            {
                Debug.LogWarning("PigsCollection located and loaded");
                pigsContainer = punctuableItemsContainer.transform.GetChild(i).gameObject;

            } else if (punctuableItemsContainer.transform.GetChild(i).name == "BlocksCollection")
            {
                Debug.LogWarning("BlocksCollection located and loaded");
                blocksContainer = punctuableItemsContainer.transform.GetChild(i).gameObject;
            }
        }

        // ahora iteraremos en ambos contenedores e iremos sumando sus puntuaciones a nuestra variable
        for (int i = 0; i < pigsContainer.transform.childCount; i++)
        {
            totalPosibleScore += pigsContainer.transform.GetChild(i).GetComponent<PunctuableItem>().GetScore();
        }
        for (int i = 0; i < blocksContainer.transform.childCount; i++)
        {
            totalPosibleScore += blocksContainer.transform.GetChild(i).GetComponent<PunctuableItem>().GetScore();
        }

        Debug.Log(totalPosibleScore);

    }
	
    public void IncreaseScore (int delta)
    {
        if (gameManager.gameStatus == GameStatus.onPlay)
        {
            currentScore += delta;
            inGameScore.text = currentScore.ToString();
        }
    }

    // funcion que caluclara el numero de estrellas y que sera llamada desde gameManager
    public int StarsCalculator(int maxNumberofStars)
    {
        // VARIABLES CON INFORMACION
        Debug.LogWarning("currentScore = " + currentScore);
        Debug.LogWarning("maxScore = " + totalPosibleScore);
        Debug.LogWarning("maxStars = " + maxNumberofStars);

        // ESTE CODIGO DEBERIA FUNCINAR, PERO POR ALGUN MOTIVO EN EL SIGUIENTE DEBUG LA OPERACION RESULTA EN 0 (muchas veces) aun siendo valores con "valor"


        Debug.LogError((currentScore / totalPosibleScore) );
        Debug.LogError(Mathf.RoundToInt((currentScore / totalPosibleScore) * maxNumberofStars));

        int starsToShow = Mathf.RoundToInt((currentScore / totalPosibleScore) * maxNumberofStars);


        Debug.LogWarning("currentScore = " + currentScore);
        Debug.LogWarning("maxScore = " + totalPosibleScore);
        Debug.LogWarning("maxStars = " + maxNumberofStars);

        return starsToShow;

    }

}
