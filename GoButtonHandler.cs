using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoButtonHandler : MonoBehaviour
{


    public void OnClickEvent()
    {
        SceneManager.LoadScene("Scenes/mainScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
