using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AISandbox
{
    [RequireComponent(typeof(IBoid))]
    public class PathFollower : MonoBehaviour
    {
        #region Variables
        public bool m_display_path = false;
        private OrientedBoid m_boid;
        private GridNode[] m_path;

        private PathFinding m_path_finder;
        private SpriteRenderer m_sprite_renderer;
        private const float m_ARRIVAL_RADIUS = 0.25f;
        private int m_node_index = 0;
        private bool m_arriving = false;
        #endregion

        #region Getters_Setters

        #endregion

        #region Unity
        private void Start()
        {
            m_boid = GetComponent<OrientedBoid>();
            m_sprite_renderer = GetComponent<SpriteRenderer>();
            m_path_finder = transform.Find("PathFinder").GetComponent<PathFinding>();

            m_path = m_path_finder.FindNewPath();
            m_boid.Position = m_path_finder.StartNode.Position;

            if (m_display_path)
            {
                m_sprite_renderer.color = Color.yellow;
                m_path_finder.MarkPath();
            }
        }

        private void Update()
        {
            if (m_arriving)
            {
                m_arriving = false;
                m_node_index = 0;
                m_path = m_path_finder.FindNewPath(m_path_finder.GoalNode);

                if (m_display_path)
                {
                    m_sprite_renderer.color = Color.yellow;
                    m_path_finder.MarkPath();
                }
            }
            m_boid.Steering = FollowPath();
        }

        #endregion

        #region Custom
        private Vector2 FollowPath()
        {
            GridNode target = null;
            
            if(m_path != null)
            {
                if(m_node_index < m_path.Length)
                {
                    target = m_path[m_node_index];
                }
            }

            if(target != null)
            {
                float distance = (m_boid.Position - target.Position).sqrMagnitude;
                if(distance <= m_ARRIVAL_RADIUS * m_ARRIVAL_RADIUS)
                {
                    if(m_node_index >= m_path.Length - 1)
                    {
                        m_node_index = m_path.Length - 1;
                    }
                    else
                    {
                        m_node_index++;
                    }
                }

                if(m_node_index < m_path.Length - 1)
                {
                    return Seek(m_path[m_node_index].Position);
                }
                else
                {
                    m_arriving = true;
                    return Arrive(m_path[m_path.Length - 1].Position);
                }
            }
            return Vector2.zero;
        }

        private Vector2 Seek(Vector2 target)
        {
            Vector2 desired_velocity = m_boid.Max_Speed * (target - m_boid.Position).normalized;
            return desired_velocity - m_boid.Velocity;
        }

        private Vector2 Arrive(Vector2 target)
        {
            Vector2 desired_velocity = target - m_boid.Position;
            float distance = desired_velocity.magnitude;
            desired_velocity.Normalize();

            if (distance < m_ARRIVAL_RADIUS)
            {
                desired_velocity = Vector2.ClampMagnitude(desired_velocity, m_boid.Max_Speed * distance / m_ARRIVAL_RADIUS);
            }
            else
            {
                desired_velocity = Vector2.ClampMagnitude(desired_velocity, m_boid.Max_Speed);
            }

            return desired_velocity - m_boid.Velocity;
        }
        #endregion
    }
}