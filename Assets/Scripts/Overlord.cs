using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class Overlord : MonoBehaviour {
    #region classVariables

    public bool m_waveProgress;
    private int m_waveNumber;
    private int m_waveMoney = 200;
    private int m_enemyTickets;
    private int m_timesEnemyUpgraded;
    public int m_money;

    [SerializeField]
    private float m_waveTimerSet;
    private float m_waveTimer;

    [Header("Classes")]
    public EnemyTickets[] m_enemyTicket;
    public EnemyStats m_enemyStats;

    [Header("Lists and Arrays")]
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
    [SerializeField]private GameObject[] m_waypoints;

    [Header("Text info Prefabs")]
    [SerializeField]private Text m_waveInfo;
    [SerializeField]private Text m_timeOutTimer;
    [SerializeField]private Text m_moneyInfo;
    [Header("Audio Clips")]
    [SerializeField]private AudioClip[] m_audioClips;
    [SerializeField]private AudioSource m_audioSource;
    [Header("Enemy Prefabs")]
    [SerializeField]private GameObject m_standardEnemy;
    [SerializeField]private GameObject m_strongEnemy;
    [SerializeField]private GameObject m_eliteEnemy;

    public GameObject[] WayPoints {
        get { return m_waypoints; }
        set { m_waypoints = value; }
    }
    #endregion
    void Start() {
        m_waveTimer = m_waveTimerSet;

        Debug.Log(Mathf.Log(1f, 2));
    }
	void Update () {

        WaveDurring();
        WaveTimeOut();

        int wavetimer = (int)m_waveTimer;

        m_timeOutTimer.text = "Time Left: " + wavetimer.ToString();
        m_moneyInfo.text = "Money: " + m_money.ToString();
    }
    void EnemySpawner() {

        Vector3 spawnrange = new Vector3(Random.Range(-26.25f, -26.25f), -4.94f, -20.83f);
        Quaternion spawnrotation = new Quaternion(0, 0, 0, 0);

        for (int i = m_enemyTicket.Length - 1; i > 0; i--) {
            if(m_enemyTicket[i].m_ticketCosts < m_enemyTickets && m_destructEnemies.Count <= 20) {
                m_enemyTickets -= m_enemyTicket[i].m_ticketCosts;
                GameObject clone = Instantiate(m_enemyTicket[i].m_spawnEnemy, spawnrange, spawnrotation) as GameObject;
                clone.name = m_enemyTicket[i].m_name;
                //clone.AddComponent<EnemyStats>();
            }
            else {
                UpgradeEnemy(m_enemyTickets);
            }
        }
    }

    private void UpgradeEnemy(int tickets) {
        for (int i = 0; i < m_destructEnemies.Count; i++) {
            float timesupgraded = 1.0f / m_timesEnemyUpgraded;
            m_timesEnemyUpgraded++;
            EnemyStats enemystats = m_destructEnemies[i].GetComponent<EnemyStats>();
     //       enemystats.m_health += m_destructEnemies[i].GetComponent<EnemyStats>().m_health m_waveNumber * (0.03 );
            enemystats.m_coolDownStart -= m_destructEnemies[i].GetComponent<EnemyStats>().m_coolDownStart * timesupgraded;
            enemystats.m_strenght += m_destructEnemies[i].GetComponent<EnemyStats>().m_strenght * timesupgraded;
            enemystats.m_speed += m_destructEnemies[i].GetComponent<EnemyStats>().m_speed * timesupgraded;
        }
    }

    public void WaveStart() {

        m_audioSource.clip = m_audioClips[0];
        m_audioSource.Play();
        m_waveTimer = m_waveTimerSet;
        EnemySpawner();
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
