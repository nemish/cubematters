using UnityEngine;
using System.Collections;

public class SenseChecker : MonoBehaviour {

    private bool _touchingObstacle = false;

    void OnTriggerStay(Collider other){
        if (other.gameObject.tag == Constants.obstacleTag) {
            _touchingObstacle = true;
        }
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == Constants.obstacleTag) {
            _touchingObstacle = true;
        }
    }

    void OnTriggerExit(Collider other){
        if (other.gameObject.tag == Constants.obstacleTag) {
            _touchingObstacle = false;
        }
    }

    public bool IsTouchingObstacle() {
        return _touchingObstacle;
    }
}
