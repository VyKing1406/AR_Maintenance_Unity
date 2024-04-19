using UnityEngine;
using System.Collections;
public class ObjectData
{
    public Vector3 position {get; set;}
    public Quaternion rotation {get; set;}
    public Vector3 scale {get; set;}

    public ObjectData(ObjectTransform objectTransform)
    {
        position = new Vector3(objectTransform.positionX, objectTransform.positionY, objectTransform.positionZ);
        rotation = new Quaternion(objectTransform.rotationX, objectTransform.rotationY, objectTransform.rotationZ, objectTransform.rotationW);
        scale = new Vector3(objectTransform.scaleX, objectTransform.scaleY, objectTransform.scaleZ);
    }

    public ObjectData()
    {
        position = new Vector3();
        rotation = new Quaternion();
        scale = new Vector3();
    }
}