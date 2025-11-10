using UnityEngine;
using UnityEngine.Events;

public class EventTriggerScript : MonoBehaviour
{
    public UnityEvent OnPlayerEnter;
    bool done = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && done == false)
        {
            print("TRIGGERED");
            done = true;

            OnPlayerEnter.Invoke();
        }
    }
}
