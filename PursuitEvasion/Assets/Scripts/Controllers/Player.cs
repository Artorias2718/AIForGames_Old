using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IBoid))]
public class Player : MonoBehaviour
{
    #region Variables
    private IBoid m_boid;
    #endregion

    #region Getters_Setters

    #endregion

    #region Unity
    private void Awake()
	{
        m_boid = GetComponent<IBoid>();
    }
	
	private void Update()
	{
        Vector2 steering = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        m_boid.Steering = steering;
    }
	#endregion

	#region Custom

	#endregion
}