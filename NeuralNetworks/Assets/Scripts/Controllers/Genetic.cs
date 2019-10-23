using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public class Genetic : MonoBehaviour
    {
        #region Variables
        public float m_command_delay = 0.25f;

        private TankBoid m_actor;
        private int m_command_index;
        private SpriteRenderer m_sprite_renderer;

        #endregion

        #region Getters_Setters
        public Chromosome Chromosome { get; set; }
        public NeuralNetwork NeuralNetwork { get; set; }
        public List<Transform> Mines { get; set; }
        #endregion

        #region Unity

        private void Awake()
        {
            m_actor = GetComponent<TankBoid>();
            m_sprite_renderer = m_actor.SpriteRenderer;

            Mines = new List<Transform>();
            Chromosome = new Chromosome();
            NeuralNetwork = new NeuralNetwork(2, 1, 2, 2);
            StartCoroutine(Move());
        }

        private void Update()
        {
            m_sprite_renderer.color = Chromosome.Elite ? Color.blue : Color.green;
        }

        private void FixedUpdate()
        {
            if (m_command_index + 1 >= NeuralNetwork.Weights.Length)
            {
                m_command_index = 0;
            }

            // Pass all parameters to the character control script.
            //m_actor.Steering = new Vector2(0, (float)Chromosome.Genes[m_command_index]);
            //m_actor.AltSteering = new Vector2(0, (float)Chromosome.Genes[m_command_index + 1]);
            m_actor.Steering = new Vector2(0, (float)NeuralNetwork.Layers[2].Neurons[0].Weights[0]);
            m_actor.AltSteering = new Vector2(0, (float)NeuralNetwork.Layers[2].Neurons[1].Weights[0]);
        }

        private void OnTriggerEnter2D(Collider2D i_collision)
        {
            if (i_collision.name.Contains("Mine"))
            {
                if (!Mines.Contains(i_collision.transform))
                {
                    Mines.Add(i_collision.transform);
                    i_collision.gameObject.SetActive(false);
                }
            }
        }

        #endregion

        #region Custom
        private IEnumerator Move()
        {
            while (true)
            {
                yield return new WaitForSeconds(m_command_delay);
                m_command_index++;
            }
        }

        public void ComputeFitness()
        {
            float exp = Mines.Count / 30.0f;
            Chromosome.Fitness = Mathf.Pow(2.0f, exp);
        }

        public void ResetTank()
        {
            ComputeFitness();
            m_actor.Position = Vector2.zero;
            m_actor.Velocity = Vector2.zero;
            m_actor.Orientation = 0.0f;
            Mines.Clear();
        }
        #endregion
    }
}