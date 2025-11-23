using System;
using System.Windows.Forms;
using StoryReaderApp.Models;

namespace StoryReaderApp.UI
{
    public class StoryEditDialog : Form
    {
        private TextBox txtTitle;
        private TextBox txtCategory;
        private TextBox txtContent;
        private Button btnOk, btnCancel;
        private ErrorProvider errorProvider;

        public Story Model { get; private set; }

        public StoryEditDialog(Story existing = null)
        {
            Text = existing == null ? "Νέα ιστορία" : "Επεξεργασία ιστορίας";
            Width = 700;
            Height = 560;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;

            errorProvider = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };

            var lblTitle = new Label { Left = 20, Top = 20, Width = 80, Text = "Title:" };
            txtTitle = new TextBox { Left = 110, Top = 16, Width = 540 };

            var lblCategory = new Label { Left = 20, Top = 55, Width = 80, Text = "Category:" };
            txtCategory = new TextBox { Left = 110, Top = 51, Width = 540 };

            var lblContent = new Label { Left = 20, Top = 90, Width = 80, Text = "Content:" };
            txtContent = new TextBox { Left = 110, Top = 86, Width = 540, Height = 360, Multiline = true, ScrollBars = ScrollBars.Vertical };

            btnOk = new Button { Left = 470, Top = 460, Width = 90, Text = "OK" };
            btnCancel = new Button { Left = 560, Top = 460, Width = 90, Text = "Cancel" };
            AcceptButton = btnOk; CancelButton = btnCancel;

            btnOk.Click += (s, e) =>
            {
                if (!ValidateAll()) return;

                if (Model == null) Model = new Story();
                Model.Title = txtTitle.Text.Trim();
                Model.Category = txtCategory.Text.Trim();
                Model.Content = txtContent.Text;
                DialogResult = DialogResult.OK;
            };
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { lblTitle, txtTitle, lblCategory, txtCategory, lblContent, txtContent, btnOk, btnCancel });

            if (existing != null)
            {
                Model = new Story { Id = existing.Id, Title = existing.Title, Category = existing.Category, Content = existing.Content };
                txtTitle.Text = existing.Title;
                txtCategory.Text = existing.Category;
                txtContent.Text = existing.Content;
            }

            txtTitle.Validating += (s, e) => { ValidateTitle(); };
            txtCategory.Validating += (s, e) => { ValidateCategory(); };
            txtContent.Validating += (s, e) => { ValidateContent(); };
        }

        private bool ValidateAll()
        {
            bool ok = ValidateTitle() & ValidateCategory() & ValidateContent();
            return ok;
        }

        private bool ValidateTitle()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                errorProvider.SetError(txtTitle, "Υποχρεωτικό.");
                return false;
            }
            errorProvider.SetError(txtTitle, null);
            return true;
        }

        private bool ValidateCategory()
        {
            if (string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                errorProvider.SetError(txtCategory, "Υποχρεωτικό.");
                return false;
            }
            errorProvider.SetError(txtCategory, null);
            return true;
        }

        private bool ValidateContent()
        {
            if (string.IsNullOrWhiteSpace(txtContent.Text))
            {
                errorProvider.SetError(txtContent, "Υποχρεωτικό.");
                return false;
            }
            errorProvider.SetError(txtContent, null);
            return true;
        }
    }
}
