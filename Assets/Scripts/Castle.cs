using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Castle : MonoBehaviour, IDamagable {

    private float m_health;
    private int m_maxHealth;
    private int m_upgrade;

    [SerializeField]
    private GameObject m_overlord;
    private Overlord m_overlordScript;

    private GameObject[] m_waypoints;
    private GameObject closestWaypoint = null;

    [SerializeField]private GameObject m_wavemenu;
    [SerializeField]private GameObject m_buildmenu;
    [SerializeField]private GameObject m_gameovermenu;


    void Start() {
        m_health = m_maxHealth = 1000;
        m_overlordScript = m_overlord.gameObject.GetComponent<Overlord>();
        m_waypoints = m_overlord.GetComponent<Overlord>().WayPoints;
        AddToWayPoint();
    }

    void Update() {
        m_upgrade = m_maxHealth / 100 * m_upgrade;
        m_maxHealth = m_maxHealth + m_upgrade;
    }

    public void TakeDamage(int damage) {
        if (m_health > 0) {
            m_health -= damage;
        }
    }

    void AddToWayPoint() {
        float closestDistance = Mathf.Infinity;
        Vector3 ts = transform.position;
        for (int i = 0; i < m_waypoints.Length; i++) {
            float distance = Vector3.Distance(ts, m_waypoints[i].transform.position);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestWaypoint = m_waypoints[i];
            }
        }
        closestWaypoint.GetComponent<WayPoint>().GetTowerList.Add(gameObject);
        m_overlordScript.DestructTowers.Add(gameObject);
    }

    public float GetHitPoints() {
        if (m_health <= 0) {
            gameObject.SetActive(false);
            m_wavemenu.SetActive(false);
            m_buildmenu.SetActive(false);
            m_gameovermenu.SetActive(true);
        }
        return m_health;
    }
    public GameObject GetGameObject() {
        return gameObject;
    }
}
