using UnityEngine;

public interface ICachedMonoBehaviour
{
    GameObject gameObject { get; }
    SpriteRenderer spriteRenderer { get; }
    Transform transform { get; }
}