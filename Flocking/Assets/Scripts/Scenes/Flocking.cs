using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Flocking : MonoBehaviour
{
    private const float SPAWN_RANGE = 10.0f;

    private Queue<Flock> m_flock = new Queue<Flock>();

    public Flock m_flocking_boid_prefab;
    public int m_flock_count = 50;
    public Slider m_neighbor_distance_slider;
    public Slider m_separation_slider;
    public Slider m_alignment_slider;
    public Slider m_cohesion_slider;

    private Flock CreateFlockingBoid()
    {
        Flock new_boid = Instantiate<Flock>(m_flocking_boid_prefab);
        new_boid.gameObject.name = "Flocking Boid";
        new_boid.transform.position = new Vector3(Random.Range(-SPAWN_RANGE, SPAWN_RANGE), Random.Range(-SPAWN_RANGE, SPAWN_RANGE), 0.0f);
        new_boid.gameObject.SetActive(true);
        return new_boid;
    }

    private void Start()
    {
        m_neighbor_distance_slider.value = m_flocking_boid_prefab.m_neighbor_distance;
        m_separation_slider.value = m_flocking_boid_prefab.m_separation_weight;
        m_alignment_slider.value = m_flocking_boid_prefab.m_alignment_weight;
        m_cohesion_slider.value = m_flocking_boid_prefab.m_cohesion_weight;
        m_flocking_boid_prefab.gameObject.SetActive(false);

        m_neighbor_distance_slider.onValueChanged.AddListener(OnNeighborDistanceSliderValueChanged);
        m_separation_slider.onValueChanged.AddListener(OnSeparationSliderValueChanged);
        m_alignment_slider.onValueChanged.AddListener(OnAlignmentSliderValueChanged);
        m_cohesion_slider.onValueChanged.AddListener(OnCohesionSliderValueChanged);

        for (int i = 0; i < m_flock_count; ++i)
        {
            m_flock.Enqueue(CreateFlockingBoid());
        }
    }

    private void OnNeighborDistanceSliderValueChanged(float neighbor_distance)
    {
        foreach (Flock boid in m_flock)
        {
            boid.m_neighbor_distance = neighbor_distance;
        }
    }

    private void OnSeparationSliderValueChanged(float separation_weight)
    {
        foreach (Flock boid in m_flock)
        {
            boid.m_separation_weight = separation_weight;
        }
    }

    private void OnAlignmentSliderValueChanged(float alignment_weight)
    {
        foreach (Flock boid in m_flock)
        {
            boid.m_alignment_weight = alignment_weight;
        }
    }

    private void OnCohesionSliderValueChanged(float cohesion_weight)
    {
        foreach (Flock boid in m_flock)
        {
            boid.m_cohesion_weight = cohesion_weight;
        }
    }
}