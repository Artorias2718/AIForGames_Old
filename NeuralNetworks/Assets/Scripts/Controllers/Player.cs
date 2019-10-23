using UnityEngine;
using System.Collections;

namespace AISandbox
{
    [RequireComponent(typeof(IBoid))]
    public class Player : MonoBehaviour
    {
        #region Variables
        private IBoid m_boid;
        #endregion

        #region Unity
        private void Awake()
        {
            m_boid = GetComponent<IBoid>();
        }

        private void FixedUpdate()
        {
            m_boid.Steering = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            m_boid.AltSteering = new Vector2(Input.GetAxis("AltH"), Input.GetAxis("AltV"));
        }
        #endregion
    }
}