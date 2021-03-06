﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BnsLauncher.Messages;
using BnsLauncher.Models;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;

namespace BnsLauncher.ViewModels
{
    public class SideViewModel : Screen
    {
        private readonly NavigationServices _navigationServices;
        private readonly IEventAggregator _eventAggregator;

        public SideViewModel(NavigationServices navigationServices, IEventAggregator eventAggregator, IFileSystem fs)
        {
            _navigationServices = navigationServices;
            _eventAggregator = eventAggregator;

            NavigationItems = new List<NavigationItem>
            {
                new NavigationItem("Profiles", PackIconKind.Wrench, GoTo<ProfilesViewModel>),
                new NavigationItem("Accounts", PackIconKind.Person, GoTo<AccountsViewModel>),
                new NavigationItem("Open profiles folder", PackIconKind.CodeTags, () =>
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = fs.Path.GetFullPath(Constants.ProfilesPath)
                    });
                    return Task.CompletedTask;
                }),
                new NavigationItem("Settings", PackIconKind.Cog, GoTo<SettingsViewModel>),
                new NavigationItem("Logs", PackIconKind.BugOutline, GoTo<LogViewModel>),
                new NavigationItem("About", PackIconKind.About, GoTo<AboutViewModel>),
                new NavigationItem("Exit", PackIconKind.ExitRun, Exit),
            };
        }

        public List<NavigationItem> NavigationItems { get; }

        public Task TabChanged(string name)
        {
            var item = NavigationItems.FirstOrDefault(x => x.Name == name);

            return item == null
                ? Task.CompletedTask
                : item.Action();
        }

        private Task ReloadProfiles()
        {
            return _eventAggregator.PublishOnUIThreadAsync(new ReloadProfilesMessage());
        }

        private Task Exit()
        {
            Application.Current.Shutdown();
            return Task.CompletedTask;
        }

        private Task GoTo<T>()
        {
            _navigationServices.Main.NavigateToViewModel<T>();
            return Task.CompletedTask;
        }
    }
}