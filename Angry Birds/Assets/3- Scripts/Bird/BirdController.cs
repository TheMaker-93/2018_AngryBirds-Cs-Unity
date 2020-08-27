using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]

public class BirdController : MonoBehaviour {

    // un rigidboy es kninematic cuando queremos tener control de su posicion sin que el rb modifique su
    // transform. Asi que el pajaro antes de ser un objeto dinamico debera seer kinematic (busca mas informacion sobre esto)

    [Header("launch Atributes")]
    public float maxDistanceForLaunch = 1.5f;
    private bool draggingBird = false;                      // controlamos si estamos arrastrando o no el pajaro (para el lanzamiento, que cuando soltemos el raton no se genere un lanzamiento erroneo)
    public AudioClip launcherSound;                // sonido al tirar del tirachinas


    [Header("Bird Atributes")]
    public bool underPlayerControl;                 // determinara si el jugador esta controlando o no el pajaro
    public bool habilityUsed;                       // boleana que controlara si hemos relaizado nuestra accion especial
    private Sprite birdSprite;                       // sprite del pajaro
    public int positionOnQeue;

    public float timeBeforeDestruction;
    public float currentTime = 0f;

    [Header("Posible Sprites")]
    public Sprite redBirdSprite;
    public Sprite blackBirdSprite;
    public Sprite yellowBirdSprite;

    [Header("AUDIO")]
    public AudioClip deathSFX;                      // sonido de muerte
    public GameObject speakerPrefab;                // prefab a instnaciar cuando morimos
    public AudioClip birdLaunchSFX;                // sonido que hara el pajaro al salir volando

    // posibles estados del paaro
    public enum BirdStatus
    {
        onQueue, onLaunch, onAir, semiStationary
    }
    public BirdStatus birdStatus;

    // positbles tipos de pajaro
    public enum BirdType
    {
        red,
        yellow,
        black,
    }
    public BirdType birdType;

    [Header("Physics")]
    private PolygonCollider2D birdCollider;           // collider 2d circunferencia
    private Rigidbody2D rigidBody;                  // RB del pajaro
    public Vector2 launchVector;                    // vector de lanzamiento generado al hacer drag
    public float launchForceMultiplier;

    [Header("Referencias y objetos externos")]
    private SpriteRenderer birdSpriteRenderer;      // renderer
    private Rigidbody2D rBody;
    public GameManager gameManager;                 // manager que se encargara de mover el resto de pajaros de sitio una vez el activo muera
    private CameraController cameraController;      // controlador de la camara par ael movimiento de seguimietno
    //private ScoreController scoreController;

    // posiciones de la cola
    [Header("Launcher positions")]
    public Transform[] positions;        // el primero sera la posicon de launch

    // efecto de video
    [Header("Visual Effects")]
    public GameObject animationPlayerPrefab;
    public RuntimeAnimatorController smokeExplosionAnimatorController;
    public string animationName = "Smoke_A_0";

    [Header("BB Propierties")]
    public PointEffector2D pEffector;
    public float explosionForce;
    public RuntimeAnimatorController fireExplosionAnimatorController;
    public string explosionAnimationName;
    public AudioClip fireExplosionAudioClip;
    public bool effectorTriggered;
    public float currentEffectorTime = 0;
    private float effectorOnlineTime = 0.05f;

    [Header("YB propierties")]
    public float yellowAccelerationMultiplier = 2;

    // Use this for initialization
    public virtual void Start () {

        // rellenamos las referencias
        rBody = GetComponent<Rigidbody2D>();
        birdSpriteRenderer = GetComponent<SpriteRenderer>();
        cameraController = Camera.main.GetComponent<CameraController>();

        if (positionOnQeue == 0)
        {
            underPlayerControl = true;
            birdStatus = BirdStatus.onLaunch;

        } else if (positionOnQeue < 0)
        {
            birdStatus = BirdStatus.onQueue;
            underPlayerControl = false;
        }

        // seteamos un sprite u otro dependiendo del tipo de pajaro
        switch (birdType)
        {
            case (BirdType.red):
                birdSprite = redBirdSprite;
                break;
            case (BirdType.yellow):
                birdSprite = yellowBirdSprite;
                break;
            case (BirdType.black):
                birdSprite = blackBirdSprite;
                pEffector.forceMagnitude = explosionForce;
                break;
            default:
                Debug.LogWarning("No type set for the bird" + transform.name);
                break;
        }

        // cargamos el spriterendere y le cargamos el sprite
        birdSpriteRenderer.sprite = birdSprite;

        // seteamos su posicion inicial
        transform.position = positions[positionOnQeue].transform.position;
        // hacemos que sea kinematic desde un principio
        rBody.isKinematic = true;

	}
	
