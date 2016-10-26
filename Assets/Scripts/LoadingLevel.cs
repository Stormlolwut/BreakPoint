using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingLevel : MonoBehaviour {
    public void StartLevel() {
        SceneManager.LoadScene("Scene01");
    }
    public void QuitGame() {
        Application.Quit();
    }
}
