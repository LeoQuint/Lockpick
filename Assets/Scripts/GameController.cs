using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Difficulty
{
    EASY,NORMAL,HARD,IMPOSSIBLE
}

public class GameController : MonoBehaviour {

    public static GameController instance;

    public Difficulty m_Difficulty;

    ///References:
    public Slider _TenstionSlider;
    public AudioClip[] soundFXs;
    public Image _UITimercenter;
    public Image _UITimerFill;
    public GameObject _UILives;
    
    public GameObject _Pick;
    private GameObject m_LockHolder;
    private List<TumblerPin> m_Cylinders = new List<TumblerPin>();
    private AudioSource m_Aud;


    /// <summary>
    /// Designed Variables
    /// </summary>
    //Easy, normal, hard, impossible values
    [SerializeField]
    float[] m_Diff_TensionRange = new float[4]{ 8f, 4f, 2f, 1f };
    //Easy, normal, hard, impossible values
    [SerializeField]
    float[] m_Diff_Tension = new float[4]{ 1f, 2f, 4f, 8f };
    [SerializeField]
    Vector4 m_Pick_Bounds = new Vector4();
    [SerializeField]
    float m_Pick_MovementSpeed = 0.5f;
    [SerializeField]
    float m_LowChance = 0.5f;
    [SerializeField]
    float m_HighChance = 5f;
    [SerializeField]
    float m_ChanceRate = 0.5f;
    [SerializeField]
    float m_BaseTime = 60f;

    private float m_StartTime = 0f;
    private float m_Timer = 0f;
    private float m_MouseInput = 0f;
    private bool m_Lifting = false;
    private float m_AppliedTension = 0f;
    private bool m_IsGameOver = false;
    private float m_CurrentTension = 0f;
    private float m_ChanceTimer = 0f;
    private int m_Lives = 5;

    void Awake()
    {
        breakageTimer = Time.time + timeBetweenBreakage;
        m_BaseTime /= m_Diff_Tension[(int)m_Difficulty];
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        m_Aud = GetComponent<AudioSource>();
#if UNITY_STANDALONE
        Cursor.visible = false;
#endif
    }

	// Use this for initialization
	void Start () {

        switch (m_Difficulty)
        {
            case Difficulty.EASY:
                m_Lives = 5;
                break;
            case Difficulty.NORMAL:
                m_Lives = 3;
                _UILives.transform.FindChild("4").gameObject.SetActive(false);
                _UILives.transform.FindChild("5").gameObject.SetActive(false);
                break;
            case Difficulty.HARD:
                m_Lives = 2;
                _UILives.transform.FindChild("3").gameObject.SetActive(false);
                _UILives.transform.FindChild("4").gameObject.SetActive(false);
                _UILives.transform.FindChild("5").gameObject.SetActive(false);
                break;
            case Difficulty.IMPOSSIBLE:
                m_Lives = 1;
                _UILives.transform.FindChild("2").gameObject.SetActive(false);
                _UILives.transform.FindChild("3").gameObject.SetActive(false);
                _UILives.transform.FindChild("4").gameObject.SetActive(false);
                _UILives.transform.FindChild("5").gameObject.SetActive(false);
                break;
        }

        _UITimercenter.fillAmount = 0f;
        m_StartTime = Time.time;
        m_Timer = Time.time + m_BaseTime;
        m_LockHolder = GameObject.FindGameObjectWithTag("Lock");
        foreach (Transform child in m_LockHolder.transform)
        {
            if (child.GetComponent<TumblerPin>())
            {
                m_Cylinders.Add(child.GetComponent<TumblerPin>());
            }
        }
        //TODO, add extra layer of difficulty.
        //float sliderSize = _TenstionSlider.GetComponent<RectTransform>().sizeDelta.y;
        //_TenstionSlider.transform.FindChild("break").GetComponent<RectTransform>().sizeDelta =  new Vector2(1f, -(sliderSize - (sliderSize / 10f * m_Diff_TensionRange[(int)m_Difficulty])));
        //_TenstionSlider.transform.FindChild("high").GetComponent<RectTransform>().sizeDelta =   new Vector2(1f, -(sliderSize - (sliderSize / 10f * m_Diff_TensionRange[(int)m_Difficulty])));
        //_TenstionSlider.transform.FindChild("low").GetComponent<RectTransform>().sizeDelta =    new Vector2(1f, -(sliderSize - (sliderSize / 10f * m_Diff_TensionRange[(int)m_Difficulty])));
        //_TenstionSlider.transform.FindChild("none").GetComponent<RectTransform>().sizeDelta =   new Vector2(1f, -(sliderSize - (sliderSize / 10f * m_Diff_TensionRange[(int)m_Difficulty]))); 
    }
	
	// Update is called once per frame
	void Update () {
        if (m_IsGameOver)
        {
            return;
        }
        PlayerInput();
        Timer();
        CheckGameOver();
       
	}

    void FixedUpdate()
    {
        if (m_IsGameOver)
        {
            return;
        }
        CalculateTension();
        CheckTension();
        MovePick();
    }

