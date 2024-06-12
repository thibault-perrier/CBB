using System;
using TMPro;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

////TODO: have updateBindingUIEvent receive a control path string, too (in addition to the device layout name)

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    /// <summary>
    /// This is an example for how to override the default display behavior of bindings. The component
    /// hooks into <see cref="RebindActionUI.updateBindingUIEvent"/> which is triggered when UI display
    /// of a binding should be refreshed. It then checks whether we have an icon for the current binding
    /// and if so, replaces the default text display with an icon.
    /// </summary>
    public class GamepadIconsExample : MonoBehaviour
    {
        public GamepadIcons xbox;
        public GamepadIcons ps4;
        public GamepadIcons switchCtrl;
        public KeyboardIcons keyboard;

        protected void OnEnable()
        {
            // Hook into all updateBindingUIEvents on all RebindActionUI components in our hierarchy.
            var rebindUIComponents = transform.GetComponentsInChildren<RebindActionUI>();
            foreach (var component in rebindUIComponents)
            {
                component.updateBindingUIEvent.AddListener(OnUpdateBindingDisplay);
                component.UpdateBindingDisplay();
            }
        }

        protected void OnUpdateBindingDisplay(RebindActionUI component, string bindingDisplayString, string deviceLayoutName, string controlPath)
        {
            if (string.IsNullOrEmpty(deviceLayoutName) || string.IsNullOrEmpty(controlPath))
                return;

            var icon = default(Sprite);
            if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "DualShockGamepad"))
                icon = ps4.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "SwitchProControllerHID"))
                icon = switchCtrl.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Gamepad"))
                icon = xbox.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Keyboard"))
                icon = keyboard.GetSprite(controlPath);

                var textComponent = component.bindingText;

            // Grab Image component.
            var iconGrp = textComponent.transform.parent.Find("IconGroup");
            var imageGO = iconGrp.transform.GetChild(0);
            var imageComponent = imageGO.GetComponent<Image>();

            if (icon != null)
            {
                textComponent.gameObject.SetActive(false);
                imageComponent.sprite = icon;
                imageComponent.gameObject.SetActive(true);
            }
            else
            {
                textComponent.gameObject.SetActive(true);
                imageComponent.gameObject.SetActive(false);
            }
        }

        [Serializable]
        public struct GamepadIcons
        {
            public Sprite buttonSouth;
            public Sprite buttonNorth;
            public Sprite buttonEast;
            public Sprite buttonWest;
            public Sprite startButton;
            public Sprite selectButton;
            public Sprite leftTrigger;
            public Sprite rightTrigger;
            public Sprite leftShoulder;
            public Sprite rightShoulder;
            public Sprite dpad;
            public Sprite dpadUp;
            public Sprite dpadDown;
            public Sprite dpadLeft;
            public Sprite dpadRight;
            public Sprite leftStick;
            public Sprite rightStick;
            public Sprite leftStickPress;
            public Sprite rightStickPress;
            public Sprite leftStickX;
            public Sprite leftStickY;
            public Sprite rightStickX;
            public Sprite rightStickY;

            public Sprite GetSprite(string controlPath)
            {
                // From the input system, we get the path of the control on device. So we can just
                // map from that to the sprites we have for gamepads.
                switch (controlPath)
                {
                    case "buttonSouth": return buttonSouth;
                    case "buttonNorth": return buttonNorth;
                    case "buttonEast": return buttonEast;
                    case "buttonWest": return buttonWest;
                    case "start": return startButton;
                    case "select": return selectButton;
                    case "leftTrigger": return leftTrigger;
                    case "rightTrigger": return rightTrigger;
                    case "leftShoulder": return leftShoulder;
                    case "rightShoulder": return rightShoulder;
                    case "dpad": return dpad;
                    case "dpad/up": return dpadUp;
                    case "dpad/down": return dpadDown;
                    case "dpad/left": return dpadLeft;
                    case "dpad/right": return dpadRight;
                    case "leftStick": return leftStick;
                    case "rightStick": return rightStick;
                    case "leftStickPress": return leftStickPress;
                    case "rightStickPress": return rightStickPress;
                    case "leftStick/x": return leftStickX;
                    case "leftStick/y": return leftStickY;
                    case "rightStick/x": return rightStickX;
                    case "rightStick/y": return rightStickY;
                }
                return null;
            }
        }

        [Serializable]
        public struct KeyboardIcons
        {
            public Sprite space;
            public Sprite leftMouseButton;
            public Sprite rightMouseButton;
            public Sprite middleMouseButton;
            public Sprite backspace;
            public Sprite capsLock;
            public Sprite enter;
            public Sprite enterRight;
            public Sprite alt;
            public Sprite ctrl;
            public Sprite shift;
            public Sprite shiftRight;
            public Sprite asterisk;
            public Sprite bracketLeft;
            public Sprite bracketRight;
            public Sprite del;
            public Sprite quote;
            public Sprite tab;
            public Sprite slash;
            public Sprite tilda;
            public Sprite questionMark;
            public Sprite semicolon;
            public Sprite plusKey;
            public Sprite minusKey;
            public Sprite markLeft;
            public Sprite markRight;
            public Sprite f1;
            public Sprite f2;
            public Sprite f3;
            public Sprite f4;
            public Sprite f5;
            public Sprite f6;
            public Sprite f7;
            public Sprite f8;
            public Sprite f9;
            public Sprite f10;
            public Sprite f11;
            public Sprite f12;
            public Sprite up;
            public Sprite down;
            public Sprite left;
            public Sprite right;
            public Sprite a;
            public Sprite b;
            public Sprite c;
            public Sprite d;
            public Sprite e;
            public Sprite f;
            public Sprite g;
            public Sprite h;
            public Sprite i;
            public Sprite j;
            public Sprite k;
            public Sprite l;
            public Sprite m;
            public Sprite n;
            public Sprite o;
            public Sprite p;
            public Sprite q;
            public Sprite r;
            public Sprite s;
            public Sprite t;
            public Sprite u;
            public Sprite v;
            public Sprite w;
            public Sprite x;
            public Sprite y;
            public Sprite z;
            public Sprite zero;
            public Sprite one;
            public Sprite two;
            public Sprite three;
            public Sprite four;
            public Sprite five;
            public Sprite six;
            public Sprite seven;
            public Sprite eight;
            public Sprite nine;

            public Sprite GetSprite(string controlPath)
            {
                // From the input system, we get the path of the control on device. So we can just
                // map from that to the sprites we have for keyboard.
                KeyControl keyControl = Keyboard.current.FindKeyOnCurrentKeyboardLayout(controlPath);
                string key = controlPath;

                if (keyControl != null)
                    key = keyControl.ToString().Split('/')[2];

                switch (key)
                {
                    //case "a": return a;
                    //case "b": return b;
                    //case "c": return c;
                    //case "d": return d;
                    //case "e": return e;
                    //case "f": return f;
                    //case "g": return g;
                    //case "h": return h;
                    //case "i": return i;
                    //case "j": return j;
                    //case "k": return k;
                    //case "l": return l;
                    //case "m": return m;
                    //case "n": return n;
                    //case "o": return o;
                    //case "p": return p;
                    //case "q": return q;
                    //case "r": return r;
                    //case "s": return s;
                    //case "t": return t;
                    //case "u": return u;
                    //case "v": return v;
                    //case "w": return w;
                    //case "x": return x;
                    //case "y": return y;
                    //case "z": return z;
                    //case "1": return one;
                    //case "2": return two;
                    //case "3": return three;
                    //case "4": return four;
                    //case "5": return five;
                    //case "6": return six;
                    //case "7": return seven;
                    //case "8": return eight;
                    //case "9": return nine;
                    //case "numpad1": return one;
                    //case "numpad2": return two;
                    //case "numpad3": return three;
                    //case "numpad4": return four;
                    //case "numpad5": return five;
                    //case "numpad6": return six;
                    //case "numpad7": return seven;
                    //case "numpad8": return eight;
                    //case "numpad9": return nine;
                    //case "space": return space;
                    //case "leftButton": return leftMouseButton;
                    //case "rightButton": return rightMouseButton;
                    //case "middleButton": return middleMouseButton;
                    //case "backspace": return backspace;
                    //case "capsLock": return capsLock;
                    //case "enter": return enter;
                    //case "numpadEnter": return enterRight;
                    //case "alt": return alt;
                    //case "rightAlt": return alt;
                    //case "ctrl": return ctrl;
                    //case "shift": return shiftRight;
                    //case "leftShift": return shift;
                    //case "tab": return tab;
                    case "downArrow": return down;
                    case "upArrow": return up;
                    case "leftArrow": return left;
                    case "rightArrow": return right;
                }
                return null;
            }
        }
    }
}
