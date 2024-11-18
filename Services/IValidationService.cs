using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PROG_ST10082700_MESSI.Models;

namespace PROG_ST10082700_MESSI.Services
{
    public interface IValidationService
    {
        bool IsValidFile(FileInfo fileInfo);
        bool ValidateForm(string title, string location, object category);
        bool ValidateIssueReport(IssueReport issue);
    }
}
