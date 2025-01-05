using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public Transform target;
    public bool isActive = false;
    private bool isCorrect = false;

    public GameObject preAlignmentImage;
    public GameObject postAlignmentImage;

    private void Start()
    {
        if (preAlignmentImage != null)
            preAlignmentImage.SetActive(true);

        if (postAlignmentImage != null)
            postAlignmentImage.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (isActive && !isCorrect)
        {
            RotateLaser();
        }
    }

    private void RotateLaser()
    {
        if (isCorrect)
        {
            return;
        }

        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        CheckAlignment();
    }

    private void CheckAlignment()
    {
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        float dotProduct = Vector2.Dot(transform.up, directionToTarget);

        if (dotProduct > 0.99f) 
        {
            isCorrect = true;
            Debug.Log($"Laser {gameObject.name} aligned with the target: {(target ? target.name : "none")}");

            transform.rotation = Quaternion.LookRotation(Vector3.forward, directionToTarget);

            if (preAlignmentImage != null)
                preAlignmentImage.SetActive(false);

            if (postAlignmentImage != null)
                postAlignmentImage.SetActive(true);

            LaserManager.Instance.OnLaserAligned(this);
        }
    }
}


