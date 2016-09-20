using UnityEngine;
using System.Collections;

public class SenseChecker : MonoBehaviour {

    private bool _touchingObstacle = false;
    private bool _touchingEnemy = false;
    private bool _touchingPlayer = false;
    private bool _isFreePlayerCube = false;
    private bool _isSame = false;
    private bool _isOtherPlayer = false;
    private bool _isSecondPlayer = false;
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
        } else if (isPlayerCube(other)) {
            _isSecondPlayer = false;
            if (isSecondPlayer(other)) {
                _isSecondPlayer = true;
            }
            _touchingPlayer = true;
            Transform mainOther = other.transform.parent.parent;
            _isFreePlayerCube = mainOther == null;
            _isOtherPlayer = transform.parent.parent != mainOther && !_isFreePlayerCube;
            _isSame = transform.parent.parent == mainOther;
            _otherPlayCube = other.transform.parent;
        }
    }

    private bool isPlayerCube(Collider other) {
        Transform p = other.gameObject.transform.parent;
        return p.gameObject.tag == Constants.PLAYER_CUBE_CONTAINER_TAG;
    }

    void OnTriggerExit(Collider other){
        if (other.gameObject.tag == Constants.obstacleTag) {
            _touchingObstacle = false;
        } else if (other.gameObject.tag == Constants.enemyTag) {
            _touchingEnemy = false;
        } else if (isPlayerCube(other)) {
            if (isSecondPlayer(other)) {
                _isSecondPlayer = false;
            }
            _isFreePlayerCube = false;
            _touchingPlayer = false;
            _isOtherPlayer = false;
            _isSame = false;
            _otherPlayCube = null;
        }
    }

    private bool isSecondPlayer(Collider other) {
        Transform p = other.gameObject.transform.parent;
        return p && p.parent && p.parent.gameObject.tag == Constants.SECOND_PLAYER_TAG;
    }

    public bool IsTouchingOtherPlayer() {
        // return _isOtherPlayer;
        return IsTouchingPlayer() && !_isSame;
    }

    public bool IsTouchingFreePlayCube() {
        Debug.Log(string.Format("IsTouchingFreePlayCube free {0} other {1} parent {2} this {3}, semi parent {4}", _isFreePlayerCube, _otherPlayCube, transform.parent.parent, transform, transform.parent));
        return _isFreePlayerCube;
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
