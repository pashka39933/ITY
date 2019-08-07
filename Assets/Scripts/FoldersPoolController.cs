using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Folder's pool controller's singleton. Handles folders spawning inside current shape and according to current player's position.
/// </summary>
public class FoldersPoolController : MonoBehaviour
{

    /// <summary>
    /// Singleton's instance.
    /// </summary>
    public static FoldersPoolController instance;
    FoldersPoolController() { instance = this; }

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).GetComponent<FolderController>().Deactivate();
        }
    }

    /// <summary>
    /// Method used to spawn new folder in proper position (according to current shape and player's position)
    /// </summary>
    /// <param name="currentDestinationVertex">Current destination vertex which is followed by player.</param>
    public FolderController SpawnFolder(VertexController currentDestinationVertex)
    {
        // NOTE: For now, only one folder possible at the same time!
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).GetComponent<FolderController>().Deactivate();
        }
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).GetComponent<FolderController>().IsFolderInPool())
            {
                this.transform.GetChild(i).GetComponent<FolderController>().Activate(ShapeController.instance.GetNextFolderPosition(currentDestinationVertex));
                return this.transform.GetChild(i).GetComponent<FolderController>();
            }
        }
        return null;
    }
}
