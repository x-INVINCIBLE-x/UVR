using System.Collections.Generic;
using System;

public static class FormationConsequenceManager
{
    public static event Action<DynamicFormationController> OnControllerAdded;
    public static event Action<DynamicFormationController> OnControllerRemoved;

    private static readonly List<DynamicFormationController> controllers = new();

    public static void Register(DynamicFormationController controller)
    {
        if (!controllers.Contains(controller))
        {
            controllers.Add(controller);
            OnControllerAdded?.Invoke(controller);
        }
    }

    public static void Unregister(DynamicFormationController controller)
    {
        if (controllers.Remove(controller))
        {
            OnControllerRemoved?.Invoke(controller);
        }
    }
}
