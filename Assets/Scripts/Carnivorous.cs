using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Carnivorous : Animal
{
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
                if (collider.GetComponent<Animal>().GetType() == typeof(Herbivorous))
                {
                    Hunt(collider.gameObject);
                }
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
    }

    protected void Hunt(GameObject _prey)
    {
        this.m_navMeshAgent.speed = m_speed * 2;
        this.m_navMeshAgent.SetDestination(_prey.transform.position);
        if (Vector3.Distance(transform.position, _prey.transform.position) < 1)
        {
            Destroy(_prey.gameObject);
            this.m_food = 100;
        }
    }

    //Good for debugging
    protected void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawSolidDisc(transform.position, Vector3.up, m_fov);
    }
}
