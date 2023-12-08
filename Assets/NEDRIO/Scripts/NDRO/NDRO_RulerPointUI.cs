using UnityEngine;
namespace NDRO.Ruler
{
    public class NDRO_RulerPointUI : MonoBehaviour
    {

        public RectTransform pointA;
        public RectTransform pointB;

        void Start()
        {

        }


        void Update()
        {

        }

        public void SetPosition(Vector3 scrPointA, Vector3 scrPointB)
        {
            pointA.anchoredPosition = scrPointA;
            pointB.anchoredPosition = scrPointB;
        }
    }
}
