using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpController : MonoBehaviour {

    private Vector3 startingPosition;
    public float speed;
    public float maxAltitude;           // maxima altura relativa a la posicion inicial

    public void SetStartingPosition (Vector3 strPosition)
    {
        startingPosition = strPosition;
    }

	// Use this for initialization
	void Start () {

        transform.position = startingPosition;
        
	}
	
	// Update is called once per frame
	void Update () {

        // augmentamos el altura del objeto
        transform.position = new Vector3(
            transform.position.x, 
            transform.position.y + speed * Time.deltaTime, 
            transform.position.z
            );

        // comprobamos que no haya pasado del margen
        if (transform.position.y > startingPosition.y + maxAltitude)
        {
            Destroy(this.gameObject);
        }

	}
}
