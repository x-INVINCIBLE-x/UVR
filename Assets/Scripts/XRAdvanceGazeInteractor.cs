using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRAdvanceGazeInteractor : XRGazeInteractor
{
    public bool showInteraction = false;
    [SerializeField] private Material interactionMaterial;
    private Material defaultMaterial = null;

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        //if ()
        if (!showInteraction) return;

        if (args.interactableObject.transform.TryGetComponent(out Renderer renderer))
        {
            defaultMaterial = renderer.material;
            renderer.material = interactionMaterial;
        }
    }
    
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);

        if (defaultMaterial == null) return;

        if (args.interactableObject.transform.TryGetComponent(out Renderer renderer))
        {
            renderer.material = defaultMaterial;
            defaultMaterial = null;
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (defaultMaterial == null) return;

        if (args.interactableObject.transform.TryGetComponent(out Renderer renderer))
        {
            renderer.material = defaultMaterial;
            defaultMaterial= null;
        }
    }
}
