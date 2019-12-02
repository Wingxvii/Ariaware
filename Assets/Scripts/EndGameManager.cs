using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SceneManagement;

public class EndGameManager : MonoBehaviour
{
    public Text EndgameUI;
    public Animation fireMissle;


    private void Start()
    {
        if (ScenePresent.Instance.gameState == 0)
        {
            EndgameUI.text = "ERROR";
        }
        else if (ScenePresent.Instance.gameState == 1) {
            Debug.Log("win");
            EndgameUI.text = "YOU WIN!";
            fireMissle.Play("Missle");
        }
        else if (ScenePresent.Instance.gameState == 2)
        {
            Debug.Log("Lose");
            EndgameUI.text = "YOU LOSE!";
        }
        else if (ScenePresent.Instance.gameState == 3)
        {
            Debug.Log("win");
            EndgameUI.text = "YOU WIN!";
        }
        else if (ScenePresent.Instance.gameState == 4)
        {
            Debug.Log("Lose");
            EndgameUI.text = "YOU LOSE!";
            fireMissle.Play("Missle");
        }

    }
}
