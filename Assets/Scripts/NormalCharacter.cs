using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;

public class NormalCharacter : MonoBehaviour {
    public float speed = 5f;
    public float changeDirectionTime = 2f;

    private float _time;
    private Vector2 _direction;

    private void Update() {
        _time += Time.deltaTime;
        if (_time >= changeDirectionTime) {
            _time = 0;
            RandomDirection();
        }

        Move();
    }

    private void RandomDirection() {
        var x = Random.Range(-100f, 100f);
        var y = Random.Range(-100f, 100f);
        _direction = new Vector2(x, y).normalized;
    }

    private void Move() {
        var curPos = transform.position;
        var newPos = curPos + (Vector3) (_direction * (speed * Time.deltaTime));
        transform.position = newPos;
    }
}