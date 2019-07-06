using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    [RequireComponent(typeof(IBoid))]
    public class Pursuit : MonoBehaviour
    {
        #region Variables
        private IBoid m_boid;
        private IBoid m_target;
        private Crosshair m_crosshair;
        #endregion

        #region Getters_Setters

        #endregion

        #region Unity
        private void Start()
        {
            m_boid = GetComponent<IBoid>();
            m_target = FindObjectOfType<PursuitEvasion>().m_target;
            m_crosshair = FindObjectOfType<PursuitEvasion>().m_crosshair;
        }

        private void FixedUpdate()
        {
            Vector3 steering = Pursue(m_target);

            // Pass all parameters to the character control script.
            m_boid.Steering = steering;
        }
        #endregion

        #region Custom
        public Vector2 Seek(IBoid target)
        {
            Vector2 desired_velocity = target.Max_Velocity * (target.Position - m_boid.Position).normalized;
            return desired_velocity - m_boid.Velocity;
        }

        public Vector3 Pursue(IBoid target)
        {
            float dist = (m_boid.Position - target.Position).magnitude;
            float dt = dist / m_boid.Max_Velocity;

            m_crosshair.Future_Position = target.Position + dt * target.Velocity;

            return Seek(target);
        }
        #endregion
    }
}