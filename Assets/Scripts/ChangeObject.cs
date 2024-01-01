using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;


public class ChangeObject : MonoBehaviour
{

    [SerializeField] GameObject plainFinder;
    private ContentPositioningBehaviour script;
   
    
    public void onDisplay(string text)

    {
      
        script = plainFinder.GetComponent<ContentPositioningBehaviour>();

        if (text.ToLower().CompareTo("apple") == 1 || text.ToLower().Remove(text.Length - 1).CompareTo("apple") == 1)
        {
            script.AnchorStage = GameObject.FindGameObjectWithTag("apple").GetComponent<AnchorBehaviour>();
        }
        if (text.ToLower() == "banana" || text.ToLower().Remove(text.Length - 1) == "banana")
        {
            script.AnchorStage = GameObject.FindGameObjectWithTag("banana").GetComponent<AnchorBehaviour>();
        }
    }
}
