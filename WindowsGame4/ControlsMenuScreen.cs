using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame4
{
    class ControlsMenuScreen : MenuScreen
    {
        MenuEntry forwardMenuEntry;
        MenuEntry backwardMenuEntry;
        MenuEntry upMenuEntry;
        MenuEntry downMenuEntry;
        MenuEntry shootMenuEntry;
        MenuEntry backMenuEntry;
        public ControlsMenuScreen()
            : base("Controls")
        {
            forwardMenuEntry = new MenuEntry("Forward : D");
            backwardMenuEntry = new MenuEntry("Backward : A");
            upMenuEntry = new MenuEntry("Up : W");
            downMenuEntry = new MenuEntry("Down : S");
            shootMenuEntry = new MenuEntry("Shoot : Space");
            backMenuEntry = new MenuEntry("Back");

            backMenuEntry.Selected += OnCancel;

            MenuEntries.Add(forwardMenuEntry);
            MenuEntries.Add(backwardMenuEntry);
            MenuEntries.Add(upMenuEntry);
            MenuEntries.Add(downMenuEntry);
            MenuEntries.Add(shootMenuEntry);
            MenuEntries.Add(backMenuEntry);
        }
    }
}
