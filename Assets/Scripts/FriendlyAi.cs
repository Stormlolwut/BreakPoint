using UnityEngine;
using System.Collections.Generic;

public class FriendlyAi : MonoBehaviour, IUpgradeable {
    #region classVariables
    private enum FriendlyState { Searching, Chasing, Attacking }
    FriendlyState friendlyState;

    [SerializeField]
    private float m_attackCoolSet;
    private float m_attackCool;
    private float m_closePos = Mathf.Infinity;

    [SerializeField]
    private int m_damageDealth;

    private GameObject m_curEnemy;
    private GameObject m_overlord;

    [SerializeField]
    private AudioClip[] m_audioArray;
    [SerializeField]
    private AudioSource m_audiosource;

    private NavMeshAgent m_navAgent;
    private Overlord m_overlordScript;
    private List<GameObject> m_destructEnemies;
    private List<GameObject> m_enemiesInRange = new List<GameObject>();

    private Animator m_animator;
    private IDamagable enemyDamagable = null;
    private Vector3 m_originalPos;

    private int m_upgradeCost;
    private int m_timesUpgraded;

    #endregion

    void OnEnable() {

        m_navAgent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
        m_overlord = GameObject.FindGameObjectWithTag("OVERLORD");
        m_attackCool = m_attackCoolSet;
        m_originalPos = transform.position;

    }

