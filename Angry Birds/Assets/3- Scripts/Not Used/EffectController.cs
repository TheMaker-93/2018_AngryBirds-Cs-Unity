using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour {

    [Header("Sound")]
    private AudioSource audioSource;     // audiosource del objeto ASIGNADO DESDE EL INSPECTOR para no empeorar el rendimiento
    public AudioClip audioClip;

    [Header("Image")]
    private Animator animator;
    public Animation animationClip;


    // setters
    public void SetAudioClip( AudioClip audioEffect)
    {
        audioClip = audioEffect;
    }
    // le pondremos un u otro controller para poder ejecutar su animacion
    public void SetAnimationController(Animation animation)
    {
        animationClip = animation;
    }

	// Use this for initialization
	void Start () {

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // si hay audioclip cargado
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        // si hay clip de animacion
        if (animationClip != null)
        {
            animator.Play(0);       // llamamos al animacion primera del controller
        }

        // destrumios el objeto despues del audio 
        Destroy(this.gameObject, audioClip.length);     // destruiremos este objeto despues que el audio haya acabado

	}



}
