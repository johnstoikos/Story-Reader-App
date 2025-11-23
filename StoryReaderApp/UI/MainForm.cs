using StoryReaderApp.Models;
using StoryReaderApp.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace StoryReaderApp.UI
{
    public class MainForm : Form
    {
        private readonly IStoryService _service;
        private readonly IStoryInfoService _infoService;

       
        private ComboBox cbCategories;
        private ListBox lbStories;
        private TextBox txtContent;

        private ComboBox cbVoices;
        private TrackBar tbVolume, tbRate;
        private Button btnPlay, btnPause, btnResume, btnStop;

        private Label lblStatus;

        private SpeechSynthesizer _tts;
        private const string ALL_LABEL = "All";

        public MainForm(IStoryService service, IStoryInfoService infoService)
        {
            _service = service;
            _infoService = infoService;

            Text = "Story Reader";
            Width = 1000;
            Height = 650;
            StartPosition = FormStartPosition.CenterScreen;

            InitializeUi();
            InitializeTts();
            LoadCategories();   
        }

        private void InitializeUi()
        {
           
            var menu = new MenuStrip();
            var mFile = new ToolStripMenuItem("File");
            mFile.DropDownItems.Add(new ToolStripMenuItem("Exit", null, (s, e) => Close()));

            var mTools = new ToolStripMenuItem("Tools");
            mTools.DropDownItems.Add(new ToolStripMenuItem("Manage Stories...", null, (s, e) =>
            {
                using (var f = new StoryManagerForm(_service))
                    f.ShowDialog(this);
                LoadCategories(); 
            }));
            mTools.DropDownItems.Add(new ToolStripMenuItem("Online Info…", null, async (s, e) => await ShowOnlineInfoAsync()));

            menu.Items.Add(mFile);
            menu.Items.Add(mTools);
            MainMenuStrip = menu;
            Controls.Add(menu);

            
            var lblCat = new Label { Left = 20, Top = 40, Width = 70, Text = "Category:" };
            cbCategories = new ComboBox { Left = 95, Top = 36, Width = 230, DropDownStyle = ComboBoxStyle.DropDownList };
            cbCategories.SelectedIndexChanged += (s, e) => LoadStories();

            lbStories = new ListBox
            {
                Left = 20,
                Top = 70,
                Width = 250,
                Height = 400,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };
            lbStories.SelectedIndexChanged += (s, e) => LoadContent();

            
            txtContent = new TextBox
            {
                Left = 340,
                Top = 70,
                Width = 630,
                Height = 420,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            Controls.AddRange(new Control[] { lblCat, cbCategories, lbStories, txtContent });

            
            var bottomPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 90,
                ColumnCount = 5,
                RowCount = 2
            };

            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));     
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));  
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));     
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));  
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));     

            bottomPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            bottomPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            
            var lblVoice = new Label { Text = "Voice:", AutoSize = true, Anchor = AnchorStyles.Left };
            cbVoices = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };

            
            var lblVol = new Label { Text = "Volume:", AutoSize = true, Anchor = AnchorStyles.Left };
            tbVolume = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 100,
                TickFrequency = 10,
                Dock = DockStyle.Fill
            };

            
            var lblRate = new Label { Text = "Rate:", AutoSize = true, Anchor = AnchorStyles.Left };
            tbRate = new TrackBar
            {
                Minimum = -10,
                Maximum = 10,
                Value = 0,
                TickFrequency = 1,
                Dock = DockStyle.Fill
            };

            
            var buttonsPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            btnPlay = new Button { Text = "Play" };
            btnPause = new Button { Text = "Pause" };
            btnResume = new Button { Text = "Resume" };
            btnStop = new Button { Text = "Stop" };
            buttonsPanel.Controls.AddRange(new Control[] { btnPlay, btnPause, btnResume, btnStop });

            bottomPanel.Controls.Add(lblVoice, 0, 0);
            bottomPanel.Controls.Add(cbVoices, 1, 0);
            bottomPanel.Controls.Add(lblVol, 2, 0);
            bottomPanel.Controls.Add(tbVolume, 3, 0);

            bottomPanel.Controls.Add(lblRate, 0, 1);
            bottomPanel.Controls.Add(tbRate, 1, 1);
            bottomPanel.Controls.Add(buttonsPanel, 4, 0);
            bottomPanel.SetRowSpan(buttonsPanel, 2);

            Controls.Add(bottomPanel);

            lblStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 22,
                Text = "Ready.",
                BorderStyle = BorderStyle.Fixed3D,
                TextAlign = ContentAlignment.MiddleLeft
            };
            Controls.Add(lblStatus);

         
            cbVoices.SelectedIndexChanged += (s, e) => SelectVoice();
            tbVolume.ValueChanged += (s, e) => { if (_tts != null) _tts.Volume = tbVolume.Value; };
            tbRate.ValueChanged += (s, e) => { if (_tts != null) _tts.Rate = tbRate.Value; };

            btnPlay.Click += (s, e) => Play();
            btnPause.Click += (s, e) => Pause();
            btnResume.Click += (s, e) => Resume();
            btnStop.Click += (s, e) => StopSpeak();
        }





        private void InitializeTts()
        {
            _tts = new SpeechSynthesizer();
            cbVoices.Items.Clear();

            var voices = _tts.GetInstalledVoices()
                             .Select(v => v.VoiceInfo)
                             .ToList();

            foreach (var vi in voices)
                cbVoices.Items.Add($"{vi.Name} ({vi.Culture.Name})");

            if (voices.Count > 0)
            {
                _tts.SelectVoice(voices[0].Name);
                cbVoices.SelectedIndex = 0;
            }

            _tts.Volume = tbVolume.Value;
            _tts.Rate = tbRate.Value;

            _tts.SpeakStarted += (s, e) => SetStatus("Speaking...");
            _tts.SpeakCompleted += (s, e) => SetStatus("Completed.");
            _tts.StateChanged += (s, e) =>
            {
                if (e.State == SynthesizerState.Paused) SetStatus("Paused.");
                else if (e.State == SynthesizerState.Speaking) SetStatus("Speaking...");
            };
        }

        private void LoadCategories()
        {
            cbCategories.Items.Clear();
            foreach (var c in _service.GetCategoriesWithAllOption(ALL_LABEL))
                cbCategories.Items.Add(c);

            if (cbCategories.Items.Count > 0)
                cbCategories.SelectedIndex = 0;
        }

        private void LoadStories()
        {
            lbStories.Items.Clear();
            string cat = cbCategories.SelectedItem as string ?? ALL_LABEL;
            foreach (var s in _service.GetStories(cat, ALL_LABEL))
                lbStories.Items.Add(s);

            if (lbStories.Items.Count > 0)
            {
                lbStories.SelectedIndex = 0;
            }
            else
            {
                txtContent.Text = string.Empty;
            }
        }

        private void LoadContent()
        {
            var story = lbStories.SelectedItem as Story;
            txtContent.Text = story?.Content ?? string.Empty;
        }

        private void SelectVoice()
        {
            try
            {
                if (_tts != null && cbVoices.SelectedItem != null)
                {
                    string selected = cbVoices.SelectedItem.ToString();
                    string voiceName = selected.Split('(')[0].Trim();
                    _tts.SelectVoice(voiceName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Voice selection failed: " + ex.Message);
            }
        }

        private void Play()
        {
            if (string.IsNullOrWhiteSpace(txtContent.Text))
            {
                MessageBox.Show("No text to speak.");
                return;
            }

            try
            {
                _tts.SpeakAsyncCancelAll();
                _tts.Volume = tbVolume.Value;
                _tts.Rate = tbRate.Value;

                var pb = new PromptBuilder(new CultureInfo("en-US"));
                pb.AppendText(txtContent.Text);

                _tts.SpeakAsync(pb);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Speech error: " + ex.Message);
            }
        }

        private void Pause()
        {
            try { if (_tts.State == SynthesizerState.Speaking) _tts.Pause(); }
            catch { }
        }

        private void Resume()
        {
            try { if (_tts.State == SynthesizerState.Paused) _tts.Resume(); }
            catch { }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.ClientSize = new System.Drawing.Size(650, 436);
            this.Name = "MainForm";
            this.ResumeLayout(false);

        }

        private void StopSpeak()
        {
            try { _tts.SpeakAsyncCancelAll(); SetStatus("Ready."); } catch { }
        }

        private void SetStatus(string msg)
        {
            if (InvokeRequired) BeginInvoke(new Action(() => lblStatus.Text = msg));
            else lblStatus.Text = msg;
        }

        private async Task ShowOnlineInfoAsync()
        {
            var story = lbStories.SelectedItem as Story;
            if (story == null)
            {
                MessageBox.Show("Επίλεξε μία ιστορία πρώτα.");
                return;
            }

            try
            {
                UseWaitCursor = true;
                var info = await _infoService.GetInfoAsync(story.Title);
                UseWaitCursor = false;

                if (info == null)
                {
                    MessageBox.Show("Δεν βρέθηκαν πληροφορίες.");
                    return;
                }

                var msg =
                    "Title: " + (info.Title ?? story.Title) + Environment.NewLine +
                    "Description: " + (info.Author ?? "—") + Environment.NewLine +
                    "Summary: " + (info.Summary ?? "—") + Environment.NewLine +
                    "Source: " + (info.SourceUrl ?? "—");

                using (var dlg = new OnlineInfoForm(msg, info.SourceUrl))
                {
                    dlg.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                UseWaitCursor = false;
                MessageBox.Show("Σφάλμα online πληροφορίας: " + ex.Message);
            }
        }


       


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try { _tts.SpeakAsyncCancelAll(); _tts.Dispose(); } catch { }
            base.OnFormClosing(e);
        }
    }
}
