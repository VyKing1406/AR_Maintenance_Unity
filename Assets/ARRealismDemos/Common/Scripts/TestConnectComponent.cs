using UnityEngine;

public class TestConnectComponent : MonoBehaviour
{

    public GameObject parentObject;
    public Transform content;
    public Transform arCorePawn;
    public LineRenderer lineConnect;
    private void Start()
    {
        // Lấy các thành phần từ parentObject
        // GameObject content = parentObject.transform.Find("Content").gameObject;
        // if(content == null) {
        //     Debug.Log("nullllll rooooooooooooooooiiiiiii");
        // }
        // GameObject arCorePawn = parentObject.transform.Find("ARCorePawn").gameObject;
        // LineRenderer lineConnect = parentObject.transform.Find("LineConnect").GetComponent<LineRenderer>();
        lineConnect.positionCount = 2;
    }

    private void Update()
    {
        lineConnect.SetPosition(0, content.position);
        lineConnect.SetPosition(1, arCorePawn.position);
    }
}