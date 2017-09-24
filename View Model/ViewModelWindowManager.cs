using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buffet
{
    public abstract class ViewModelWindowManager
    {
        public virtual string[] RequestOpenFile(string initialPath, string filter, bool multiValue)
        {
            var dlg = new OpenFileDialog();
            dlg.Multiselect = multiValue;
            dlg.Filter = filter;
            dlg.FileName = initialPath;
            var res = dlg.ShowDialog();
            if(res.HasValue && res.Value)
            {
                if (multiValue) return dlg.FileNames;
                else return new string[] { dlg.FileName };
            }
            return Array.Empty<string>();
        }

        public virtual string RequestSaveFile(string initialPath, string filter, bool allowOverwrite)
        {
            var dlg = new SaveFileDialog();
            dlg.OverwritePrompt = allowOverwrite;
            dlg.Filter = filter;
            dlg.FileName = initialPath;
            var res = dlg.ShowDialog();
            if(res.HasValue && res.Value)
            {
                return dlg.FileName;
            }
            return null;
        }

    }
}
