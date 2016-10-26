using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class Overlord : MonoBehaviour {

    [SerializeField]private List<GameObject> m_destructTowers = new List<GameObject>();
    public List<GameObject> DestructTowers
    {
        get { return m_destructTowers; }
        set { m_destructTowers = value; }
    }
    [SerializeField]private List<GameObject> m_destructEnemies = new List<GameObject>();
    public List<GameObject> DestructEnemies
    {
        get { return m_destructEnemies; }
        set { m_destructEnemies = value; }
    }
    public bool m_waveProgress;

    private int m_waveNumber;
    private int m_waveMoney = 200;
    private int m_enemyTickets;
    public int m_money;

    [SerializeField]
    private float m_waveTimerSet;
    private float m_waveTimer;

    [SerializeField]private Text m_waveInfo;
    [SerializeField]private Text m_timeOutTimer;
    [SerializeField]private Text m_moneyInfo;


    [SerializeField]private AudioClip[] m_audioClips;
    [SerializeField]private AudioSource m_audioSource;

    [SerializeField]private GameObject m_standardEnemy;
    [SerializeField]private GameObject m_strongEnemy;
    [SerializeField]private GameObject m_eliteEnemy;
    [SerializeField]private GameObject[] m_waypoints;
    public GameObject[] WayPoints {
        get { return m_waypoints; }
        set { m_waypoints = value; }
    }

    void Start() {
        m_waveTimer = m_waveTimerSet;
    }
	void Update () {
        EnemySpawner();
        WaveDurring();
        WaveTimeOut();
        int wavetimer = (int)m_waveTimer;
        m_timeOutTimer.text = "Time Left: " + wavetimer.ToString();
        m_moneyInfo.text = "Money: " + m_money.ToString();
    }
    void EnemySpawner() {
        if (m_waveProgress && m_enemyTickets > 24) {
            Instantiate(m_eliteEnemy, new Vector3(Random.Range(-26.25f, -26.25f), -4.94f, -20.83f), new Quaternion(0, 0, 0, 0));
        }
            if (m_waveProgress && m_enemyTickets > 17) {
            Instantiate(m_strongEnemy, new Vector3(Random.Range(-26.25f, -26.25f), -4.94f, -20.83f), new Quaternion(0, 0, 0, 0));
        }
        if (m_waveProgress && m_enemyTickets > 0) {
            Instantiate(m_standardEnemy, new Vector3(Random.Range(-26.25f, -26.25f), -4.94f, -20.83f), new Quaternion(0, 0, 0, 0));
            m_enemyTickets--;
        }
    }
    public void WaveStart() {
        m_audioSource.clip = m_audioClips[0];
        m_audioSource.Play();
        m_waveTimer = m_waveTimerSet;
        m_waveNumber++;
        m_waveInfo.text = "WAVE: " + m_waveNumber.ToString();
        m_enemyTickets = m_waveNumber * 5;
        m_waveProgress = true;
    }
    public void WaveEnd() {
        m_audioSource.clip = m_audioClips[2];
        m_audioSource.Play();
        m_waveProgress = false;
        m_destructEnemies.Clear();
        m_waveInfo.text = "WAVE: " + m_waveNumber.ToString();
        m_money += m_waveMoney * m_waveNumber;
    }    
    public void StopGame() {
        SceneManager.LoadScene("scene 2");
    }
    public void RestartGame() {
        SceneManager.LoadScene("Scene01");
    }
    void WaveDurring() {
        if(m_waveProgress && m_destructEnemies.Count == 0) {
            WaveEnd();
        }
    }
    void WaveTimeOut() {
        if(m_waveTimer > 0 && m_waveProgress == false) {
            m_waveTimer -= Time.deltaTime;
            
        }
        else if (m_waveTimer <= 0 && m_waveProgress == false){
            WaveStart();
        }
    }
}
