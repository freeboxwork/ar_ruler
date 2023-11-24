using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class RulerManager : MonoBehaviour
{

    public ARRaycastManager _rayManager;
    public List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    public Vector2 _centerVec;
    public Transform _camPivot;
    public Transform _pivot;
    public Transform _rulerPool;
    public Transform _rulerObj;
    RulerObjST _nowRulerObj;
    public List<RulerObjST> _rulerObjList = new List<RulerObjST>();
    bool rulerEnable = false;
    Vector3 _rulerPosSave;

    public Button btn;


    void Start()
    {
        _centerVec = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

        _pivot.gameObject.SetActive(rulerEnable);

        if (_rayManager.Raycast(_centerVec, _hits, TrackableType.PlaneWithinPolygon))
        {
            // 첫번째로 측정된 면의 정보를 가지고옴
            Pose hitPose = _hits[0].pose;
            rulerEnable = true;
            _rulerPosSave = hitPose.position;
            _pivot.rotation = Quaternion.Lerp(_pivot.rotation, hitPose.rotation, 0.2f);
            if (_nowRulerObj != null)
            {
                _nowRulerObj.SetObj(hitPose.position);
            }

        }
        else
        {
            rulerEnable = false;
            Quaternion tRot = Quaternion.Euler(90f, 0, 0);
            _pivot.rotation = Quaternion.Lerp(_pivot.rotation, tRot, 0.5f);
        }
    }


    public void MakeRulerObj()
    {
        if (rulerEnable)
        {
            Transform tObj = Instantiate(_rulerObj, _rulerPool);
            tObj.position = Vector3.zero;
            tObj.localScale = Vector3.one;

            RulerObjST tRulerObj = tObj.GetComponent<RulerObjST>();
            tRulerObj._mainCam = _camPivot;
            tRulerObj.SetInits(_rulerPosSave);
            _rulerObjList.Add(tRulerObj);
            _nowRulerObj = tRulerObj;
        }
        else
        {
            _nowRulerObj = null;
        }
    }
}
