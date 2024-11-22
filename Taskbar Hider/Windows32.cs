using System;

namespace Tbh;

public enum AppBarMessages
{
    New = 0x00,
    Remove = 0x01,
    QueryPos = 0x02,
    SetPos = 0x03,
    GetState = 0x04,
    GetTaskBarPos = 0x05,
    Activate = 0x06,
    GetAutoHideBar = 0x07,
    SetAutoHideBar = 0x08,
    WindowPosChanged = 0x09,
    SetState = 0x0a
}

[Flags]
public enum AppBarStates
{
    AutoHide = 0x01,
    AlwaysOnTop = 0x02
}