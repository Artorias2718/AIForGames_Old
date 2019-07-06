using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AISandbox
{
    [RequireComponent(typeof(IBoid))]
    public class Flock : MonoBehaviour
    {
        #region Variables
        public float m_neighbor_distance = 5.0f;
        public float m_separation_weight = 1.0f;
        public float m_alignment_weight = 0.5f;
        public float m_cohesion_weight = 0.5f;

        private OrientedBoid m_boid;

        private OrientedBoid[] m_boids;

        #endregion

        #region Getters_Setters

        #endregion

        #region Unity
        private void Awake()
        {
            m_boid = GetComponent<OrientedBoid>();
            m_boid.Velocity = Random.insideUnitSphere;

            m_boids = FindObjectsOfType<OrientedBoid>().Where(boid => boid != m_boid).ToArray();
        }

        private void Update()
        {
            Vector2 alignment = m_alignment_weight * Alignment();
            Vector2 cohesion = m_cohesion_weight * Cohesion();
            Vector2 separation = m_separation_weight * Separation();

            // Pass all parameters to the character control script.

            m_boid.Velocity += alignment + cohesion + separation;
            m_boid.Steering += m_boid.Velocity;
        }

        #endregion

        #region Custom

        private List<OrientedBoid> GetNeighbors()
        {
            List<OrientedBoid> neighbors = new List<OrientedBoid>();
            foreach (OrientedBoid boid in m_boids)
            {
                if ((boid.Position - m_boid.Position).sqrMagnitude < m_neighbor_distance * m_neighbor_distance)
                {
                    neighbors.Add(boid);
                }
            }

            return neighbors;
        }

        private Vector2 Alignment()
        {
            List<OrientedBoid> neighbors = GetNeighbors();
            Vector2 alignment = Vector2.zero;
            int neighbor_count = neighbors.Count;

            foreach (OrientedBoid neighbor in neighbors)
            {
                alignment += neighbor.Velocity;
            }

            if (neighbor_count > 0)
            {
                alignment /= neighbor_count;
            }

            return alignment.normalized;
        }

        private Vector2 Cohesion()
        {
            List<OrientedBoid> neighbors = GetNeighbors();

            Vector2 cohesion = Vector2.zero;
            int neighbor_count = neighbors.Count;

            foreach (OrientedBoid neighbor in neighbors)
            {
                cohesion += neighbor.Position;
            }

            if (neighbor_count > 0)
            {
                cohesion /= neighbor_count;
                cohesion -= m_boid.Position;
            }

            return cohesion.normalized;
        }

        private Vector2 Separation()
        {
            List<OrientedBoid> neighbors = GetNeighbors();

            Vector2 separation = Vector2.zero;
            int neighbor_count = neighbors.Count;

            foreach (OrientedBoid neighbor in neighbors)
            {
                separation += neighbor.Position - m_boid.Position;
            }

            if (neighbor_count > 0)
            {
                separation /= neighbor_count;
                //separation *= -1;
            }

            return separation.normalized;
        }
        #endregion
    }
}