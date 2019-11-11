using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SceneManagement;

public class StartManager : MonoBehaviour
{
    public Text ipText;

    public void SelectFPS() {
        ScenePresent.Instance.IP = ipText.text;
        ScenePresent.Instance.SwapScene(3);
    }
    public void SelectRTS() {
        ScenePresent.Instance.IP = ipText.text;
        ScenePresent.Instance.SwapScene(2);
    }
}
