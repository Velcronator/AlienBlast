using UnityEngine;

public interface IItem
{
    string name { get; }
    GameObject gameObject { get; }
    Transform transform { get; }

    void Use();
}