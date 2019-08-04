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

        private Genetic[] m_tanks;
        private GeneticAlgorithm m_genetic_algorithm;
        private Timer m_timer;

        #endregion

        #region Getters_Setters

        #endregion

        #region Unity
        private void Start()
        {
            m_tanks = FindObjectsOfType<Genetic>();
            m_genetic_algorithm = new GeneticAlgorithm(m_tanks.Length);
            m_timer = FindObjectOfType<Timer>();

            for (int i = 0; i < m_tanks.Length; ++i)
            {
                m_genetic_algorithm.Population.Add(m_tanks[i].Chromosome);
            }
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
                m_genetic_algorithm.Run();
            }
        }
        #endregion

        #region Custom

        #endregion
    }
}