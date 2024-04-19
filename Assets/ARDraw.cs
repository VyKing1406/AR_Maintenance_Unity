using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class ARDraw : MonoBehaviour
{
    Vector3 anchor = new Vector3(0, 0, 0.3f);

    bool anchorUpdate = false; //should anchor update or not

    public GameObject linePrefab; //prefab which genrate the line for user

    LineRenderer lineRenderer; //LineRenderer which connects and generate line

    public List<LineRenderer> lineList = new List<LineRenderer>(); //List of lines drawn

    public Transform linePool; //parent object

    public bool use; //code is in use or not

    public bool startLine = false; //already started line or not




    void Start()
    {
        DepthSource.SwitchToRawDepth(false);
    }

    void Update()
    {
        if (use)
        {
            if (startLine)
            {
                UpdateAnchor();
                DrawLinewContinue();
            }
        }
    }

    void UpdateAnchor()
    {
        Vector3 ScreenPosition = Input.mousePosition;
        Vector2Int depthXY = DepthSource.ScreenToDepthXY(
            (int)ScreenPosition.x, (int)ScreenPosition.y);
        float realDepth = DepthSource.GetDepthFromXY(depthXY.x, depthXY.y, DepthSource.DepthArray);
        ScreenPosition.z = realDepth;
        anchor = DepthSource.ARCamera.ScreenToWorldPoint(ScreenPosition);
    }


    public void MakeLineRenderer()
    {
        GameObject tempLine = Instantiate(linePrefab);
        tempLine.transform.SetParent(linePool);
        tempLine.transform.position = Vector3.zero;
        tempLine.transform.localScale = new Vector3(1, 1, 1);

        anchorUpdate = true;
        UpdateAnchor();

        lineRenderer = tempLine.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, anchor);

        startLine = true;
        lineList.Add(lineRenderer);
    }


    public void DrawLinewContinue()
    {
        lineRenderer.positionCount = lineRenderer.positionCount + 1;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, anchor);
    }

    //to start drawing line
    public void StartDrawLine()
    {
        use = true;

        if (!startLine)
        {
            MakeLineRenderer();
        }
    }

    //to End the line which user started drawing
    public void StopDrawLine()
    {
        use = false;
        startLine = false;
        lineRenderer = null;

        anchorUpdate = false;
    }

    //To Undo Last Drawn Line
    public void Undo()
    {
        LineRenderer undo = lineList[lineList.Count - 1];
        Destroy(undo.gameObject);
        lineList.RemoveAt(lineList.Count - 1);
    }

    //To clear all the lines
    public void ClearScreen()
    {
        foreach (LineRenderer item in lineList)
        {
            Destroy(item.gameObject);
        }
        lineList.Clear();
    }


}






