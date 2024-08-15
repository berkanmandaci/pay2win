using System.Collections.Generic;
using System.Linq;
using _Project.Core.Scripts.Enums;
using _Project.Runtime.Core.Extensions.Singleton;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Core.Scripts
{
    public class UIManager : SingletonBehaviour<UIManager>
    {

        [SerializeField] private List<GameObject> uiLayersList;

        public Dictionary<UILayerKey, Transform> UILayers;

        private Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();


        private void Awake()
        {
            Init();
        }
        private void Init()
        {
            UILayers = uiLayersList.ToDictionary(x => x.GetComponent<UILayer>().Key, x => x.transform);
        }
        private async UniTask<GameObject> InitializeUI(string prefabName)
        {
            if (uiElements.TryGetValue(prefabName, out GameObject screenGo))
            {
                Debug.LogWarning($"UI Element {prefabName} is already initialized.");
                return screenGo;
            }

            try
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(prefabName);
                GameObject uiPrefab = await handle.Task;

                if (uiPrefab != null)
                {
                    uiElements[prefabName] = uiPrefab;
                    Debug.Log($"UI Element {prefabName} initialized successfully.");


                    var baseUIScreenView = uiPrefab.GetComponent<BaseUIScreenView>();

                    if (UILayers.TryGetValue(baseUIScreenView.UILayerKey, out Transform layerTransform))
                    {
                        uiPrefab.transform.SetParent(layerTransform);
                        uiPrefab.transform.localPosition = Vector3.zero;
                    }

                    return uiPrefab;
                }
                else
                {
                    Debug.LogError("Failed to initialize UI prefab: Prefab is null.");
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load UI prefab: {ex.Message}");
                return null;
            }
        }

        // Method to open the UI
        public async UniTask<GameObject> OpenUI(string uiName)
        {
            if (!uiElements.TryGetValue(uiName, out GameObject screenGo))
            {
                screenGo = await InitializeUI(uiName);
                if (screenGo == null)
                {
                    Debug.LogError($"Failed to open UI Element {uiName}: Prefab could not be loaded or initialized.");
                    return null;
                }
            }

            GameObject uiInstance = Instantiate(screenGo, transform);
            uiInstance.name = uiName;
            uiInstance.SetActive(true);

            return uiInstance;
        }

        // Method to close the UI
        public void CloseUI(string uiName)
        {
            if (uiElements.TryGetValue(uiName, out GameObject screenGo))
            {
                if (screenGo != null)
                {
                    Destroy(screenGo);
                    Debug.Log($"UI Element {uiName} closed successfully.");
                }
                else
                {
                    Debug.LogWarning($"UI Element {uiName} is not currently active.");
                }
            }
            else
            {
                Debug.LogError("UI Element not found.");
            }
        }
    }
}
