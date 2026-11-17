using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    public static MouseWorld Instance { get; private set; }

    [SerializeField] private LayerMask mousePlaneLayerMask;
    
    //[SerializeField] private float maxDistance;

    //private PlayerInput playerInput;

    private void Awake()
    {
        if (Camera.main == null)
        {
            Debug.LogError("没有找到主摄像机，请确保场景中有一个标记为MainCamera的摄像机。");
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }


    }

    private void Update()
    {
        transform.position = GetMouseWorldPosition();
    }

    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, mousePlaneLayerMask))
        {
            return raycastHit.point;
        }
        else
        {
           // Debug.LogWarning("鼠标位置未击中任何物体，请检查鼠标平面图层设置。");
            return Vector3.zero; // 返回一个默认值，避免返回未初始化的变量
        }
    }
}
