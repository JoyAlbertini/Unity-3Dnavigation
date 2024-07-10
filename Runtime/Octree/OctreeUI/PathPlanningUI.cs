using UnityEngine;
using UnityEngine.UI;
using Octree.Agent;
using Octree.Agent.Pathfinding;
using TMPro;
using Octree.Agent.Utils;
using UnityEngine.SceneManagement;
using System.Collections;
namespace Octree.UI
{
    public class PathPlanningUI : MonoBehaviour
    {
        public static PathPlanningUI Instance { get; private set; }

        private void Awake()
        {
            Instance = this; 
        }

        private TMP_Dropdown multiSingle;
        private TMP_Dropdown drop;
        private Slider sliderG;
        private TextMeshProUGUI sliderGtext;
        private Slider sliderH;
        private TextMeshProUGUI sliderHtext;
        private Toggle toggleCleanStart;
        private Toggle togglePOSTSmoothing;
        private Toggle togglePathStatistics;
        private Slider enemySpeed;
        private Button shuffle;
        private OctreeSource source;
        private TargetInstantiator targetInstantiator;
        private TMP_InputField inputField;
        private Toggle toggleReplanning;
        private MultiSingle multiSingelVal; 
        private PFA algorithm;
        private float G;
        private float H;
        private bool cleanStart;
        private bool postSmoothing;
        private bool statistics;
        private bool replanning; 

        private void Start()
        {
            multiSingle = transform.GetChild(0).GetComponent<TMP_Dropdown>();
            drop = transform.GetChild(1).GetComponent<TMP_Dropdown>();
            sliderG = transform.GetChild(2).GetComponent<Slider>();
            sliderGtext = transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
            sliderH = transform.GetChild(3).GetComponent<Slider>();
            sliderHtext = transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>();
            toggleCleanStart = transform.GetChild(4).GetComponent<Toggle>();
            togglePOSTSmoothing = transform.GetChild(5).GetComponent<Toggle>();
            togglePathStatistics = transform.GetChild(6).GetComponent<Toggle>();
            enemySpeed = transform.GetChild(7).GetComponent<Slider>();
            shuffle = transform.GetChild(8).GetComponent<Button>();
            inputField = transform.GetChild(9).GetComponent<TMP_InputField>();
            toggleReplanning = transform.GetChild(10).GetComponent<Toggle>();


          source = FindObjectOfType<OctreeSource>();
            targetInstantiator = source.GetComponent<TargetInstantiator>();
            multiSingle.onValueChanged.AddListener(delegate
            {
                OnDropMulti(multiSingle);
            });
            drop.onValueChanged.AddListener(delegate
            {
                OnDropDownChanged(drop);
            });
            sliderG.onValueChanged.AddListener(delegate
            {
                OnSliderCheanged(sliderG, 0);
            });

            sliderH.onValueChanged.AddListener(delegate
            {
                OnSliderCheanged(sliderH, 1);
            });
            toggleCleanStart.onValueChanged.AddListener(delegate
            {
                OnToggleCheanged(toggleCleanStart, 0);
            });
            togglePOSTSmoothing.onValueChanged.AddListener(delegate
            {
                OnToggleCheanged(togglePOSTSmoothing, 1);
            });
            togglePathStatistics.onValueChanged.AddListener(delegate
            {
                OnToggleCheanged(togglePathStatistics, 2);
            });

            enemySpeed.onValueChanged.AddListener(delegate
            {
                OnSliderCheanged(enemySpeed, 2);
            });

            shuffle.onClick.AddListener(delegate
            {
                OnButtonClick(0);
            });

            inputField.onSubmit.AddListener(delegate {
                OnInputFieldCheange(inputField);
            });

            toggleReplanning.onValueChanged.AddListener(delegate
            {
                OnToggleCheanged(toggleReplanning, 3);
            });
            algorithm = PFA.LazyThetaStar;
            drop.value = (int)PFA.LazyThetaStar;
            sliderG.value = 10;
            sliderH.value = 10;
            replanning = true;
            toggleReplanning.isOn = true; 
            G = 1;
            H = 1;
            //setParameters();
            SetActive(false);
        }

        public void UpdateInforamtionUI()
        {
            InformationUI.Instance.UpdateText(SceneManager.GetActiveScene().name, source.targets.Count);
        }

