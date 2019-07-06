using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    [RequireComponent(typeof(IBoid))]
    public class Avoidance : MonoBehaviour
    {
        #region Variables
        public Transform m_obstalces_container;
        private const float m_MAX_AVOIDANCE_FORCE = 15.0f;
        private const float m_LINE_OF_SIGHT = 20.0f;
        private Vector2 m_ahead;
        private IBoid m_boid;

        private Obstacle[] m_obstalces;

        private Vector2[] m_vertices;
        private LineRenderer m_box;
        private Vector2 m_projected_vec;

        private SpriteRenderer m_sprite_renderer;

        #endregion

        #region Getters_Setters

        #endregion

        #region Unity
        private void Awake()
        {
            m_boid = GetComponentInChildren<Boid>();
            m_sprite_renderer = GetComponent<SpriteRenderer>();

            m_box = transform.Find("Box").GetComponent<LineRenderer>();
            m_vertices = new Vector2[4];

            m_obstalces = m_obstalces_container.GetComponentsInChildren<Obstacle>();
            m_boid.Velocity = Random.insideUnitSphere;
        }

        private void Update()
        {
            DrawBox();
        }

        private void FixedUpdate()
        {
            Vector2 steering = Avoid();

            Vector2 TotalForce = new Vector2(0, 0);
            float T;
            if (LineIntersectsCircle())
            {
                T = CalculateT(m_projected_vec);
                m_boid.Steering = TotalForce = CalculateBreakingForce(T) + CalculateSteeringForce(T);
            }
            else
            {
                TotalForce = Vector2.zero;
                m_boid.Steering = Vector2.zero;

                if (m_boid.Velocity.magnitude < m_boid.Max_Speed)
                {
                    m_boid.Steering = m_boid.Velocity.normalized * 0.1f;
                }
            }

            // Pass all parameters to the character control script.

            m_boid.Steering = steering;
        }
        #endregion

        #region Custom

        private void DrawBox()
        {
            m_ahead = m_boid.Velocity.normalized * m_LINE_OF_SIGHT;
            m_vertices[0] = new Vector2(m_ahead.y, m_ahead.x * -1).normalized * m_boid.Radius;

            m_vertices[1] = m_vertices[0] + m_ahead;

            m_vertices[2] = new Vector2(m_ahead.y * -1, m_ahead.x).normalized * m_boid.Radius + m_ahead;

            m_vertices[3] = new Vector2(m_ahead.y * -1, m_ahead.x).normalized * m_boid.Radius;

            for (int i = 0; i < m_vertices.Length; i++)
            {
                m_box.SetPosition(i, m_vertices[i]);
            }
        }

        private Vector3 Avoid()
        {
            m_ahead = m_boid.Position + m_LINE_OF_SIGHT * m_boid.Velocity.normalized;
            Obstacle nearest_obstacle = FindNearestObstacle();

            Vector2 avoidance = Vector2.zero;

            if (nearest_obstacle != null)
            {
                avoidance = m_MAX_AVOIDANCE_FORCE * (m_ahead - nearest_obstacle.Position).normalized;
            }
            else
            {
                avoidance = m_boid.Velocity;
            }

            return avoidance;
        }

        private float CalculateT(Vector2 projectedVec)
        {
            Vector2 mypos = transform.position;
            float distance = (mypos - projectedVec).magnitude;
            return 1.0f - (distance / m_LINE_OF_SIGHT);
        }
        private Vector2 CalculateBreakingForce(float T)
        {
            return m_boid.Velocity.normalized * -1 * T * T;
        }
        private Vector2 CalculateSteeringForce(float T)
        {
            return m_boid.Steering * T;
        }

        private bool LineIntersectsCircle()
        {
            Obstacle obstacle = FindNearestObstacle();

            if (obstacle != null)
            {
                m_projected_vec = GetProjectedVector(obstacle.transform.position, m_ahead);
                m_boid.Steering = (obstacle.Position - (m_projected_vec + m_boid.Position)).normalized;
                return true;
            }
            return false;
        }

        private Obstacle FindNearestObstacle()
        {
            Obstacle nearest_obstacle = null;
            float mostThreateningDistance = m_LINE_OF_SIGHT;
            List<Obstacle> inSight = GetObstaclesInLineOfSight();
            if (inSight.Count == 0)
                return nearest_obstacle;
            foreach (Obstacle obstacle in inSight)
            {
                Vector2 obstaclePos = obstacle.gameObject.transform.position;
                m_projected_vec = GetProjectedVector(obstaclePos, m_ahead.normalized);
                float projectedMag = m_projected_vec.magnitude;

                if (projectedMag < mostThreateningDistance)
                {
                    //this obstacle is the nearest threat to the boid
                    nearest_obstacle = obstacle;
                }

            }
            return nearest_obstacle;
        }

        private List<Obstacle> GetObstaclesInLineOfSight()
        {
            List<Obstacle> in_sight = new List<Obstacle>();

            float minProjectedMagnitude = m_LINE_OF_SIGHT;

            List<Obstacle> collidableObstacles = GetNearestObstacles(GetRelevantObstacles());
            foreach (Obstacle obstacle in collidableObstacles)
            {
                m_projected_vec = GetProjectedVector(obstacle.Position, m_ahead);
                float projectedMag = m_projected_vec.magnitude;

                if (projectedMag < minProjectedMagnitude)
                {
                    //these are the obstacles that fall within the vision of our avoiding actor
                    in_sight.Add(obstacle);
                }
            }
            return in_sight;

        }
        private List<Obstacle> GetNearestObstacles(List<Obstacle> relevants)
        {
            List<Obstacle> collidable_obstacles = new List<Obstacle>();
            foreach (Obstacle relevant in relevants)
            {
                float min_distance_to_projection = (relevant.Radius + m_boid.Radius) * (relevant.Radius + m_boid.Radius);
                Vector2 mypos = transform.position;
                m_projected_vec = GetProjectedVector(relevant.Position, m_ahead.normalized);
                float projected_distance_from_obstacle = ((m_projected_vec + mypos) - relevant.Position).sqrMagnitude;

                if (projected_distance_from_obstacle < min_distance_to_projection)
                {   
                    //these objects will collide in the near future
                    collidable_obstacles.Add(relevant);
                }
            }
            return collidable_obstacles;

        }

        private List<Obstacle> GetRelevantObstacles()
        {
            m_obstalces = FindObjectsOfType<Obstacle>();
            List<Obstacle> relevantCircles = new List<Obstacle>();
            foreach (Obstacle obstacle in m_obstalces)
            {
                Vector2 distance_vector = obstacle.gameObject.transform.position - transform.position;
                
                float cos_theta = Vector2.Dot(distance_vector, m_boid.Velocity) / (distance_vector.magnitude * m_boid.Velocity.magnitude);
                
                if (cos_theta > 0)
                {
                    relevantCircles.Add(obstacle);
                }
            }
            return relevantCircles;
        }

        private Vector2 GetProjectedVector(Vector2 obstacle_vector, Vector2 normal)
        {
            return Vector3.Project(obstacle_vector - m_boid.Position, m_ahead);
        }

        #endregion
    }
}