    void TheFriendlyState() {
        switch (friendlyState) {
            case FriendlyState.Searching:

                ObjectDistance tower = GetClosestObject(m_enemiesInRange, m_curEnemy);
                m_curEnemy = tower.Object;

                break;
            case FriendlyState.Chasing:
                MoveToTarget();
                break;

            case FriendlyState.Attacking:
                AttackingTarget();
                break;
            default:
                break;
        }
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == 8)
            m_enemiesInRange.Add(col.gameObject);
    }
    void OnTriggerExit(Collider col) {
        if (col.gameObject.layer == 8)
            m_enemiesInRange.Remove(col.gameObject);
    }

    void MoveToTarget() {

        float enemiedistance = Mathf.Infinity;

        if (m_enemiesInRange.Count > 0 && m_curEnemy != null) {

            m_animator.SetBool("isWalking", true);
            enemiedistance = Vector3.Distance(transform.position, m_curEnemy.transform.position);
            m_navAgent.SetDestination(m_curEnemy.transform.position);
            m_curEnemy.GetComponent<AIMovement>().m_curTower = gameObject;
            enemyDamagable = m_curEnemy.GetComponent<IDamagable>();

        }
        else {

            m_animator.SetBool("isWalking", false);
            friendlyState = FriendlyState.Searching;
        }

        if (enemiedistance < 2.5f)
            friendlyState = FriendlyState.Attacking;
    }

    void AttackingTarget() {

        m_animator.SetBool("isWalking", false);

        if (m_curEnemy != null) {

            transform.LookAt(m_curEnemy.transform.position);
            m_attackCool -= Time.deltaTime;

            if (enemyDamagable.GetHitPoints() <= 0) {

                m_enemiesInRange.Remove(m_curEnemy);
                m_attackCool = m_attackCoolSet;
                enemyDamagable = null;
                m_curEnemy = null;
                friendlyState = FriendlyState.Searching;

            }

            if (m_attackCool < 0) {

                m_animator.SetTrigger("isTriggerPunch");
                enemyDamagable.TakeDamage(m_damageDealth);
                m_attackCool = m_attackCoolSet;

            }
        }
    }

    ObjectDistance GetClosestObject(List<GameObject> yourlist, GameObject yourobject) {

        float closestDisSqr = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject potentialObject in yourlist) {

            if (potentialObject != null) {

                Vector3 directionToTarget = potentialObject.transform.position - currentPos;
                float sqrRootToTarget = directionToTarget.sqrMagnitude;

                if (sqrRootToTarget < closestDisSqr) {
                    closestDisSqr = sqrRootToTarget;
                    yourobject = potentialObject;
                }
            }
        }
        friendlyState = FriendlyState.Chasing;
        return new ObjectDistance(yourobject, closestDisSqr);
    }

    private class ObjectDistance {
        private GameObject m_obj;
        private float m_dist;
        public GameObject Object { get { return m_obj; } }
        public float Distance { get { return m_dist; } }
        public ObjectDistance(GameObject obj, float distance) {
            m_obj = obj;
            m_dist = distance;
        }
    }

    void Update() {
        TheFriendlyState();
    }
    public int UpgradeObject(int money) {
        return money;
    }
    public int UpgradeCost() {
        return m_upgradeCost;
    }
    public int UpgradeDamage() {
        return m_damageDealth;
    }
    public int TimesUpgraded() {
        return m_timesUpgraded;
    }
    void CopyList(ref List<GameObject> list1, List<GameObject> list2) {
        list1 = new List<GameObject>();
        for (int i = 0; i < list2.Count; i++) {
            list1.Add(list2[i]);
        }

    }
}
/*
void OnTriggerStay(Collider col) {
    if (col.gameObject.layer == 8) {
        CopyList(ref m_destructEnemies, m_overlord.GetComponent<Overlord>().DestructEnemies);
        if (m_curEnemy == null) {
            m_closePos = Mathf.Infinity;
            if (m_destructEnemies != null) {
                for (int i = 0; i < m_destructEnemies.Count; i++) {
                    if (m_destructEnemies[i] != null) {
                        if (m_closePos > Vector3.Distance(transform.position, m_destructEnemies[i].transform.position)) {
                            m_closePos = Vector3.Distance(transform.position, m_destructEnemies[i].transform.position);
                            m_curEnemy = m_destructEnemies[i];
                            m_curEnemy.GetComponent<AIMovement>().m_curTower = gameObject;
                            enemyDamagable = m_destructEnemies[i].GetComponent<IDamagable>();
                        }
                    }
                }
            }
        }
    }
}
void FightingEnemy() {
    if (m_curEnemy != null && m_navAgent.isActiveAndEnabled == true) {
        m_animator.SetBool("isWalking", true);
        m_navAgent.destination = m_curEnemy.transform.position;
        transform.LookAt(m_curEnemy.transform.position, Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, m_curEnemy.transform.position - transform.position, out hit, 1 << 8)) {
            Debug.DrawRay(transform.position, m_curEnemy.transform.position - transform.position, Color.red);
            if (hit.collider.gameObject == m_curEnemy) {
                m_animator.SetBool("isWalking", false);
                if (m_attackCool > 0) { m_attackCool -= Time.deltaTime; }
                else if (m_attackCool <= 0) {
                    m_animator.SetTrigger("isTriggerPunch");
                    m_audiosource.clip = m_audioArray[0];
                    m_audiosource.Play();
                    enemyDamagable.TakeDamage(m_damageDealth);
                    m_attackCool = m_attackCoolSet;
                }
                if (enemyDamagable.GetHitPoints() <= 0 && m_curEnemy != null && m_curEnemy.GetComponent<EnemyHealth>().m_health <= 0) {
                    m_attackCool = m_attackCoolSet;
                    enemyDamagable = null;
                    m_curEnemy = null;
                    m_navAgent.SetDestination(m_originalPos);
                }
            }
        }
    }
}
public int UpgradeObject(int money) {
    return money;
}
public int UpgradeCost() {
    return m_upgradeCost;
}
public int UpgradeDamage() {
    return m_damageDealth;
}
public int TimesUpgraded() {
    return m_timesUpgraded;
}
void CopyList(ref List<GameObject> list1, List<GameObject> list2) {
    list1 = new List<GameObject>();
    for (int i = 0; i < list2.Count; i++) {
        list1.Add(list2[i]);
    }

}
}
*/
