using UnityEngine;
using System.Collections;

public class StartMenu : MonoBehaviour {

    Vector3 m_beginPos;
    [SerializeField]
    float m_yDistance;
    float m_diffrenceMeter;

    bool m_UpDown;

    // Use this for initialization
    void Start() {
        m_beginPos = transform.position;
    }

    // Update is called once per frame
    void Update() {

        m_diffrenceMeter = (m_yDistance - transform.position.y);
        Debug.Log(m_diffrenceMeter);

        if (m_diffrenceMeter > 0 && m_UpDown) {
            Debug.Log("Up");
            transform.Translate(0, 0.0008f, 0);
        }
        else if(m_diffrenceMeter < 1)
            m_UpDown = false;

        if (m_diffrenceMeter < 1 && !m_UpDown) {
            Debug.Log("Down");
            transform.Translate(0, -0.0008f, 0);
        }
        else if (m_diffrenceMeter > 0)
            m_UpDown = true;
    }
}
