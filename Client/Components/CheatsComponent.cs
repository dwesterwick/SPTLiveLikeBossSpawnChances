using Comfort.Common;
using EFT;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DansDevTools.Components
{
    internal class CheatsComponent : MonoBehaviour
    {
        private Player mainPlayer = null!;

        protected void Update()
        {
            if (mainPlayer == null)
            {
                mainPlayer = Singleton<GameWorld>.Instance.MainPlayer;
                return;
            }

            if (DansDevToolsPlugin.InfiniteStamina.Value)
            {
                ResetStamina();
            }

            if (DansDevToolsPlugin.InfiniteHydrationAndEnergy.Value)
            {
                ResetHydrationAndEnergy();
            }
        }

        private void ResetStamina()
        {
            mainPlayer.Physical.Stamina.Current = mainPlayer.Physical.Stamina.TotalCapacity.Value;
            mainPlayer.Physical.HandsStamina.Current = mainPlayer.Physical.HandsStamina.TotalCapacity.Value;
            mainPlayer.Physical.Oxygen.Current = mainPlayer.Physical.Oxygen.TotalCapacity.Value;
        }

        private void ResetHydrationAndEnergy()
        {
            mainPlayer.ActiveHealthController.ChangeHydration(mainPlayer.ActiveHealthController.Hydration.Maximum);
            mainPlayer.ActiveHealthController.ChangeEnergy(mainPlayer.ActiveHealthController.Energy.Maximum);
        }
    }
}
