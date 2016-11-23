using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{

    private Vector3 m_moveDir;
    private Vector2 m_startPos;
    private Vector2 m_endPos;


    private float m_zoomSpeed = 0.5f;
    private float m_swipeStartTime;
    public float m_movementSpeed = 10f;

    private bool m_couldBeSwipe;


    void Update()
    {
        MoveCamera();
        //ZoomCamera();
        SwipeCamera();
    }
    void MoveCamera()
    {

        m_moveDir.x = Input.GetAxis("Horizontal");
        m_moveDir.y = Input.GetAxis("Vertical");
        m_moveDir = m_moveDir * m_movementSpeed * Time.deltaTime;
        transform.Translate(m_moveDir);

    }
    void SwipeCamera()
    {
        if (Input.touchCount == 1 && GetComponent<FirstPerson>().m_infirstPerson == false)
        {
            Touch touchZero = Input.GetTouch(0);
            Vector2 touchZeroPos = touchZero.deltaPosition;
            Vector2 touchNormalized = touchZeroPos.normalized;
            transform.Translate(touchNormalized);
        }
    }


    void ZoomCamera()
    {
#if UNITY_ANDROID
        if (Input.touchCount == 2 && GetComponent<FirstPerson>().m_infirstPerson == false) {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnituedeDiff = prevTouchDeltaMag - touchDeltaMag;

            Camera.main.fieldOfView += deltaMagnituedeDiff * m_zoomSpeed;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.transform.position.y, 0.1f, 179.9f);
#endif
    }
}

