using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Herbivorous : Animal
{
    protected List<Vector3> m_vegetationList = new();
    protected List<GameObject> m_predatorList = new();

    protected override void Update()
    {
        base.Update();

        Flee();

        m_predatorList.Clear();
    }

    //Method to add colliders to the list of detected colliders
    protected override void HandleColliderDetection(Collider collider)
    {
        base.HandleColliderDetection(collider);

        if (collider.CompareTag("Vegetation"))
        {
            m_vegetationList.Add(collider.ClosestPoint(transform.position));
        }

        if (collider.GetComponent<Animal>() != null && collider.GetComponent<Animal>().GetType() == typeof(Carnivorous))
        {
            m_predatorList.Add(collider.gameObject);
        }
    }

    //Method to make the animal flee
    protected void Flee()
    {
        if (m_predatorList.Count > 0)
        {

            m_navMeshAgent.SetDestination(transform.position + (transform.position - m_predatorList[0].transform.position));
            m_navMeshAgent.speed = m_runspeed*3;
        }
        else
        {
            m_navMeshAgent.speed = m_speed;
        }
    }

    //Method to make the animal eat
    protected override void Eat()
    {
        if ((m_vegetationList.Count > 0 && m_food < m_foodtreshold) || isEating)
        {
            Vector3 nearestVegetation = NearestVegetation();
            m_navMeshAgent.SetDestination(nearestVegetation);

            if (Vector3.Distance(transform.position, nearestVegetation) < 1)
            {
                isEating = true;

                m_food = Mathf.Min(m_food + 10 * Time.deltaTime, 100f);

                if (m_food == 100)
                {
                    isEating = false;
                }
            }
        }
        else
        {
            isEating = false;
        }
    }

    //Method to get the nearest vegetation
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
