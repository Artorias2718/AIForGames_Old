using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant : MonoBehaviour
{
    #region Variables
    private IBoid m_boid;

    public float m_speed = 1.0f;
    #endregion

    #region Getters_Setters

    #endregion

    #region Unity
    private void Start()
	{
        m_boid = GetComponent<IBoid>();
        m_boid.Velocity = m_speed * Random.insideUnitCircle;
    }
	
	private void Update()
	{
        m_boid.Steering = Vector2.zero;
    }
	#endregion

	#region Custom

	#endregion
}