using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;
using TMPro;

public class LineManager : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public ARPlacementInteractable placementInteractable;
    public TextMeshPro mTex;
    LineRenderer line;
    int pointCount = 0;

    // 연속된 라인을 그릴지 여부 
    public bool continuous;

    void Start()
    {
        placementInteractable.objectPlaced.AddListener(DrawLine);
    }

    // set continuous
    public void ContinuousValueChange()
    {
        continuous = !continuous;
    }


    void DrawLine(ARObjectPlacementEventArgs args)
    {
        pointCount++;

        if (pointCount < 2)
        {
            line = Instantiate(lineRenderer);
            line.positionCount = 1;
        }
        else
        {
            line.positionCount = pointCount;
            if (!continuous)
                pointCount = 0;
        }


        // set position
        line.SetPosition(line.positionCount - 1, args.placementObject.transform.position);
        if (line.positionCount > 1)
        {
            Vector3 pointA = line.GetPosition(line.positionCount - 1);
            Vector3 pointB = line.GetPosition(line.positionCount - 2);
            var dist = Vector3.Distance(pointA, pointB);

            var distText = Instantiate(mTex);
            distText.text = "" + dist;

            Vector3 directionVector = (pointB - pointA);
            Vector3 normal = args.placementObject.transform.up;

            Vector3 upd = Vector3.Cross(directionVector, normal).normalized;
            Quaternion rotation = Quaternion.LookRotation(-normal, upd);

            distText.transform.rotation = rotation;
            distText.transform.position = (pointA + directionVector * 0.5f) + upd * 0.05f;

        }


    }

}
