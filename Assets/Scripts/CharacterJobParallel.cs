using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

/// <summary>
/// https://docs.unity3d.com/Manual/JobSystemParallelForJobs.html
/// </summary>
[BurstCompile]
public struct CharacterJobParallel : IJobParallelFor {
    private readonly float _speed;
    private readonly float _changeDirectionTime;
    private readonly float _deltaTime;
    private Unity.Mathematics.Random _randomGenerator;

    private NativeArray<float> _moveTime;
    private NativeArray<Vector2> _moveDirection;
    private NativeArray<Vector2> _currentPosition;

    public CharacterJobParallel(
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
    
    public void Execute(int index) {
        TryRandomDirection(index);
        Move(index);
        SlowProcess();
    }
    
    private void SlowProcess() {
        for (var i = 0; i < 10000; i++) {
            var _ = Math.Sqrt(9999);
        }
    }

    private void TryRandomDirection(int index) {
        var moveTime = _moveTime[index];
        moveTime += _deltaTime;
        if (moveTime > _changeDirectionTime) {
            var x = _randomGenerator.NextFloat(-100f, 100f);
            var y = _randomGenerator.NextFloat(-100f, 100f);
            _moveDirection[index] = new Vector2(x, y).normalized;
            _moveTime[index] = 0;
        }
        else {
            _moveTime[index] = moveTime;
        }
    }

    private void Move(int index) {
        var curPos = _currentPosition[index];
        var curDirection = _moveDirection[index];
        var newPos = curPos + curDirection * (_speed * _deltaTime);
        _currentPosition[index] = newPos;
    }
}