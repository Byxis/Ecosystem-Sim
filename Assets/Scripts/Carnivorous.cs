using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class Carnivorous : Animal
{
    protected List<GameObject> m_preyList = new();

    [SerializeField] protected bool m_isHunting = false;

    protected override void Update()
    {
        base.Update();

        m_preyList.Clear();
    }

    //Method to add colliders to the list of detected colliders
    protected override void HandleColliderDetection(Collider _collider)
    {
        base.HandleColliderDetection(_collider);

        if (_collider.GetComponent<Animal>() != null && _collider.GetComponent<Animal>().GetType() == typeof(Herbivorous))
        {
            m_preyList.Add(_collider.gameObject);
        }
    }

    //Method to consume each seconds
    protected override void Consume()
    {
        base.Consume();

        if (m_isHunting)
        {
            m_energy = Mathf.Max(m_energy - 15, 0);
        }
    }

    //Method to make the animal eat
    protected override void Eat()
    {
        if (m_preyList.Count > 0 && m_food < m_foodtreshold)
        {
            m_isHunting = true;
            Attack(m_preyList[0]);
        }
        else
        {
            m_isHunting = false;
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
            m_food = 100;
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
