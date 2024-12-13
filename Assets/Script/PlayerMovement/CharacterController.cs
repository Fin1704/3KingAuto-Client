using UnityEngine;
using Spine.Unity;

public class CharacterController : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;  // Tham chiếu đến SkeletonAnimation của Spine2D
    public float moveSpeed = 5f;                 // Tốc độ di chuyển nhân vật
    private Rigidbody2D rb;                      // Rigidbody2D của nhân vật
    private Vector2 movement;                   // Vector lưu hướng di chuyển

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Lấy Rigidbody2D từ nhân vật
    }

    void Update()
    {
        // Lấy input từ bàn phím
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Tính hướng di chuyển
        movement = new Vector2(horizontal, vertical).normalized;

        // Điều chỉnh hướng của Spine animation
        if (horizontal > 0)
        {
            skeletonAnimation.skeleton.ScaleX = 1; // Nhìn sang phải
        }
        else if (horizontal < 0)
        {
            skeletonAnimation.skeleton.ScaleX = -1; // Nhìn sang trái
        }

        // Cập nhật animation dựa trên trạng thái di chuyển
        if (movement.magnitude >= 0.1f)
        {
            PlayAnimation("run", true); // Chạy animation "run"
        }
        else
        {
            PlayAnimation("idle", true); // Chạy animation "idle"
        }
    }

    void FixedUpdate()
    {
        // Di chuyển nhân vật bằng Rigidbody2D
        rb.velocity = movement * moveSpeed;
    }

    private void PlayAnimation(string animationName, bool loop)
    {
        if (skeletonAnimation.AnimationState.GetCurrent(0)?.Animation?.Name != animationName)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
        }
    }
}
