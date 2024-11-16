using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Windows.Documents;
using System.Security.Cryptography;

namespace PROG_ST10082700_MESSI
{
    public partial class ReportIssuesWindow : Window
    {
        private readonly IIssueReportService _issueReportService;
        private readonly IFileService _fileService;
        private readonly IValidationService _validationService;

        // Constructor now takes interfaces as parameters
        public ReportIssuesWindow(IIssueReportService issueReportService, IFileService fileService, IValidationService validationService)
        {
            InitializeComponent();
            _issueReportService = issueReportService;
            _fileService = fileService;
            _validationService = validationService;
        }

        // Event handler for file attachment
        private void BtnAttachFile_Click(object sender, RoutedEventArgs e)
        {
            var fileInfo = _fileService.OpenFile();
            if (fileInfo != null)
            {
                if (_validationService.IsValidFile(fileInfo))
                {
                    UpdateAttachmentUI(fileInfo);
                }
            }
        }

        // Updates UI after file attachment
        private void UpdateAttachmentUI(FileInfo fileInfo)
        {
            txtAttachedFileName.Text = fileInfo.Name;
            btnDownloadAttachment.Visibility = Visibility.Visible;
        }

        // Event handler for form submission
        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (_validationService.ValidateForm(txtIssueTitle.Text, txtLocation.Text, cmbCategory.SelectedItem))
            {
                var newIssue = CreateIssueReport();
                _issueReportService.AddIssue(newIssue);
                MessageBox.Show("Issue reported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
            }
        }

        // Creates a new IssueReport object from form data
        private IssueReport CreateIssueReport()
        {
            return new IssueReport
            {
                Title = txtIssueTitle.Text,
                Location = txtLocation.Text,
                Category = (cmbCategory.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Description = new TextRange(rtbDescription.Document.ContentStart, rtbDescription.Document.ContentEnd).Text,
                AttachedFileContent = _fileService.GetAttachedFileContent(),
                AttachedFileName = _fileService.GetAttachedFileName(),
                ReportDate = DateTime.Now
            };
        }

        // Event handler for returning to home screen
        private void BtnBackToHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Clears all form fields
        private void ClearForm()
        {
            txtIssueTitle.Clear();
            txtLocation.Clear();
            cmbCategory.SelectedIndex = -1;
            rtbDescription.Document.Blocks.Clear();
            txtAttachedFileName.Text = "No file chosen";
            _fileService.ClearAttachedFile();
            btnDownloadAttachment.Visibility = Visibility.Collapsed;
        }

        // Event handler for downloading attached file
        private void BtnDownloadAttachment_Click(object sender, RoutedEventArgs e)
        {
            _fileService.DownloadAttachedFile();
        }
    }

    // Interface for issue report service
    public interface IIssueReportService
    {
        void AddIssue(IssueReport issue);
    }

    // Interface for file service
    public interface IFileService
    {
        FileInfo OpenFile();
        byte[] GetAttachedFileContent();
        string GetAttachedFileName();
        void ClearAttachedFile();
        void DownloadAttachedFile();
    }

    // Interface for validation service
    public interface IValidationService
    {
        bool IsValidFile(FileInfo fileInfo);
        bool ValidateForm(string title, string location, object category);
    }

    // Implementation of IssueReportService
    public class IssueReportService : IIssueReportService
    {
        private List<IssueReport> reportedIssues = new List<IssueReport>();

        public void AddIssue(IssueReport issue)
        {
            reportedIssues.Add(issue);
        }
    }

    // Implementation of FileService
    public class FileService : IFileService
    {
        private byte[] attachedFileContent;
        private string attachedFileName;
        public static readonly long MAX_FILE_SIZE = 5 * 1024 * 1024; // 5 MB, now public and static

        public FileInfo OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Allowed files (*.png;*.jpeg;*.jpg;*.doc;*.docx;*.pdf)|*.png;*.jpeg;*.jpg;*.doc;*.docx;*.pdf"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return new FileInfo(openFileDialog.FileName);
            }

            return null;
        }

        public byte[] GetAttachedFileContent()
        {
            return attachedFileContent;
        }

        public string GetAttachedFileName()
        {
            return attachedFileName;
        }

        public void ClearAttachedFile()
        {
            attachedFileContent = null;
            attachedFileName = null;
        }

        public void DownloadAttachedFile()
        {
            if (attachedFileContent != null && !string.IsNullOrEmpty(attachedFileName))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = attachedFileName,
                    Filter =  "Allowed files (*.png;*.jpeg;*.jpg;*.doc;*.docx;*.pdf)|*.png;*.jpeg;*.jpg;*.doc;*.docx;*.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllBytes(saveFileDialog.FileName, attachedFileContent);
                    MessageBox.Show("File downloaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No file attached to download.", "No Attachment", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    // Implementation of ValidationService
    public class ValidationService : IValidationService
    {
        public bool IsValidFile(FileInfo fileInfo)
        {
            if (!IsValidFileType(fileInfo.Extension))
            {
                MessageBox.Show("Please upload a valid file type (Only Image, Word document, or PDF).",
                                "Invalid File Type", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!IsValidFileSize(fileInfo.Length))
            {
                MessageBox.Show("The uploaded file exceeds the maximum allowed size of 5 MB.",
                                "File Too Large", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public bool ValidateForm(string title, string location, object category)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter an issue title.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                MessageBox.Show("Please enter a location.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (category == null)
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool IsValidFileType(string extension)
        {
            string[] validExtensions = { ".png", ".jpeg", ".jpg", ".doc", ".docx", ".pdf" };
            return Array.Exists(validExtensions, e => e.Equals(extension.ToLower()));
        }

        private bool IsValidFileSize(long fileSize)
        {
            return fileSize <= FileService.MAX_FILE_SIZE;
        }
    }

    // Model class for IssueReport
    public class IssueReport
    {
        public string Title { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public byte[] AttachedFileContent { get; set; }
        public string AttachedFileName { get; set; }
        public DateTime ReportDate { get; set; }
    }
}