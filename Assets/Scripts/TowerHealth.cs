using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerHealth : MonoBehaviour, IDamagable, ISellable, ISelectable {

    [SerializeField]
    public float m_setHealth;
    [SerializeField]
    private int m_objValue;
    [SerializeField]
    public int m_upgradeValue;

    private float m_health;

    private GameObject[] m_waypoints;

    private GameObject m_closestWaypoint = null;
    private GameObject m_overlord;

    private Overlord m_overlordScript;

    private float m_closestDistance = Mathf.Infinity;
    public bool m_isSelected = false;

    void OnEnable() {

        m_overlord = GameObject.FindGameObjectWithTag("OVERLORD");
        m_overlordScript = m_overlord.gameObject.GetComponent<Overlord>();
        m_waypoints = m_overlord.GetComponent<Overlord>().WayPoints;

        AddToWayPoint();
        m_health = m_setHealth;
    }
    void AddToWayPoint() {
        Vector3 ts = transform.position;
        for (int i = 0; i < m_waypoints.Length; i++) {

            float distance = Vector3.Distance(ts, m_waypoints[i].transform.position);

            if (distance < m_closestDistance) {
                m_closestDistance = distance;
                m_closestWaypoint = m_waypoints[i];
            }
        }
        if(m_closestDistance < 20) {
            m_closestWaypoint.GetComponent<WayPoint>().GetTowerList.Add(gameObject);
            m_overlordScript.DestructTowers.Add(gameObject);
        }
    }
    void Update() {
            HealthCircle();
    }
    void HealthCircle() {
        if (m_isSelected) {
            BuildCircleMesh bm = GetComponentInChildren<BuildCircleMesh>();
            bm.GetComponent<MeshRenderer>().enabled = true;
            float hp = (m_health / m_setHealth * 100f);
            bm.endAngle = hp / 100 * 360;
        }
        else if (!m_isSelected  && GetComponentInChildren<MeshRenderer>()){
            BuildCircleMesh bm = GetComponentInChildren<BuildCircleMesh>();
            m_isSelected = false;
            bm.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    public void TakeDamage(int damage) {
        if (m_health > 0) {
            m_health -= damage;
        }
    }
    public int SellObject(int money) {
        if (m_objValue >= money) {
            gameObject.SetActive(false);
            m_overlordScript.DestructTowers.Remove(gameObject);
            m_closestWaypoint.GetComponent<WayPoint>().GetTowerList.Remove(gameObject);
        }
        return m_objValue;
    }
    public int BuyObject(int money) {
        money = m_objValue;
        return money;
    }
    public float GetHitPoints() {
        if (m_health <= 0) {
            m_overlordScript.DestructTowers.Remove(gameObject);
            m_closestWaypoint.GetComponent<WayPoint>().GetTowerList.Remove(gameObject);
            gameObject.SetActive(false);
        }
        return m_health;
    }
    public bool IsSelected(bool Choice) {
        m_isSelected = Choice;
        return Choice;
    }
    public GameObject GetGameObject() {
        return gameObject;
    }
}
