using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class Chromosome
    {
        #region Variables
        public static int m_CHROMOSOME_LENGTH = 2;
        private const double m_MUTATION_RATE = 0.15;
        #endregion

        #region Getters_Setters
        public bool Elite { get; set; }
        public List<double> Genes { get; set; }
        public double Fitness { get; set; }
        #endregion

        #region Custom
        public Chromosome(bool i_randomize = true)
        {
            Genes = new List<double>(m_CHROMOSOME_LENGTH);

            if (i_randomize)
            {
                for (int i = 0; i < m_CHROMOSOME_LENGTH; ++i)
                {
                    Genes.Add(Random.Range(-1.0f, 1.0f));
                }
            }
        }

        public static Chromosome Crossover(Chromosome i_parent_1, Chromosome i_parent_2)
        {
            Chromosome child = new Chromosome(false);

            int split_index = Random.Range(0, m_CHROMOSOME_LENGTH);

            Debug.Log("Parent 1: " + i_parent_1.Genes.Count);
            Debug.Log("Parent 2: " + i_parent_2.Genes.Count);

            for (int i = 0; i < i_parent_1.Genes.Count; ++i)
            {
                child.Genes.Add((i > split_index) ? i_parent_1.Genes[i] : i_parent_2.Genes[i]);
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