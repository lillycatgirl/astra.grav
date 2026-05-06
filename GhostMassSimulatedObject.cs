using UnityEngine;

public class GhostMassSimulatedObject : MassSimulatedObject
{
    
    public override void AfterMovementApplied()
    {
    }

    public override void BeforeMovementApplied()
    {
    }

    public override void OnClicked()
    {
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
