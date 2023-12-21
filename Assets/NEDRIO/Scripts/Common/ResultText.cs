using UnityEngine;
using TMPro;

public class ResultText : MonoBehaviour
{
    public TextMeshPro text;

    void Start()
    {

    }


    public void SetPostion(Vector3 pos)
    {
        text.transform.position = pos;
    }

    public void SetRoation(Vector3 rot)
    {
        text.transform.eulerAngles = rot;
    }
    public void SetTxtValue(string txt)
    {
        text.text = txt + "cm";
    }

}
