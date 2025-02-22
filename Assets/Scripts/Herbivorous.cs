using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Herbivorous : Animal
{
    protected List<Vector3> m_vegetationList = new();
    protected List<GameObject> m_predatorList = new();

    [SerializeField] protected bool m_isFleeing = false;

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
            m_vegetationList.Add(_collider.ClosestPoint(transform.position));
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
        if ((m_vegetationList.Count > 0 && m_food < m_foodTreshold) || m_isEating)
        {
            Vector3 nearestVegetation = NearestVegetation();
            m_navMeshAgent.SetDestination(nearestVegetation);

            if (Vector3.Distance(transform.position, nearestVegetation) < 1)
            {
                m_isEating = true;

                m_food = Mathf.Min(m_food + 10 * Time.deltaTime, 100f);

                if (m_food == 100)
                {
                    m_isEating = false;
                }
            }
        }
        else
        {
            m_isEating = false;
        }
    }

    //Method to get the nearest vegetation, vegetationList need to have at least one element
    protected Vector3 NearestVegetation()
    {
        if (m_vegetationList.Count == 1)
        {
            return m_vegetationList[0];
        }
        else
        {
            float distance = Vector3.Distance(m_vegetationList[0], transform.position);
            Vector3 nearestVegetation = m_vegetationList[0];
            for (int i = 1; i < m_vegetationList.Count; i++)
            {
                if (Vector3.Distance(m_vegetationList[i], transform.position) < distance)
                {
                    distance = Vector3.Distance(m_vegetationList[i], transform.position);
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
