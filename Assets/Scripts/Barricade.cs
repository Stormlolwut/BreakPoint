using UnityEngine;
using System.Collections;

public class Barricade : MonoBehaviour, IDamagable, IUpgradeable, ISellable, ISelectable {


    [SerializeField]
    private int m_objValue;
    private float m_health;
    private int m_upgradeValue;
    [SerializeField]private int m_setHealth;
    private int m_upgradedHealth;
    private int m_timesUpgraded;
    private int m_upgradeCost;

    private GameObject[] m_waypoints;
    private GameObject m_closestWaypoint = null;
    private float m_closestDistance = Mathf.Infinity;

    [SerializeField]
    private GameObject m_overlord;
    private Overlord m_overlordscript;

    private bool m_isSelected = false;

    void OnEnable() {
        m_overlord = GameObject.FindGameObjectWithTag("OVERLORD");
        m_overlordscript = m_overlord.gameObject.GetComponent<Overlord>();
        m_overlordscript.DestructTowers.Add(gameObject);
        m_waypoints = m_overlord.GetComponent<Overlord>().WayPoints;
        AddToWayPoint();
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
        if (m_closestDistance < 20) {
            m_closestWaypoint.GetComponent<WayPoint>().GetTowerList.Add(gameObject);
            m_overlordscript.DestructTowers.Add(gameObject);
        }
    }
    void Start() {
        m_health = 200;
        m_setHealth = 200;
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
        else if (!m_isSelected && GetComponentInChildren<MeshRenderer>()) {
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
    public float GetHitPoints() {
        if (m_health <= 0) {
            gameObject.SetActive(false);
            m_overlordscript.DestructTowers.Remove(gameObject);
            m_closestWaypoint.GetComponent<WayPoint>().GetTowerList.Remove(gameObject);
        }
        return m_health;
    }
    public GameObject GetGameObject() {
        return gameObject;
    }
    public int SellObject(int money) {
        if (m_objValue >= money) {
            gameObject.SetActive(false);
            m_overlordscript.DestructTowers.Remove(gameObject);
            m_closestWaypoint.GetComponent<WayPoint>().GetTowerList.Remove(gameObject);
        }
        return m_objValue;
    }
    public int BuyObject(int money) {
        money = m_objValue;
        return money;
    }
    public int UpgradeObject(int money) {
        m_upgradeValue = m_objValue * m_timesUpgraded / 2; 
        money = m_upgradeValue * m_timesUpgraded;
        m_upgradeCost = money;
        m_timesUpgraded++;
        m_upgradedHealth = m_setHealth / 100 * m_timesUpgraded;
        m_setHealth = m_setHealth + m_upgradedHealth;
        m_health = m_setHealth;
        return money;
    }
    public int TimesUpgraded() {
        return m_timesUpgraded;
    }
    public int UpgradeCost() {
        return m_upgradeCost;
    }
    public bool IsSelected(bool choice) {
        m_isSelected = choice;
        return choice;
    }
    public int UpgradeDamage() {
        return 0;
    }
}
