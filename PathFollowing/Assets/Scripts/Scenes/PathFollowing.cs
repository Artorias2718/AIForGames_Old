using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class PathFollowing : MonoBehaviour
    {
        #region Variables
        public int m_path_follower_count = 10;
        public PathFollower m_path_follower_boid_prefab;
        private List<PathFollower> m_path_follower_list = new List<PathFollower>();
        #endregion

        #region Getters_Setters

        #endregion

        #region Unity
        private void Start()
        {
            for (int i = 0; i < m_path_follower_count; ++i)
			{
                m_path_follower_list.Add(CreatePathFollowerBoid());
            }

            m_path_follower_list[Random.Range(0, m_path_follower_list.Count)].m_display_path = true;
        }

        private void Update()
        {

        }
        #endregion

        #region Custom
        private PathFollower CreatePathFollowerBoid()
        {
            PathFollower new_boid = Instantiate<PathFollower>(m_path_follower_boid_prefab);
            new_boid.gameObject.name = "Path Follower Boid";
            new_boid.gameObject.SetActive(true);
            return new_boid;
        }

        #endregion
    }
}