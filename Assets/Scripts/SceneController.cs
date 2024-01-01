using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCOntroller : MonoBehaviour
{
    // Start is called before the first frame update
   public void SwitchScences(string sceneName){
        SceneManager.LoadScene(sceneName);
    }
}
