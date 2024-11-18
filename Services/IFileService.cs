using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG_ST10082700_MESSI.Services
{
    public interface IFileService
    {
        FileInfo OpenFile();
        byte[] GetAttachedFileContent();
        string GetAttachedFileName();
        void ClearAttachedFile();
        void DownloadAttachedFile();
    }
}
