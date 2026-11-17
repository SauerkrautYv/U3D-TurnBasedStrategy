using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 200f;
    [SerializeField] private TrailRenderer 拖尾;
    [SerializeField] private Transform 子弹命中粒子效果预制体;
    private Vector3 targetPosition;

    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(
        transform.position,
        targetPosition,
        moveSpeed * Time.fixedDeltaTime);

        if (transform.position == targetPosition)
        {
            OnArrived();
        }
    }

    private void OnArrived()
    {
        //让拖尾独立，防止立即销毁，之后会通过TrailRenderer的功能自动销毁
        拖尾.transform.parent = null;

        Destroy(gameObject);

        Instantiate(子弹命中粒子效果预制体, targetPosition, Quaternion.identity);
    }
}
