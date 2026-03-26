using UnityEngine;

    public class ComponentsAndInspectorExample : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private string objectLabel = "Player";

        private void Start()
        {
            Debug.Log($"Object: {objectLabel}, speed: {moveSpeed}");
        }
    }
