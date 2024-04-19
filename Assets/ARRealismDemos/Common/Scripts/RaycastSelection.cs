using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RaycastSelection : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public static GameObject StartSelect()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {

            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector3 ScreenPosition = touch.position;
                Vector2Int depthXY = DepthSource.ScreenToDepthXY(
            (int)ScreenPosition.x, (int)ScreenPosition.y);
                float realDepth = DepthSource.GetDepthFromXY(depthXY.x, depthXY.y, DepthSource.DepthArray);
                ScreenPosition.z = realDepth;
                Ray ray = Camera.main.ScreenPointToRay(ScreenPosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, realDepth);
                if (hits[0].collider.gameObject != null)
                {
                    return hits[0].collider.gameObject;
                }

            }
        }
        return null;
    }

    IEnumerator ResetColorAfterDelay(GameObject gameObject)
    {
        yield return new WaitForSeconds(3f);

        Renderer renderer = gameObject.GetComponent<Renderer>();
        Material material = renderer.material;
        material.color = Color.black;
    }

    public void EndSelect()
    {
    }

}
