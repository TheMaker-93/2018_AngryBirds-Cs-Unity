using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]


public class ScriptReference : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

// hacemos una bola roja y le asignamos este script
public class BallController : MonoBehaviour
{

    public Rigidbody2D myRigidBody;               // asignamos este componente dedse el inspector
    public CapsuleCollider2D myCollider;          // mi collider
    public float force = 10f;

    private void Start()
    {
        
    }

    private void Update()
    {
        // despues de cargar el componente esto nos permiitira ver la velocidad del objeto (vector 2)
        Debug.Log(myRigidBody.velocity);

        // para saber la velocidad preguntamos por la magintud del vector 
        // el problema de esto es que es caro
        Debug.Log(myRigidBody.velocity.magnitude);

        // para comparar un vector con otro usamos el vector en la raiz cuadrada para ahorrarnos llevar a cabo el calculo de la razi cuadrada
        //Debug.Log(myRigidBody.velocity.SqrMagnitude);

        myRigidBody.isKinematic = true;         // a partir de este moemton es knimetico y poemos trabajar con su transformada

        // generamos una fuerza en una direccion
        myRigidBody.AddForce(Vector2.one);       

        myRigidBody.AddForce(Vector2.one * force, ForceMode2D.Impulse);     // impulso en un momento

        // para que se apliqie en su sistema de coordenadas y no en el del mudno:
        myRigidBody.AddRelativeForce(Vector2.one * force, ForceMode2D.Impulse);     // impulso en un momento

        // para aplicarle una fuerza en la posicion del objeto que quiera
        //myRigidBody.AddForceAtPosition(Vector2.one * force);     

        if (myRigidBody.velocity.magnitude <= 0.1f){
            // podriamos iniciar el timer de destruccion y preparar el nuevo pajaro
        }

    
        


    }


    // para detectar las colisiones de un rb cn otro rb:
    
    // cuando empieza la colision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // podemos acceder a la informacion del objeto con cuyo collider estamos colisionando
        //collision.transform.name;

        // velocidad relativa entre los dos objetos (diferencia de las velocidades entre los dos a la hora de impactar
        // a esta velocidad relativa tambien puedes preguntarle la magnitud
        //collision.relativeVelocity;
    }
    // cuando acaba
    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }
    // durante
    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }

    // cuando clicamos con el raton
    private void OnMouseUp()
    {
        myRigidBody.isKinematic = false;

        // podemos hacer un debug y preguntar la posicion dle mouuse
        Debug.Log(Input.mousePosition);

        Vector3 mouseCordinates = Input.mousePosition;

        // convertimos en coordenadas de patalla nuestra posicion
        Camera.main.WorldToScreenPoint(transform.position);

    }

    // para poder arrastar un objeto
   private void OnMouseDrag()
    {
        // guardamos la posicion del raton en screen y lo convertimos a mundo 
        // para hacer el vector de disparo del pajaro
        Vector3 mouseCordinates = Input.mousePosition;
        Vector3 ratonEnMundoCoordinates = Camera.main.ScreenToWorldPoint(mouseCordinates);

        // si no funciona bien y no se coloca como debe
        // habria que ponerlo en knematic para que funicione bien y la camara ha de estasr en ortografica
        myRigidBody.isKinematic = true;

        transform.position = new Vector3(ratonEnMundoCoordinates.x, ratonEnMundoCoordinates.y, transform.position.z);
        // lo hacemos de este modo para no perder nuestra posicion en la z

    }

    // cuando presionamos en el objeto es knematic
    private void OnMouseDown()
    {
        myRigidBody.isKinematic = true;
    }

    private void OnMouseExit()
    {
        
    }

    private void OnMouseEnter()
    {
        
    }
}


public class Hinges : MonoBehaviour
{
    // EL POINT EFFECTOR APLICA UN EFECTO DE ATRACCION O LO CONTRARIO A LOS RIGIDBODIES QUE ESTEN EN LA PERIFERI DE ESTE
    // ESTO SE PUEDE USAR PARA HACER LA EXPLOSION

    // el area effector nos permite generar una fuerza continua (campo de fuerza) 

    // PARA HACER LOS CERDOS COLGANDO DE UNA CUERDA
    // usamos el componene Joint (podriamos usar la distance Joint) esta nos pedire de donde quremos anclarnos y a que distancia de ste queremos estar
    // Otra opcion seria usar el sprint joint que añade un componente elastico a la junta



}