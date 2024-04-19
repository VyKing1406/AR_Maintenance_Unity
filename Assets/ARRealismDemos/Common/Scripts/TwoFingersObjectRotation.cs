using static UnityEngine.TouchPhase;
using UnityEngine;

public class TwoFingersObjectRotation : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    private Vector2 _startPosition;
    private float rotationSpeed = 1;
    private float tiltSpeed = 1;
    void Update()
    {
        if (Input.touchCount == 2)
        {
            var touchOne = Input.GetTouch(0);
            var touchTwo = Input.GetTouch(1);

            if (AnyTouchBegan(ref touchOne, ref touchTwo))
            {
                _startPosition = touchTwo.position - touchOne.position;
            }

            if (AnyTouchMoved(ref touchOne, ref touchTwo))
            {
                var currVector = touchTwo.position - touchOne.position;
                var angle = Vector2.SignedAngle(_startPosition, currVector);
                _target.transform.rotation = Quaternion.Euler(0.0f, _target.transform.rotation.eulerAngles.y + angle, 0.0f);
                _startPosition = currVector;
            }
        }
    }

    private static bool AnyTouchMoved(ref Touch touchOne, ref Touch touchTwo) =>
        touchOne.phase == Moved || touchTwo.phase == Moved;

    private static bool AnyTouchBegan(ref Touch touchOne, ref Touch touchTwo) =>
        touchOne.phase == Began || touchTwo.phase == Began;
}
