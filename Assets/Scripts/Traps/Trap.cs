using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Trap : MonoBehaviour
{
    [SerializeField] Material brightMaterial;
    [SerializeField] bool useBrighterMaterial = true;

    Material originalMaterial;
    Renderer trapRenderer;

    public event System.Action<Trap> TrapTriggered;

    // Start is called before the first frame update
    void Start()
    {
        trapRenderer = GetComponent<Renderer>();
        originalMaterial = trapRenderer.material;
        UpdateMaterial();
    }

    public void UpdateMaterial()
    {
        if (useBrighterMaterial && brightMaterial != null)
            trapRenderer.material = brightMaterial;
        else
            trapRenderer.material = originalMaterial;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.Stun();
            TrapTriggered?.Invoke(this);
        }
    }
}
