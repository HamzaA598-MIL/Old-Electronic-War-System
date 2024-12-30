using System;

namespace ElecWarSystem.Models
{
    [Flags]
    public enum UserRoles : byte
    {
        Analyzer = 0b00_000_001,
        Unit = 0b00_000_010,
        Viewer = 0b00_000_100,
        Admin = 0b00_001_000,
        SuperAdmin = 0b00_010_000,


    }
}