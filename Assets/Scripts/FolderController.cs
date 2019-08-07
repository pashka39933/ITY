using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents and controls single folder object.
/// </summary>
public class FolderController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
            // Folder's movement according to it's spawn vertices
            Vector2 translationVector = Vector2.zero;
            if (latestSpawnVerticesPositions != null)
            {
                for (int i = 0; i < latestSpawnVerticesPositions.Length; i++)
                {
                    translationVector += ((Vector2)latestSpawnVerticesPositions[i].Key.transform.position - latestSpawnVerticesPositions[i].Value) / latestSpawnVerticesPositions.Length;
                    latestSpawnVerticesPositions[i] = new KeyValuePair<VertexController, Vector2>(latestSpawnVerticesPositions[i].Key, latestSpawnVerticesPositions[i].Key.transform.position);
                }
            }
            this.transform.position += (Vector3)translationVector;
        }
    }

    /// <summary>
    /// Flag determining if this folder's object is active and visible on the screen (isn't sleeping in the pool)
    /// </summary>
    public bool isActive = false;

    /// <summary>
    /// Method returning <see langword="true"/> if this folder is currently in pool and can be reused
    /// </summary>
    /// <returns><c>true</c>, if folder is pooled, <c>false</c> otherwise.</returns>
    public bool IsFolderInPool()
    {
        return !isActive && !this.GetComponent<Animation>().isPlaying;
    }

    // Local help variables
    private KeyValuePair<VertexController, Vector2>[] latestSpawnVerticesPositions = null;

    /// <summary>
    /// Activate this folder at the specified position.
    /// </summary>
    /// <param name="position">Position.</param>
    public void Activate(KeyValuePair<VertexController[], Vector2> spawnPosition)
    {
        if (!isActive)
        {
            latestSpawnVerticesPositions = new KeyValuePair<VertexController, Vector2>[3];
            latestSpawnVerticesPositions[0] = new KeyValuePair<VertexController, Vector2>(spawnPosition.Key[0], spawnPosition.Key[0].transform.position);
            latestSpawnVerticesPositions[1] = new KeyValuePair<VertexController, Vector2>(spawnPosition.Key[1], spawnPosition.Key[1].transform.position);
            latestSpawnVerticesPositions[2] = new KeyValuePair<VertexController, Vector2>(spawnPosition.Key[2], spawnPosition.Key[2].transform.position);
            this.transform.position = spawnPosition.Value;
            this.GetComponent<Animation>().Play("FolderActivate");
            isActive = true;
        }
    }

    /// <summary>
    /// Deactivate this folder.
    /// </summary>
    public void Deactivate()
    {
        if (isActive)
        {
            string[] possibleWords = { "MISS.", "BAD.", "NOPE.", "ARGH.", "GONE.", "WTF.", "EH.", "NAH.", "OOPS.", "EWW.", "OY.", "NOOB.", "WHAT.", "SUCK." };
            this.GetComponentInChildren<TextMesh>().text = "<color=#FF0000>" + possibleWords[Random.Range(0, possibleWords.Length)] + "</color>";
            this.GetComponent<Animation>().Play("FolderDeactivate");
            this.isActive = false;

            UICollectedFoldersController.instance.ReportMissedFolder();
        }
    }

    // Collision callback
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Collect();
    }

    private void Collect()
    {
        if (isActive)
        {
            string[] possibleWords = { "WOW!", "NICE!", "OMG!", "YEAH!", "YES!", "HA!", "GOTCHA!", "CATCH!", "HIT!", "AWW!", "WHOA!", "YAY!", "PRO!", "SKILL!" };
            this.GetComponentInChildren<TextMesh>().text = "<color=#00FF00>" + possibleWords[Random.Range(0, possibleWords.Length)] + "</color>";
            this.GetComponent<Animation>().Play("FolderCollect");
            isActive = false;

            UICollectedFoldersController.instance.ReportCollectedFolder();
        }
    }
}
