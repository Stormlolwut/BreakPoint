using UnityEngine;
using System.Collections;

public class MusicLord : MonoBehaviour {

    [SerializeField]
    AudioClip[] m_music;
    [SerializeField]
    AudioSource m_musicSource;
    [SerializeField]GameObject m_overlord;
    private Overlord m_overlordscript;
	// Use this for initialization
	void Start () {
        m_overlord = GameObject.FindGameObjectWithTag("OVERLORD");
        m_overlordscript = m_overlord.GetComponent<Overlord>();

    }
	
	// Update is called once per frame
	void Update () {

        if (m_overlordscript.m_waveProgress && m_musicSource.clip != m_music[0]) {
            m_musicSource.Stop();
            m_musicSource.clip = m_music[0];
            m_musicSource.Play();
        }

        if (!m_overlordscript.m_waveProgress && m_musicSource.clip != m_music[1]) {
            m_musicSource.Stop();
            m_musicSource.clip = m_music[1];
            m_musicSource.Play();
        }
    }
}
