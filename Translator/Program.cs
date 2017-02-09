/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Windows.Forms;

namespace Translator
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      string filePath = (args.Length == 0 ? AskUserForFile() : args[0]);
      if (!File.Exists(filePath))
      {
        return; // minimum error check...
      }

      Application.Run(new Progress(filePath));
    }

    private static string AskUserForFile()
    {
      OpenFileDialog selectFile = new OpenFileDialog();
      selectFile.Filter = "Revit file (RVT)|*.RVT";
      selectFile.Multiselect = false;
      selectFile.CheckFileExists = true;
      selectFile.CheckPathExists = true;
      selectFile.Title = "Select a Revit file to extract information";
      selectFile.ShowDialog();
      return selectFile.FileName;
    }
  }
}