	// Update is called once per frame
	public virtual void Update () {

        // comprovamos nuestra velocidad y en caso de ser mas lento de x lo borramos
        InactivityChecker();
        ExecuteSpecialAction();


    }


    // operacion especial del pajaro
    public virtual void ExecuteSpecialAction ()
    {
        // si lo estasmos controlando. si no ha usado el habilidad y si hemos pulsado el boton derecho del raton
        if (underPlayerControl && habilityUsed == false && Input.GetMouseButtonDown(0) && birdStatus == BirdStatus.onAir)
        {

            // dependiendo del tipo de pajaro hara una u otra cosa
            switch (birdType)
            {
                case (BirdType.red):

                    Debug.Log("The red bird do nothing");

                    break;
                case (BirdType.yellow):

                    rBody.AddForce(rBody.velocity * yellowAccelerationMultiplier, ForceMode2D.Impulse);

                    Debug.LogWarning("The yellow bird do an acceletartion");

                    break;
                case (BirdType.black):

                    Debug.LogWarning("I, the black bird, I just explode");

                    pEffector.enabled = true;

                    // instanciamos la explosion (Audio y video)
                    GameObject animationContainer = Instantiate(animationPlayerPrefab);
                    animationContainer.transform.position = transform.position;
                    animationContainer.GetComponent<VisualEffectControler>().setAnimatorController(fireExplosionAnimatorController, explosionAnimationName);

                    
                    GameObject audioContainer = Instantiate(speakerPrefab);
                    audioContainer.transform.position = transform.position;
                    audioContainer.GetComponent<AudioEffectController>().PlayAudioclip(fireExplosionAudioClip);
                    

                    Destroy(pEffector, effectorOnlineTime);

                    break;
            }

            // marcasmoe el accion como llevada a cabo
            habilityUsed = true;

        }


    }

    // funcion que se encargara de cambiar el estado del pajaro
    public void SetStatus (BirdStatus bStatus)
    {
        birdStatus = bStatus;
    }

    // para avanzar en la cola (solo se llamara cuando el pajaro actual haya muerto)
    public void AdvanceOnQueue()
    {
        // comprobaremos en que posicion esta actualmente en la cola
        positionOnQeue = positionOnQeue - 1;        // si estamos en segunda posicion iremos a primera..

        if (positionOnQeue == -1)
        {
            // lo marcamos como objeivo de la camara
            cameraController.SetTarget(this.gameObject);
            SetStatus(BirdStatus.onAir);
            rBody.isKinematic = false;
        }
        // si la nueva posicion en la cola es 0
        else if (positionOnQeue == 0)
        {

            // lo marcamos como objeivo de la camara
            cameraController.SetTarget(this.gameObject);
            // colocamos el objeto en posicion de lanzamiento
            transform.position = positions[positionOnQeue].transform.position;
            // activamos su estado de lanzamiento
            SetStatus(BirdStatus.onLaunch);
            underPlayerControl = true;

        // si estamos por detras del que esta para lanzar
        } else if (positionOnQeue >= 1 )
        {
            rBody.isKinematic = true;
            underPlayerControl = false;
            transform.position = positions[positionOnQeue].transform.position;
        } 

    }

    // comprovamos que nuestra velocidad no sea muy baja, si lo es y estamos en el estado adecuado nos acabamos destruyendo
    public void InactivityChecker ()
    {
        // si la velcoidad del objeto es menor a 0.1... y se trata del pajaro que estamos controlando
        if (rBody.velocity.magnitude <= 0.1f && underPlayerControl == true && birdStatus == BirdStatus.onAir)
        {
            currentTime += Time.deltaTime;
            //Debug.Log(rBody.velocity.magnitude);
            //Debug.LogWarning("ctime: " + currentTime + "tbd: " + timeBeforeDestruction);

            // en el momento que llevemos mas tiempo del marcado sin movernos nos destruiremos
            if (currentTime > timeBeforeDestruction)
            {
                KillBird();
            }

        }
        else
        {
            currentTime = 0f;
        }
    }

