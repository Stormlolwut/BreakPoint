using UnityEngine;
using System.Collections;

public class FriendlyHealth : MonoBehaviour, IDamagable, ISellable, ISelectable {

    [SerializeField]
    private float m_setHealth;
    private float m_health;
    [SerializeField]
    public int m_objValue;
    [SerializeField]
    private GameObject m_overlord;
    private GameObject m_closestWaypoint = null;
    private GameObject[] m_waypoints;
    [SerializeField]
    private Overlord m_overlordScript;
    private Animator m_animator;

    private bool m_bodyDecay;
    private float m_bodyTimer = 7;

    private float m_closestDistance = Mathf.Infinity;

    public bool m_isSelected = false;

    void OnEnable() {
        m_overlord = GameObject.FindGameObjectWithTag("OVERLORD");
        m_overlordScript = m_overlord.gameObject.GetComponent<Overlord>();
        m_overlordScript.DestructTowers.Add(gameObject);
        m_animator = GetComponent<Animator>();
        //m_waypoints = m_overlord.GetComponent<Overlord>().WayPoints;
        m_health = m_setHealth;
        //AddToWayPoint();
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
        else if (!m_isSelected && GetComponentInChildren<MeshRenderer>()) {
            BuildCircleMesh bm = GetComponentInChildren<BuildCircleMesh>();
            m_isSelected = false;
            bm.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    public bool IsSelected(bool Choice) {
        m_isSelected = Choice;
        return Choice;
    }
    public void TakeDamage(int damage) {
        if (m_health > 0) {
            m_health -= damage;
        }
    }
    public float GetHitPoints() {
        if (m_health <= 0) {
            m_closestWaypoint.GetComponent<WayPoint>().GetTowerList.Remove(gameObject);
            m_overlordScript.DestructTowers.Remove(gameObject);
            m_animator.SetTrigger("isDead");
            gameObject.GetComponent<FriendlyAi>().enabled = false;
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
            m_bodyDecay = true;
            StartCoroutine(RemoveBody());
        }
        return m_health;
    }
    private IEnumerator RemoveBody() {
        while (m_bodyDecay) {
            if (m_bodyTimer > 0) {
                m_bodyTimer -= Time.deltaTime;
            }
            else if (m_bodyTimer <= 0) {
                m_bodyDecay = false;
                m_overlordScript.m_money += m_objValue;
                gameObject.GetComponent<FriendlyAi>().enabled = false;
                Destroy(gameObject);
            }
            yield return new WaitForEndOfFrame();
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
    public GameObject GetGameObject() {
        return gameObject;
    }
}
