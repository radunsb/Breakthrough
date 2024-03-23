using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.InputSystem.Layouts;

#if UNITY_EDITOR
using UnityEditor;
#endif

[assembly: AlwaysLinkAssembly]

namespace UnityEngine.InputSystem.PS4
{
    /// <summary>
    /// Adds support for PS4 controllers.
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
#if UNITY_DISABLE_DEFAULT_INPUT_PLUGIN_INITIALIZATION
    public
#else
    internal
#endif
    static class PS4Support
    {
        static PS4Support()
        {
#if UNITY_EDITOR || UNITY_PS4
            InputSystem.runInBackground = true;
            InputSystem.RegisterLayout<MoveControllerPS4>("PS4MoveController",
                matches: new InputDeviceMatcher()
                    .WithInterface("PS4")
                    .WithDeviceClass("PS4MoveController"));

            InputSystem.RegisterLayout<PS4TouchControl>("PS4Touch");
            InputSystem.RegisterLayout<DualShockGamepadPS4>("PS4DualShockGamepad",
                matches: new InputDeviceMatcher()
                    .WithInterface("PS4")
                    .WithDeviceClass("PS4DualShockGamepad"));
#endif
        }

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeInPlayer() {}
    }
}
