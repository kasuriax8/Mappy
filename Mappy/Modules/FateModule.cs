﻿using System;
using System.Drawing;
using Dalamud.Interface;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using Lumina.Text.ReadOnly;
using Mappy.Classes;

namespace Mappy.Modules;

public class FateModule : ModuleBase {
    public override unsafe bool ProcessMarker(MarkerInfo markerInfo) {
        var markerName = markerInfo.PrimaryText?.Invoke();
        if (markerName.IsNullOrEmpty()) return false;
        
        foreach (var fate in Service.FateTable) {
            var name = new ReadOnlySeStringSpan(((FateContext*) fate.Address)->Name.AsSpan()).ExtractText();
            
            if (name.Equals(markerName, StringComparison.OrdinalIgnoreCase)) {
                var timeRemaining = TimeSpan.FromSeconds(fate.TimeRemaining);
                
                markerInfo.PrimaryText = () => $"Lv. {fate.Level} {fate.Name}";

                if (timeRemaining >= TimeSpan.Zero) {
                    markerInfo.SecondaryText = () => $"Time Remaining {timeRemaining:mm\\:ss}\nProgress {fate.Progress}%";

                    if (timeRemaining.TotalSeconds <= 300) {
                        var hue = (float)(timeRemaining.TotalSeconds / 300.0f * 25.0f);

                        var hsvColor = new ColorHelpers.HsvaColor(hue / 100.0f, 1.0f, 1.0f, 0.33f);
                        markerInfo.RadiusColor = ColorHelpers.HsvToRgb(hsvColor);
                    }
                }
                else {
                    markerInfo.SecondaryText = () => $"Progress {fate.Progress}%";
                }
                
                return true;
            }
        }

        return false;
    }
}