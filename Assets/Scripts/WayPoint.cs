using UnityEngine;
using System.Collections.Generic;

public class WayPoint : MonoBehaviour {

    [SerializeField]private GameObject m_overlord;

    private List<GameObject> m_destructEnemies;
    [SerializeField]private List<GameObject> m_destructTowers = new List<GameObject>();
    
    public List<GameObject> GetTowerList { get { return m_destructTowers; } set { m_destructTowers = value; } }

    void Update() {
        CopyList(ref m_destructEnemies, m_overlord.GetComponent<Overlord>().DestructEnemies);

    }
    void CopyList(ref List<GameObject> list1, List<GameObject> list2) {
        list1 = new List<GameObject>();
        for (int i = 0; i < list2.Count; i++) {
            list1.Add(list2[i]);
        }
    }
}
