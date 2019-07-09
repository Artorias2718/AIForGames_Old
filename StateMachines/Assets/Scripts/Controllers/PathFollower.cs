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
        private SpriteRenderer m_sprite_renderer;
        private const float m_ARRIVAL_RADIUS = 0.25f;
        private int m_node_index = 0;
        #endregion

        #region Getters_Setters
        public PathFinding PathFinder { get; private set; }
        public bool Arriving { get; set; }
        public GridNode[] Path { get; set; }
        public IBoid Boid { get; private set; }
        #endregion

        #region Unity
        private void Awake()
        {
            Boid = GetComponent<OrientedBoid>();
            m_sprite_renderer = GetComponent<SpriteRenderer>();
            PathFinder = transform.Find("PathFinder").GetComponent<PathFinding>();
        }

        private void Update()
        {
            if (Arriving)
            {
                if (Boid.Position == PathFinder.GoalNode.Position)
                {
                    Arriving = false;
                    m_node_index = 0;
                }
            }

            if (m_display_path)
            {
                m_sprite_renderer.color = Color.yellow;
                PathFinder.MarkPath();
            }
            Boid.Steering = FollowPath();
        }

        #endregion

        #region Custom
        public Vector2 FollowPath()
        {
            GridNode target = null;
            
            if(Path != null)
            {
                if(m_node_index < Path.Length)
                {
                    target = Path[m_node_index];
                }
            }

            if(target != null)
            {
                float distance = (Boid.Position - target.Position).sqrMagnitude;
                if(distance <= m_ARRIVAL_RADIUS * m_ARRIVAL_RADIUS)
                {
                    if(m_node_index >= Path.Length - 1)
                    {
                        m_node_index = Path.Length - 1;
                    }
                    else
                    {
                        m_node_index++;
                    }
                }

                if(m_node_index < Path.Length - 1)
                {
                    return Seek(Path[m_node_index].Position);
                }
                else
                {
                    Arriving = true;
                    return Arrive(Path[Path.Length - 1].Position);
                }
            }
            return Vector2.zero;
        }

        private Vector2 Seek(Vector2 target)
        {
            Vector2 desired_velocity = Boid.Max_Speed * (target - Boid.Position).normalized;
            return desired_velocity - Boid.Velocity;
        }

        private Vector2 Arrive(Vector2 target)
        {
            Vector2 desired_velocity = target - Boid.Position;
            float distance = desired_velocity.magnitude;
            desired_velocity.Normalize();

            if (distance < m_ARRIVAL_RADIUS)
            {
                desired_velocity = Vector2.ClampMagnitude(desired_velocity, Boid.Max_Speed * distance / m_ARRIVAL_RADIUS);
            }
            else
            {
                desired_velocity = Vector2.ClampMagnitude(desired_velocity, Boid.Max_Speed);
            }

            return desired_velocity - Boid.Velocity;
        }
        #endregion
    }
}