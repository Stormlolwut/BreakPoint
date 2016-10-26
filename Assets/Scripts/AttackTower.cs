using UnityEngine;
using System.Collections.Generic;

public class AttackTower : MonoBehaviour, IUpgradeable {
    #region classVariables
    enum TowerState { Searching, Attacking }
    TowerState towerState;


    [SerializeField]
    private float m_splashRadius;
    [SerializeField]
    private int m_splashDamage;

    [SerializeField]
    private float m_attackCoolSet;
    private float m_attackCool;

    private float m_closePos = Mathf.Infinity;

    [SerializeField]
    private int m_damageDealth;
    private int m_timesUpgraded = 1;
    private int m_upgradeCost;

    [SerializeField]
    private AudioClip[] m_audioArray;
    [SerializeField]
    private AudioSource m_audiosource;

    [SerializeField]
    private bool m_doesSplashDamage;

    private GameObject m_curEnemy;
    private GameObject m_overlord;

    private Overlord m_overlordScript;
    private List<GameObject> m_destructEnemies;

    public List<GameObject> m_enemiesInRange = new List<GameObject>();

    private Animator m_animator;
    private IDamagable enemyDamagable = null;

    private LayerMask m_enemyMask = 1 << 8;


    #endregion

    void OnEnable() {

        m_animator = GetComponent<Animator>();
        m_overlord = GameObject.FindGameObjectWithTag("OVERLORD");

        m_attackCool = m_attackCoolSet;
        m_audiosource.clip = m_audioArray[0];

    }

    void Update() {
        TheTowerState();
    }

    void TheTowerState() {
        switch (towerState) {
            case TowerState.Searching:

                if (m_enemiesInRange.Count > 0)
                    FindNextTarget();
                break;
            case TowerState.Attacking:
                AttackingTarget();
                break;
            default:
                towerState = TowerState.Searching;
                break;
        }
    }


    void FindNextTarget() {

        float closestDisSqr = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject potentialEnemy in m_enemiesInRange) {

            Vector3 directionToTarget = potentialEnemy.transform.position - currentPos;
            float sqrRootToTarget = directionToTarget.sqrMagnitude;

            if (sqrRootToTarget < closestDisSqr) {
                closestDisSqr = sqrRootToTarget;
                m_curEnemy = potentialEnemy;
                Debug.Log(m_curEnemy);
            }
        }
        enemyDamagable = m_curEnemy.GetComponent<IDamagable>();
        towerState = TowerState.Attacking;
    }

    void AttackingTarget() {

        Vector3 LookAtPosition = new Vector3(m_curEnemy.transform.position.x, transform.position.y, m_curEnemy.transform.position.z);
        transform.LookAt(LookAtPosition, Vector3.up);

        if (m_curEnemy != null) {

            m_attackCool -= Time.deltaTime;

            if (enemyDamagable.GetHitPoints() <= 0) {

                //m_destructEnemies.Remove(m_curEnemy);
                m_enemiesInRange.Remove(m_curEnemy);
                m_curEnemy = null;
                m_attackCool = m_attackCoolSet;
                towerState = TowerState.Searching;

            }

            if (m_attackCool < 0) {

                enemyDamagable.TakeDamage(m_damageDealth);
                m_audiosource.clip = m_audioArray[0];
                m_audiosource.Play();
                m_animator.SetTrigger("isFireing");
                m_attackCool = m_attackCoolSet;

                Collider[] col = Physics.OverlapSphere(m_curEnemy.transform.position, m_splashRadius, m_enemyMask);
                Debug.Log(col.Length);

                foreach(Collider enemies in col) {
                    enemies.GetComponent<IDamagable>().TakeDamage(m_splashDamage);
                }
            }
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


    public int UpgradeObject(int money) {

        float upgradeHealth = GetComponent<TowerHealth>().m_setHealth / 4 * m_timesUpgraded;
        GetComponentInChildren<SphereCollider>().radius += 0.5f;
        m_upgradeCost = GetComponent<TowerHealth>().m_upgradeValue * m_timesUpgraded;

        m_timesUpgraded++;

        if (m_attackCool > 0)
            m_attackCoolSet -= 0.02f;

        if (m_splashRadius < 1)
            m_splashRadius += 0.02f;

        m_splashDamage += 1;
        m_damageDealth += 1;

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

//if (col.gameObject.layer == 8) {
//            CopyList(ref m_destructEnemies, m_overlord.GetComponent<Overlord>().DestructEnemies);
//            if (m_curEnemy == null) {
//                m_closePos = Mathf.Infinity;
//                if (m_destructEnemies != null) {
//                    for (int i = 0; i<m_destructEnemies.Count; i++) {
//                        if (m_closePos > Vector3.Distance(transform.position, m_destructEnemies[i].transform.position)) {
//                            m_closePos = Vector3.Distance(transform.position, m_destructEnemies[i].transform.position);
//                            m_curEnemy = m_destructEnemies[i];
//                            m_enemyDamagable = m_destructEnemies[i].GetComponent<IDamagable>();
//                        }
//                    }
//                }
//            }
//        }
//    }
//    void Update() {
//    if (m_curEnemy != null) {
//        AttackEnemy();
//    }
//}
//void AttackEnemy() {
//    transform.LookAt(new Vector3(m_curEnemy.transform.position.x, transform.position.y, m_curEnemy.transform.position.z), Vector3.up);
//    if (Physics.Raycast(transform.position, m_curEnemy.transform.position - transform.position)) {
//        Debug.DrawRay(transform.position, m_curEnemy.transform.position - transform.position, Color.red);
//        if (m_attackCool > 0) { m_attackCool -= Time.deltaTime; }
//        if (m_curEnemy != null && (m_enemyDamagable.GetHitPoints() <= 0)) {
//            m_enemyDamagable = null;
//            m_curEnemy = null;
//            m_attackCool = m_attackCoolSet;
//        }
//        else if (m_attackCool <= 0) {
//            m_enemyDamagable.TakeDamage(m_damageDealth);
//            m_audiosource.clip = m_audioArray[0];
//            m_audiosource.Play();
//            m_animator.SetTrigger("isFireing");
//            m_attackCool = m_attackCoolSet;
//            if (m_doesSplashDamage) {
//                for (int i = 0; i < m_destructEnemies.Count; i++) {
//                    if (Vector3.Distance(m_curEnemy.transform.position, m_destructEnemies[i].transform.position) < m_splashDamageRange) {
//                        m_destructEnemies[i].GetComponent<IDamagable>().TakeDamage(m_splashDamageDealth);
//                    }
//                }
//            }
//        }
//    }