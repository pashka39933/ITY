using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player's controller's singleton. Handles player's movement according to current shape and user's input.
/// </summary>
public class PlayerController : MonoBehaviour
{

    /// <summary>
    /// Singleton's instance.
    /// </summary>
    public static PlayerController instance;
    PlayerController() { instance = this; }

    /// <summary>
    /// Normal cursor's speed
    /// </summary>
    public float Speed = 3f; 

    /// <summary>
    /// Speed during cursor's jump / flight.
    /// </summary>
    public float JumpSpeed = 5f;

    // Local help variables
    private VertexController currentDestinationVertex = null;
    private Vector2 currentDestinationJumpPoint = Vector2.zero, currentFolderCheckPoint = Vector2.zero;
    private Quaternion currentDestinationRotation = Quaternion.identity;
    private bool oddVertexFollowing = true;
    private FolderController currentFolder;

    // Update is called once per frame
    void Update()
    {
        if (!UIController.instance || !UIController.instance.GameStarted)
            return;

        if (this.GetComponent<PolygonCollider2D>().enabled)
        {
            // Calculating jump position
            if (currentDestinationJumpPoint == Vector2.zero)
            {
                currentDestinationJumpPoint = ShapeController.instance.GetJumpDestinationPosition(this.transform.position, currentDestinationVertex);
                Vector3 vectorToTarget = currentDestinationJumpPoint - (Vector2)this.transform.position;
                currentDestinationRotation = Quaternion.AngleAxis(Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg, Vector3.forward);
                currentDestinationRotation *= Quaternion.Euler(Vector3.back * 114f);
            }

            // Calculating folder's touch checkpoint
            if (!oddVertexFollowing && currentFolder != null)
            {
                if (currentFolderCheckPoint == Vector2.zero)
                {
                    currentFolderCheckPoint = Math3d.ProjectPointOnLineSegment(this.transform.position, currentDestinationJumpPoint, currentFolder.transform.position);
                    if (Math3d.PointOnWhichSideOfLineSegment(this.transform.position, currentDestinationJumpPoint, currentFolderCheckPoint) != 0)
                        currentFolderCheckPoint = Vector2.one;
                }

                // Checking miss conditions
                if (currentFolderCheckPoint == Vector2.one || Vector2.Distance(currentFolderCheckPoint, this.transform.position) <= Time.deltaTime * Speed)
                {
                    // Folder missed!
                    if (currentFolder.isActive)
                    {
                        currentFolder.Deactivate();

                        // Shape modification (previous vertex translation)
                        float distance = Vector2.Distance(this.transform.position, currentFolder.transform.position);
                        VertexController previousDestinationVertex = ShapeController.instance.GetPreviousVertexFollowed(currentDestinationVertex);
                        Vector2 destinationPosition = (currentDestinationVertex.transform.position - previousDestinationVertex.transform.position).normalized;
                        previousDestinationVertex.TranslateByVector(destinationPosition * distance * 0.75f);
                    }
                }

            }

            // Following jump position
            this.transform.position = Vector2.MoveTowards(this.transform.position, currentDestinationJumpPoint, Time.deltaTime * JumpSpeed);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, currentDestinationRotation, Time.deltaTime * 16f);

            // TODO: Cursor dash effects (shaking, particles... ???)

            // Finishing jump, spawning folder and following next vertex
            if (Vector2.Distance(this.transform.position, currentDestinationJumpPoint) <= Time.deltaTime * Speed)
            {
                currentDestinationVertex.Show();
                currentDestinationVertex = ShapeController.instance.GetNextVertexToFollow(currentDestinationVertex);
                currentDestinationRotation = Quaternion.identity;
                oddVertexFollowing = !oddVertexFollowing;
                if (UIController.instance.GameInteractive && oddVertexFollowing)
                    currentFolder = FoldersPoolController.instance.SpawnFolder(currentDestinationVertex);
                this.transform.position = currentDestinationJumpPoint;
                this.GetComponent<PolygonCollider2D>().enabled = false;
            }
        }
        else
        {
            // Following next vertex
            if (currentDestinationVertex == null || Vector2.Distance(this.transform.position, currentDestinationVertex.transform.position) < Time.deltaTime)
            {
                if (currentDestinationVertex != null)
                {
                    if (!currentDestinationVertex.isHidden)
                    {
                        UIController.instance.FireGameOver();
                    }

                    this.transform.position = currentDestinationVertex.transform.position;
                    currentDestinationVertex.Show();
                }
                currentDestinationVertex = ShapeController.instance.GetNextVertexToFollow(currentDestinationVertex);
                currentDestinationRotation = Quaternion.identity;
                oddVertexFollowing = !oddVertexFollowing;
                if (UIController.instance.GameInteractive && oddVertexFollowing)
                    currentFolder = FoldersPoolController.instance.SpawnFolder(currentDestinationVertex);
            }
            this.transform.position = Vector2.MoveTowards(this.transform.position, currentDestinationVertex.transform.position, Time.deltaTime * Speed);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, currentDestinationRotation, Time.deltaTime * 16f);

            // User input for jumping
            if (Input.GetMouseButtonDown(0))
            {
                ShapeController.instance.GetNextVertexToFollow(currentDestinationVertex).Hide();
                currentDestinationJumpPoint = Vector2.zero;
                currentFolderCheckPoint = Vector2.zero;
                this.GetComponent<PolygonCollider2D>().enabled = true;
            }
        }
    }
}
