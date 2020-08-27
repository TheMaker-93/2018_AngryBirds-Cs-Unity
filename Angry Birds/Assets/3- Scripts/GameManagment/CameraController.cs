using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public bool follow;                         // seguir o no seguir
    public Vector3 initialPosition;             // posicion inicial
    public Transform target;                    // transformada del objeto a seguir

    // limites a partir de los caules nos moveremos
    public float leftCameraLimit = 0.2f;
    public float rightCameraLimit = 0.8f;
    public float topCameraLimit = 0.8f;
    public float bottomCameraLimit = 0.1f;
    public float speed;

	// Use this for initialization
	void Start () {
        initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
	}
	

	// Update is called once per frame
	void Update () {
		
        // si estamos en modo de seguimiento
        if (follow && target != null)
        {
            // si salimos por el lado derecho
            if (Camera.main.WorldToViewportPoint(target.transform.position).x > rightCameraLimit)
            {              
                transform.position = new Vector3(
                    Mathf.Lerp(transform.position.x, target.position.x, speed * Time.deltaTime), 
                    transform.position.y,
                    transform.position.z);
            }

            // si nos salimos (vertical)
            if (Camera.main.WorldToViewportPoint(target.transform.position).y > topCameraLimit ||
                Camera.main.WorldToViewportPoint(target.transform.position).y < bottomCameraLimit)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    Mathf.Lerp(transform.position.y, target.position.y, speed * Time.deltaTime),
                    transform.position.z);
            }

        }
	}

    // funcion que usaremos para cambiar el ojeto a seguir
    public void SetTarget(GameObject tgt)
    {
        // cambiamos el target
        target = tgt.transform;
        // volvemos a la posicion inicial
        transform.position = initialPosition;
    }

}
