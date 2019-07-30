using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class OrientedBoid : MonoBehaviour, IBoid
    {
        #region Variables
        public LineRenderer m_steering_line;

        public LineRenderer m_velocity_line;

        private bool m_wrap_screen;
        private bool m_wrap_screen_x;
        private bool m_wrap_screen_y;
        private const float m_MAX_SPEED = 5.0f;
        private const float m_STEERING_ACCEL = 50.0f;

        private const float m_VELOCITY_LINE_SCALE = 0.1f;
        private const float m_STEERING_LINE_SCALE = 4.0f;

        private Renderer m_renderer;

        [SerializeField]
        private bool m_draw_vectors = true;

        private Vector2 m_steering = Vector2.zero;

        private Vector2 m_acceleration = Vector2.zero;

        private Vector2 m_velocity = Vector2.zero;

        #endregion

        #region Getters_Setters
        [SerializeField]
        public bool DrawVectors
        {
            get
            {
                return m_draw_vectors;
            }
            set
            {
                m_draw_vectors = value;
                m_steering_line.gameObject.SetActive(m_draw_vectors);
                m_velocity_line.gameObject.SetActive(m_draw_vectors);
            }
        }
        public float Radius { get { return m_renderer.bounds.extents.x; } }
        public float Max_Speed { get { return m_MAX_SPEED; } }
        public float Max_Steering_Acceleration { get { return m_STEERING_ACCEL; } }

        public Vector2 Position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        public Vector2 Velocity
        {
            get { return m_velocity; }
            set { m_velocity = Vector2.ClampMagnitude(value, m_MAX_SPEED); }
        }

        public Vector2 Steering
        {
            get { return m_acceleration; }
            set
            {
                m_steering = Vector2.ClampMagnitude(value, 1.0f);
                m_acceleration = m_STEERING_ACCEL * m_steering;
            }
        }
        #endregion

        #region Unity
        private void Start()
        {
            m_renderer = GetComponent<Renderer>();
            DrawVectors = m_draw_vectors;
        }

        private void Update()
        {
            Vector2 position = ScreenWrap();

            m_velocity += m_acceleration * Time.deltaTime;
            m_velocity = Vector2.ClampMagnitude(m_velocity, m_MAX_SPEED);

            position += m_velocity * Time.deltaTime;
            transform.position = position;
            transform.rotation = Quaternion.LookRotation(-Vector3.forward, m_velocity.normalized);

            m_steering_line.transform.rotation = Quaternion.identity;
            m_velocity_line.transform.rotation = Quaternion.identity;

            m_steering_line.SetPosition(1, m_steering * m_STEERING_LINE_SCALE);
            m_velocity_line.SetPosition(1, m_velocity * m_VELOCITY_LINE_SCALE);

            // Steering is reset every frame so SetInput() must be called every frame for continuous s2eering
            m_steering = Vector2.zero;
            m_acceleration = Vector2.zero;
        }
        #endregion

        #region Custom

        private Vector2 ScreenWrap()
        {
            Vector2 position = transform.position;
            if (m_renderer.isVisible)
            {
                m_wrap_screen_x = false;
                m_wrap_screen_y = false;
                return position;
            }
            else
            {
                Vector2 viewportPosition = (Vector2)Camera.main.WorldToViewportPoint(Position);
                if (!m_wrap_screen_x && (viewportPosition.x > 1 || viewportPosition.x < 0))
                {
                    position.x = -position.x;
                    m_wrap_screen_x = true;
                }
                if (!m_wrap_screen_y && (viewportPosition.y > 1 || viewportPosition.y < 0))
                {
                    position.y = -position.y;
                    m_wrap_screen_y = true;
                }
            }
            return position;
        }

        #endregion
    }
}