        private void OnDropMulti(TMP_Dropdown drop)
        {
            multiSingelVal = (MultiSingle)drop.value;
            SetParameters();
        }
        
        private void OnInputFieldCheange(TMP_InputField input)
        {
            int nrOfAgents;
            if (int.TryParse(input.text, out nrOfAgents))
            {
                if (nrOfAgents > 400)
                {
                    nrOfAgents = 400;
                }
                input.text = nrOfAgents.ToString();
                targetInstantiator.AddOrDeleteTargets(nrOfAgents);
                InformationUI.Instance.UpdateText(SceneManager.GetActiveScene().name, source.targets.Count);
                StartCoroutine(WaitForPhysics());
            }
        }

        IEnumerator WaitForPhysics()
        {
            GlobalNavigationParameters.globalSpeed = 0;
            yield return new WaitForSeconds(0.3f);
            GlobalNavigationParameters.globalSpeed = enemySpeed.value;
            SetParameters();
        }

        private void OnDropDownChanged(TMP_Dropdown drop)
        {
            algorithm = (PFA) drop.value;
            switch(algorithm)
            {
                case PFA.BestFirstSearch:
                    togglePOSTSmoothing.interactable = true;
                    sliderH.interactable = false;
                    sliderG.interactable = false;
                    sliderG.value = 0;
                    sliderH.value = 10;
                    G = 0;
                    H = 1; 
                    break;
                case PFA.Dijkstra:
                    togglePOSTSmoothing.interactable = true;
                    sliderH.interactable = false;
                    sliderG.interactable = false;
                    sliderG.value = 10;
                    sliderH.value = 0;
                    G = 1;
                    H = 0;
                    break;
                case PFA.Astar:
                    togglePOSTSmoothing.interactable = true;
                    sliderH.interactable = true;
                    sliderG.interactable = true;
                    sliderG.value = 10;
                    sliderH.value = 10;
                    G = 1;
                    H = 1;
                    break;
                default:
                    togglePOSTSmoothing.interactable = false;
                    sliderH.interactable = true;
                    sliderG.interactable = true;
                    sliderG.value = 10;
                    sliderH.value = 10;
                    G = 1;
                    H = 1;
                    break;
            } 

            SetParameters();
        }
        private void OnSliderCheanged(Slider slider, int type)
        {
            switch (type)
            {
                case 0:
                    G = slider.value / 10;
                    SetParameters();
                    break;
                case 1:
                    H = slider.value / 10;
                    SetParameters();
                    break;
                case 2:
                    GlobalNavigationParameters.globalSpeed = slider.value;
                    break;
            }
        }

        private void OnToggleCheanged(Toggle toggle, int type)
        {
            switch(type)
            {
                case 0:
                    cleanStart = toggle.isOn;
                    break;
                case 1:
                    postSmoothing = toggle.isOn;
                    break;
                case 2:
                    statistics = toggle.isOn;
                    break;
                case 3:
                    replanning = toggle.isOn;
                    break;

            }
            SetParameters();
        }

        private void OnButtonClick(int type)
        {
            switch (type)
            {
                case 0:
                    source.ComputeShuffle();
                    break;
                case 1:
                    targetInstantiator.AddTarget();
                    SetParameters();
                    break;
                case 2:
                    targetInstantiator.RemoveTarget();
                    InformationUI.Instance.UpdateText(SceneManager.GetActiveScene().name, source.targets.Count);
                    SetParameters();
                    break;
            }
        }

        public void SetParameters()
        {
            sliderGtext.text = "G: " + G;
            sliderHtext.text = "H: " + H;
            source.SetParamters(multiSingelVal, algorithm, G, H, cleanStart, postSmoothing, statistics, replanning);
            SetBasedOnParameters(source.targets.Count);
        }
        private void SetBasedOnParameters(int nrOfAgents)
        {
            if (nrOfAgents > 30)
            {
                cleanStart = true;
                toggleCleanStart.isOn = true;
                toggleCleanStart.interactable = false;
            }
            else
            {
                toggleCleanStart.interactable = true;
            }
        }

        public void SetActive(bool val)
        {
            inputField.text = source.targets.Count.ToString();
            transform.gameObject.SetActive(val);
        }
    }
}