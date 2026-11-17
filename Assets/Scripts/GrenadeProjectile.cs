using System;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static EventHandler OnAnyGrenadeExploaded;

    [SerializeField] private Transform 手榴弹爆炸VFX预制件;
    [SerializeField] private TrailRenderer 拖尾Renderer;
    [SerializeField] private AnimationCurve arcY动画曲线;

    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float damageRadius = 4f;

    private Vector3 targetPosition;
    private Action OnGrenadeBehaviourComplete;
    private float totalDistance;
    private Vector3 positionXZ;

    private void FixedUpdate()
    {
        Vector3 moveDir = (targetPosition - positionXZ).normalized;

        positionXZ += moveDir * moveSpeed * Time.fixedDeltaTime;

        float reachedTargetDistance = .2f;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;

        float maxHeight = totalDistance / 4f;

        float positionY = arcY动画曲线.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        if (Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
        {
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);

            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(30);
                }
                if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructibleCrate))
                {
                    destructibleCrate.Damage();
                }

            }

            OnAnyGrenadeExploaded?.Invoke(this, EventArgs.Empty);

            拖尾Renderer.transform.parent = null;

            Instantiate(手榴弹爆炸VFX预制件, targetPosition + Vector3.up * 1f, Quaternion.identity);
            OnGrenadeBehaviourComplete();
            Destroy(gameObject);
        }

    }

    public void Setup(GridPosition gridPosition, Action OnGrenadeBehaviourComplete)
    {
        this.OnGrenadeBehaviourComplete = OnGrenadeBehaviourComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

        positionXZ = transform.position;
        positionXZ.y = targetPosition.y;

        transform.position = positionXZ;

        totalDistance = Vector3.Distance(transform.position, targetPosition);
    }
}
