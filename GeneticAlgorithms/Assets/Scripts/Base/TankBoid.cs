using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class TankBoid : MonoBehaviour, IBoid
    {
        #region Variables

        private bool m_wrap_screen;
        private bool m_wrap_screen_x;
        private bool m_wrap_screen_y;

        private const float m_MAX_SPEED = 25.0f;
        private const float m_STEERING_SPEED = 12.5f;
        private const float m_NO_INPUT_DECEL = -75.0f;
        private const float m_ROT_SPEED = 5.0f;
        private const float m_STEERING_LINE_SCALE = 4.0f;

        private SpriteRenderer m_sprite_renderer;

        [SerializeField]
        private bool m_draw_vectors = true;

        public LineRenderer m_left_steering_line;
        public LineRenderer m_right_steering_line;

        private Vector2 m_left_steering = Vector2.zero;
        private Vector2 m_right_steering = Vector2.zero;

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
                m_left_steering_line.gameObject.SetActive(m_draw_vectors);
                m_right_steering_line.gameObject.SetActive(m_draw_vectors);
            }
        }

        public float Orientation { get; set; }

        public float MaxSpeed { get { return m_MAX_SPEED; } }

        public SpriteRenderer SpriteRenderer
        {
            get
            {
                if(m_sprite_renderer == null)
                {
                    m_sprite_renderer = GetComponent<SpriteRenderer>();
                }

                return m_sprite_renderer;
            }
        }

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
            get { return m_left_steering; }
            set
            {
                m_left_steering.y = Mathf.Clamp(value.y, -1.0f, 1.0f);
                
                // Can only go half as fast backward
                if(m_left_steering.y < 0.0f)
                {
                    m_left_steering.y *= 0.5f;
                }
            }
        }

        public Vector2 AltSteering
        {
            get { return m_right_steering; }
            set
            {
                m_right_steering.y = Mathf.Clamp(value.y, -1.0f, 1.0f);

                // Can only go half as fast backward
                if (m_right_steering.y < 0.0f)
                {
                    m_right_steering.y *= 0.5f;
                }
            }
        }

        #endregion

        #region Unity

        private void Start()
        {
            DrawVectors = m_draw_vectors;
        }

        private void Update()
        {
            Vector2 position = ScreenWrap();

            float speed = m_left_steering.y * m_STEERING_SPEED + m_right_steering.y * m_STEERING_SPEED;
            float rot = (m_right_steering.y - m_left_steering.y) * m_ROT_SPEED;

            Orientation += rot;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, Orientation);

            m_velocity = transform.up * speed;
            if (m_left_steering.y == 0.0f && m_right_steering.y == 0.0f && m_velocity.sqrMagnitude > 0.0f)
            {
                m_velocity += Vector2.ClampMagnitude(m_velocity.normalized * m_NO_INPUT_DECEL * Time.deltaTime, m_velocity.magnitude);
            }
            m_velocity = Vector2.ClampMagnitude(m_velocity, m_MAX_SPEED);

            position += m_velocity * Time.deltaTime;
            transform.position = position;

            //_left_steering_line.transform.rotation = Quaternion.identity;
            m_left_steering_line.SetPosition(1, m_left_steering * m_STEERING_LINE_SCALE);
            //_right_steering_line.transform.rotation = Quaternion.identity;
            m_right_steering_line.SetPosition(1, m_right_steering * m_STEERING_LINE_SCALE);
        }
        #endregion

        #region Custom

        private Vector2 ScreenWrap()
        {
            Vector2 position = transform.position;
            if (m_sprite_renderer.isVisible)
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