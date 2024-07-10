using UnityEngine;
using TMPro;

namespace Octree.UI
{
    public class InformationUI : MonoBehaviour
    {
        public static InformationUI Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }

        private TextMeshProUGUI infos;

        private void Start()
        {
            infos = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            PathPlanningUI.Instance.UpdateInforamtionUI();
        }

        public void UpdateText(string sceneName, int targets)
        {
            infos.text = "World: " + sceneName + "\n" + "Nr targets: " + targets; 
        }

        public void SetActive(bool val)
        {
            transform.gameObject.SetActive(val);
        }

    }
}