using System;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace MemoryTrainer
{
    public class KeyboardController
    {
        public static IKeyboardMouseEvents GlobalHook;
        public static bool IsEnabled { get; set; }
        public static bool BlockState { get; set; }

        public static void Start()
        {
            GlobalHook = Hook.GlobalEvents();
            IsEnabled = false;
            BlockState = false;
            Subscribe();
        }

        private static void GlobalHookKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) Environment.Exit(0);
        }

        private static void Subscribe()
        {
            if (IsEnabled) return;
            GlobalHook.KeyDown += GlobalHookKeyDown;
            IsEnabled = true;
        }

        private static void Unsubscribe()
        {
            if (!IsEnabled) return;
            GlobalHook.KeyDown -= GlobalHookKeyDown;
            IsEnabled = false;
        }

        public static void BlockInput(bool state)
        {
            BlockState = state;
        }
    }
}