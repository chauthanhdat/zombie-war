using UnityEngine;
using System.Collections.Generic;

namespace ZombieWar.Core.Events
{
    public static class GameEvent
    {
        #region Gameplay Events

        public static System.Action OnSwapWeaponRequested;
        public static System.Action OnWeaponSwitched;
        public static System.Action OnAttack;
        public static System.Action OnDeath;

        public static System.Action OnZombieKilled;

        #endregion
    }
}