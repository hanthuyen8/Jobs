using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class JobPool : MonoBehaviour {
    public GameObject characterPrefab;
    public int poolSize = 5000;
    
    public float speed = 5f;
    public float changeDirectionTime = 2f;

    private Unity.Mathematics.Random _randomGenerator;
    private JobHandle _jobHandle;
    private GameObject[] _jobCharacters;

    [ReadOnly] private NativeArray<float> _moveTime;
    [ReadOnly] private NativeArray<Vector2> _moveDirection;
    [ReadOnly] private NativeArray<Vector2> _currentPosition;
    

    private void Awake() {
        _jobCharacters = new GameObject[poolSize];
        var tr = transform;
        for (var i = 0; i < poolSize; i++) {
            _jobCharacters[i] = Instantiate(characterPrefab, tr.position, Quaternion.identity, tr);
        }
        
        _randomGenerator = new Unity.Mathematics.Random((uint) Random.Range(1, 1000));
        _moveTime = new NativeArray<float>(poolSize, Allocator.Persistent);
        _moveDirection = new NativeArray<Vector2>(poolSize, Allocator.Persistent);
        _currentPosition = new NativeArray<Vector2>(poolSize, Allocator.Persistent);
    }
    
    private void OnDestroy() {
        _moveTime.Dispose();
        _moveDirection.Dispose();
        _currentPosition.Dispose();
    }
    
    private void Update() {
        var job = new CharacterJobParallel(
            speed,
            changeDirectionTime,
            Time.deltaTime,
            _randomGenerator,
            _moveTime,
            _moveDirection,
            _currentPosition
        );
        _jobHandle = job.Schedule(_currentPosition.Length, 1);
    }

    private void LateUpdate() {
        _jobHandle.Complete();
        for (var i = 0; i < _jobCharacters.Length; i++) {
            _jobCharacters[i].transform.position = _currentPosition[i];
        }
    }
}