using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ConnectionLoader : MonoBehaviour
{
    public InputField userInput;
    public static string ip = null;

    public void OnLoadMovementScene() {
        ip = userInput.text;

        SceneManager.LoadScene("JakobScene");
    }

    public void OnLoadMessageScene() {
        ip = userInput.text;

        SceneManager.LoadScene("Marshaling Test");
    }


}
