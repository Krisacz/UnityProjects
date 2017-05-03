using Assets.Scripts.Models;
using UnityEngine;

public abstract class InteractController: MonoBehaviour
{
    public abstract void Init(string title);
    public abstract void AddBlueprint(Blueprint blueprint);
}