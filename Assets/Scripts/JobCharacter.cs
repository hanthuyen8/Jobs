using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;

public class JobCharacter : MonoBehaviour {
    public float speed = 5f;
    public float changeDirectionTime = 2f;

    private Unity.Mathematics.Random _randomGenerator;
    private JobHandle _jobHandle;

    [ReadOnly] private NativeArray<float> _moveTime;
    [ReadOnly] private NativeArray<Vector2> _moveDirection;
    [ReadOnly] private NativeArray<Vector2> _currentPosition;

    private void Awake() {
        _randomGenerator = new Unity.Mathematics.Random((uint) Random.Range(1, 1000));
        _moveTime = new NativeArray<float>(1, Allocator.Persistent);
        _moveDirection = new NativeArray<Vector2>(1, Allocator.Persistent);
        _currentPosition = new NativeArray<Vector2>(1, Allocator.Persistent);
    }

    private void OnDestroy() {
        _moveTime.Dispose();
        _moveDirection.Dispose();
        _currentPosition.Dispose();
    }

    private void Update() {
        var job = new JobCharacterJob(
            speed,
            changeDirectionTime,
            Time.deltaTime,
            _randomGenerator,
            _moveTime,
            _moveDirection,
            _currentPosition
        );
        _jobHandle = job.Schedule();
    }

    private void LateUpdate() {
        _jobHandle.Complete();
        transform.position = _currentPosition[0];
    }
}

[BurstCompile]
public struct JobCharacterJob : IJob {
    private readonly float _speed;
    private readonly float _changeDirectionTime;
    private readonly float _deltaTime;
    private Unity.Mathematics.Random _randomGenerator;

    private NativeArray<float> _moveTime;
    private NativeArray<Vector2> _moveDirection;
    private NativeArray<Vector2> _currentPosition;

    public JobCharacterJob(
        float speed,
        float changeDirectionTime,
        float deltaTime,
        Unity.Mathematics.Random randomGenerator,
        NativeArray<float> moveTime,
        NativeArray<Vector2> moveDirection,
        NativeArray<Vector2> currentPosition
    ) {
        _speed = speed;
        _changeDirectionTime = changeDirectionTime;
        _deltaTime = deltaTime;
        _randomGenerator = randomGenerator;

        _moveTime = moveTime;
        _moveDirection = moveDirection;
        _currentPosition = currentPosition;
    }

    public void Execute() {
        TryRandomDirection();
        Move();
    }

    private void TryRandomDirection() {
        var moveTime = _moveTime[0];
        moveTime += _deltaTime;
        if (moveTime > _changeDirectionTime) {
            var x = _randomGenerator.NextFloat(-100f, 100f);
            var y = _randomGenerator.NextFloat(-100f, 100f);
            _moveDirection[0] = new Vector2(x, y).normalized;
            _moveTime[0] = 0;
        }
        else {
            _moveTime[0] = moveTime;
        }
    }

    private void Move() {
        var curPos = _currentPosition[0];
        var curDirection = _moveDirection[0];
        var newPos = curPos + curDirection * (_speed * _deltaTime);
        _currentPosition[0] = newPos;
    }
}