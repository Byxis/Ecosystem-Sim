using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Animal : MonoBehaviour
{
    public float m_health = 100f;
    public float m_water = 100f;
    public float m_food = 100f;
    public float m_speed = 2f;
    public float m_runspeed = 4f;
    public float m_recoveryspeed = 2f;
    public float m_energy = 100f;
    public float m_fov = 3f;

    public float m_watertreshold = 60f;
    public float m_foodtreshold = 60f;

    public GameObject m_map;

    protected List<Collider> m_detectedColliders = new();
    protected List<Vector3> m_waterList = new();
    protected NavMeshAgent m_navMeshAgent;

    protected bool m_isDrinking = false;
    protected bool m_isEating = false;

    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_navMeshAgent.speed = m_speed;

        InvokeRepeating(nameof(Consume), 1f, 1f);
    }

    protected virtual void Update()
    {
        AddColliders();

        Drink();
        Eat();

        if (!m_isDrinking && !m_isEating)
        {
            Move();
        }
    }

    //Method to add colliders to the list of detected colliders
    protected virtual void AddColliders()
    {
        m_detectedColliders.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_fov);
        foreach (Collider collider in colliders)
        {
            m_detectedColliders.Add(collider);

            HandleColliderDetection(collider);
        }
    }

    //Method to handle the detection of colliders
    protected virtual void HandleColliderDetection(Collider _collider)
    {
        if (_collider.CompareTag("Water"))
        {
            m_waterList.Add(_collider.ClosestPoint(transform.position));
        }
    }

    //Method to move the animal randomly
    protected void Move()
    {
        Bounds bounds = m_map.GetComponent<Renderer>().bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        if (!m_navMeshAgent.hasPath)
        {
            m_navMeshAgent.SetDestination(new Vector3(x, m_map.transform.position.y, z));
        }
    }

    //Method to Drink
    protected void Drink()
    {
        if (m_water < m_watertreshold && m_waterList.Count > 0 || m_isDrinking)
        {
            Vector3 nearestWater = NearestWaterSource();
            m_navMeshAgent.SetDestination(nearestWater);

            if (Vector3.Distance(transform.position, nearestWater) < 1)
            {
                m_isDrinking = true;
                m_water = Mathf.Min(m_water + 10 * Time.deltaTime, 100f);

                if (m_water == 100)
                {
                    m_isDrinking = false;
                }
            }
        }
        else
        {
            m_isDrinking = false;
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

    //Method to consume resources each seconds
    protected virtual void Consume()
    {
        m_water = Mathf.Max(0, m_water - 1);
        m_food = Mathf.Max(0, m_food - 1);
        m_energy = Mathf.Min(100, m_energy + m_recoveryspeed);

        if (m_water == 0 || m_food == 0)
        {
            m_health = Mathf.Max(0, m_health - 5);
        }

        if (m_health <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Eat()
    {

    }
}
