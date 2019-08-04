using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class Chromosome
    {
        #region Variables
        public static int m_CHROMOSOME_LENGTH = 250;
        private const float m_MUTATION_RATE = 0.05f;
        #endregion

        #region Getters_Setters
        public bool Elite { get; set; }
        public float[] Genes { get; set; }
        public float Fitness { get; set; }
        #endregion

        #region Custom
        public Chromosome(bool i_randomize = true)
        {
            Genes = new float[m_CHROMOSOME_LENGTH];

            if (i_randomize)
            {
                for (int i = 0; i < m_CHROMOSOME_LENGTH; ++i)
                {
                    Genes[i] = Random.Range(-1.0f, 1.0f);
                }
            }
        }

        public static Chromosome Crossover(Chromosome i_parent_1, Chromosome i_parent_2)
        {
            Chromosome child = new Chromosome(false);

            for (int i = 0; i < m_CHROMOSOME_LENGTH; ++i)
            {
                if (i + 1 < m_CHROMOSOME_LENGTH)
                {
                    int split_index = Random.Range(0, m_CHROMOSOME_LENGTH);
                    child.Genes[i] = (i < split_index) ? i_parent_1.Genes[i] : i_parent_2.Genes[i];
                    child.Genes[i + 1] = ((i + 1) < split_index) ? i_parent_1.Genes[i + 1] : i_parent_2.Genes[i + 1];
                }
            }
            return child;
        }

        public void Mutate()
        {
            for (int i = 0; i < m_CHROMOSOME_LENGTH; ++i)
            {
                float mutate = Random.Range(0.0f, 1.0f);
                if (mutate < m_MUTATION_RATE)
                {
                    Genes[i] = Random.Range(-1.0f, 1.0f);
                }
            }
        }
        #endregion
    }
}