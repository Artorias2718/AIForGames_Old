using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class NeuralNetwork
    {
        #region Variables
        private const double BIAS = 0.75;
        #endregion

        #region Getters_Setters
        public int NumInputs { get; set; }
        public int NumWeights { get; private set; }
        public int NumOutputs { get; set; }
        public int NumHiddenLayers { get; set; }
        public int NeuronsPerHiddenLayer { get; set; }
        //public List<NeuronLayer> Layers { get; private set; }
        public NeuronLayer[] Layers { get; private set; }
        //public List<double> Weights
        public double[] Weights
        {
            get
            {
                // Store weights
                List<double> weights = new List<double>();

                // For each layer
                for(int i = 0; i < NumHiddenLayers + 1; ++i)
                {
                    // For each neuron 
                    for(int j = 0; j < Layers[i].NumNeurons; ++j)
                    {
                        // For each weight
                        for(int k = 0; k < Layers[i].Neurons[j].NumInputs; ++k)
                        {
                            weights.Add(Layers[i].Neurons[j].Weights[k]);
                        }
                    }
                }

                return weights.ToArray();
            }

            set
            {
                NumWeights = 0;

                // For each layer
                for(int i = 0; i < NumHiddenLayers + 1; ++i)
                {
                    // For each neuron 
                    for(int j = 0; j < Layers[i].NumNeurons; ++j)
                    {
                        // For each weight
                        for(int k = 0; k < Layers[i].Neurons[j].NumInputs; ++k)
                        {
                            Layers[i].Neurons[j].Weights[k] = Weights[NumWeights++];
                        }
                    }
                }
            }
        }
        public double[] Outputs { get; set; }
        #endregion

        #region Custom
        public NeuralNetwork(int num_inputs, int num_hidden_layers, int neurons_per_hidden_layer, int num_outputs)
        {
            NumInputs = num_inputs;
            NumHiddenLayers = num_hidden_layers;
            NeuronsPerHiddenLayer = neurons_per_hidden_layer;
            NumOutputs = num_outputs;

            //Layers = new List<NeuronLayer>(NumHiddenLayers + 2);
            Layers = new NeuronLayer[NumHiddenLayers + 2];

            // Create the layers of the network
            if(NumHiddenLayers > 0)
            {
                // Input layer
                //Layers.Add(new NeuronLayer(NeuronsPerHiddenLayer, NumInputs));
                Layers[0] = new NeuronLayer(NeuronsPerHiddenLayer, NumInputs);

                // Hidden layers
                for(int i = 1; i < NumHiddenLayers + 1; ++i)
                {
                    //Layers.Add(new NeuronLayer(NeuronsPerHiddenLayer, NeuronsPerHiddenLayer));
                    Layers[i] = new NeuronLayer(NeuronsPerHiddenLayer, NeuronsPerHiddenLayer);
                }

                // Output layer
                //Layers.Add(new NeuronLayer(NumOutputs, NeuronsPerHiddenLayer));
                Layers[Layers.Length - 1] = new NeuronLayer(NumOutputs, NeuronsPerHiddenLayer);
            }
            else
            {
                // Create output layer
                //Layers.Add(new NeuronLayer(NumOutputs, NumInputs));
                Layers[0] = new NeuronLayer(NumOutputs, NumInputs);
            }
        }

        public double[] Update(List<double> inputs)
        {
            // Stores resultant outputs from each layer
            List<double> outputs = new List<double>();

            int weightIdx = 0;

            // First, check that we have the correct number of inputs
            if(inputs.Count != NumInputs)
            {
                // Just return an empty list if this is incorrect
                Debug.Log("Inconsistent number of inputs");
                return outputs.ToArray();
            }

            // For each layer
            for (int i = 0; i < NumHiddenLayers + 1; ++i)
            {
                if (i > 0)
                {
                    inputs = outputs;
                }

                outputs.Clear();
                weightIdx = 0;

                // For each neuron, sum up its input-weight products.
                // Then, normalize it with the activation function

                for (int j = 0; j < Layers[i].NumNeurons; ++j)
                {
                    double net_input = 0;
                    int num_inputs = Layers[i].Neurons[j].NumInputs;

                    // For each weight
                    for (int k = 0; k < NumInputs; ++k)
                    {
                        // Sum up each input-weight product
                        if (weightIdx >= inputs.Count - 1)
                        {
                            weightIdx = inputs.Count - 1;
                        }
                        else
                        {
                            net_input += Layers[i].Neurons[j].Weights[k] * inputs[weightIdx++];
                        }
                    }

                    // Add in the bias
                    net_input += BIAS * Layers[i].Neurons[j].Weights[NumInputs - 1];

                    // Normalize the activation value,
                    // then store outputs from each layer as they are generated

                    outputs.Add(TanH(net_input));
                    weightIdx = 0;
                }
            }
            return outputs.ToArray();
        }

        public double TanH(double activation)
        {
            return 2.0 * Sigmoid(2.0 * activation) - 1.0;
        }

        private double Sigmoid(double activation)
        {
            double exp = System.Math.Exp(activation);
            return exp / (1.0 + exp);
        }
        #endregion
    }
}