using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using MultiplayerExtensions.OverrideClasses;
using Polyglot;
using System.Linq;
using UnityEngine;
using Zenject;

namespace MultiplayerExtensions.UI
{
    class QuickPlaySetupPanel : BSMLResourceViewController
    {

        public override string ResourceName => "MultiplayerExtensions.UI.QuickPlaySetupPanel.bsml";
        private IMultiplayerSessionManager sessionManager;

        CurvedTextMeshPro? modifierText;

        [Inject]
        internal void Inject(IMultiplayerSessionManager sessionManager, QuickPlaySetupViewController quickPlayViewController, MultiplayerLevelLoader levelLoader)
        {
            this.sessionManager = sessionManager;
            base.DidActivate(true, false, true);

            quickPlayViewController.didActivateEvent += OnActivate;
        }

        #region UIComponents
        [UIComponent("CustomSongsToggle")]
        public ToggleSetting customSongsToggle = null!;

        [UIComponent("VerticalHUDToggle")]
        public ToggleSetting verticalHUDToggle = null!;

        [UIComponent("DefaultHUDToggle")]
        public ToggleSetting defaultHUDToggle = null!;

        [UIComponent("HologramToggle")]
        public ToggleSetting hologramToggle = null!;

        [UIComponent("LagReducerToggle")]
        public ToggleSetting lagReducerToggle = null!;

        [UIComponent("MissLightingToggle")]
        public ToggleSetting missLightingToggle = null!;

        [UIComponent("DownloadProgressText")]
        public FormattableText downloadProgressText = null!;
        #endregion

        #region UIValues
        [UIValue("CustomSongs")]
        public bool CustomSongs
        {
            get => MPState.CustomSongsEnabled;
            set {
                if (MPState.CustomSongsEnabled != value)
                {
                    MPState.CustomSongsEnabled = value;
                    MPEvents.RaiseCustomSongsChanged(this, value);
                }
            }
        }

        [UIValue("VerticalHUD")]
        public bool VerticalHUD
        {
            get => Plugin.Config.VerticalHUD;
            set { Plugin.Config.VerticalHUD = value; }
        }

        [UIValue("DefaultHUD")]
        public bool DefaultHUD
        {
            get => Plugin.Config.SingleplayerHUD;
            set { Plugin.Config.SingleplayerHUD = value; }
        }

        [UIValue("Hologram")]
        public bool Hologram
        {
            get => Plugin.Config.Hologram;
            set { Plugin.Config.Hologram = value; }
        }

        [UIValue("LagReducer")]
        public bool LagReducer
        {
            get => Plugin.Config.LagReducer;
            set { Plugin.Config.LagReducer = value; }
        }

        [UIValue("MissLighting")]
        public bool MissLighting
        {
            get => Plugin.Config.MissLighting;
            set { Plugin.Config.MissLighting = value; }
        }

        [UIValue("DownloadProgress")]
        public string DownloadProgress
        {
            get => downloadProgressText.text;
            set { downloadProgressText.text = value; }
        }
        #endregion

        #region UIActions
        [UIAction("SetCustomSongs")]
        public void SetCustomSongs(bool value)
        {
            CustomSongs = value;
            customSongsToggle.Value = value;
        }

        [UIAction("SetVerticalHUD")]
        public void SetVerticalHUD(bool value)
        {
            VerticalHUD = value;
            verticalHUDToggle.Value = value;

            DefaultHUD = !(!DefaultHUD || !value);
            defaultHUDToggle.Value = !(!DefaultHUD || !value);
        }

        [UIAction("SetDefaultHUD")]
        public void SetDefaultHUD(bool value)
        {
            DefaultHUD = value;
            defaultHUDToggle.Value = value;

            VerticalHUD = VerticalHUD || value;
            verticalHUDToggle.Value = VerticalHUD || value;
        }

        [UIAction("SetHologram")]
        public void SetHologram(bool value)
        {
            Hologram = value;
            hologramToggle.Value = value;
        }

        [UIAction("SetLagReducer")]
        public void SetLagReducer(bool value)
        {
            LagReducer = value;
            lagReducerToggle.Value = value;
        }

        [UIAction("SetMissLighting")]
        public void SetMissLighting(bool value)
        {
            MissLighting = value;
            missLightingToggle.Value = value;
        }
        #endregion

        private void OnActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            sessionManager.playerStateChangedEvent += OnPlayerStateChanged;
            customSongsToggle.interactable = false;

            if (firstActivation)
            {
                //Transform spectatorText = transform.Find("Wrapper").Find("SpectatorModeWarningText");
                //spectatorText.position = new Vector3(spectatorText.position.x, 0.25f, spectatorText.position.z);
            }

            if (sessionManager.connectionOwner != null)
                OnPlayerStateChanged(sessionManager.connectionOwner);
        }

        private void OnPlayerStateChanged(IConnectedPlayer player)
        {
            customSongsToggle.interactable = false;
            if (player.userId != sessionManager.localPlayer.userId && player.isConnectionOwner)
            {
                SetCustomSongs(player.HasState("customsongs"));
            }
        }
    }
}
