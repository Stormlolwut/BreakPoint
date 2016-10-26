using UnityEngine;
using System.Collections;

public class StayInArena : MonoBehaviour {

    private GameObject m_camera;
    private Vector3 m_basePos;

    void OnTriggerExit(Collider col) {
        if (col.tag == "MainCamera")
            m_camera.transform.position = m_basePos;
    }
    void OnTriggerEnter(Collider col) {
        if (col.tag == "MainCamera") {
            m_camera = col.gameObject;
            m_basePos = m_camera.transform.position;
        }
    }
}
