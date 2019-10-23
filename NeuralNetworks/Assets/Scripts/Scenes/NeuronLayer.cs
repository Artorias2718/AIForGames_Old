using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class NeuronLayer
    {
        #region Variables

        #endregion

        #region Getters_Setters
        public int NumNeurons { get; private set; }
        //public List<Neuron> Neurons { get; private set; }
        public Neuron[] Neurons { get; private set; }
        #endregion

        #region Custom
        public NeuronLayer(int num_neurons, int num_inputs_per_neuron)
        {
            NumNeurons = num_neurons;
            //Neurons = new List<Neuron>(num_neurons);
            Neurons = new Neuron[NumNeurons];
            for(int i = 0; i < NumNeurons; ++i)
            {
                //Neurons.Add(new Neuron(num_inputs_per_neuron));
                Neurons[i] = new Neuron(num_inputs_per_neuron);
            }
        }

        #endregion
    }
}