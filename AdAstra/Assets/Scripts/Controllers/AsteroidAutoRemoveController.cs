using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class AsteroidAutoRemoveController : MonoBehaviour
    {
        private float _delay = 3f;
        private void Update()
        {
            if (_delay > 0f)
            {
                _delay -= Time.deltaTime;
                return;
            }

            if (Vector2.Distance(Vector2.zero, transform.position) > 65f)
            {
                Destroy(this.transform.gameObject);
            }
            else
            {
                _delay = 3f;
            }
        }
    }
}
