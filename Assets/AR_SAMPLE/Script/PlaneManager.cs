using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneManager : MonoBehaviour
{

    public ARPlaneManager arPlaneManager;

    void Start()
    {


    }

    void GetCurrentPlane()
    {
        foreach (var plane in arPlaneManager.trackables)
        {
            Debug.Log(plane.gameObject.name);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DisableAllPlaneVisuals();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            EnableAllPlaneVisuals();
        }
    }


    public void DisableAllPlaneVisuals()
    {
        foreach (var plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
    }

    public void EnableAllPlaneVisuals()
    {
        foreach (var plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(true);
        }
    }
}
