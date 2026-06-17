using UnityEngine;

public abstract class PickUp : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 50f;
    const string playerString = "Player";
    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
    void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag(playerString))
        {
            OnPickup();
            Destroy(gameObject);
        }        
    }

    protected abstract void OnPickup();
}
