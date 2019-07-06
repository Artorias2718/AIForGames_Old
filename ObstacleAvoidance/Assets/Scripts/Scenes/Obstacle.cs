using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Obstacle : MonoBehaviour
    {
        #region Variables
        private readonly Color m_COLLIDE_COLOR = Color.red;

        private SpriteRenderer m_sprite_renderer;
        private Color m_original_color;
        #endregion

        #region Getters_Setters
        public Vector2 Position
        {
            get
            {
                return transform.position;
            }
        }

        public float Radius
        {
            get
            {
                return m_sprite_renderer.bounds.extents.x;
            }
        }
        #endregion

        #region Unity
        private void Start()
        {
            m_sprite_renderer = GetComponent<SpriteRenderer>();
            m_original_color = m_sprite_renderer.color;
        }

        private void Update()
        {
            bool collision = false;
            Boid[] boids = FindObjectsOfType<Boid>();
            foreach (Boid boid in boids)
            {
                float sqr_dist = (boid.transform.position - transform.position).sqrMagnitude;
                float boid_radius = boid.gameObject.GetComponent<SpriteRenderer>().bounds.extents.x;
                float sqr_min_dist = (Radius + boid_radius) * (Radius + boid_radius);
                if (sqr_min_dist > sqr_dist)
                {
                    collision = true;
                    break;
                }
            }

            m_sprite_renderer.color = collision ? m_COLLIDE_COLOR : m_original_color;
        }
        #endregion

        #region Custom

        #endregion
    }
}