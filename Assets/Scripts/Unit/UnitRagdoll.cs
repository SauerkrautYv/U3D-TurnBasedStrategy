using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform 布娃娃骨骼根节点;

    public void Setup(Transform 原始骨骼根节点)
    {
        MatchAllChildTransforms(原始骨骼根节点, 布娃娃骨骼根节点);

        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        ApplyExplosionToRagdoll(布娃娃骨骼根节点, 300f, transform.position + randomDir, 10f);
    }

    private void MatchAllChildTransforms(Transform root, Transform clone)
    {
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if (cloneChild != null)
            {
                cloneChild.SetPositionAndRotation(child.position, child.rotation);

                MatchAllChildTransforms(child, cloneChild);
            }
        }
    }

    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
