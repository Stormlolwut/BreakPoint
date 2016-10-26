using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IDamagable {

    [SerializeField]public float m_health = 100;
    [SerializeField]public int m_objValue;
    [SerializeField]private GameObject m_overlord;
    [SerializeField]private Overlord m_overlordScript;
    private Animator m_animator;

    private bool m_bodyDecay;
    private float m_bodyTimer = 7;


    void OnEnable() {
        m_overlord = GameObject.FindGameObjectWithTag("OVERLORD");
        m_overlordScript = m_overlord.gameObject.GetComponent<Overlord>();
        m_overlordScript.DestructEnemies.Add(gameObject);
        m_animator = GetComponent<Animator>();
    }
    public void TakeDamage(int damage) {
        if (m_health > 0) {
            m_health -= damage;
        }
    }
    public float GetHitPoints() {
        if (m_health <= 0) {
            m_overlordScript.DestructEnemies.Remove(gameObject);
            m_animator.SetTrigger("isDead");
            gameObject.GetComponent<AIMovement>().enabled = false;
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
                Destroy(gameObject);
            }
            yield return new WaitForEndOfFrame();
        }
    }
    public GameObject GetGameObject() {
        return gameObject;
    }
}
