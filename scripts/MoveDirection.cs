using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveDirection {
    public enum Direction {Left, Right, Up, Down};
    private Dictionary<Direction, Direction> oppositeDirections = new Dictionary<Direction, Direction>();
    private Dictionary<Direction, Vector3> toVectorMap = new Dictionary<Direction, Vector3>();
    private Direction currentDirection;
    private Direction primaryDirection;

    public MoveDirection(Direction dir) {
        primaryDirection = dir;
        currentDirection = primaryDirection;
        oppositeDirections.Add(Direction.Left, Direction.Right);
        oppositeDirections.Add(Direction.Right, Direction.Left);
        oppositeDirections.Add(Direction.Down, Direction.Up);
        oppositeDirections.Add(Direction.Up, Direction.Down);

        toVectorMap.Add(Direction.Left, Vector3.left);
        toVectorMap.Add(Direction.Right, Vector3.right);
        toVectorMap.Add(Direction.Up, Vector3.forward);
        toVectorMap.Add(Direction.Down, Vector3.back);
    }

    public Direction GetOpposite() {
        return _getOppositeDirection(currentDirection);
    }

    public void SetOpposite() {
        currentDirection = GetOpposite();
    }

    public bool IsVertical() {
        return currentDirection == Direction.Up || currentDirection == Direction.Down;
    }

    public bool IsHorizontal() {
        return !IsVertical();
    }

    public bool IsPrimary() {
        return currentDirection == primaryDirection;
    }

    public int GetSign() {
        if (currentDirection == Direction.Right || currentDirection == Direction.Up) {
            return 1;
        }
        return -1;
    }

    public Vector3 AsVector() {
        return toVectorMap[currentDirection];
    }

    public Vector3 AsVectorOpposite() {
        return toVectorMap[_getOppositeDirection(currentDirection)];
    }

    public void SetExact(Direction dir) {
        currentDirection = dir;
    }

    public bool needCheckerCompensationRatio() {
        return currentDirection == Direction.Left || currentDirection == Direction.Up;
    }

    public void Invert() {
        primaryDirection = _getOppositeDirection(primaryDirection);
    }

    public bool IsLeft () {
        return currentDirection == Direction.Left;
    }

    public bool IsRight () {
        return currentDirection == Direction.Right;
    }

    public bool IsUp () {
        return currentDirection == Direction.Up;
    }

    public bool IsDown () {
        return currentDirection == Direction.Down;
    }

    private Direction _getOppositeDirection(Direction dir) {
        return oppositeDirections[dir];
    }

}