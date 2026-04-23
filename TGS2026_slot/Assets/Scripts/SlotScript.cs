using UnityEngine;

public class SlotScript : MonoBehaviour
{
    
    
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StopSlotLeft();
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StopSlotCenter();
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StopSlotRight();
        }
    }

    private void StopSlotLeft()
    {
        // code;
    }

    private void StopSlotCenter()
    {
        // code;
    }

    private void StopSlotRight()
    {
        // code;
    }
}
