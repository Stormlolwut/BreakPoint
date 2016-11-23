using UnityEngine;
using System.Collections.Generic;

public class FirstPerson : MonoBehaviour
{

    [SerializeField]
    private int m_damageUp;
    private float m_closePos;
    [SerializeField]
    private GameObject m_overlord;
    [SerializeField]
    private GameObject m_wavemenu;
    [SerializeField]
    private GameObject m_buildmenu;
    [SerializeField]
    private GameObject m_firstmenu;

    private LayerMask m_enemyLayer = 1 << 9;
    private Vector3 m_originalPos;
    private Quaternion m_originalRot;
    private List<GameObject> m_destructEnemies;
    private GameObject m_curEnemy;
    private IDamagable m_enemyDamagable;

    public bool m_infirstPerson;
    private bool m_inTopVieuw;

    void Start()
    {
        m_originalPos = transform.position;
        m_originalRot = transform.rotation;
    }
    void Update()
    {
        SelectTower();
        FirstPersonVieuw();
        if (m_curEnemy != null)
            transform.LookAt(m_curEnemy.transform.position);
        CopyList(ref m_destructEnemies, m_overlord.GetComponent<Overlord>().DestructEnemies);
    }
    void SelectTower()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#if UNITY_ANDROID
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#endif
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Spawnable" && Input.GetMouseButtonDown(0) && m_overlord.GetComponent<Overlord>().m_waveProgress == true)
        {
            transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y + 15, hit.transform.position.z);
            m_infirstPerson = true;
            m_inTopVieuw = false;
        }
        if (m_overlord.GetComponent<Overlord>().m_waveProgress == false)
        {
            m_infirstPerson = false;
        }
    }
    void FirstPersonVieuw()
    {
        if (m_infirstPerson && !m_inTopVieuw)
        {
            m_wavemenu.SetActive(false);
            m_buildmenu.SetActive(false);
            m_firstmenu.SetActive(true);
            if (m_curEnemy == null)
            {
                m_closePos = Mathf.Infinity;
                for (int i = 0; i < m_destructEnemies.Count; i++)
                {
                    if (m_destructEnemies[i] != null && m_closePos > Vector3.Distance(transform.position, m_destructEnemies[i].transform.position))
                    {
                        m_closePos = Vector3.Distance(transform.position, m_destructEnemies[i].transform.position);
                        m_curEnemy = m_destructEnemies[i];
                    }
                }
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#if UNITY_ANDROID
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#endif
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, m_enemyLayer) && hit.collider.gameObject.layer == 8 && Input.GetMouseButtonDown(0))
            {
                hit.collider.GetComponent<IDamagable>().GetHitPoints();
                hit.collider.GetComponent<IDamagable>().TakeDamage(m_damageUp);
            }
        }
        if (!m_infirstPerson && !m_inTopVieuw)
        {
            m_curEnemy = null;
            m_firstmenu.SetActive(false);
            m_wavemenu.SetActive(true);
            m_buildmenu.SetActive(true);
            m_inTopVieuw = true;
            transform.position = m_originalPos;
            transform.rotation = m_originalRot;
        }
    }
    public void GoTopVieuw()
    {
        m_curEnemy = null;
        m_firstmenu.SetActive(false);
        m_wavemenu.SetActive(true);
        m_buildmenu.SetActive(true);
        m_inTopVieuw = true;
        transform.position = m_originalPos;
        transform.rotation = m_originalRot;
    }
    void CopyList(ref List<GameObject> list1, List<GameObject> list2)
    {
        list1 = new List<GameObject>();
        for (int i = 0; i < list2.Count; i++)
        {
            list1.Add(list2[i]);
        }
    }
}
