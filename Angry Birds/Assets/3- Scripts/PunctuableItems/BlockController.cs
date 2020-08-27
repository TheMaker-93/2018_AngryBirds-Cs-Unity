using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]

// hijo de punctuable item para poder trabajar con los puntos que nos puede dar
public class BlockController : PunctuableItem {

    [Header("Block Destruction SFX")]
    public AudioClip destructionClip;
    public RuntimeAnimatorController smokeExplosionAnimatorController;
    public string animationName = "Smoke_A_0";

    public enum Material
    {
        wood,stone,glass,
    }
    [Header("Material Atributes")]
    public Material material; 

    // Use this for initialization
    public override void Start  () {

        Debug.Log("Hemos ejecutado el start de pI desde BC");
        base.Start();

    }

    public override void Death()
    {
        // instanciamos el altavoz y le pasamos nuestro sonido
        GameObject speaker = Instantiate(speakerPrefab);
        speaker.transform.position = transform.position;
        speaker.GetComponent<AudioEffectController>().PlayAudioclip(destructionClip);

        // instanciamos el efecto visual
        GameObject animationContainer = Instantiate(animationPlayerPrefab);
        animationContainer.transform.position = transform.position;
        animationContainer.GetComponent<VisualEffectControler>().setAnimatorController(smokeExplosionAnimatorController, animationName);

        base.Death();
    }
}
