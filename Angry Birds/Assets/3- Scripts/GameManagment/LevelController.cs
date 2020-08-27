using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

    public int currentLevelIndex;       // escena actual


    // actioners
    // cambiamos de escena SI SE PUEDE
    public void NextScene()
    {
        try
        {
            SceneManager.LoadScene(currentLevelIndex + 1);
        } catch
        {
            Debug.LogError("ESCENA INEXISTENTE");
        }
    }

	// Use this for initialization
	void Start () {

        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

	}
	
    
}
