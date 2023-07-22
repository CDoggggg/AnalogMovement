using InControl;
using Modding.Converters;
using Newtonsoft.Json;
using Satchel.BetterMenus;

namespace AnalogMovement;

public sealed class Settings
{
    public float deadzonePercentage = 30;
    public float verticalSwingAngle = 45;
    public bool enableVerticalSwingKeyBinds = false;
    public static KeyBinds upSwingKeyBind = new KeyBinds();
    public static ButtonBinds upSwingButtonBind = new ButtonBinds();
    public static KeyBinds downSwingKeyBind = new KeyBinds();
    public static ButtonBinds downSwingButtonBind = new ButtonBinds();
    [JsonConverter(typeof(PlayerActionSetConverter))]
    public KeyBinds swingKeyBinds = new KeyBinds();
    public ButtonBinds swingButtonBinds = new ButtonBinds();
}