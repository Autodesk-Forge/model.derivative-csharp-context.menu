using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        MessageBox.Show(filePath + File.Exists(filePath));
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