    // aquello que succedera cuando arrastremos el raton sobre el objeto
    // LA POSICION 0 DE POSICINES ES LA POSICION DE LANZAMIENTO (SIEMPRE)
    private void OnMouseDrag()
    {
        if (underPlayerControl && birdStatus == BirdStatus.onLaunch)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mouseWordlPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // calculamos la distancia desde donde esta ahora mismo el raton hasta la posicion de lanzamiento
            float distanceToLaunch =  Mathf.Abs( new Vector2(positions[0].transform.position.x - mouseWordlPosition.x, positions[0].transform.position.y - mouseWordlPosition.y).magnitude);

            // moveremos el pajaro de sitio si el calculo de la distancia entre el raton y el punto de lanzamiento es menor a x
            if (distanceToLaunch <= maxDistanceForLaunch)
            {
                draggingBird = true;
                // arrastramos al pajaro con el raton mientras cargamos el disparo
                transform.position = new Vector2(mouseWordlPosition.x, mouseWordlPosition.y);

                // vector de lanzamiento que aplicaremos desde el centro del objeto
                launchVector = positions[0].transform.position - mouseWordlPosition;
            } else
            {
                draggingBird = false;
                // recolocamos el pajaro en el centro
                transform.position = positions[0].transform.position;
                // resetamos el vector de lanzamiento
                launchVector = new Vector2();
            }

            Debug.DrawLine(positions[0].transform.position, mouseWordlPosition,Color.red);
            Debug.Log("Estas arrastrando el pajaro " + transform.name);
        }
    }



    private void OnMouseUp()
    {
        if(underPlayerControl && birdStatus == BirdStatus.onLaunch && draggingBird == true)
        {
            Debug.Log("Has soltado el boton del raton");
            // pasmos al siguiente estado
            AdvanceOnQueue();
            rBody.AddForce(launchVector * 10, ForceMode2D.Impulse);

            // generamos el sonido del pajaro
            GameObject crySpeaker = Instantiate(speakerPrefab);
            //crySpeaker.transform.position = transform.position;
            crySpeaker.GetComponent<AudioEffectController>().PlayAudioclip(birdLaunchSFX,this.gameObject);

            // generamos el sonido de la catapulta
            GameObject launchSpeaker = Instantiate(speakerPrefab);
            launchSpeaker.transform.position = transform.position;
            launchSpeaker.GetComponent<AudioEffectController>().PlayAudioclip(launcherSound);

        }   
    }


    // comprobamos la colision con el resto de elementos y los bordes 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Block")
        {
            // DANI BUSCA OTRA MANERA DE CARGAR ESTA FUNCION YA QUE ESTO PUEDE CARGAR DEMASIADO LA MAQUINA
            collision.gameObject.GetComponent<BlockController>().CheckImpactDamage(collision.relativeVelocity.magnitude);
        } else if (collision.transform.tag == "Pig")
        {
            collision.gameObject.GetComponent<PigController>().CheckImpactDamage(collision.relativeVelocity.magnitude);

        }

        // si se trata de uno de los limites del escenario
        if (collision.transform.tag == "LevelLimit")
        {
            KillBird();
        }

    }

    // aquello que succede cuando el pajaro muere
    public void KillBird()
    {
        // generamos el evento de sonido
        GameObject speaker =   Instantiate(speakerPrefab);
        speaker.GetComponent<AudioEffectController>().PlayAudioclip(deathSFX);

        // generamos la explosion
        // instanciamos el efecto visual
        GameObject animationContainer = Instantiate(animationPlayerPrefab);
        animationContainer.transform.position = transform.position;
        animationContainer.GetComponent<VisualEffectControler>().setAnimatorController(smokeExplosionAnimatorController, animationName);

        gameManager.MoveBirdsOnePosition(/* this.gameObject */ );
        Destroy(this.gameObject);
    }


    public bool MyTimer (float time)
    {
        currentEffectorTime += Time.deltaTime;

        if (currentEffectorTime >= time)
        {
            return false;           // si nos hemos pasado del tiempo devolvermos falso
        } else
        {
            return true;
        }
    }

}
