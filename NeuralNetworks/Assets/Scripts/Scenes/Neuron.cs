using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class Neuron
    {
        #region Variables

        #endregion

        #region Getters_Setters
        // The number of inputs into the neuron
        public int NumInputs { get; private set; }

        // The weights for each input
        //public List<double> Weights { get; private set; }
        public double[] Weights { get; private set; }
        #endregion

        #region Custom
        // Constructor
        public Neuron(int num_inputs)
        {
            NumInputs = num_inputs + 1;
            //Weights = new List<double>(NumInputs);
            Weights = new double[NumInputs];
            // We need an additional weight for the bias neuron hence the + 1
            for(int i = 0; i < NumInputs; ++i)
            {
                // Initialize weights with random values
                //Weights.Add(Random.Range(0.0f, 1.0f));
                Weights[i] = Random.Range(0.0f, 1.0f);
            }
        }
        #endregion
    }
}
