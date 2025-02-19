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
        base.Update();
        
        foreach (Collider collider in m_detectedColliders)
        {

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
    }
}
