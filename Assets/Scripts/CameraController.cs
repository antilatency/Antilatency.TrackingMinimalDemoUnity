using Antilatency.Integration;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float MinHeight = 0.2f;
    public float MaxHeight = 10f;
    public float Radius = 10.0f;

    public float _lookSpeedMouse = 10.0f;
    public float _moveSpeed = 5.0f;
    public float _moveSpeedIncrement = 2.5f;
    public float _turbo = 2.5f;

    private static string _mouseX = "Mouse X";
    private static string _mouseY = "Mouse Y";
    private static string _speedAxis = "Mouse ScrollWheel";
    private static string _vertical = "Vertical";
    private static string _horizontal = "Horizontal";
    private static string _yAxis = "YAxis";

    private void LateUpdate()
    {
        float inputRotateAxisX = 0.0f;
        float inputRotateAxisY = 0.0f;
        if (Input.GetMouseButton(1))
        {
            inputRotateAxisX = Input.GetAxis(_mouseX) * _lookSpeedMouse;
            inputRotateAxisY = Input.GetAxis(_mouseY) * _lookSpeedMouse;
        }

        float inputChangeSpeed = Input.GetAxis(_speedAxis);
        if (inputChangeSpeed != 0.0f)
        {
            _moveSpeed += inputChangeSpeed * _moveSpeedIncrement;
            if (_moveSpeed < _moveSpeedIncrement) _moveSpeed = _moveSpeedIncrement;
        }

        float inputVertical = Input.GetAxis(_vertical);
        float inputHorizontal = Input.GetAxis(_horizontal);
        float inputYAxis = Input.GetAxis(_yAxis);

        bool moved = inputRotateAxisX != 0.0f || inputRotateAxisY != 0.0f || inputVertical != 0.0f || inputHorizontal != 0.0f || inputYAxis != 0.0f;
        if (moved)
        {
            float rotationX = transform.localEulerAngles.x;
            float newRotationY = transform.localEulerAngles.y + inputRotateAxisX;

            float newRotationX = (rotationX - inputRotateAxisY);
            if (rotationX <= 90.0f && newRotationX >= 0.0f)
                newRotationX = Mathf.Clamp(newRotationX, 0.0f, 90.0f);
            if (rotationX >= 270.0f)
                newRotationX = Mathf.Clamp(newRotationX, 270.0f, 360.0f);

            transform.localRotation = Quaternion.Euler(newRotationX, newRotationY, transform.localEulerAngles.z);

            float moveSpeed = Time.deltaTime * _moveSpeed;
            if (Input.GetMouseButton(1))
                moveSpeed *= Input.GetKey(KeyCode.LeftShift) ? _turbo : 1.0f;
            else
                moveSpeed *= Input.GetAxis("Fire1") > 0.0f ? _turbo : 1.0f;
            transform.position += transform.forward * moveSpeed * inputVertical;
            transform.position += transform.right * moveSpeed * inputHorizontal;
            transform.position += Vector3.up * moveSpeed * inputYAxis;
        }

        float dist = Vector3.Distance(transform.position, Vector3.zero);

        if (dist > Radius)
        {
            transform.position *= Radius / dist;
        }

        transform.position = new Vector3(
            transform.position.x,
            Mathf.Clamp(transform.position.y, MinHeight, MaxHeight),
            transform.position.z
        );
    }
}