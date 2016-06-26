using UnityEngine;
using System.Collections;

public class SenseChecker : MonoBehaviour {

    private bool _touchingObstacle = false;
    private bool _touchingEnemy = false;
    private bool _touchingPlayer = false;

    void OnTriggerStay(Collider other){
        if (other.gameObject.tag == Constants.obstacleTag) {
            _touchingObstacle = true;
        } else if (other.gameObject.tag == Constants.enemyTag) {
            _touchingEnemy = true;
        } else if (other.gameObject.tag == Constants.playerTag) {
            _touchingPlayer = true;
        }
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == Constants.obstacleTag) {
            _touchingObstacle = true;
        } else if (other.gameObject.tag == Constants.enemyTag) {
            _touchingEnemy = true;
        } else if (other.gameObject.tag == Constants.playerTag) {
            _touchingPlayer = true;
        }
    }

    void OnTriggerExit(Collider other){
        if (other.gameObject.tag == Constants.obstacleTag) {
            _touchingObstacle = false;
        } else if (other.gameObject.tag == Constants.enemyTag) {
            _touchingEnemy = false;
        } else if (other.gameObject.tag == Constants.playerTag) {
            _touchingPlayer = true;
        }
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

}
