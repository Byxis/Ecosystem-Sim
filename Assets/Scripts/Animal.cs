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
    [SerializeField] protected float m_runSpeed;
    [SerializeField] protected float m_recoverySpeed;
    [SerializeField] protected float m_energy;
    [SerializeField] protected float m_fov;
    [SerializeField] protected bool m_gender; //True is a male, False is a female

    [SerializeField] protected float m_waterTreshold;
    [SerializeField] protected float m_foodTreshold;

    [Header("References")]
    [SerializeField] protected GameObject m_map;
    
    protected NavMeshAgent m_navMeshAgent;

    protected List<Collider> m_detectedColliders = new();
    protected List<GameObject> m_pretendantsList = new();
    protected List<Vector3> m_waterList = new();

    [SerializeField] protected bool m_isDrinking = false;
    [SerializeField] protected bool m_isEating = false;
    [SerializeField] protected bool m_canReproduce = false;
    [SerializeField] protected float m_reproductionCooldownTime = 0f;

    //Method to set the stats of the animal
    public void SetStats(float _speed, float _runSpeed, float _recoverySpeed, float _energy, float _fov, float _waterTreshold, float _foodTreshold, GameObject _map, bool _gender)
    {
        m_speed = _speed;
        m_runSpeed = _runSpeed;
        m_recoverySpeed = _recoverySpeed;
        m_energy = _energy;
        m_fov = _fov;
        m_waterTreshold = _waterTreshold;
        m_foodTreshold = _foodTreshold;
        m_map = _map;
        m_gender = _gender;
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
            Reproduce();
        }

        m_pretendantsList.Clear();
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

        if (_collider.GetComponent<Animal>() != null && _collider.GetComponent<Animal>().GetType() == this.GetType())
        {
            m_pretendantsList.Add(_collider.gameObject);
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
        if (m_water < m_waterTreshold && m_waterList.Count > 0 || m_isDrinking)
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

    //Method to detect water sources, waterList need to have at least one element
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

    //Method to reproduce with a pretendant of the opposite 
    protected void Reproduce()
    {
        if (m_energy >= 80f && m_canReproduce)
        {
            foreach (GameObject pretendant in m_pretendantsList)
            {
                Animal pretendantAnimal = pretendant.GetComponent<Animal>();

                if (pretendantAnimal != null && pretendantAnimal.m_gender != m_gender && pretendantAnimal.m_energy >= 80f)
                {
                    m_navMeshAgent.SetDestination(pretendant.transform.position);

                    if (Vector3.Distance(transform.position, pretendant.transform.position) < 1f && !this.m_gender)
                    {
                        m_canReproduce = false;
                        pretendantAnimal.m_canReproduce = false;

                        m_energy = Mathf.Max(0, m_energy - 50f);
                        pretendantAnimal.m_energy = Mathf.Max(0, pretendantAnimal.m_energy - 50f);

                        Vector3 spawnPosition = (transform.position + pretendant.transform.position) / 2;
                        GameObject babyAnimalObj = Instantiate(gameObject, spawnPosition, Quaternion.identity);
                        Animal babyAnimal = babyAnimalObj.GetComponent<Animal>();

                        if (babyAnimal != null)
                        {
                            float babySpeed = Random.Range(m_speed, pretendantAnimal.m_speed);
                            float babyRunSpeed = Random.Range(m_runSpeed, pretendantAnimal.m_runSpeed);
                            float babyRecoverySpeed = Random.Range(m_recoverySpeed, pretendantAnimal.m_recoverySpeed);
                            float babyEnergy = Random.Range(m_energy, pretendantAnimal.m_energy);
                            float babyFov = Random.Range(m_fov, pretendantAnimal.m_fov);
                            float babyWaterThreshold = Random.Range(m_waterTreshold, pretendantAnimal.m_waterTreshold);
                            float babyFoodThreshold = Random.Range(m_foodTreshold, pretendantAnimal.m_foodTreshold);
                            bool babyGender = Random.Range(0f, 1f) > 0.5f;

                            babyAnimal.SetStats(babySpeed, babyRunSpeed, babyRecoverySpeed, babyEnergy, babyFov, babyWaterThreshold, babyFoodThreshold, m_map, babyGender);
                        }

                        break;
                    }
                }
            }
        }
    }

    //Method to consume resources each seconds
    protected virtual void Consume()
    {
        m_water = Mathf.Max(0, m_water - 1);
        m_food = Mathf.Max(0, m_food - 1);
        m_energy = Mathf.Min(100, m_energy + m_recoverySpeed);

        if (m_water == 0 || m_food == 0)
        {
            m_health = Mathf.Max(0, m_health - 5);
        }

        if (m_health <= 0)
        {
            Destroy(gameObject);
        }
        if (!m_canReproduce)
        {
            m_reproductionCooldownTime += 1f;

            if (m_reproductionCooldownTime >= 30f)
            {
                m_canReproduce = true;
                m_reproductionCooldownTime = 0f;
            }
        }
    }

    //Required method to be implemented in the child classes
    protected virtual void Eat()
    {

    }
}
