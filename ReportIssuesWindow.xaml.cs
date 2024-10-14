using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Windows.Documents;

namespace PROG_ST10082700_MESSI
{
    public partial class ReportIssuesWindow : Window
    {
        private List<IssueReport> reportedIssues = new List<IssueReport>();
        private byte[] attachedFileContent;
        private string attachedFileName;
        private const long MAX_FILE_SIZE = 5 * 1024 * 1024; // 5 MB in bytes

        public ReportIssuesWindow()
        {
            InitializeComponent();
        }

        private void BtnAttachFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Allowed files (*.png;*.jpeg;*.jpg;*.doc;*.docx;*.pdf)|*.png;*.jpeg;*.jpg;*.doc;*.docx;*.pdf"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (IsValidFileType(openFileDialog.FileName))
                {
                    if (IsValidFileSize(openFileDialog.FileName))
                    {
                        attachedFileContent = File.ReadAllBytes(openFileDialog.FileName);
                        attachedFileName = Path.GetFileName(openFileDialog.FileName);
                        txtAttachedFileName.Text = attachedFileName;
                        btnDownloadAttachment.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MessageBox.Show("The uploaded file exceeds the maximum allowed size of 5 MB.",
                                        "File Too Large", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Please upload a valid file type (Only Image, Word document, or PDF).",
                                    "Invalid File Type", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private bool IsValidFileType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".png" || extension == ".jpeg" || extension == ".jpg"
                   || extension == ".doc" || extension == ".docx" || extension == ".pdf";
        }

        private bool IsValidFileSize(string filePath)
        {
            try
            {
                long fileSize = new FileInfo(filePath).Length;
                return fileSize <= MAX_FILE_SIZE;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking file size: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                IssueReport newIssue = new IssueReport
                {
                    Title = txtIssueTitle.Text,
                    Location = txtLocation.Text,
                    Category = (cmbCategory.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    Description = new TextRange(rtbDescription.Document.ContentStart, rtbDescription.Document.ContentEnd).Text,
                    AttachedFileContent = attachedFileContent,
                    AttachedFileName = attachedFileName,
                    ReportDate = DateTime.Now
                };

                reportedIssues.Add(newIssue);
                MessageBox.Show("Issue reported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
            }
        }

        private void BtnBackToHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtIssueTitle.Text))
            {
                MessageBox.Show("Please enter an issue title.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Please enter a location.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            txtIssueTitle.Clear();
            txtLocation.Clear();
            cmbCategory.SelectedIndex = -1;
            rtbDescription.Document.Blocks.Clear();
            txtAttachedFileName.Text = "No file chosen";
            attachedFileContent = null;
            attachedFileName = null;
            btnDownloadAttachment.Visibility = Visibility.Collapsed;
        }

        private void BtnDownloadAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (attachedFileContent != null && !string.IsNullOrEmpty(attachedFileName))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = attachedFileName,
                    Filter = "All Files (*.*)|*.*"
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