using UnityEngine;

public class Bushes : MonoBehaviour, IVegetation
{
    private int m_maxNbOfFruits = 6;
    [SerializeField] private int m_nbOfFruits;
    private float m_fruitGrowthTime = 10f;

    [SerializeField] private float m_energyProvided = 20f;
    [SerializeField] private float m_timeToConsume = 3f;

    private void Start()
    {
        InvokeRepeating(nameof(updateFruits), 1f, m_fruitGrowthTime);
    }

    private void updateFruits()
    {
        if (m_nbOfFruits < m_maxNbOfFruits)
        {
            m_nbOfFruits++;
        }
    }

    public float[] Consume()
    {
        if (IsConsumable())
        {
            m_nbOfFruits--;
            return new float[] { m_energyProvided, m_timeToConsume };
        }
        return new float[] { 0, 0 };
    }

    public bool IsConsumable()
    {
        return m_nbOfFruits > 0;
    }
}
