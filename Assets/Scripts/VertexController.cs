using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents and controls single shape's vertex object.
/// </summary>
public class VertexController : MonoBehaviour {

    public Animation VertexAnim;

    /// <summary>
    /// Flag determining if this vertex is hidden right now
    /// </summary>
    [HideInInspector]
    public bool isHidden = true;

    /// <summary>
    /// Hide this vertex to pass cursor
    /// </summary>
    public void Hide()
    {
        if (UIController.instance.GameInteractive && !isHidden)
        {
            VertexAnim.Play("VertexHide");
            isHidden = true;
        }
    }

    /// <summary>
    /// Show this vertex to kill cursor
    /// </summary>
    public void Show()
    {
        if (UIController.instance.GameInteractive && isHidden)
        {
            VertexAnim.Play("VertexShow");
            isHidden = false;
        }
    }

    /// <summary>
    /// Translates this vertex using given translation vector
    /// </summary>
    /// <param name="translationVector">Translation vector.</param>
    public void TranslateByVector(Vector2 translationVector)
    {
        if (TranslateByVectorCoroutineVar != null)
            StopCoroutine(TranslateByVectorCoroutineVar);
        TranslateByVectorCoroutineVar = StartCoroutine(TranslateByVectorCoroutine(translationVector));
    }

    private Coroutine TranslateByVectorCoroutineVar = null;
    private IEnumerator TranslateByVectorCoroutine(Vector2 translationVector)
    {
        Vector2 destinationPosition = (Vector2)this.transform.position + translationVector;
        while(Vector2.Distance(this.transform.position, destinationPosition) > Time.deltaTime / 3)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, destinationPosition, Time.deltaTime / 3);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        this.transform.position = destinationPosition;
    }
}
