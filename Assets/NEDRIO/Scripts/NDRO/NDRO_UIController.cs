using UnityEngine;
using TMPro;

public class NDRO_UIController : MonoBehaviour
{

    // plane detection anim ui
    public GameObject UI_InfoAnim_PlaneDetection;
    // 거리 측정 실패 UI SET
    public GameObject UI_InfoAnimDistanceFail;

    // distance text ui
    public GameObject UISetDistanceText;
    public TextMeshProUGUI txtDistance;
    public GameObject objAddPointBtn;




    void Start()
    {

    }

    public void EnablePlaneDetectionAnimUI(bool enable)
    {
        UI_InfoAnim_PlaneDetection.SetActive(enable);
    }

    public void EnableUISetDistanceText(bool enable)
    {
        UISetDistanceText.SetActive(enable);
        objAddPointBtn.SetActive(enable);
        EnableDistanceFailUI(!enable);
    }

    public void SetDistanceText(float distance)
    {
        txtDistance.text = "대상까지 " + distance.ToString("N0") + " cm";
    }

    public void EnableDistanceFailUI(bool enable)
    {
        if (UI_InfoAnim_PlaneDetection.activeSelf == true) return;
        UI_InfoAnimDistanceFail.SetActive(enable);
    }

}
