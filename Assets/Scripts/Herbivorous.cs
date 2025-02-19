using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Herbivorous : Animal
{
    protected List<Vector3> m_vegetationList = new List<Vector3>();

    // Update is called once per frame
    protected override void Update()
    {
        this.m_navMeshAgent.speed = m_speed;

        base.Update();

        foreach (Collider collider in m_detectedColliders)
        {
            //If the animal sees another animal
            if (collider.GetComponent<Animal>() != null)
            {
                //If the animal is a herbivorous
                if (collider.GetComponent<Animal>().GetType() == typeof(Carnivorous))
                {
                    this.m_navMeshAgent.SetDestination(transform.position + (transform.position - collider.transform.position));
                    this.m_navMeshAgent.speed = m_speed * 3;
                }
            }

            //If the animal sees vegetation
            if (collider.CompareTag("Vegetation"))
            {
                m_vegetationList.Add(collider.ClosestPoint(transform.position));
            }
        }

        //If the animal needs to drink water
        if (m_water < 60)
        {
            if (m_waterList.Count > 0)
            {
                this.m_navMeshAgent.SetDestination(NearestWaterSource());
                this.m_water = 100;
            }
            else
            {
                RandomMove();
            }
        }

        //If the animal needs to eat
        if (m_food < 80)
        {
            if (m_vegetationList.Count > 0)
            {
                this.m_navMeshAgent.SetDestination(NearestVegetation());
                this.m_food = 100;
            }
            else
            {
                RandomMove();
            }
        }
    }

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
