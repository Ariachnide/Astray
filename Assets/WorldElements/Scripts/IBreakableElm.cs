using System;

public interface IBreakableElm {
    public bool GetMeleeWeaponBreakableAuth(string weaponName = "", Int16 damage = 0);
    public void BreakElm(string weaponName = "", Int16 damage = 0, bool allowLoot = true);
}
