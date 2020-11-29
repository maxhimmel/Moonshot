// <auto-generated>
// Rewired Constants
// This list was generated on 11/28/2020 5:59:10 PM
// The list applies to only the Rewired Input Manager from which it was generated.
// If you use a different Rewired Input Manager, you will have to generate a new list.
// If you make changes to the exported items in the Rewired Input Manager, you will
// need to regenerate this list.
// </auto-generated>

namespace Moonshot.ReConsts {
    public static partial class Action {
        // Default
        // Gameplay
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Gameplay", friendlyName = "Jump")]
        public const int Jump = 1;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Gameplay", friendlyName = "Move Horizontally")]
        public const int MoveHorizontal = 2;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Gameplay", friendlyName = "Move Vertically")]
        public const int MoveVertical = 3;
        // UI
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "UI", friendlyName = "Confirm")]
        public const int Confirm = 4;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "UI", friendlyName = "Cancel")]
        public const int Cancel = 5;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "UI", friendlyName = "UI Vertical")]
        public const int Ui_Vertical = 6;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "UI", friendlyName = "UI Horizontal")]
        public const int Ui_Horizontal = 7;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "UI", friendlyName = "Pause")]
        public const int Pause = 8;
    }
    public static partial class Category {
        public const int Default = 0;
        public const int Gameplay = 1;
        public const int UI = 2;
    }
    public static partial class Layout {
        public static partial class Joystick {
            public const int Default = 0;
        }
        public static partial class Keyboard {
            public const int Default = 0;
        }
        public static partial class Mouse {
            public const int Default = 0;
        }
        public static partial class CustomController {
            public const int Default = 0;
        }
    }
    public static partial class Player {
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "System")]
        public const int System = 9999999;
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "Player0")]
        public const int Player0 = 0;
    }
}
