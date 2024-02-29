using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private Rigidbody2D _pivot;
    [SerializeField] private float _detouchDelay;
    [SerializeField] private float _respawnDelay;

    private Rigidbody2D _currentBallRigidbody;
    private SpringJoint2D _currentBallSpringJoint;

    private Vector2 _touchPosition;
    private Vector3 _worldPosition;
    private bool _isDrugging;

    void Start()
    {
        SpawnNewBall();
    }

    void Update()
    {
        if (_currentBallRigidbody == null)
            return;

        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (_isDrugging)
                LaunchBall();

            _isDrugging = false;
            
            return;
        }

        _isDrugging = true;
        _currentBallRigidbody.isKinematic = true;
        
        _touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        _worldPosition = _mainCamera.ScreenToWorldPoint(_touchPosition);
        _currentBallRigidbody.position = _worldPosition;
    }

    private void LaunchBall()
    {
        _currentBallRigidbody.isKinematic = false;
        _currentBallRigidbody = null;
        Invoke(nameof(DetouchBall), _detouchDelay);
    }

    private void DetouchBall()
    {
        _currentBallSpringJoint.enabled = false;
        _currentBallSpringJoint = null;

        Invoke(nameof(SpawnNewBall), _respawnDelay);
    }

    private void SpawnNewBall()
    {
        GameObject _ballInstance = Instantiate(_ballPrefab, _pivot.position, Quaternion.identity);
        _currentBallRigidbody = _ballInstance.GetComponent<Rigidbody2D>();
        _currentBallSpringJoint = _ballInstance.GetComponent<SpringJoint2D>();
        _currentBallSpringJoint.connectedBody = _pivot;
    }
}
