using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class Overlord : MonoBehaviour {
    #region classVariables

    public bool m_waveProgress;
    private bool m_canUpgrade = true;
    private int m_waveNumber;
    private int m_waveMoney = 200;
    private int m_enemyTickets;
    private int m_timesEnemyUpgraded = 1;
    public int m_money;

    [SerializeField]
    private float m_waveTimerSet;
    private float m_waveTimer;

    [SerializeField]
    private GameObject m_buildMenu;
    [SerializeField]
    private GameObject m_waveMenu;
    [SerializeField]
    private GameObject m_toggleBuildButton;

    [Header("Classes")]
    public EnemyTickets[] m_enemyTicket;
    public EnemyStats m_enemyStats;

    [Header("Lists and Arrays")]
    [SerializeField]
    private List<GameObject> m_destructTowers = new List<GameObject>();
    public List<GameObject> DestructTowers
    {
        get { return m_destructTowers; }
        set { m_destructTowers = value; }
    }
    [SerializeField]
    private List<GameObject> m_destructEnemies = new List<GameObject>();
    public List<GameObject> DestructEnemies
    {
        get { return m_destructEnemies; }
        set { m_destructEnemies = value; }
    }
    [SerializeField]
    private GameObject[] m_waypoints;

    [Header("Text info Prefabs")]
    [SerializeField]
    private Text m_waveInfo;
    [SerializeField]
    private Text m_timeOutTimer;
    [SerializeField]
    private Text m_moneyInfo;
    [SerializeField]
    private Button m_nextWave;
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip[] m_audioClips;
    [SerializeField]
    private AudioSource m_audioSource;

    public GameObject[] WayPoints
    {
        get { return m_waypoints; }
        set { m_waypoints = value; }
    }
    #endregion
    void Start() {
        m_waveTimer = m_waveTimerSet;
    }
    void Update() {

        WaveDurring();
        WaveTimeOut();

        int wavetimer = (int)m_waveTimer;

        m_timeOutTimer.text = "Time Left: " + wavetimer.ToString();
        m_moneyInfo.text = "Money: " + m_money.ToString();
    }
    void EnemySpawner() {

        Vector3 spawnrange = new Vector3(Random.Range(-26.25f, -26.25f), -4.94f, -20.83f);
        Quaternion spawnrotation = new Quaternion(0, 0, 0, 0);

        for (int i = 0; i < m_enemyTickets; i++) {

            //Debug.Log("Enemy tickets: " + m_enemyTickets);

            foreach (EnemyTickets enemies in m_enemyTicket) {

                int maxenemies = m_destructEnemies.Count;

                if (enemies.m_ticketCosts < m_enemyTickets && maxenemies < 50) {

                    m_enemyTickets -= enemies.m_ticketCosts;
                    GameObject clone = Instantiate(enemies.m_spawnEnemy, spawnrange, spawnrotation) as GameObject;
                    clone.name = enemies.m_name;
                    m_canUpgrade = true;
                    //clone.AddComponent<EnemyStats>();
                }

            }
            if (!m_canUpgrade)
                UpgradeEnemy(m_enemyTickets);
        }
        m_canUpgrade = true;
    }




    private void UpgradeEnemy(int tickets) {
        for (int i = 0; i < m_destructEnemies.Count; i++) {

            float timesupgraded = Mathf.Clamp(m_enemyTickets, 1f, 100) + (m_waveNumber / m_timesEnemyUpgraded);
            Debug.Log("Times upgraded: " + timesupgraded);
            m_timesEnemyUpgraded++;
            EnemyStats enemystats = m_destructEnemies[i].GetComponent<EnemyStats>();

            float cooldownstat = enemystats.m_coolDownStart -= m_destructEnemies[i].GetComponent<EnemyStats>().m_coolDownStart;
            //enemystats.m_health += m_destructEnemies[i].GetComponent<EnemyStats>().m_health m_waveNumber * (0.03 );
            if (cooldownstat > 1)
                cooldownstat = cooldownstat * (timesupgraded / 0.5f);

            enemystats.m_strenght += m_destructEnemies[i].GetComponent<EnemyStats>().m_strenght * timesupgraded;
            enemystats.m_speed += m_destructEnemies[i].GetComponent<EnemyStats>().m_speed * timesupgraded;

        }
    }

    public void WaveStart() {
        m_buildMenu.SetActive(false);
        m_waveMenu.SetActive(true);
        m_toggleBuildButton.SetActive(false);
        m_nextWave.enabled = false;
        m_nextWave.GetComponent<Image>().color = Color.gray;
        m_audioSource.clip = m_audioClips[0];
        m_audioSource.Play();
        m_waveTimer = m_waveTimerSet;
        m_waveNumber++;
        m_waveInfo.text = "WAVE: " + m_waveNumber.ToString();
        m_enemyTickets = (10 * m_waveNumber);
        EnemySpawner();
        m_waveProgress = true;

    }
    public void WaveEnd() {
        m_buildMenu.SetActive(true);
        m_toggleBuildButton.SetActive(true);
        m_waveMenu.SetActive(false);
        m_nextWave.GetComponent<Image>().color = Color.white;
        m_nextWave.enabled = true;
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
        if (m_waveProgress && m_destructEnemies.Count == 0) {
            WaveEnd();
        }
    }
    void WaveTimeOut() {
        if (m_waveTimer > 0 && m_waveProgress == false) {
            m_waveTimer -= Time.deltaTime;

        }
        else if (m_waveTimer <= 0 && m_waveProgress == false) {
            WaveStart();
        }
    }
}
