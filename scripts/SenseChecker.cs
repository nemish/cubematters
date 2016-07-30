using UnityEngine;
using System.Collections;

public class SenseChecker : MonoBehaviour {

    private bool _touchingObstacle = false;
    private bool _touchingEnemy = false;
    private bool _touchingPlayer = false;
    private bool _isOtherPlayer = false;

    void OnTriggerStay(Collider other){
        if (other.gameObject.tag == Constants.obstacleTag) {
            _touchingObstacle = true;
        } else if (other.gameObject.tag == Constants.enemyTag) {
            _touchingEnemy = true;
        } else if (isPlayer(other)) {
            _touchingPlayer = true;
            _isOtherPlayer = transform.parent.parent != other.transform.parent.parent;
        }
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == Constants.obstacleTag) {
            _touchingObstacle = true;
        } else if (other.gameObject.tag == Constants.enemyTag) {
            _touchingEnemy = true;
        } else if (isPlayer(other)) {
            _touchingPlayer = true;
            _isOtherPlayer = transform.parent.parent != other.transform.parent.parent;
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
        }
    }

    private bool isPlayer(Collider other) {
        return other.gameObject.tag == Constants.playerTag;
    }

    public bool IsTouchingOtherPlayer() {
        return _isOtherPlayer;
    }

    public bool IsTouchingPlayer() {
        return _touchingPlayer;
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

}