    void PlayerInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            m_AppliedTension = m_Diff_Tension[(int)m_Difficulty]/8f;
        }
        else
        {
            m_AppliedTension = -m_Diff_Tension[(int)m_Difficulty]/8f;
        }
        //Get mouse movement.
        m_MouseInput = Input.GetAxis("Mouse Y");
       
        if (Input.GetMouseButton(0))
        {
            m_Lifting = true;
        }
        else
        {
            m_Lifting = false;
        }        
       

    }

    void MovePick()
    {
        _Pick.transform.Translate((m_Lifting?_Pick.transform.up * m_MouseInput : _Pick.transform.forward * -m_MouseInput)  * Time.deltaTime * m_Pick_MovementSpeed);
        _Pick.transform.position = new Vector3( _Pick.transform.position.x,
                                                Mathf.Clamp(_Pick.transform.position.y, m_Pick_Bounds.z, m_Pick_Bounds.w), 
                                                Mathf.Clamp(_Pick.transform.position.z, m_Pick_Bounds.x, m_Pick_Bounds.y));
    }

    void CalculateTension()
    {
        _TenstionSlider.value = Mathf.Clamp01(_TenstionSlider.value += m_AppliedTension * Time.deltaTime);
        m_CurrentTension = _TenstionSlider.value;
    }

    void CheckTension()
    {
        Debug.Log(m_CurrentTension);
        if (m_CurrentTension > 0.9f)
        {
            //break the tool
            CheckToolBreakage(100f);
        }
        else if (m_CurrentTension > 0.75f)
        {
            //high chance of break
            if (Time.time > m_ChanceTimer)
            {
                m_ChanceTimer = Time.time + m_ChanceRate;
                CheckToolBreakage(m_HighChance * m_Diff_Tension[(int)m_Difficulty]);
            }
        }
        else if (m_CurrentTension > 0.6f)
        {
            //Small break chance
            if (Time.time > m_ChanceTimer)
            {
                m_ChanceTimer = Time.time + m_ChanceRate;
                CheckToolBreakage(m_LowChance * m_Diff_Tension[(int)m_Difficulty]);
            }
            
        }
        else if (m_CurrentTension < 0.4f)
        {
            //small chance pin go down
            if (Time.time > m_ChanceTimer)
            {
                m_ChanceTimer = Time.time + m_ChanceRate;
                CheckPinDown(m_LowChance * m_Diff_Tension[(int)m_Difficulty]);
            }
        }
        else if (m_CurrentTension < 0.25f)
        {
            // high change pin go down
            if (Time.time > m_ChanceTimer)
            {
                m_ChanceTimer = Time.time + m_ChanceRate;
                CheckPinDown(m_HighChance * m_Diff_Tension[(int)m_Difficulty]);
            }
        }
        else if(m_CurrentTension < 0.1f)
        {
            //pins all go down
            CheckPinDown(100f);
        }
    }

    float timeBetweenBreakage = 5f;
    float breakageTimer = 0f;

    void CheckToolBreakage(float chance)
    {
        if (Random.Range(0f, 100f) > chance || Time.time < breakageTimer)
        {
            return;
        }
        breakageTimer = Time.time + timeBetweenBreakage;
        Debug.Log("Tools broke");
        m_Aud.PlayOneShot(soundFXs[2]);
        _UILives.transform.FindChild(m_Lives.ToString()).gameObject.SetActive(false);
        if (--m_Lives <= 0)
        {
            m_IsGameOver = true;
        }
       
    }

    void CheckPinDown(float chance)
    {
        foreach (TumblerPin tp in m_Cylinders)
        {
            if (tp.m_IsInPosition)
            {
                if (Random.Range(0f, 100f) < chance)
                {
                    tp.DropDown();
                    Debug.Log("Pin out of position");
                }
            }
        } 
        
        
    }

    void CheckGameOver()
    {
        if (Time.time > m_Timer)
        {
            m_IsGameOver = true;
            return;
        }
        foreach (TumblerPin tp in m_Cylinders)
        {
            if (!tp.m_IsInPosition)
            {
                return;
            }
        }
        m_IsGameOver = true;
        Debug.Log("GameOver");
    }

    void Timer()
    {
        _UITimerFill.fillAmount  += (1f/m_BaseTime) * Time.deltaTime;
        _UITimercenter.color = Color.Lerp(Color.green, Color.red, _UITimerFill.fillAmount);
        Debug.Log("Fill amount: " + _UITimercenter.fillAmount);
    }

    //This bottom section is bad
    public void PlayLock()
    {
        m_Aud.PlayOneShot(soundFXs[1]);
    }

    public void PlayUnlock()
    {
        m_Aud.PlayOneShot(soundFXs[0]);
    }

    float soundfxDuration = 2.5f;
    float soundfxTimer = 0;

    public void PlayPicking()
    {
        if (Time.time > soundfxTimer)
        {
            soundfxTimer = Time.time + soundfxDuration;
            m_Aud.PlayOneShot(soundFXs[2]);
        }
        
    }


}
