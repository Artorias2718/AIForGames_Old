using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AISandbox
{
    public class GeneticAlgorithm
    {
        #region Getters_Setters
        public double Max_Fitness { get; private set; }
        public int Max_Elite_Size { get; private set; }
        public int Population_Size { get; set; }
        public int Generation { get; set; }
        public List<Chromosome> Population { get; set; }

        #endregion

        #region Custom
        public GeneticAlgorithm(int i_population_size)
        {
            Population_Size = i_population_size;
            Max_Elite_Size = Mathf.RoundToInt(0.1f * Population_Size);
            Population = new List<Chromosome>();
            Generation = 1;
        }

        public void Run()
        {
            Generation++;

            foreach (Chromosome member in Population)
            {
                member.Elite = false;
            }

            Population.Sort(CompareTo);
            Max_Fitness = Population.Max(chromosome => chromosome.Fitness);

            Selection();
        }

        public void Selection()
        {
            List<Chromosome> new_population = new List<Chromosome>();
            for (int i = 0; i < Population.Count; ++i)
            {
                if (i < Max_Elite_Size)
                {
                    Population[i].Elite = true;
                    new_population.Add(Population[i]);
                }
                else
                {
                    Chromosome parent_1 = AcceptReject(Max_Fitness);
                    Chromosome parent_2 = AcceptReject(Max_Fitness);
                    Chromosome child = Chromosome.Crossover(parent_1, parent_2);
                    child.Mutate();
                    new_population.Add(child);
                }
            }
            Population.Clear();
            Population.AddRange(new_population);
        }

        public Chromosome AcceptReject(double i_max_value)
        {
            int limit = 0;
            while (true)
            {
                int index = Mathf.FloorToInt(Random.Range(0, Population.Count));
                double rand = Random.Range(0.0f, (float)i_max_value);
                Chromosome partner = Population[index];

                if (rand < partner.Fitness)
                {
                    return partner;
                }

                limit++;

                if (limit > 10000)
                {
                    return null;
                }
            }
        }

        private int CompareTo(Chromosome i_chromosome_1, Chromosome i_chromosome_2)
        {
            if (i_chromosome_1.Fitness > i_chromosome_2.Fitness)
            {
                return -1;
            }
            else if (i_chromosome_1.Fitness < i_chromosome_2.Fitness)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}