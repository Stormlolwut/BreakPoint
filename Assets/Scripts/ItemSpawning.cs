using UnityEngine;
using UnityEngine.UI;

public class ItemSpawning : MonoBehaviour {

    [SerializeField]private GameObject m_canon;
    [SerializeField]private GameObject m_balista;
    [SerializeField]private GameObject m_catepult;
    [SerializeField]private GameObject m_barricade;
    [SerializeField]private GameObject m_soldier;
    [SerializeField]private GameObject m_overlord;

    private GameObject m_placeObject;
    private GameObject m_upgradeObject;
    private GameObject m_prevSelectedObj;
    [SerializeField]private GameObject m_menuhidden;
    private Overlord m_overlordScript;

    private bool m_toggleMenu;

    [SerializeField]
    private Text m_upgradeinfo;
    [SerializeField]
    private Text m_damageInfo;
    [SerializeField]
    private Text m_health;

    [SerializeField]
    private AudioClip[] m_audioClips;
    [SerializeField]
    private AudioSource m_audioSource;

    private LayerMask m_uiLayer = 1 << 5;

    private Vector3 m_beginPos;
    private Vector3 m_endPos;

    private int m_value;
    private int m_TimesUpgraded;
    private float m_infoHealth;
    private int m_infoMoney;
    private int m_infoDamage;

    void Start() {
        m_overlordScript = m_overlord.GetComponent<Overlord>();
        m_beginPos = m_menuhidden.transform.position;
}
    void Update() {
        PlaceObject();
        LerpingMenu();
    }
    public void PlaceCannon() {
        m_placeObject = m_canon;
    }
    public void PlaceBalista() {
        m_placeObject = m_balista;
    }
    public void PlaceCatepult() {
        m_placeObject = m_catepult;
    }
    public void PlaceBarricade() {
        m_placeObject = m_barricade;
    }
    public void PlaceSoldier() {
        m_placeObject = m_soldier;
    }
    public void UpgradeObject() {
        Debug.Log(m_upgradeObject);
        if (m_upgradeObject != null) {
            Debug.Log("Upgrading");
            int tempUpgrade = m_upgradeObject.GetComponent<IUpgradeable>().UpgradeObject(m_value);
            if (tempUpgrade <= m_overlordScript.m_money) {
                m_value = tempUpgrade;
                m_overlordScript.m_money -= m_value;
                m_infoHealth = m_upgradeObject.GetComponent<IDamagable>().GetHitPoints();
                m_infoMoney = m_upgradeObject.GetComponent<IUpgradeable>().UpgradeCost();
                m_infoDamage = m_upgradeObject.GetComponent<IUpgradeable>().UpgradeDamage();
                m_upgradeinfo.text = "Cost: " + m_infoMoney.ToString();
                m_damageInfo.text = "Damage: " + m_infoDamage.ToString();
                m_health.text = "Health: " + m_infoHealth.ToString();
            }
        }
    }
    void PlaceObject() {
#if UNITY_EDITOR
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#elif UNITY_ANDROID
        Debug.Log("Hallo");
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#endif
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            if (m_overlordScript.m_waveProgress == false) {
                Debug.DrawRay(transform.position, hit.point, Color.yellow);
                if (hit.collider != null && Input.GetMouseButtonDown(0)) {
                    if ((m_placeObject != null && m_placeObject != m_soldier) && hit.collider.tag == "Road" && m_placeObject.GetComponent<ISellable>().BuyObject(m_value) <= m_overlordScript.m_money) {
                        int tempPlaceObject = m_placeObject.GetComponent<ISellable>().BuyObject(m_value);
                        m_value = tempPlaceObject;
                        m_overlordScript.m_money -= m_value;
                        Vector3 m_objectPos = new Vector3(Mathf.Round(hit.point.x), hit.point.y + 0.3f, Mathf.Round(hit.point.z));
                        Instantiate(m_placeObject, m_objectPos, m_placeObject.transform.rotation);
                        m_audioSource.clip = m_audioClips[1];
                        m_audioSource.Play();
                        m_placeObject = null;
                    }
                    if (hit.collider.tag == "Obstacle") {
                        m_prevSelectedObj = m_upgradeObject = hit.collider.gameObject;
                        m_upgradeObject.GetComponent<ISelectable>().IsSelected(true);
                        m_infoHealth = m_upgradeObject.GetComponent<IDamagable>().GetHitPoints();
                        m_infoMoney = m_upgradeObject.GetComponent<IUpgradeable>().UpgradeCost();
                        m_infoDamage = m_upgradeObject.GetComponent<IUpgradeable>().TimesUpgraded();
                        m_upgradeinfo.text = "Cost: " + m_infoMoney.ToString();
                        m_damageInfo.text = "Damage: " + m_infoDamage.ToString();
                        m_health.text = "Health: " + m_infoHealth.ToString();
                    }
                    else if (m_prevSelectedObj != null && hit.collider.tag != "Obstacle") {
                        m_prevSelectedObj.GetComponent<ISelectable>().IsSelected(false);
                        m_prevSelectedObj = null;
                        m_upgradeinfo.text = "";
                        m_damageInfo.text = "";
                        m_health.text = "";
                    }
                }
                if (Input.GetMouseButtonDown(1) && hit.collider.tag == "Obstacle") {
                    m_overlord.GetComponent<Overlord>().DestructTowers.Remove(hit.collider.gameObject);
                    Destroy(hit.collider.gameObject);
                    m_TimesUpgraded = hit.collider.GetComponent<IUpgradeable>().TimesUpgraded();
                    m_value = hit.collider.GetComponent<ISellable>().SellObject(m_value);
                    m_overlordScript.m_money += m_value * m_TimesUpgraded / 2;
                }
            }
        }
        if (hit.collider != null && m_placeObject == m_soldier && Input.GetMouseButtonDown(0)) {
            if (hit.collider.tag == "Road" && m_placeObject.GetComponent<ISellable>().BuyObject(m_value) <= m_overlordScript.m_money) {
                int tempPlaceObject = m_placeObject.GetComponent<ISellable>().BuyObject(m_value);
                m_value = tempPlaceObject;
                m_overlordScript.m_money -= m_value;
                Vector3 m_objectPos = new Vector3(Mathf.Round(hit.point.x), hit.point.y + 0.3f, Mathf.Round(hit.point.z));
                Instantiate(m_placeObject, m_objectPos, m_placeObject.transform.rotation);
                m_audioSource.clip = m_audioClips[0];
                m_audioSource.Play();
                m_placeObject = null;
            }
        }
    }
    public void HideAndShowMenu() {
        if (m_toggleMenu) {
            m_toggleMenu = false;
           // m_menuhidden.SetActive(false);
        }
        else if (!m_toggleMenu) {
            m_toggleMenu = true;
            m_menuhidden.SetActive(true);
        }
    }
    public void LerpingMenu() {
        m_endPos = m_menuhidden.transform.position + new Vector3(50, 0, 0);
        if (m_toggleMenu) {
            m_menuhidden.transform.position = Vector3.Lerp(m_beginPos, m_endPos, 1f);
        }
        else if (!m_toggleMenu) {
            m_menuhidden.transform.position = m_beginPos;
        }
    }
}
