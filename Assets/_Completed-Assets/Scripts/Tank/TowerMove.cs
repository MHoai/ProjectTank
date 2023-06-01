using UnityEngine;
using UnityEngine.UI;

public class TowerMove: MonoBehaviour
{
    public float rotationInterval = 5f; // Thời gian giữa các lần xoay
    public float rotationAngle = 30f;   // Góc xoay
    public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.

    private void Start()
    {
        // Bắt đầu gọi hàm RotateTower sau mỗi rotationInterval giây, lặp lại mỗi rotationInterval giây.
        InvokeRepeating("RotateTower", 0f, rotationInterval);
    }

    private void RotateTower()
    {
        // Xoay vị trí của tháp
        transform.Rotate(0f, rotationAngle, 0f);
    }

}
