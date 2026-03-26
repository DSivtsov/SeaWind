using UnityEngine;

    public class SimpleBehaviourScript : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;

        private void Update()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }
