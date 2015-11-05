using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GameLauncherV1;

/// <summary>
/// Author: Matt Gipson
/// Contact: Deadwynn@gmail.com
/// Domain: www.livingvalkyrie.com
/// 
/// Description: Launcher 
/// </summary>
class Launcher {
    #region Fields

    #endregion

    public static void Launch() {
        Process.Start( Path.GetFullPath( "Testing Environment/Build.exe" ) );
        if (MainWindow.close) {
            Environment.Exit(0);
        }
    }

}