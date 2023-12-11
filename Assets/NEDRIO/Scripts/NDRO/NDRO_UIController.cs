using UnityEngine;
using TMPro;

public class NDRO_UIController : MonoBehaviour
{

    // plane detection anim ui
    public GameObject planeDetectionAnimUI;

    // distance text ui
    public GameObject UISetDistanceText;
    public TextMeshProUGUI txtDistance;

    // 거리 측정 실패 UI


    void Start()
    {

    }

    public void EnablePlaneDetectionAnimUI(bool enable)
    {
        planeDetectionAnimUI.SetActive(enable);
    }

}
