using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RulerObjST : MonoBehaviour
{

    public List<Transform> _objList = new List<Transform>();
    public LineRenderer _lineObj;

    public Transform txtSet;
    public TextMeshPro txtValue;

    public Transform _mainCam;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tVec = _objList[1].transform.position - _objList[0].transform.position;
        txtSet.position = _objList[0].position + tVec * 0.5f;

        float tDis = tVec.magnitude;
        string tDisTxt = string.Format("{0}mm", tDis.ToString("N2"));
        txtValue.text = tDisTxt;
        txtSet.LookAt(_mainCam);
    }


    public void SetInits(Vector3 pos)
    {
        _objList[0].transform.position = pos;
        _lineObj.SetPosition(0, pos);
    }

    public void SetObj(Vector3 pos)
    {
        _objList[1].transform.position = pos;
        _lineObj.SetPosition(1, pos);
    }
}
