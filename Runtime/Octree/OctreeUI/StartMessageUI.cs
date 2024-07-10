using UnityEngine.UI;
using UnityEngine;
using Octree.Agent; 

namespace Octree.UI
{
    using TMPro;
    public class StartMessageUI : MonoBehaviour
    {
        private Button button;
    
        private void Start()
        {
            InformationUI.Instance.SetActive(false);
            PathStatisticsUI.Instance.SetActive(false);
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
            button = transform.GetChild(0).GetComponent<Button>();
            button.onClick.AddListener(delegate
            {
                OnButtonClick();
            });
        }

        private void OnButtonClick()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            GlobalNavigationParameters.canMove = true;
          
            PathStatisticsUI.Instance.SetActive(true);
            InformationUI.Instance.SetActive(true);
            transform.gameObject.SetActive(false);
            PathPlanningUI.Instance.SetParameters();
        }
    }
}