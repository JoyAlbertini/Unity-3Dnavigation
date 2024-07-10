using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Octree.UI
{
    public class PathStatisticsUI : MonoBehaviour
    {
        public static PathStatisticsUI Instance { get; private set; }

        private void Awake()
        {
            textMesh = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            SetText("", "", "", "", "");
            Instance = this;
        }

        private TextMeshProUGUI textMesh;

        private float roundFloat(float val)
        {
            return Mathf.Round(val * 100f) / 100f;
        }

        private void SetText(string time, string travelledDistance, string lineOfSights, string closed, string ratio)
        {
            textMesh.text = "<color=orange>Time: </color>" + time + " ms" +"\n" +
                            "<color=orange>Distance: </color>" + travelledDistance + "\n" +
                            "<color=orange>Line-of-sight: </color>" + lineOfSights + "\n" +
                            "<color=orange>Closed: </color>" + closed + "\n" +
                            "<color=orange>Tradeoff: </color>" + ratio;
        }

        public void SetData(float time, float travelledDistance, uint lineOfSights, int closed)
        {
            time = Mathf.Round(time * 100000f) / 100000f;
            float ratio = roundFloat((time / (1 / travelledDistance)) / 100);
            travelledDistance = roundFloat(travelledDistance);
            SetText(time.ToString(), travelledDistance.ToString(), lineOfSights.ToString(), closed.ToString(), ratio.ToString());

        }

        public void SetActive(bool val)
        {
            transform.gameObject.SetActive(val);
        }
    }
}