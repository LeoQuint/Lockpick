using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Difficulty
{
    EASY,NORMAL,HARD,IMPOSSIBLE
}

public class GameController : MonoBehaviour {

    public Difficulty m_Difficulty;

    ///References:
    public Slider _TenstionSlider;
    public GameObject _Pick;

    /// <summary>
    /// Designed Variables
    /// </summary>
    //Easy, normal, hard, impossible values
    [SerializeField]
    float[] m_Diff_Tension = new float[4]{ 1f, 2f, 4f, 8f };
    [SerializeField]
    Vector4 m_Pick_Bounds = new Vector4();
    [SerializeField]
    float m_Pick_MovementSpeed = 0.5f;

    private float m_MouseInput = 0f;
    private bool m_Lifting = false;
    private float m_AppliedTension = 0f;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        PlayerInput();        
	}

    void FixedUpdate()
    {
        CalculateTension();
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
       
    }
}
