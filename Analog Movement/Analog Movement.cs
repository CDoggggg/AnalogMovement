using GlobalEnums;
using HutongGames.PlayMaker;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;
using System.Reflection;
using System.Security.Permissions;
using Modding.Menu.Config;
using System.EnterpriseServices;
using System.Runtime.CompilerServices;
using On;
using IL.InControl;
using InControl;
using System.Linq;
using System.Net.Mail;

namespace AnalogMovement
{
    public class AnalogMovement : Mod, ICustomMenuMod, ITogglableMod, IGlobalSettings<Settings>
    {
        new public string GetName() => "Analog Movement";
        public override string GetVersion() => "1.0";
        public bool ToggleButtonInsideMenu { get; } = true;
        public static Settings globalSettings = new();
        public void OnLoadGlobal(Settings settings) => globalSettings = settings;
        public Settings OnSaveGlobal()
        {
            return globalSettings;
        }
        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggle)
        {
            return SettingsMenu.GetMenu(modListMenu, toggle);
        }
        private InControl.PlayerAction oldAttack;
        public override void Initialize()
        {
            On.HeroController.FilterInput += RemoveFilterInput;
            On.HeroController.DoAttack += FixAttackDirection;
            ModHooks.HeroUpdateHook += ListenForSwingBindings;
            oldAttack = InputHandler.Instance.inputActions.attack;
        }

        public void ListenForSwingBindings()
        {
            if (globalSettings.enableVerticalSwingKeyBinds)
            {
                InControl.PlayerAction upKey = globalSettings.swingKeyBinds.upswingAction;
                InControl.PlayerAction downKey = globalSettings.swingKeyBinds.downswingAction;
                InControl.PlayerAction upButton = globalSettings.swingButtonBinds.upswingAction;
                InControl.PlayerAction downButton = globalSettings.swingButtonBinds.downswingAction;
                InputHandler.Instance.inputActions.attack = (upKey.IsPressed) ? upKey : (downKey.IsPressed) ? downKey :
                                                            (upButton.IsPressed) ? upButton : (downButton.IsPressed) ? downButton : oldAttack;
            }
        }

        public void FixAttackDirection(On.HeroController.orig_DoAttack orig, HeroController self)
        {
            globalSettings.verticalSwingAngle = (globalSettings.verticalSwingAngle > 90) ? 90f : globalSettings.verticalSwingAngle;
            globalSettings.verticalSwingAngle = (globalSettings.verticalSwingAngle < 0) ? 0f : globalSettings.verticalSwingAngle;
            ResetLookParameters(self);

            double swingAngle = Math.Atan(Math.Abs(self.vertical_input) / Math.Abs(self.move_input)) * (180f / Math.PI);
            if (swingAngle >= globalSettings.verticalSwingAngle || 
                ((globalSettings.swingKeyBinds.upswingAction.IsPressed || globalSettings.swingKeyBinds.downswingAction.IsPressed ||
                  globalSettings.swingButtonBinds.upswingAction.IsPressed || globalSettings.swingButtonBinds.downswingAction.IsPressed)
                 && globalSettings.enableVerticalSwingKeyBinds))
            {
                if ((globalSettings.swingKeyBinds.upswingAction.IsPressed || globalSettings.swingButtonBinds.upswingAction.IsPressed) && globalSettings.enableVerticalSwingKeyBinds)
                {
                    self.Attack(AttackDirection.upward);
                    self.StartCoroutine(self.CheckForTerrainThunk(AttackDirection.upward));
                }
                else if ((globalSettings.swingKeyBinds.downswingAction.IsPressed || globalSettings.swingButtonBinds.downswingAction.IsPressed) && globalSettings.enableVerticalSwingKeyBinds)
                {
                    if (self.hero_state != ActorStates.idle && self.hero_state != ActorStates.running)
                    {
                        self.Attack(AttackDirection.downward);
                        self.StartCoroutine(self.CheckForTerrainThunk(AttackDirection.downward));
                    }
                }
                else if (self.vertical_input > 0f)
                {
                    self.Attack(AttackDirection.upward);
                    self.StartCoroutine(self.CheckForTerrainThunk(AttackDirection.upward));
                }
                else
                {
                    if (self.hero_state != ActorStates.idle && self.hero_state != ActorStates.running)
                    {
                        self.Attack(AttackDirection.downward);
                        self.StartCoroutine(self.CheckForTerrainThunk(AttackDirection.downward));
                    }
                    else
                    {
                        self.Attack(AttackDirection.normal);
                        self.StartCoroutine(self.CheckForTerrainThunk(AttackDirection.normal));
                    }
                }
            }
            else
            {
                self.Attack(AttackDirection.normal);
                self.StartCoroutine(self.CheckForTerrainThunk(AttackDirection.normal));
            }
        }

        private void ResetLookParameters(HeroController self)
        {
            self.cState.lookingUp = false;
            self.cState.lookingDown = false;
            self.cState.lookingUpAnim = false;
            self.cState.lookingDownAnim = false;
            // self.lookDelayTimer = 0f;
            typeof(HeroController).GetField("lookDelayTimer", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(self, 0f);
            self.cState.recoiling = false;

            if (self.playerData.GetBool("equippedCharm_32"))
            {
                // self.attack_cooldown = self.ATTACK_COOLDOWN_TIME_CH;
                typeof(HeroController).GetField("attack_cooldown", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(self, self.ATTACK_COOLDOWN_TIME_CH);
            }
            else
            {
                // self.attack_cooldown = self.ATTACK_COOLDOWN_TIME;
                typeof(HeroController).GetField("attack_cooldown", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(self, self.ATTACK_COOLDOWN_TIME);
            }
        }

        public void RemoveFilterInput(On.HeroController.orig_FilterInput orig, HeroController self)
        {
            globalSettings.deadzonePercentage = (globalSettings.deadzonePercentage > 100) ? 100f : globalSettings.deadzonePercentage;
            globalSettings.deadzonePercentage = (globalSettings.deadzonePercentage < 0) ? 0f : globalSettings.deadzonePercentage;
            if (!(Math.Abs(HeroController.instance.move_input) >= globalSettings.deadzonePercentage / 100f))
            {
                self.move_input = 0f;
            }
        }
        public void Unload()
        {
            On.HeroController.FilterInput -= RemoveFilterInput;
            On.HeroController.DoAttack -= FixAttackDirection;
        }
    }
}