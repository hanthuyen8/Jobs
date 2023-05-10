using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pool : MonoBehaviour {
    public GameObject characterPrefab;
    public int poolSize = 5000;

    public float speed = 5f;
    public float changeDirectionTime = 2f;

    private float[] _times;
    private Vector2[] _directions;
    private GameObject[] _characters;

    private void Awake() {
        _characters = new GameObject[poolSize];
        _times = new float[poolSize];
        _directions = new Vector2[poolSize];

        var tr = transform;
        for (var i = 0; i < poolSize; i++) {
            _characters[i] = Instantiate(characterPrefab, tr.position, Quaternion.identity, tr);
        }
    }

    private void Update() {
        for (var i = 0; i < _characters.Length; i++) {
            _times[i] += Time.deltaTime;
            if (_times[i] >= changeDirectionTime) {
                _times[i] = 0;
                RandomDirection(i);
            }

            Move(i);
            SlowProcess();
        }
    }

    private void SlowProcess() {
        for (var i = 0; i < 10000; i++) {
            var _ = Math.Sqrt(9999);
        }
    }

    private void RandomDirection(int index) {
        var x = Random.Range(-100f, 100f);
        var y = Random.Range(-100f, 100f);
        _directions[index] = new Vector2(x, y).normalized;
    }

    private void Move(int index) {
        var tr = _characters[index].transform;
        var curPos = (Vector2) tr.position;
        var curDir = _directions[index];
        var newPos = curPos + (curDir * (speed * Time.deltaTime));
        tr.position = newPos;
    }
}