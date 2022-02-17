using System;

namespace Cthangband.Enumerations
{
    [Flags]
    internal enum ProjectionFlag
    {
        ProjectNone = 0x00,
        ProjectJump = 0x01,
        ProjectBeam = 0x02,
        ProjectThru = 0x04,
        ProjectStop = 0x08,
        ProjectGrid = 0x10,
        ProjectItem = 0x20,
        ProjectKill = 0x40,
        ProjectHide = 0x80,
    }
}