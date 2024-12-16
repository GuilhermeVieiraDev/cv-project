using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] 
    private float speed;
    private Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;
        transform.rotation =  Quaternion.Lerp(transform.rotation, target.rotation, speed * Time.deltaTime);
        
    }
}
