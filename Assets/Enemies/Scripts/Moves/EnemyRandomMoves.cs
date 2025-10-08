using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRandomMoves : DefaultEnemyMovement {
    public bool areMovesFree;
    private bool
        isRmAllowed = true,
        isMovingRandomly = false,
        isReturningToRMArea = false,
        checkCollision = false;
    public Int16 maxRedirections = 3;
    [SerializeField]
    private float
        rmCDMin = 1f,
        rmCDMax = 3f;
    private Coroutine
        movingRandomlyCo = null,
        returningToRMAreaCo = null;
    private MoveDirections
        rmChosenDirection = MoveDirections.None,
        rmPreviousDirection = MoveDirections.None;
    [SerializeField] private List<string> neutralObstacles;
    [SerializeField] private List<MoveDirections> defaultDirections;
    private readonly Dictionary<MoveDirections, bool> DirectionStates = new();
    [SerializeField] private Transform rmArea;
    private Vector3
        rmAreaColliderSize = Vector3.zero,
        rmDestination = Vector3.zero;

    protected override void Awake() {
        base.Awake();
        rmAreaColliderSize = rmArea.GetComponent<BoxCollider2D>().size;
        foreach (MoveDirections md in defaultDirections)
            DirectionStates.Add(md, true);
        RestoreAvailableDirections();
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (isRmAllowed && (
                main.SelfState == EnemyState.Knockbacked
                || main.SelfState == EnemyState.Immobilized
            ))
            isRmAllowed = false;

        if (isRmAllowed && !isMovingRandomly) {
            movingRandomlyCo = StartCoroutine(MovingRandomly());
        } else if (!isRmAllowed && isMovingRandomly) {
            EndMovingRandomly();
        }
    }

    private void OnDisable() {
        InterruptMoves();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (
            (
                collision.gameObject.CompareTag("Player")
                && (isMovingRandomly || isReturningToRMArea)
            ) || (
                neutralObstacles.Contains(collision.gameObject.tag)
                && isReturningToRMArea
            )
        )
            InterruptMoves();
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (neutralObstacles.Contains(collision.gameObject.tag) && checkCollision) {
            if (rmDestination.x > transform.position.x) {
                for (Int16 i = 0; i < collision.contactCount; i++)
                    if (collision.GetContact(i).normal.x < 0f) {
                        StopDirection(); return;
                    }
            } else if (rmDestination.x < transform.position.x) {
                for (Int16 i = 0; i < collision.contactCount; i++)
                    if (collision.GetContact(i).normal.x > 0f) {
                        StopDirection(); return;
                    }
            }
            if (rmDestination.y > transform.position.y) {
                for (Int16 i = 0; i < collision.contactCount; i++)
                    if (collision.GetContact(i).normal.y < 0f) {
                        StopDirection(); return;
                    }
            } else if (rmDestination.y < transform.position.y) {
                for (Int16 i = 0; i < collision.contactCount; i++)
                    if (collision.GetContact(i).normal.y > 0f) {
                        StopDirection(); return;
                    }
            }
        }
    }

    public void RestoreAvailableDirections() {
        foreach (MoveDirections md in defaultDirections) DirectionStates[md] = true;
    }

    public void UpdateCurrentMoveDirections(List<MoveDirections> directionsToAuthorize) {
        foreach (MoveDirections md in defaultDirections) {
            if (directionsToAuthorize.Contains(md) && !DirectionStates[md]) {
                DirectionStates[md] = true;
            } else if (!directionsToAuthorize.Contains(md) && DirectionStates[md]) {
                DirectionStates[md] = false;
            }
        }
    }

    private void StopDirection() {
        rmPreviousDirection = rmChosenDirection;
        rmChosenDirection = MoveDirections.None;
    }

    public override void HandleRegularMoves(bool allow) {
        base.HandleRegularMoves(allow);
        isRmAllowed = allow;
    }

    public override void InterruptMoves() {
        base.InterruptMoves();
        if (isMovingRandomly) EndMovingRandomly();
        if (isReturningToRMArea) EndReturningToRMArea();
    }

    private Vector2 ComputeRMDestination() {
        float objX, objY, modifier, limit;
        switch (rmChosenDirection) {
            case MoveDirections.Right:
                objX = (rmArea.position.x + rmAreaColliderSize.x / 2) - transform.position.x;
                objY = transform.position.y;
                if (objX < 1f) {
                    objX += transform.position.x;
                } else if (objX < 5f) {
                    objX = transform.position.x + UnityEngine.Random.Range(1f, objX);
                } else {
                    objX = transform.position.x + UnityEngine.Random.Range(1f, 5f);
                }
                if (areMovesFree) {
                    modifier = UnityEngine.Random.Range(-2.5f, 2.5f);
                    if (modifier > 0) {
                        objY = (rmArea.position.y + rmAreaColliderSize.y / 2) - transform.position.y;
                        if (objY < 1f) {
                            objY += transform.position.y;
                        } else if (objY < 2.5f) {
                            objY = transform.position.y + UnityEngine.Random.Range(1f, objY);
                        } else {
                            objY = transform.position.y + UnityEngine.Random.Range(1f, 2.5f);
                        }
                    } else if (modifier < 0) {
                        objY = transform.position.y - (rmArea.position.y - rmAreaColliderSize.y / 2);
                        if (objY < 1f) {
                            objY = transform.position.y - objY;
                        } else if (objY < 2.5f) {
                            objY = transform.position.y - UnityEngine.Random.Range(1f, objY);
                        } else {
                            objY = transform.position.y - UnityEngine.Random.Range(1f, 2.5f);
                        }
                    }
                }
                return new Vector2(objX, objY);

            case MoveDirections.Left:
                objX = transform.position.x - (rmArea.position.x - rmAreaColliderSize.x / 2);
                objY = transform.position.y;
                if (objX < 1f) {
                    objX = transform.position.x - objX;
                } else if (objX < 5f) {
                    objX = transform.position.x - UnityEngine.Random.Range(1f, objX);
                } else {
                    objX = transform.position.x - UnityEngine.Random.Range(1f, 5f);
                }
                if (areMovesFree) {
                    modifier = UnityEngine.Random.Range(-2.5f, 2.5f);
                    if (modifier > 0) {
                        objY = (rmArea.position.y + rmAreaColliderSize.y / 2) - transform.position.y;
                        if (objY < 1f) {
                            objY += transform.position.y;
                        } else if (objY < 2.5f) {
                            objY = transform.position.y + UnityEngine.Random.Range(1f, objY);
                        } else {
                            objY = transform.position.y + UnityEngine.Random.Range(1f, 2.5f);
                        }
                    } else if (modifier < 0) {
                        objY = transform.position.y - (rmArea.position.y - rmAreaColliderSize.y / 2);
                        if (objY < 1f) {
                            objY = transform.position.y - objY;
                        } else if (objY < 2.5f) {
                            objY = transform.position.y - UnityEngine.Random.Range(1f, objY);
                        } else {
                            objY = transform.position.y - UnityEngine.Random.Range(1f, 2.5f);
                        }
                    }
                }
                return new Vector2(objX, objY);

            case MoveDirections.Up:
                objX = transform.position.x;
                objY = (rmArea.position.y + rmAreaColliderSize.y / 2) - transform.position.y;
                if (objY < 1f) {
                    objY += transform.position.y;
                } else if (objY < 5f) {
                    objY = transform.position.y + UnityEngine.Random.Range(1f, objY);
                } else {
                    objY = transform.position.y + UnityEngine.Random.Range(1f, 5f);
                }
                if (areMovesFree) {
                    modifier = UnityEngine.Random.Range(-2.5f, 2.5f);
                    if (modifier > 0) {
                        objX = (rmArea.position.y + rmAreaColliderSize.y / 2) - transform.position.y;
                        if (objX < 1f) {
                            objX += transform.position.y;
                        } else if (objX < 2.5f) {
                            objX = transform.position.y + UnityEngine.Random.Range(1f, objX);
                        } else {
                            objX = transform.position.y + UnityEngine.Random.Range(1f, 2.5f);
                        }
                    } else if (modifier < 0) {
                        objX = transform.position.y - (rmArea.position.y - rmAreaColliderSize.y / 2);
                        if (objX < 1f) {
                            objX = transform.position.y - objX;
                        } else if (objX < 2.5f) {
                            objX = transform.position.y - UnityEngine.Random.Range(1f, objX);
                        } else {
                            objX = transform.position.y - UnityEngine.Random.Range(1f, 2.5f);
                        }
                    }
                }
                return new Vector2(objX, objY);

            case MoveDirections.Down:
                objX = transform.position.x;
                objY = transform.position.y - (rmArea.position.y - rmAreaColliderSize.y / 2);
                if (objY < 1f) {
                    objY = transform.position.y - objY;
                } else if (objY < 5f) {
                    objY = transform.position.y - UnityEngine.Random.Range(1f, objY);
                } else {
                    objY = transform.position.y - UnityEngine.Random.Range(1f, 5f);
                }
                if (areMovesFree) {
                    modifier = UnityEngine.Random.Range(-2.5f, 2.5f);
                    if (modifier > 0) {
                        objX = (rmArea.position.y + rmAreaColliderSize.y / 2) - transform.position.y;
                        if (objX < 1f) {
                            objX += transform.position.y;
                        } else if (objX < 2.5f) {
                            objX = transform.position.y + UnityEngine.Random.Range(1f, objX);
                        } else {
                            objX = transform.position.y + UnityEngine.Random.Range(1f, 2.5f);
                        }
                    } else if (modifier < 0) {
                        objX = transform.position.y - (rmArea.position.y - rmAreaColliderSize.y / 2);
                        if (objX < 1f) {
                            objX = transform.position.y - objX;
                        } else if (objX < 2.5f) {
                            objX = transform.position.y - UnityEngine.Random.Range(1f, objX);
                        } else {
                            objX = transform.position.y - UnityEngine.Random.Range(1f, 2.5f);
                        }
                    }
                }
                return new Vector2(objX, objY);

            case MoveDirections.TopRight:
                objX = (rmArea.position.x + rmAreaColliderSize.x / 2) - transform.position.x;
                objY = (rmArea.position.y + rmAreaColliderSize.y / 2) - transform.position.y;

                if (areMovesFree) {
                    if (objX < 1f) {
                        objX += transform.position.x;
                    } else if (objX < 5f) {
                        objX = transform.position.x + UnityEngine.Random.Range(1f, objX);
                    } else {
                        objX = transform.position.x + UnityEngine.Random.Range(1f, 5f);
                    }
                    if (objY < 1f) {
                        objY += transform.position.y;
                    } else if (objY < 5f) {
                        objY = transform.position.y + UnityEngine.Random.Range(1f, objY);
                    } else {
                        objY = transform.position.y + UnityEngine.Random.Range(1f, 5f);
                    }
                } else {
                    limit = objX < objY ? objX : objY;
                    if (limit < 1f) {
                        objX = limit + transform.position.x;
                        objY = limit + transform.position.y;
                    } else if (limit < 5f) {
                        modifier = UnityEngine.Random.Range(1f, limit);
                        objX = transform.position.x + modifier;
                        objY = transform.position.y + modifier;
                    } else {
                        modifier = UnityEngine.Random.Range(1f, 5f);
                        objX = transform.position.x + modifier;
                        objY = transform.position.y + modifier;
                    }
                }

                return new Vector2(objX, objY);

            case MoveDirections.TopLeft:
                objX = transform.position.x - (rmArea.position.x - rmAreaColliderSize.x / 2);
                objY = (rmArea.position.y + rmAreaColliderSize.y / 2) - transform.position.y;

                if (areMovesFree) {
                    if (objX < 1f) {
                        objX = transform.position.x - objX;
                    } else if (objX < 5f) {
                        objX = transform.position.x - UnityEngine.Random.Range(1f, objX);
                    } else {
                        objX = transform.position.x - UnityEngine.Random.Range(1f, 5f);
                    }
                    if (objY < 1f) {
                        objY += transform.position.y;
                    } else if (objY < 5f) {
                        objY = transform.position.y + UnityEngine.Random.Range(1f, objY);
                    } else {
                        objY = transform.position.y + UnityEngine.Random.Range(1f, 5f);
                    }
                } else {
                    limit = objX < objY ? objX : objY;
                    if (limit < 1f) {
                        objX = limit - transform.position.x;
                        objY = limit + transform.position.y;
                    } else if (limit < 5f) {
                        modifier = UnityEngine.Random.Range(1f, limit);
                        objX = transform.position.x - modifier;
                        objY = transform.position.y + modifier;
                    } else {
                        modifier = UnityEngine.Random.Range(1f, 5f);
                        objX = transform.position.x - modifier;
                        objY = transform.position.y + modifier;
                    }
                }

                return new Vector2(objX, objY);

            case MoveDirections.BottomRight:
                objX = (rmArea.position.x + rmAreaColliderSize.x / 2) - transform.position.x;
                objY = transform.position.y - (rmArea.position.y - rmAreaColliderSize.y / 2);

                if (areMovesFree) {
                    if (objX < 1f) {
                        objX += transform.position.x;
                    } else if (objX < 5f) {
                        objX = transform.position.x + UnityEngine.Random.Range(1f, objX);
                    } else {
                        objX = transform.position.x + UnityEngine.Random.Range(1f, 5f);
                    }
                    if (objY < 1f) {
                        objY = transform.position.y - objY;
                    } else if (objY < 5f) {
                        objY = transform.position.y - UnityEngine.Random.Range(1f, objY);
                    } else {
                        objY = transform.position.y - UnityEngine.Random.Range(1f, 5f);
                    }
                } else {
                    limit = objX < objY ? objX : objY;
                    if (limit < 1f) {
                        objX = limit + transform.position.x;
                        objY = limit - transform.position.y;
                    } else if (limit < 5f) {
                        modifier = UnityEngine.Random.Range(1f, limit);
                        objX = transform.position.x + modifier;
                        objY = transform.position.y - modifier;
                    } else {
                        modifier = UnityEngine.Random.Range(1f, 5f);
                        objX = transform.position.x + modifier;
                        objY = transform.position.y - modifier;
                    }
                }

                return new Vector2(objX, objY);

            case MoveDirections.BottomLeft:
                objX = transform.position.x - (rmArea.position.x - rmAreaColliderSize.x / 2);
                objY = transform.position.y - (rmArea.position.y - rmAreaColliderSize.y / 2);

                if (areMovesFree) {
                    if (objX < 1f) {
                        objX = transform.position.x - objX;
                    } else if (objX < 5f) {
                        objX = transform.position.x - UnityEngine.Random.Range(1f, objX);
                    } else {
                        objX = transform.position.x - UnityEngine.Random.Range(1f, 5f);
                    }
                    if (objY < 1f) {
                        objY = transform.position.y - objY;
                    } else if (objY < 5f) {
                        objY = transform.position.y - UnityEngine.Random.Range(1f, objY);
                    } else {
                        objY = transform.position.y - UnityEngine.Random.Range(1f, 5f);
                    }
                } else {
                    limit = objX < objY ? objX : objY;
                    if (limit < 1f) {
                        objX = limit - transform.position.x;
                        objY = limit - transform.position.y;
                    } else if (limit < 5f) {
                        modifier = UnityEngine.Random.Range(1f, limit);
                        objX = transform.position.x - modifier;
                        objY = transform.position.y - modifier;
                    } else {
                        modifier = UnityEngine.Random.Range(1f, 5f);
                        objX = transform.position.x - modifier;
                        objY = transform.position.y - modifier;
                    }
                }

                return new Vector2(objX, objY);

            case MoveDirections.None:
                return Vector2.zero;

            default:
                Debug.LogError($"UNABLE TO SET DESTINATION FOR THIS MOVEDIRECTION: {rmChosenDirection}");
                return Vector2.zero;
        }
    }

    private void MoveTowardsDestination() {
        bool stop;
        switch (rmChosenDirection) {
            case MoveDirections.Right:
                stop = transform.position.x > rmDestination.x;
                break;
            case MoveDirections.Left:
                stop = transform.position.x < rmDestination.x;
                break;
            case MoveDirections.Up:
                stop = transform.position.y > rmDestination.y;
                break;
            case MoveDirections.Down:
                stop = transform.position.y < rmDestination.y;
                break;
            case MoveDirections.TopRight:
                stop = transform.position.x > rmDestination.x || transform.position.y > rmDestination.y;
                break;
            case MoveDirections.TopLeft:
                stop = transform.position.x < rmDestination.x || transform.position.y > rmDestination.y;
                break;
            case MoveDirections.BottomRight:
                stop = transform.position.x > rmDestination.x || transform.position.y < rmDestination.y;
                break;
            case MoveDirections.BottomLeft:
                stop = transform.position.x < rmDestination.x || transform.position.y < rmDestination.y;
                break;
            case MoveDirections.None:
                stop = true;
                movement = Vector3.zero;
                break;
            default:
                stop = true;
                Debug.LogError($"UNABLE TO SET MOVEMENT FOR THIS MOVEDIRECTION: {rmChosenDirection}");
                break;
        }
        if (stop) {
            movement = Vector3.zero;
            StopDirection();
        } else {
            movement = (rmDestination - transform.position).normalized;
        }
    }

    private MoveDirections ChooseNewDirection() {
        Int16 nbOfAvailableDirections = 0;
        foreach (KeyValuePair<MoveDirections, bool> kvp in DirectionStates)
            if (kvp.Value && kvp.Key != rmPreviousDirection) nbOfAvailableDirections++;

        if (nbOfAvailableDirections == 0) return MoveDirections.None;

        Int16 draw = (Int16)UnityEngine.Random.Range(0, nbOfAvailableDirections);
        foreach (KeyValuePair<MoveDirections, bool> kvp in DirectionStates)
            if (kvp.Value && kvp.Key != rmPreviousDirection)
                if (draw > 0) {
                    draw--;
                } else {
                    return kvp.Key;
                }

        return MoveDirections.None;
    }

    private IEnumerator MovingRandomly() {
        isMovingRandomly = true;
        
        rmChosenDirection = rmPreviousDirection = MoveDirections.None;
        Int16 redirections = (Int16)(UnityEngine.Random.Range(1, maxRedirections + 1) + 1);
        rmDestination = Vector2.zero;
        
        yield return new WaitForSeconds(UnityEngine.Random.Range(rmCDMin, rmCDMax));
        
        checkCollision = true;
        while (true) {
            if (rmChosenDirection == MoveDirections.None) {
                if (redirections < 1) break;

                if (hasAggroEF) aggroEF.ResetAggro();
                movement = Vector3.zero;
                redirections--;
                
                rmChosenDirection = ChooseNewDirection();
                rmDestination = ComputeRMDestination();
            } else {
                if (DirectionStates[rmChosenDirection]) {
                    MoveTowardsDestination();
                } else {
                    StopDirection();
                }
            }
            yield return null;
        }
        EndMovingRandomly();
    }

    private void EndMovingRandomly() {
        if (movingRandomlyCo != null) {
            StopCoroutine(movingRandomlyCo);
            movingRandomlyCo = null;
        }

        isMovingRandomly = checkCollision = false;
        movement = rmDestination = Vector2.zero;
        rmChosenDirection = rmPreviousDirection = MoveDirections.None;
    }

    public bool CheckIfStillInRMArea() {
        return rmArea.gameObject.GetComponent<EnemyRMArea>().IsOwnerInArea;
    }

    public void ReturnToRMArea() {
        movement = (rmArea.position - transform.position).normalized;
        returningToRMAreaCo = StartCoroutine(ReturningToRMArea());
    }

    private IEnumerator ReturningToRMArea() {
        isReturningToRMArea = true;
        while (!CheckIfStillInRMArea()) yield return null;
        movement = Vector2.zero;
        HandleRegularMoves(true);
        EndReturningToRMArea();
    }

    private void EndReturningToRMArea() {
        if (returningToRMAreaCo != null) {
            StopCoroutine(returningToRMAreaCo);
            returningToRMAreaCo = null;
            isReturningToRMArea = false;
        }
        HandleRegularMoves(true);
    }
}
