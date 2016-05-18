using UnityEngine;
using System.Collections;

public class SetLineRendererSortingLayer : MonoBehaviour
{
    public string SortingLayerName;
    public int SortingOrder;

    private void Start ()
	{
	    this.GetComponent<LineRenderer>().sortingLayerName = SortingLayerName;
        this.GetComponent<LineRenderer>().sortingOrder = SortingOrder;
    }

    
}
