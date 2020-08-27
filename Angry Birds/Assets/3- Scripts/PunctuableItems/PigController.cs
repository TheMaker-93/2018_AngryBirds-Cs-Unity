using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(DistanceJoint2D))]

public class PigController : PunctuableItem {

    [Header("pig SFX")]
    public AudioClip pìgDeathSound;
    public RuntimeAnimatorController smokeExplosionAnimatorController;
    public string animationName = "Smoke_A_0";

    [Header("Dangling Physics")]
    private DistanceJoint2D distanceJoint;
    private LineRenderer lineRenderer;
    public bool dangling;           // es un cerdo colgante o no?

    // Use this for initialization
    public override void Start () {
        base.Start();

        distanceJoint = GetComponent<DistanceJoint2D>();
        lineRenderer = GetComponent<LineRenderer>();

        if (dangling == true)
        {

            distanceJoint.enabled = true;
            lineRenderer.enabled = true;
        }
        else
        {
            distanceJoint.enabled = false;
            lineRenderer.enabled = false;
        }

    }

    // Update is called once per frame
    void Update () {
   
        if (dangling)
        {
            // buscamos el punto en el mundo en el cual el distanceJoint se ancla
            Vector3 positionOnWordlSpace = transform.TransformPoint(distanceJoint.connectedAnchor);

            // seteamos los puntos del line renderer
            lineRenderer.GetPosition(0).Set(transform.position.x, transform.position.y, transform.position.z);
            lineRenderer.GetPosition(1).Set(positionOnWordlSpace.x, positionOnWordlSpace.y, positionOnWordlSpace.z);
        }

	}

    // DESDE EL PADRE LLAMAMOS A ESTA FUNCION
    public override void Death()
    {
        // instanciamos el altavoz y le pasamos nuestro sonido
        GameObject speaker = Instantiate(speakerPrefab);
        speaker.transform.position = transform.position;
        speaker.GetComponent<AudioEffectController>().PlayAudioclip(pìgDeathSound);

        // instanciamos el efecto visual
        GameObject animationContainer = Instantiate(animationPlayerPrefab);
        animationContainer.transform.position = transform.position;
        animationContainer.GetComponent<VisualEffectControler>().setAnimatorController(smokeExplosionAnimatorController, animationName);

        base.Death();

    }
}
