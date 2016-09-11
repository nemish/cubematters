using UnityEngine;
using System.Collections;

public class SenseChecker : MonoBehaviour {

    private bool _touchingObstacle = false;
    private bool _touchingEnemy = false;
    private bool _touchingPlayer = false;
    private bool _isOtherPlayer = false;
    private Transform _otherPlayCube;

    void OnTriggerStay(Collider other){
        handleCollision(other);
    }

    void OnTriggerEnter(Collider other){
        handleCollision(other);
    }

    void handleCollision(Collider other) {
        if (other.gameObject.tag == Constants.obstacleTag) {
            _touchingObstacle = true;
        } else if (other.gameObject.tag == Constants.enemyTag) {
            _touchingEnemy = true;
        } else if (isPlayer(other)) {
            _touchingPlayer = true;
            _isOtherPlayer = transform.parent.parent != other.transform.parent.parent;
            _otherPlayCube = other.transform.parent;
        }
    }

    void OnTriggerExit(Collider other){
        if (other.gameObject.tag == Constants.obstacleTag) {
            _touchingObstacle = false;
        } else if (other.gameObject.tag == Constants.enemyTag) {
            _touchingEnemy = false;
        } else if (isPlayer(other)) {
            _touchingPlayer = false;
            _isOtherPlayer = false;
            _otherPlayCube = null;
        }
    }

    private bool isPlayer(Collider other) {
        return other.gameObject.tag == Constants.PLAYER_TAG;
    }

    public bool IsTouchingOtherPlayer() {
        return _isOtherPlayer;
    }

    public Transform GetTouchingPlayCube() {
        return _otherPlayCube;
    }

    public bool IsTouchingPlayer() {
        return _touchingPlayer;
    }

    public bool IsTouchingNeighbourCube() {
        return IsTouchingPlayer() && !IsTouchingOtherPlayer();
    }

    public bool IsTouchingConnectedNeighbourCube() {
        return _touchingPlayer && !IsTouchingOtherPlayer();
    }

    public bool IsTouchingObstacle() {
        return _touchingObstacle;
    }

    public bool IsTouchingEnemy() {
        return _touchingEnemy;
    }

    public bool ObstacleOk() {
        return !IsTouchingObstacle();
    }

    public Transform GetTouchingConnectedNeighbourCube() {
        return _otherPlayCube;
    }

}
