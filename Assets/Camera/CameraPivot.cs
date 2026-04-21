using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    private float targetAngle = 0f;
    private float currentAngle = 0;
    private float mouseSensitivity = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField][Range(1, 50)] private float Zoom = 5f;
    [SerializeField][Range(0, 90)] private float verticalAngle = 45f;

    private Transform parent;
    private Camera cameraChild;

    private void Awake() {
        parent = GetComponentInParent<Transform>();
        cameraChild = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        //float mouseY = Input.GetAxis("Mouse Y");

        if (Input.GetMouseButton(1)) {
            targetAngle += mouseX * mouseSensitivity;
        } else {
            targetAngle = Mathf.Round(targetAngle / 45);
            targetAngle *= 45;
        }
        if (targetAngle < 0) {
            targetAngle += 360;
        } else if (targetAngle > 360) {
            targetAngle -= 360;
        }

        currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(verticalAngle, currentAngle, 0);
        //transform.position = new Vector3(transform.position.x, (parent.position.z - Mathf.Abs(transform.position.z)) / Mathf.Tan(Mathf.Deg2Rad * verticalAngle), transform.position.z);
        cameraChild.orthographicSize = Zoom;
    }
}
