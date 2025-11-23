using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace StoryReaderApp.UI
{
    public class OnlineInfoForm : Form
    {
        private TextBox txtInfo;
        private Button btnClose;
        private Button btnWikipedia;
        private string url;

        public OnlineInfoForm(string infoText, string url)
        {
            this.url = url;

            Text = "Online Information";
            Width = 600;
            Height = 400;
            StartPosition = FormStartPosition.CenterParent;

            txtInfo = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Top,
                Height = 300,
                Text = infoText
            };

            btnClose = new Button
            {
                Text = "OK",
                Left = 400,
                Top = 320,
                Width = 80
            };
            btnClose.Click += (s, e) => Close();

            btnWikipedia = new Button
            {
                Text = "Wikipedia",
                Left = 490,
                Top = 320,
                Width = 80,
                Enabled = !string.IsNullOrEmpty(url)
            };
            btnWikipedia.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(url))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
            };

            Controls.Add(txtInfo);
            Controls.Add(btnClose);
            Controls.Add(btnWikipedia);
        }
    }
}
