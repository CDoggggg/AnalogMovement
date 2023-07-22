using InControl;

namespace AnalogMovement;
public class KeyBinds : PlayerActionSet
{
    public PlayerAction upswingAction;
    public PlayerAction downswingAction;
    public KeyBinds()
    {
        upswingAction = CreatePlayerAction("Upswing");
        downswingAction = CreatePlayerAction("Downswing");
        DefaultBinds();
    }
    private void DefaultBinds()
    {
        upswingAction.AddDefaultBinding(Key.Key1);
        downswingAction.AddDefaultBinding(Key.Key2);
    }
}

public class ButtonBinds : PlayerActionSet
{
    public PlayerAction upswingAction;
    public PlayerAction downswingAction;
    public ButtonBinds()
    {
        upswingAction = CreatePlayerAction("Upswing");
        downswingAction = CreatePlayerAction("Downswing");
        DefaultBinds();
    }
    private void DefaultBinds()
    {
        upswingAction.AddDefaultBinding(InputControlType.LeftStickButton);
        downswingAction.AddDefaultBinding(InputControlType.RightStickButton);
    }
}