using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AISandbox
{
    public class Process : MonoBehaviour
    {
        #region Variables
        public Text m_generation;
        public Transform m_mine_prefab;
        public int m_mine_count = 30;

        private Genetic[] m_tanks;
        private GeneticAlgorithm m_genetic_algorithm;
        private Timer m_timer;
        private List<Transform> m_mine_list;
        private const float m_SPAWN_RANGE = 15.0f;
        private const float m_SPAWN_OFFSET = 4.0f;

        #endregion

        #region Getters_Setters

        #endregion

        #region Unity
        private void Start()
        {
            m_mine_prefab = transform.Find("Mine").GetComponentInChildren<Transform>(true);
            m_tanks = FindObjectsOfType<Genetic>();
            m_mine_list = new List<Transform>();

            m_genetic_algorithm = new GeneticAlgorithm(m_tanks.Length);
            m_timer = FindObjectOfType<Timer>();

            for (int i = 0; i < m_tanks.Length; ++i)
            {
                m_genetic_algorithm.Population.Add(m_tanks[i].Chromosome);
            }

            ResetMines();
        }

        private void Update()
        {
            m_generation.text = string.Format("Generation {0}", m_genetic_algorithm.Generation);

            if (Timer.Reset)
            {
                Timer.Reset = false;
                foreach (Genetic tank in m_tanks)
                {
                    tank.ResetTank();
                }

                ResetMines();
                m_genetic_algorithm.Run();

                foreach (Genetic tank in m_tanks)
                {
                    tank.NeuralNetwork.Update(tank.Chromosome.Genes);
                }
            }
        }
        #endregion

        #region Custom

        private void ResetMines()
        {
            for (int i = 0; i < m_mine_count; ++i)
            {
                Transform mine = Instantiate(m_mine_prefab);
                mine.name = string.Format("Mine {0}", (i + 1));
                mine.position = new Vector2(Random.Range(-m_SPAWN_RANGE, m_SPAWN_RANGE), Random.Range(-m_SPAWN_RANGE, m_SPAWN_RANGE)) + new Vector2(m_SPAWN_OFFSET, m_SPAWN_OFFSET);
                mine.gameObject.SetActive(true);
                m_mine_list.Add(mine);
            }
        }

        #endregion
    }
}