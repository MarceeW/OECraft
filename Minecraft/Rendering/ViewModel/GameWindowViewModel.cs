﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Minecraft.Misc;
using Minecraft.Terrain;
using Minecraft.UI.Logic;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace Minecraft.Rendering.ViewModel
{
    internal class GameWindowViewModel : ObservableRecipient
    {
        public ICommand BackToGame { get; private set; }
        public ICommand Settings { get; private set; }
        public ICommand SettingsMenuBack { get; private set; }
        public ICommand MainMenuSettings { get; private set; }
        public ICommand SaveAndExit { get; private set; }
        public ICommand LoadTexture { get; private set; }
        public ICommand Create { get; private set; }
        public ICommand WorldDetailsCancel { get; private set; }
        public ICommand CreateNewWorld { get; private set; }
        public ICommand Play { get; private set; }
        public ICommand BackToMainMenu { get; private set; }
        public ICommand EnterSelectedWorld { get; private set; }
        public ICommand DeleteSelectedWorld { get; private set; }
        public ICommand ExitGame { get; private set; }

        private IUILogic uiLogic;
        private IInventoryLogic inventoryLogic;
        public GameWindowViewModel(GameWindow gameWindow)
        {
            uiLogic = Ioc.Default.GetService<IUILogic>();
            inventoryLogic = Ioc.Default.GetService<IInventoryLogic>();

            BackToGame = new RelayCommand(() => uiLogic.OpenClosePauseMenu());

            Settings = new RelayCommand(() => uiLogic.OpenCloseSettingsMenu());
            SettingsMenuBack = Settings;
            MainMenuSettings = Settings;

            ExitGame = new RelayCommand(() => gameWindow.Close());

            SaveAndExit = new RelayCommand(() =>
            {
                uiLogic.OnSaveAndExit();
            });

            LoadTexture = new RelayCommand(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Png images (*.png;)|*.png;";

                var result = openFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    AtlasTexturesData.TexturePath = openFileDialog.FileName;
                    inventoryLogic.ReloadTextures();
                }
            });

            Create = new RelayCommand(() => uiLogic.CreateWorld(gameWindow.WorldName.Text, gameWindow.WorldSeed.Text));

            WorldDetailsCancel = new RelayCommand(() => uiLogic.OpenCloseWorldCreationMenu());
            CreateNewWorld = new RelayCommand(() => uiLogic.OpenCloseWorldCreationMenu());

            Play = new RelayCommand(() =>
            {
                uiLogic.ReadWorlds();
                uiLogic.OpenCloseWorldSelectorMenu();
                gameWindow.WorldSelector.Items.Refresh();
            });
            BackToMainMenu = new RelayCommand(() =>
            {
                uiLogic.OpenCloseWorldSelectorMenu();
                gameWindow.WorldSelector.Items.Refresh();
            });


            EnterSelectedWorld = new RelayCommand(() =>
            {
                uiLogic.EnterWorld();
            },
            () => gameWindow.WorldSelector.SelectedIndex >= 0);

            DeleteSelectedWorld = new RelayCommand(() =>
            {
                var world = gameWindow.WorldSelector.SelectedItem as WorldData;

                if (world != null)
                {
                    Directory.Delete(world.WorldPath, true);
                    (gameWindow.WorldSelector.ItemsSource as List<WorldData>)?.Remove(world);
                    gameWindow.WorldSelector.Items.Refresh();
                }
            },
            () => gameWindow.WorldSelector.SelectedIndex >= 0);

            gameWindow.EnterSelectedWorldButton.Opacity = gameWindow.WorldSelector.SelectedIndex >= 0 ? 1 : 0.5;
            gameWindow.DeleteSelectedWorldButton.Opacity = gameWindow.WorldSelector.SelectedIndex >= 0 ? 1 : 0.5;

            gameWindow.WorldSelector.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
            {
                (DeleteSelectedWorld as RelayCommand).NotifyCanExecuteChanged();
                (EnterSelectedWorld as RelayCommand).NotifyCanExecuteChanged();

                gameWindow.EnterSelectedWorldButton.Opacity = gameWindow.WorldSelector.SelectedIndex >= 0 ? 1 : 0.5;
                gameWindow.DeleteSelectedWorldButton.Opacity = gameWindow.WorldSelector.SelectedIndex >= 0 ? 1 : 0.5;
            };
        }
    }
}