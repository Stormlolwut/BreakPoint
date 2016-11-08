using UnityEngine;
using System.Collections.Generic;

public class AIMovement : MonoBehaviour {
    #region classVariables
    private enum AiState { Searching, Chasing, Attacking }
    AiState m_currentState;

    private List<GameObject> m_destructTowers;
    private List<GameObject> m_waypoints;

    private NavMeshAgent m_enemyAgent;
    private Animator m_animator;

    public EnemyStats m_enemyStats;

    IDamagable towerDamagable = null;

    public GameObject m_curTower;
    private GameObject m_curWayPoint;
    #endregion


    void OnEnable() {

        GameObject overlord = GameObject.FindGameObjectWithTag("OVERLORD");

        m_waypoints = new List<GameObject>(overlord.GetComponent<Overlord>().WayPoints);
        m_enemyAgent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();

        m_enemyStats = GetComponent<EnemyStats>();

        m_currentState = AiState.Searching;
    }

    void Update() {
        TheAiState();

        if(m_enemyAgent.speed != m_enemyStats.m_speed)
            m_enemyAgent.speed = m_enemyStats.m_speed;
    }

    void TheAiState() {

        switch (m_currentState) {
            case AiState.Searching:

                ObjectDistance waypoint = GetClosestObject(m_waypoints, m_curWayPoint);
                CopyList(ref m_destructTowers, waypoint.Object.GetComponent<WayPoint>().GetTowerList);
                ObjectDistance tower = GetClosestObject(m_destructTowers, m_curTower);

                m_curWayPoint = waypoint.Object;
                m_curTower = tower.Object;

                if (m_destructTowers.Count > 0)
                    towerDamagable = m_curTower.GetComponent<IDamagable>();

                m_currentState = AiState.Chasing;

                break;
            case AiState.Chasing:
                MoveToTarget();
                break;
            case AiState.Attacking:
                AttackingTarget();
                break;
        }
    }

    ObjectDistance GetClosestObject(List<GameObject> yourlist, GameObject yourobject) {

        float closestDisSqr = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject potentialObject in yourlist) {

            Vector3 directionToTarget = potentialObject.transform.position - currentPos;
            float sqrRootToTarget = directionToTarget.sqrMagnitude;

            if (sqrRootToTarget < closestDisSqr) {
                closestDisSqr = sqrRootToTarget;
                yourobject = potentialObject;
            }
        }
        return new ObjectDistance(yourobject, closestDisSqr);
    }


    void MoveToTarget() {

        float waypointdistance = Mathf.Infinity;
        float towerdistance = Mathf.Infinity;

        if (m_destructTowers.Count > 0) {

            towerdistance = Vector3.Distance(transform.position, m_curTower.transform.position);
            m_enemyAgent.SetDestination(m_curTower.transform.position);

        }
        else {

            ObjectDistance waypoint = GetClosestObject(m_waypoints, m_curWayPoint);
            m_curWayPoint = waypoint.Object;
            waypointdistance = Vector3.Distance(transform.position, m_curWayPoint.transform.position);
            m_enemyAgent.SetDestination(m_curWayPoint.transform.position);

        }

        m_animator.SetBool("isWalking", true);

        if (waypointdistance < 4f) {
            m_waypoints.Remove(m_curWayPoint);
            m_curWayPoint = null;
            m_currentState = AiState.Searching;
        }

        if (towerdistance < 2.5f)
            m_currentState = AiState.Attacking;

    }

    void AttackingTarget() {

        if (m_curTower != null) {

            m_animator.SetBool("isWalking", false);
            transform.LookAt(m_curTower.transform.position);
            m_enemyStats.CoolDown -= Time.deltaTime;

            if (towerDamagable.GetHitPoints() <= 0) {

                m_enemyStats.CoolDown = m_enemyStats.m_coolDownStart;
                towerDamagable = null;
                m_curTower = null;
                m_currentState = AiState.Searching;

            }

            if (m_enemyStats.CoolDown < 0) {

                m_animator.SetTrigger("isTriggerPunch");
                towerDamagable.TakeDamage((int)m_enemyStats.m_strenght);
                m_enemyStats.CoolDown = m_enemyStats.m_coolDownStart;

            }
        }
    }

    void CopyList(ref List<GameObject> list1, List<GameObject> list2) {
        list1 = new List<GameObject>();
        for (int i = 0; i < list2.Count; i++) {
            list1.Add(list2[i]);
        }
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
}


//Oude Code

