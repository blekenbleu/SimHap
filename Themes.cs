﻿using System;
using System.Windows;

namespace sierses.Sim
{
    internal class Themes
    {
        public static void ChangeTheme(Uri themeuri)
        {
            ResourceDictionary Theme = new ResourceDictionary() { Source = themeuri };
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(Theme);
        }
    }
}
