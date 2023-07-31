﻿using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Image2Display.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Image2Display.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _version = Utils.Version;

        [ObservableProperty]
        private string _newVersion = "";

        [ObservableProperty]
        private bool _hasNewVersion = false;

        [ObservableProperty]
        private string[] _languagesList = SettingModel.SupportLanguages;

        /// <summary>
        /// 被选中的语言index
        /// 不会在别处被更改，所以不需要notify
        /// </summary>
        public int LanguagesSelected
        {
            get
            {
                int index = 0;
                for (int i = 0; i < LanguagesList.Length; i++)
                {
                    if (LanguagesList[i] == Utils.Settings.Language)
                    {
                        index = i;
                        break;
                    }
                }
                return index;
            }
            set
            {
                Utils.Settings.Language = LanguagesList[value];
                Utils.ChangeLanguage(Utils.Settings.Language);
                Utils.SaveSettings();
            }
        }

        [RelayCommand]
        private async Task CheckUpdate()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("user-agent", "Image2Display");
                string data = await client.GetStringAsync("https://api.github.com/repos/chenxuuu/Image2Display/releases/latest");
                var jo = JsonSerializer.Deserialize<JsonObject>(data);
                var ver = (string)jo!["tag_name"]!;

                var vo = Utils.Version.Split('.');
                var vl = ver.Split('.');
                var hasNew = false;
                for (int i = 0; i < 3; i++)
                {
                    if (int.Parse(vl[i]) > int.Parse(vo[i]))
                        break;
                    else if (int.Parse(vl[i]) < int.Parse(vo[i]))
                    {
                        hasNew = true;
                        break;
                    }
                }
                if (hasNew)
                {
                    NewVersion = ver;
                    HasNewVersion = true;
                }
            }
            catch //(Exception ex)
            {
                //暂时先不管
            }
        }

        [RelayCommand]
        private void Update()
        {
            if (HasNewVersion)
                OpenUrl($"https://github.com/chenxuuu/llcom/releases/tag/{NewVersion}");
        }

        [RelayCommand]
        private void OpenUrl(string url)
        {
            Utils.OpenWebLink(url);
        }
    }
}