/*void FindNextTarget() {

        CheckDistance(m_waypoints, m_curWayPoint);


        float closestDisSqr = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject potentialWayPoint in m_waypoints) {

            Vector3 directionToTarget = potentialWayPoint.transform.position - currentPos;
            float sqrRootToTarget = directionToTarget.sqrMagnitude;

            if (sqrRootToTarget < closestDisSqr) {
                closestDisSqr = sqrRootToTarget;
                m_curWayPoint = potentialWayPoint;
            }
        }

        CopyList(ref m_destructTowers, m_curWayPoint.GetComponent<WayPoint>().GetTowerList);
        closestDisSqr = Mathf.Infinity;

        foreach (GameObject potentialEnemy in m_destructTowers) {

            Vector3 directionToTarget = potentialEnemy.transform.position - currentPos;
            float sqrRootToTarget = directionToTarget.sqrMagnitude;

            if (sqrRootToTarget < closestDisSqr) {

                closestDisSqr = sqrRootToTarget;
                m_curTower = potentialEnemy;
                towerDamagable = m_curTower.GetComponent<IDamagable>();
            }
        }
        Debug.Log(m_curTower);
        Debug.Log(m_curWayPoint);
        m_currentState = AiState.Chasing;
    }
    */


/*
        Vector3 ts = transform.position;
        if (m_curWayPoint == null) {
            float closeWayPos = Mathf.Infinity;
            for (int i = 0; i < m_waypoints.Count; i++) {
                if (Vector3.Distance(ts, m_waypoints[i].transform.position) < closeWayPos) {
                    m_closeWayPos = closeWayPos = Vector3.Distance(ts, m_waypoints[i].transform.position);
                    m_curWayPoint = m_waypoints[i];
                }
            }
        }
        if (m_curWayPoint != null && m_curTower == null) {
            CopyList(ref m_destructTowers, m_curWayPoint.GetComponent<WayPoint>().GetTowerList);
            float closeObjPos = Mathf.Infinity;
            for (int i = 0; i < m_destructTowers.Count; i++) {
                if (closeObjPos > Vector3.Distance(ts, m_destructTowers[i].transform.position)) {
                    closeObjPos = Vector3.Distance(ts, m_destructTowers[i].transform.position);
                    m_curTower = m_destructTowers[i];
                    towerDamagable = m_curTower.GetComponent<IDamagable>();
                }
            }
        

    void CombatAttack() {
        if (m_curTower != null) {
            if (Vector3.Distance(transform.position, m_curTower.transform.position) < 4) {
                transform.LookAt(m_curTower.transform.position + new Vector3(0, 0.4f, 0));
            }
            else { transform.LookAt(m_curTower.transform.position); }
            m_enemyAgent.destination = m_curTower.transform.position;
            m_animator.SetBool("isWalking", true);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward * (m_enemyAgent.stoppingDistance), out hit, m_towerLayer)) {
                Debug.DrawRay(transform.position, transform.forward * (m_enemyAgent.stoppingDistance), Color.blue);
                if (hit.collider.gameObject == m_curTower && m_curTower != null) {

                    Debug.Log(hit.collider.gameObject.name);
                    m_animator.SetBool("isWalking", false);
                    if (m_attackCool > 0) {
                        m_attackCool -= Time.deltaTime;
                    }
                    if (m_attackCool <= 0) {
                        m_animator.SetTrigger("isTriggerPunch");
                        towerDamagable.TakeDamage(m_attackDamage);
                        m_attackCool = m_attackCoolSet;
                    }
                    if (m_curTower != null && towerDamagable.GetHitPoints() <= 0) {
                        m_attackCool = m_attackCoolSet;
                        towerDamagable = null;
                        m_curTower = null;
                    }
                }
            }
            if (m_curTower != null && !m_curTower.activeSelf) {
                m_attackCool = m_attackCoolSet;
                towerDamagable = null;
                m_curTower = null;
                CopyList(ref m_destructTowers, m_curWayPoint.GetComponent<WayPoint>().GetTowerList);
            }
        }
        if (m_curWayPoint != null && m_destructTowers.Count == 0) {
            CopyList(ref m_destructTowers, m_curWayPoint.GetComponent<WayPoint>().GetTowerList);
            m_enemyAgent.destination = m_curWayPoint.transform.position;
            m_animator.SetBool("isWalking", true);
            if (Vector3.Distance(transform.position, m_curWayPoint.transform.position) < 4) {
                transform.LookAt(m_curWayPoint.transform.position, Vector3.up);
            }
            m_closeWayPos = Vector3.Distance(transform.position, m_curWayPoint.transform.position);
            if (m_closeWayPos < 4f) {
                m_waypoints.Remove(m_curWayPoint);
                m_curWayPoint = null;
            }
        }
    }

    */
