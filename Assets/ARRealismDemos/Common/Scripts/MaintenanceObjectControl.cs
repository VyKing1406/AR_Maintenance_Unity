using UnityEngine;
using System.Collections;

public class MaintenanceObjectControl : MonoBehaviour
{
    public GameObject Button;
    public static int NUM_OF_OBJECT_INITIAL = 0;


    void Update()
    {
        // if (Input.touchCount > 0)
        // {
        //     bool isSelected = RaycastSelection.StartSelect(Button);
        //     if(isSelected) {
        //         MaintenanceObjectControl.NUM_OF_OBJECT_INITIAL = 1;
        //     }
        // }
    }

    IEnumerator ResetColorAfterDelay(GameObject gameObject)
    {
        yield return new WaitForSeconds(3f);

        Renderer renderer = Button.GetComponent<Renderer>();
        Material material = renderer.material;
        material.color = Color.black;
    }

}