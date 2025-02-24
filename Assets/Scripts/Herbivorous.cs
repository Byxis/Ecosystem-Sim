using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Herbivorous : Animal
{
    [SerializeField] protected List<PositionedGameObject> m_vegetationList = new();
    protected List<GameObject> m_predatorList = new();

    [SerializeField] protected bool m_isFleeing = false;
    private float m_timeEating = 0;
    private float m_totalTimeEating = 0;
    private float m_energyGain = 0;

    protected override void Update()
    {
        base.Update();

        Flee();

        m_predatorList.Clear();
    }

    //Method to add colliders to the list of detected colliders
    protected override void HandleColliderDetection(Collider _collider)
    {
        base.HandleColliderDetection(_collider);

        if (_collider.CompareTag("Vegetation"))
        {
            IVegetation vegetation = _collider.GetComponent<IVegetation>();

            if (vegetation != null && vegetation.IsConsumable())
            {
                m_vegetationList.Add(new PositionedGameObject(_collider, gameObject));
            }
        }

        if (_collider.GetComponent<Animal>() != null && _collider.GetComponent<Animal>().GetType() == typeof(Carnivorous))
        {
            m_predatorList.Add(_collider.gameObject);
        }
    }

    //Method to consume each seconds
    protected override void Consume()
    {
        base.Consume();

        if (m_isFleeing) 
        {
             m_energy = Mathf.Max(m_energy - 20, 0);
        }
    }

    //Method to make the animal flee
    protected void Flee()
    {
        if (m_predatorList.Count > 0)
        {
            m_isFleeing = true;
            m_navMeshAgent.SetDestination(transform.position + (transform.position - m_predatorList[0].transform.position));

            if(m_energy > 0)
            {
                m_navMeshAgent.speed = m_runSpeed;
            }
        }
        else
        {
            m_isFleeing = false;
            m_navMeshAgent.speed = m_speed;
        }
    }

    //Method to make the animal eat
    protected override void Eat()
    {
        if (m_timeEating > 0)
        {
            m_isEating = true;
            m_timeEating -= Time.deltaTime;
            m_food += m_energyGain / m_totalTimeEating * Time.deltaTime;
            PositionedGameObject nearestVegetation = NearestVegetation();
            Debug.Log(Vector3.Distance(nearestVegetation.Position, transform.position));
            if (m_timeEating <= 0 || Vector3.Distance(nearestVegetation.Position, transform.position) > 1.5f)
            {
                m_isEating = false;
                m_timeEating = 0;
                m_energyGain = 0;
            }
            if (m_food >= 100)
            {
                m_food = 100;
                m_isEating = false;
                m_timeEating = 0;
                m_energyGain = 0;
            }
            return;
        }


        if (m_vegetationList.Count > 0 && m_food < m_foodTreshold)
        {
            PositionedGameObject nearestVegetation = NearestVegetation();
            m_navMeshAgent.SetDestination(nearestVegetation.Position);

            if (Vector3.Distance(transform.position, nearestVegetation.Position) < 1)
            {
                IVegetation vegetation = nearestVegetation.GameObject.GetComponent<IVegetation>();

                if (vegetation.IsConsumable())
                {
                    float[] gains = vegetation.Consume();
                    m_energyGain = gains[0];
                    m_totalTimeEating = gains[1];
                    m_timeEating = m_totalTimeEating;
                }
                else
                {
                    if (m_timeEating <= 0)
                        m_vegetationList.Remove(nearestVegetation);
                }
            }
        }
    }

    //Method to get the nearest vegetation
    protected PositionedGameObject NearestVegetation()
    {
        if (m_vegetationList.Count == 1)
        {
            return m_vegetationList[0];
        }
        else
        {
            float distance = Vector3.Distance(m_vegetationList[0].Position, transform.position);
            PositionedGameObject nearestVegetation = m_vegetationList[0];
            for (int i = 1; i < m_vegetationList.Count; i++)
            {
                if (Vector3.Distance(m_vegetationList[i].Position, transform.position) < distance)
                {                    
                    distance = Vector3.Distance(m_vegetationList[i].Position, transform.position);
                    nearestVegetation = m_vegetationList[i];
                }
            }
            return nearestVegetation;
        }
    }

    //Good for debugging
    protected void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawSolidDisc(transform.position, Vector3.up, m_fov);
    }

}
