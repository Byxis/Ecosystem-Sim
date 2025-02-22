using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject m_herbivorousPrefabs;
    [SerializeField] private GameObject m_carnivorousPrefabs;

    [Header("Number of animals at the spawn")]
    [SerializeField] private int m_herbivorousNumber = 15;
    [SerializeField] private int m_carnivorousNumber = 5;

    [Header("Herbivorous Stats")]
    [SerializeField] private float m_herbivorousEnergy = 100f;
    [SerializeField] private float m_herbivorousFov = 5f;

    [SerializeField] private float m_herbivorousWaterTreshold = 30f;
    [SerializeField] private float m_herbivorousFoodTreshold = 50f;

    [Header("Carnivorous Stats")]
    [SerializeField] private float m_carnivorousEnergy = 100f;
    [SerializeField] private float m_carnivorousFov = 5f;

    [SerializeField] private float m_carnivorousWaterTreshold = 30f;
    [SerializeField] private float m_carnivorousFoodTreshold = 40f;

    [Header("Map")]
    [SerializeField] private GameObject m_map;

    void Start()
    {
        SpawnAnimals(m_herbivorousPrefabs, m_herbivorousNumber, true);
        SpawnAnimals(m_carnivorousPrefabs, m_carnivorousNumber, false);
    }

    //Method to spawn animals on the map with the given prefab and count of animals 
    private void SpawnAnimals(GameObject _prefab, int _count, bool _isHerbivore)
    {
        Bounds bounds = m_map.GetComponent<Renderer>().bounds;

        for (int i = 0; i < _count; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                m_map.transform.position.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            GameObject animal = Instantiate(_prefab, randomPosition, Quaternion.identity);
            Animal animalScript = animal.GetComponent<Animal>();

            if (animalScript != null)
            {
                if (_isHerbivore)
                {
                    SetupHerbivoreStats(animalScript);
                }
                else
                {
                    SetupCarnivoreStats(animalScript);
                }
            }
        }
    }

    //Method to set up the stats of the herbivorous animals
    private void SetupHerbivoreStats(Animal _animal)
    {
        _animal.SetStats(
            Random.Range(2f, 3f),
            Random.Range(6f, 8f),
            Random.Range(1f, 2f),
            m_herbivorousEnergy,
            m_herbivorousFov,
            m_herbivorousWaterTreshold,
            m_herbivorousFoodTreshold,
            m_map
        );
    }

    //Method to set up the stats of the carnivorous animals
    private void SetupCarnivoreStats(Animal _animal)
    {
        _animal.SetStats(
            Random.Range(2f, 3f),
            Random.Range(6f, 8.5f),
            Random.Range(1f, 2f),
            m_carnivorousEnergy,
            m_carnivorousFov,
            m_carnivorousWaterTreshold,
            m_carnivorousFoodTreshold,
            m_map
        );
    }
}
