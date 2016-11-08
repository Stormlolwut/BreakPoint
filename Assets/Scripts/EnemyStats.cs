using UnityEngine;
using System.Collections;


[System.Serializable]
public class EnemyStats : MonoBehaviour{

    public float m_health;
    public float m_speed;
    private float m_coolDown; 
    public float CoolDown { get { return m_coolDown; } set { m_coolDown = value; } }
    public float m_coolDownStart;
    public float m_strenght;

}
