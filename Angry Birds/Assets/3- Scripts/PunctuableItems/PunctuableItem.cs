using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunctuableItem : MonoBehaviour {

    [Header("Score & Health Atributes")]
    public int score;           // puntuacion que este objeto nos dara al ser destruido
    public float healthPoints;        // salud total del objeto
    private float maxHealth;     // salud actual
    public bool destructible = true;            // el bloque sera destructible por defecto
    public float damageMultiplier;    // este valor controlara el daño del impacto 

    [Header("Referncias")]
    public ScoreController scoreController;
    //private Rigidbody2D rBody;
    private SpriteRenderer sprRenderer;

    [Header("Damage States")]
    public int damageState;         // esatdo de daño actual
    public Sprite[] damageStates;   // diferentes sprites de estado para la pieza/cerdo

    [Header("Audio")]
    public GameObject speakerPrefab;
    public AudioClip impactSFX;         // sonido para cuando el bloque impacta con otro bloque o el pajaro
    private float impactSFXmargin = 2;

    [Header("Video")]
    public GameObject animationPlayerPrefab;

    public GameObject scorePopUpPrefab;     // prefab que instanciaremos cuando muramos mostrando nuestro valor en puntuacion


    public virtual void Start()
    {
        // cargamos las referencias de los componentes
        sprRenderer = GetComponent<SpriteRenderer>();
        sprRenderer.sprite = damageStates[damageState];

        maxHealth = healthPoints;

    }

    // para recuperar la score que este objeto puede dar
    public int GetScore()
    {
        return score;
    }


    // esta funcion se encargara de mediante el calculo de la velocidad de impacto relativa a este objeto destruirlo o restarle salud
    public void CheckImpactDamage(float relativeVelocity)
    {
        if (destructible)
        {
            // restamos a la vida la velocidad relativa 
            healthPoints -= relativeVelocity * damageMultiplier;
            if (healthPoints <= 0)
            {
                Death();
            } else
            {
                damageState = Mathf.RoundToInt((((healthPoints / maxHealth) * (damageStates.Length - 1)) - (damageStates.Length - 1)) * -1);
                sprRenderer.sprite = damageStates[damageState];
            }

        }
    }

    // calculamos las colisiones del objeto con el rsto de elementos (menos con el pajaro ya que lo calcula el)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // calculamos las colisiones con los objetos que no sean bird
        if (collision.transform.tag == "Block" || collision.transform.tag == "Pig" || collision.transform.tag == "Ground")
        {
            // pinesa que ambos elmentos han de sufrir daño en el impacto con lo cual
            // ambos ejecutran este codigo al impactar
            CheckImpactDamage(collision.relativeVelocity.magnitude);

        }

        // si la velocidad es mayor que una variable fija (ya que es para evitar que suene todo el raton, el artista no debe tocar esto)
        if (collision.relativeVelocity.magnitude >= impactSFXmargin)
        {
            GameObject speaker = Instantiate(speakerPrefab);
            speaker.GetComponent<AudioEffectController>().PlayAudioclip(impactSFX);
            speaker.transform.position = transform.position;
        }

    }

    // muerte o destruccion del bloque
    public virtual void Death ()
    {
        // cuando llamemos esto desde la clase hija se generara el sonido y despues se subira la puntucaion y morira (inmediatmaente despues)
        scoreController.IncreaseScore(score);

        // generamos el popup de puntuacion dependiendo de nuestro valor en puntuacion
        GameObject scorePopUp = Instantiate(scorePopUpPrefab);
        scorePopUp.GetComponent<PopUpController>().SetStartingPosition(transform.position);


        Destroy(this.gameObject);
    }

}
