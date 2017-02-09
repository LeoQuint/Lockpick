using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumblerPin : MonoBehaviour {

    /// <summary>
    /// References
    /// </summary>
    [SerializeField]
    GameObject _TopCylinder;
    [SerializeField]
    Material _CorrectMat;
    [SerializeField]
    Material _WrongMat;

    //The sweet spot we are trying to reach.
    [SerializeField]
    float _Target_Y;
    //The max distance the tumbler can go up.
    [SerializeField]
    float _Max_Y;
    //Upward movespeed
    [SerializeField]
    float _Up_Speed;
    //Downward movespeed
    [SerializeField]
    float _Down_Speed;
    [SerializeField]
    float _Pin_DeadZone = 0.02f;
    

    //The lower resting place of the tumbler.
    private float m_Starting_Y;
    //Tracks if the pick is touching the cylinder.
    private bool m_IsPicking = false;
    public bool m_IsInPosition = false;
    private Rigidbody m_Rb;

    // Use this for initialization
    void Start () {
        m_Starting_Y = transform.position.y;
        _Target_Y = _Target_Y - GetComponent<Collider>().bounds.extents.x;
    }
	
	// Update is called once per frame
	void Update () {
        if (Mathf.Abs(transform.position.y - _Target_Y) < _Pin_DeadZone)
        {
            if (!m_IsInPosition && m_IsPicking)
            {
                m_IsInPosition = true;
                _TopCylinder.GetComponent<Renderer>().material = _CorrectMat;
            }
            
        }
        else
        {
            if (m_IsInPosition)
            {
                m_IsInPosition = false;
                _TopCylinder.GetComponent<Renderer>().material = _WrongMat;
            }
        }
	}

    void FixedUpdate()
    {
        if (!m_IsPicking && transform.position.y > m_Starting_Y && !m_IsInPosition)
        {
            transform.Translate(Vector3.down * _Down_Speed * Time.deltaTime);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            m_IsPicking = true;
            if (transform.position.y < _Max_Y)
            {
                transform.Translate(Vector3.up * _Up_Speed * Time.deltaTime);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            m_IsPicking = false;
        }
        
    }

    public void DropDown()
    {
        m_IsInPosition = false;
        _TopCylinder.GetComponent<Renderer>().material = _WrongMat;
    }
}
