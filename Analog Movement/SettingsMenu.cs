using IL.InControl;
using Modding;
using Satchel.BetterMenus;
using System;

namespace AnalogMovement;

internal static class SettingsMenu
{
    internal static Menu MenuRef = null;
    internal static MenuScreen GetMenu(MenuScreen lastMenu, ModToggleDelegates? toggleDelegates)
    {
        MenuRef = PrepareMenu((ModToggleDelegates)toggleDelegates);
        return MenuRef.GetMenuScreen(lastMenu);
    }

    internal static Menu PrepareMenu(ModToggleDelegates toggleDelegates)
    {
        return new
        ("Analog Movement", new Element[]
            {
                Blueprints.CreateToggle(toggleDelegates, "Analog Movement Toggle", "Enable or disable this mod.", "Enabled", "Disabled"),
                Blueprints.FloatInputField("Deadzone Percent", (choose) => AnalogMovement.globalSettings.deadzonePercentage = choose, () => AnalogMovement.globalSettings.deadzonePercentage, 30, "", 5, Id:"DeadzonePercentage"),
                new TextPanel("Percent from 0% to 100% of the radius of the joystick for which no input will be registered.", fontSize: 24),
                Blueprints.FloatInputField("Vertical Swing Angle", (choose) => AnalogMovement.globalSettings.verticalSwingAngle = choose, () => AnalogMovement.globalSettings.verticalSwingAngle, 45, "", 5, Id:"VerticalSwingAngle"),
                new TextPanel("Angle from 0° to 90° off of the horizontal for which the joystick needs to be pushed in order to swing upwards or downwards.", fontSize: 24),
                Blueprints.HorizontalBoolOption("Vertical Swing Keybinds", "Enable or disable the keybinds for automatic vertical swinging.", (choose) => AnalogMovement.globalSettings.enableVerticalSwingKeyBinds = choose, () => AnalogMovement.globalSettings.enableVerticalSwingKeyBinds, Id: "EnableVerticalSwingKeyBinds"),
                Blueprints.KeyAndButtonBind("Upswing", AnalogMovement.globalSettings.swingKeyBinds.upswingAction, AnalogMovement.globalSettings.swingButtonBinds.upswingAction),
                Blueprints.KeyAndButtonBind("Downswing", AnalogMovement.globalSettings.swingKeyBinds.downswingAction, AnalogMovement.globalSettings.swingButtonBinds.downswingAction)
            }
        );
    }
}