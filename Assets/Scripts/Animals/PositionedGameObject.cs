using UnityEngine;

public class PositionedGameObject
{
    private Vector3 m_position;
    private GameObject m_gameObject;

    public PositionedGameObject(Vector3 _position, GameObject _gameObject)
    {
        m_position = _position;
        m_gameObject = _gameObject;
    }

    public PositionedGameObject(Collider _collider, GameObject _animal)
    {
        m_position = _collider.ClosestPoint(_animal.transform.position);
        m_gameObject = _collider.gameObject;
    }

    public Vector3 Position
    {
        get => m_position;
        set => m_position = value;
    }

    public GameObject GameObject
    {
        get => m_gameObject;
        set => m_gameObject = value;
    }
}
