using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Homing : MonoBehaviour
{
    Rigidbody2D rb;
    float _timer = 0;
    private bool _homingOn = false;
    [SerializeField] private float _detectSpeed = 12f;
    [SerializeField] private float _lockOnTime = 0.8f;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;

        if (!_homingOn)
        {
            transform.Translate(Vector3.up * 9f * Time.deltaTime);
        }

        if (_timer < _lockOnTime)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0)
            {
                _homingOn = false;
                return;
            }
            _homingOn = true;
            GameObject closest;
            closest = enemies.OrderBy(nice => (this.gameObject.transform.position - nice.transform.position).sqrMagnitude).First();

            Vector2 direction = closest.transform.position - this.gameObject.transform.position;
            direction.Normalize();
            rb.velocity = direction * _detectSpeed;
        }
        if (_timer > 1.5)
        {
            Destroy(this.gameObject);
        }
    }
}
