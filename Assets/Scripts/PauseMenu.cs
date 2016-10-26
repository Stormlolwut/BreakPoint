using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {
    private bool m_isPause = true;
    [SerializeField]private GameObject m_pauseMenu;
    [SerializeField]private GameObject m_waveMenu;
    [SerializeField]private GameObject m_buildMenu;

    public void OnButtonPress() {
        if (m_isPause) {
            m_isPause = false;
            m_pauseMenu.SetActive(true);
            m_waveMenu.SetActive(false);
            m_buildMenu.SetActive(false);
            Time.timeScale = 0;
        }
        else if (!m_isPause) {
            m_isPause = true;
            m_pauseMenu.SetActive(false);
            m_waveMenu.SetActive(true);
            Time.timeScale = 1;
        }    
    }
}
