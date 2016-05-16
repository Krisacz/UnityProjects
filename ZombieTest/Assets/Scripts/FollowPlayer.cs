using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour
{
    public GameObject Player;
    private Vector3 _offset;

    private void Start ()
    {
        _offset = transform.position - Player.transform.position;
    }

    private void Update ()
    {
        transform.position = Player.transform.position + _offset;
    }
}
