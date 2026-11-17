using System;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    public static event EventHandler OnAnyDestroyed;

    [SerializeField] private Transform createDestroyedPrefab;

    private GridPosition gridPosition;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public void Damage()
    {
        Transform createDestroyedTransform = Instantiate(createDestroyedPrefab, transform.position, transform.rotation);

        ApplyExplosionToChildren(createDestroyedTransform, 150f, transform.position, 10f);
        Destroy(gameObject);

        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
