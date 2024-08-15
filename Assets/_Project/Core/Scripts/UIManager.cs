using System.Collections.Generic;
using _Project.Runtime.Core.Extensions.Singleton;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace _Project.Core.Scripts
{
    public class UIManager : SingletonBehaviour<UIManager>
    {
        private Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();

        private void InitializeUI(string prefabName)
        {
            if (uiElements.ContainsKey(prefabName))
            {
                Debug.LogWarning($"UI Element {prefabName} is already initialized.");
                return;
            }

            Addressables.LoadAssetAsync<GameObject>(prefabName).Completed += OnPrefabLoaded;
        }

        private void OnPrefabLoaded(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject uiPrefab = handle.Result;
                if (uiPrefab != null)
                {
                    uiElements[uiPrefab.name] = uiPrefab;
                    Debug.Log($"UI Element {uiPrefab.name} initialized successfully.");
                }
                else
                {
                    Debug.LogError("Failed to initialize UI prefab: Prefab is null.");
                }
            }
            else
            {
                Debug.LogError($"Failed to load UI prefab: {handle.OperationException}");
            }
        }

        // Method to open the UI
        public void OpenUI(string uiName)
        {
            if (!uiElements.ContainsKey(uiName))
            {
                Debug.LogError($"UI Element {uiName} has not been initialized.");
                return;
            }

            GameObject uiPrefab = uiElements[uiName];
            if (uiPrefab != null)
            {
                InitializeUI(uiName);
                
                GameObject uiInstance = Instantiate(uiPrefab, transform);
                uiInstance.name = uiName;
                uiInstance.SetActive(true);
            }
            else
            {
                Debug.LogError($"Failed to open UI Element {uiName}: Prefab reference is null.");
            }
        }

        // Method to close the UI
        public void CloseUI(string uiName)
        {
            Transform uiTransform = transform.Find(uiName);
            if (uiTransform != null)
            {
                Destroy(uiTransform.gameObject);
            }
            else
            {
                Debug.LogWarning($"UI Element {uiName} is not currently active.");
            }
        }
    }
}
