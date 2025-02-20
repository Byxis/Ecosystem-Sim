using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Carnivorous : Animal
{
    protected List<GameObject> m_preyList = new();

    protected override void Update()
    {
        base.Update();

        m_preyList.Clear();
    }

    //Method to add colliders to the list of detected colliders
    protected override void HandleColliderDetection(Collider collider)
    {
        base.HandleColliderDetection(collider);

        if (collider.GetComponent<Animal>() != null && collider.GetComponent<Animal>().GetType() == typeof(Herbivorous))
        {
            m_preyList.Add(collider.gameObject);
        }
    }

    //Method to make the animal eat
    protected override void Eat()
    {
        if (m_preyList.Count > 0 && m_food < m_foodtreshold)
        {
            Attack(m_preyList[0]);
        }
    }

    //Method to make the animal attack
    protected void Attack(GameObject _prey)
    {
        m_navMeshAgent.speed = m_runspeed;
        m_navMeshAgent.SetDestination(_prey.transform.position);
        if (Vector3.Distance(transform.position, _prey.transform.position) < 1)
        {
            Destroy(_prey);
            this.m_food = 100;
            m_navMeshAgent.speed = m_speed;
        }
    }

    //Good for debugging
    protected void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawSolidDisc(transform.position, Vector3.up, m_fov);
    }
}
