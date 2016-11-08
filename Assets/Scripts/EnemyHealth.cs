using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour, IDamagable {

    //private float m_enemyStats.m_health;
    [SerializeField]
    public int m_objValue;
    [SerializeField]
    private GameObject m_overlord;
    [SerializeField]
    private Overlord m_overlordScript;
    private Animator m_animator;

    public EnemyStats m_enemyStats;

    private bool m_bodyDecay;
    private float m_bodyTimer = 7;

    private float m_hitTimer;

    private Color m_colorWhite;


    void OnEnable() {
        m_enemyStats = GetComponent<EnemyStats>();
       
        m_overlord = GameObject.FindGameObjectWithTag("OVERLORD");
        m_overlordScript = m_overlord.gameObject.GetComponent<Overlord>();
        m_overlordScript.DestructEnemies.Add(gameObject);
        m_animator = GetComponent<Animator>();
        Color colorwhite = Color.white;
    }

    void HitChangeMaterial() {

        Material[] colormat = gameObject.GetComponentsInChildren<Material>();
        List<Color> oldcolor = new List<Color>();
        Color colorwhite = Color.white;

        while (m_hitTimer > 0) {
            m_hitTimer -= Time.deltaTime;

        }

        if(m_hitTimer < 0) {
            foreach (Material mat in colormat) {
                m_hitTimer = 3;
                //mat.color = oldcolor[mat];
                oldcolor.Remove(mat.color);
                mat.color = colorwhite;
            }

        }
    }

    public void TakeDamage(int damage) {
        if (m_enemyStats.m_health > 0) {
            m_enemyStats.m_health -= damage;
        }
    }
    public float GetHitPoints() {
        if (m_enemyStats.m_health <= 0) {
            m_overlordScript.DestructEnemies.Remove(gameObject);
            m_animator.SetTrigger("isDead");
            gameObject.GetComponent<AIMovement>().enabled = false;
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
            m_bodyDecay = true;
            StartCoroutine(RemoveBody());
        }
        return m_enemyStats.m_health;
    }
    private IEnumerator RemoveBody() {
        while (m_bodyDecay) {
            if (m_bodyTimer > 0) {
                m_bodyTimer -= Time.deltaTime;
            }
            else if (m_bodyTimer <= 0) {
                m_bodyDecay = false;
                m_overlordScript.m_money += m_objValue;
                Destroy(gameObject);
            }
            yield return new WaitForEndOfFrame();
        }
    }
    public GameObject GetGameObject() {
        return gameObject;
    }
}
