using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;

namespace RMenu.Extensions;

public static class PlayerExtensions {
  public static void Freeze(this CCSPlayerController player) {
    if (player.Pawn.Value is not { } pawn) return;
    pawn.MoveType = MoveType_t.MOVETYPE_OBSOLETE;
    Schema.SetSchemaValue(pawn.Handle, "CBaseEntity", "m_nActualMoveType", 1);
    Utilities.SetStateChanged(pawn, "CBaseEntity", "m_MoveType");
  }

  public static void Unfreeze(this CCSPlayerController player) {
    if (player.Pawn.Value is not { } pawn) return;
    pawn.MoveType = MoveType_t.MOVETYPE_WALK;
    Schema.SetSchemaValue(pawn.Handle, "CBaseEntity", "m_nActualMoveType", 2);
    Utilities.SetStateChanged(pawn, "CBaseEntity", "m_MoveType");
  }
  
  public static bool IsAlive(this CCSPlayerController player) {
    return player is { IsValid: true, PawnIsAlive: true };
  }
}