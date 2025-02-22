using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Animal : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float m_health;
    [SerializeField] protected float m_water;
    [SerializeField] protected float m_food;
    [SerializeField] protected float m_speed;
    [SerializeField] protected float m_runspeed;
    [SerializeField] protected float m_recoveryspeed;
    [SerializeField] protected float m_energy;
    [SerializeField] protected float m_fov;

    [SerializeField] protected float m_watertreshold;
    [SerializeField] protected float m_foodtreshold;

    [Header("References")]
    [SerializeField] protected GameObject m_map;
    
    protected NavMeshAgent m_navMeshAgent;

    protected List<Collider> m_detectedColliders = new();
    protected List<Vector3> m_waterList = new();

    protected bool m_isDrinking = false;
    protected bool m_isEating = false;

    //Method to set the stats of the animal
    public void SetStats(float _speed, float _runSpeed, float _recoverySpeed, float _energy, float _fov, float _waterTreshold, float _foodTreshold, GameObject _map)
    {
        m_speed = _speed;
        m_runspeed = _runSpeed;
        m_recoveryspeed = _recoverySpeed;
        m_energy = _energy;
        m_fov = _fov;
        m_watertreshold = _waterTreshold;
        m_foodtreshold = _foodTreshold;
        m_map = _map;
    }

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

    //Required method to be implemented in the child classes
    protected virtual void Eat()
    {

    }
}
