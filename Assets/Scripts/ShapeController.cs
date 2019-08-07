using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shape controller's singleton. Controls it's line renderer to keep desired shape according to child's vertices.
/// </summary>
public class ShapeController : MonoBehaviour
{

    /// <summary>
    /// Singleton's instance.
    /// </summary>
    public static ShapeController instance;
    ShapeController() { instance = this; }

    /// <summary>
    /// Main LineRenderer component rendering current game state's shape.
    /// </summary>
    public LineRenderer lineRenderer;

    // Update is called once per frame
    void Update()
    {
        if (!UIController.instance || !UIController.instance.GameStarted)
            return;

        if (this.transform.childCount > 1)
        {
            lineRenderer.loop = true;
            lineRenderer.positionCount = this.transform.childCount;
            for (int i = 0; i < this.transform.childCount; i++)
            {
                lineRenderer.SetPosition(i, this.transform.GetChild(i).transform.position);
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    /// <summary>
    /// Method returning all vertices current positions
    /// </summary>
    /// <returns>All vertices current positions.</returns>
    public List<Vector2> GetAllVerticesPositions()
    {
        List<Vector2> positions = new List<Vector2>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            positions.Add(this.transform.GetChild(i).transform.position);
        }
        return positions;
    }

    /// <summary>
    /// Method used to fetch previous vertex which was followed before current one
    /// </summary>
    /// <returns>The previous vertex which was followed in the past</returns>
    /// <param name="currentVertexFollowed">Current vertex.</param>
    public VertexController GetPreviousVertexFollowed(VertexController currentVertexFollowed = null)
    {
        if (currentVertexFollowed)
        {
            int previousVertexIndex = currentVertexFollowed.transform.GetSiblingIndex();
            previousVertexIndex = previousVertexIndex - 1;
            previousVertexIndex = previousVertexIndex < 0 ? (previousVertexIndex + this.transform.childCount) : previousVertexIndex;
            return this.transform.GetChild(previousVertexIndex).GetComponent<VertexController>();
        }
        else
        {
            return this.transform.childCount > 0 ? this.transform.GetChild(0).GetComponent<VertexController>() : null;
        }
    }

    /// <summary>
    /// Method used to fetch next vertex which should be used for player movement
    /// </summary>
    /// <returns>The next vertex which will be followed by the player</returns>
    /// <param name="currentVertexFollowed">Current vertex.</param>
    public VertexController GetNextVertexToFollow(VertexController currentVertexFollowed = null)
    {
        if (currentVertexFollowed)
        {
            int currentVertexIndex = currentVertexFollowed.transform.GetSiblingIndex();
            currentVertexIndex = (currentVertexIndex + 1) % this.transform.childCount;
            return this.transform.GetChild(currentVertexIndex).GetComponent<VertexController>();
        }
        else
        {
            return this.transform.childCount > 0 ? this.transform.GetChild(0).GetComponent<VertexController>() : null;
        }
    }

    /// <summary>
    /// Method used to calculate destination position when player jumps.
    /// </summary>
    /// <returns>The player's jump destination position.</returns>
    /// <param name="jumpStartPosition">Jump start position (typically, player's current position).</param>
    /// <param name="currentVertexFollowed">Current vertex that is being followed.</param>
    public Vector2 GetJumpDestinationPosition(Vector2 jumpStartPosition, VertexController currentVertexFollowed)
    {
        int currentVertexIndex = currentVertexFollowed.transform.GetSiblingIndex();
        int previousVertexIndex = currentVertexIndex - 1;
        previousVertexIndex = previousVertexIndex < 0 ? previousVertexIndex + this.transform.childCount : previousVertexIndex;
        VertexController previousVertex = this.transform.GetChild(previousVertexIndex).GetComponent<VertexController>();
        int nextVertexIndex = (currentVertexIndex + 1) % this.transform.childCount;
        VertexController nextVertex = this.transform.GetChild(nextVertexIndex).GetComponent<VertexController>();

        Vector2 line = (nextVertex.transform.position - previousVertex.transform.position).normalized;
        line = line.y < 0 ? -line : line;
        float lineA = Mathf.Tan(Vector2.Angle(line, Vector2.right) * Mathf.Deg2Rad);
        float lineB = jumpStartPosition.y - (lineA * jumpStartPosition.x);

        Vector2 linePoint1 = new Vector2(0, lineB);
        Vector2 linePoint2 = new Vector2(1, lineA + lineB);

        // Debug lines drawing, uncomment to debug this method
        //Debug.DrawLine(previousVertex.transform.position, nextVertex.transform.position, Color.green, 3f);
        //Debug.DrawLine(linePoint1, linePoint2, Color.red, 3f);
        //Debug.DrawLine(jumpStartPosition + Vector2.left, jumpStartPosition + Vector2.right, Color.white, 3f);
        //Debug.DrawLine(jumpStartPosition + Vector2.down, jumpStartPosition + Vector2.up, Color.white, 3f);

        Vector3 intersectionPoint = Vector3.zero;
        Math3d.LineLineIntersection(out intersectionPoint, currentVertexFollowed.transform.position, (nextVertex.transform.position - currentVertexFollowed.transform.position).normalized, linePoint1, (linePoint2 - linePoint1).normalized);

        return intersectionPoint;
    }

    /// <summary>
    /// Gets proper next folder's position. Used to spawn folders in available positions.
    /// </summary>
    /// <returns>KeyValuePair of vertices used to calculate spawn position and spawn position.</returns>
    /// <param name="currentVertexFollowed">Current vertex followed.</param>
    public KeyValuePair<VertexController[], Vector2> GetNextFolderPosition(VertexController currentVertexFollowed)
    {
        int currentVertexIndex = currentVertexFollowed.transform.GetSiblingIndex();
        int nextVertexIndex = (currentVertexIndex + 1) % this.transform.childCount;
        VertexController nextVertex = this.transform.GetChild(nextVertexIndex).GetComponent<VertexController>();
        int nextNextVertexIndex = (currentVertexIndex + 2) % this.transform.childCount;
        VertexController nextNextVertex = this.transform.GetChild(nextNextVertexIndex).GetComponent<VertexController>();

        float availableSpaceScale = 0.5f;
        Vector2 availableSpaceVertex1 = currentVertexFollowed.transform.position + (nextVertex.transform.position - currentVertexFollowed.transform.position) * (1f - availableSpaceScale) / 3f;
        availableSpaceVertex1 += (Vector2)(nextNextVertex.transform.position - currentVertexFollowed.transform.position) * (1f - availableSpaceScale) / 3f;

        Vector2 availableSpaceVertex2 = nextVertex.transform.position + (currentVertexFollowed.transform.position - nextVertex.transform.position) * (1f - availableSpaceScale) / 3f;
        availableSpaceVertex2 += (Vector2)(nextNextVertex.transform.position - nextVertex.transform.position) * (1f - availableSpaceScale) / 3f;

        Vector2 availableSpaceVertex3 = nextNextVertex.transform.position + (currentVertexFollowed.transform.position - nextNextVertex.transform.position) * (1f - availableSpaceScale) / 3f;
        availableSpaceVertex3 += (Vector2)(nextVertex.transform.position - nextNextVertex.transform.position) * (1f - availableSpaceScale) / 3f;

        // Debug lines drawing, uncomment to debug this method
        //Debug.DrawLine(availableSpaceVertex1, availableSpaceVertex2, Color.white, 3f);
        //Debug.DrawLine(availableSpaceVertex2, availableSpaceVertex3, Color.white, 3f);
        //Debug.DrawLine(availableSpaceVertex3, availableSpaceVertex1, Color.white, 3f);

        VertexController[] returnList = { currentVertexFollowed, nextVertex, nextNextVertex };

        return new KeyValuePair<VertexController[], Vector2>(returnList, Math3d.GetRandomPointInsideTriangle(availableSpaceVertex1, availableSpaceVertex2, availableSpaceVertex3));
    }
}