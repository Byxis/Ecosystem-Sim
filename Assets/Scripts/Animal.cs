using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Animal : MonoBehaviour
{
    //Animal attributes that are common to all animals
    public float m_health = 100f;
    public float m_water = 100f;
    public float m_food = 100f;

    public float m_speed = 2f;
    public float m_recovery_speed = 2f;
    public float m_energy = 100f;

    public float m_fov = 3f;

    //Map GameObject
    public GameObject m_map;

    protected List<Collider> m_detectedColliders = new List<Collider>();
    protected List<Vector3> m_waterList = new List<Vector3>();

    //NavMeshAgent component of the animal
    protected NavMeshAgent m_navMeshAgent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_navMeshAgent.speed = m_speed;
    }

    // Update is called once per frame
    protected virtual void Update()
    {

        m_detectedColliders.Clear();

        //When the animal sees something in its field of vision
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_fov);
        foreach (Collider collider in colliders)
        {
            m_detectedColliders.Add(collider);

            if (collider.CompareTag("Water"))
            {
                m_waterList.Add(collider.ClosestPoint(transform.position));
            }
        }

            RandomMove();

    }

    //Method to move the animal randomly
    protected void RandomMove()
    {
        Bounds bounds = m_map.GetComponent<Renderer>().bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        if (!m_navMeshAgent.hasPath)
        {
            m_navMeshAgent.SetDestination(new Vector3(x, m_map.transform.position.y, z));
        }
    }

    //Method to detect water sources
    protected Vector3 NearestWaterSource()
    {
        if (m_waterList.Count == 1)
            {
                return m_waterList[0];
            }
        else
            {
                float distance = Vector3.Distance(m_waterList[0], transform.position);
                Vector3 nearestWater = m_waterList[0];
                for (int i = 1; i < m_waterList.Count; i++)
                {
                    if (Vector3.Distance(m_waterList[i], transform.position) < distance)
                    {
                        distance = Vector3.Distance(m_waterList[i], transform.position);
                        nearestWater = m_waterList[i];
                    }
                }
                return nearestWater;
        }
    }   
}
