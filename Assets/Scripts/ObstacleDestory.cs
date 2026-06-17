using UnityEngine;

public class ObstacleDestory : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {                
        Destroy(other.gameObject);        
    }
}
