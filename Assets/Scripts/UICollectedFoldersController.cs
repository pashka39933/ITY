using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICollectedFoldersController : MonoBehaviour {

    /// <summary>
    /// Singleton's instance.
    /// </summary>
    public static UICollectedFoldersController instance;
    UICollectedFoldersController() { instance = this; }

    // Folder's merge animation
    public Animation CollectedFoldersAnim;

    // Folders collection animations
    public Animation[] CollectedFolderFillAnims;

    // Help variable
    private int CurrentCollectedFoldersCount = 0;

    /// <summary>
    /// Method used to report that folder was just collected
    /// </summary>
    public void ReportCollectedFolder()
    {
        // Filling indicators
        CollectedFolderFillAnims[CurrentCollectedFoldersCount++].Play("CollectedFolderFillIn");

        // Performing merge if needed
        if(CurrentCollectedFoldersCount == 3)
        {
            CollectedFoldersAnim.Play("CollectedFoldersMerge");
            CurrentCollectedFoldersCount = 0;
        }
    }

    /// <summary>
    /// Method used to report that folder was just missed
    /// </summary>
    public void ReportMissedFolder()
    {
        // Clearing indicators and counter
        foreach (Animation CollectedFolderFillAnim in CollectedFolderFillAnims)
            if (CollectedFolderFillAnim.GetComponent<CanvasGroup>().alpha > 0)
                CollectedFolderFillAnim.Play("CollectedFolderFillOut");
        CurrentCollectedFoldersCount = 0;
    }

}
