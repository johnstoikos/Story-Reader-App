using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using StoryReaderApp.Models;
using StoryReaderApp.Services;

namespace StoryReaderApp.UI
{
    public class StoryManagerForm : Form
    {
        private readonly IStoryService _service;
        private DataGridView grid;
        private ComboBox cbCategoryFilter;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;

        private const string ALL_LABEL = "Όλες";

        public StoryManagerForm(IStoryService service)
        {
            _service = service;
            Text = "Διαχείριση Ιστοριών";
            Width = 900;
            Height = 600;
            StartPosition = FormStartPosition.CenterParent;

            InitializeUi();
            LoadCategories();
            LoadData();
        }

        private void InitializeUi()
        {
            var lblFilter = new Label { Left = 20, Top = 20, Width = 120, Text = "Κατηγορία:" };
            cbCategoryFilter = new ComboBox { Left = 100, Top = 16, Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
            cbCategoryFilter.SelectedIndexChanged += (s, e) => LoadData();

            btnAdd = new Button { Left = 370, Top = 14, Width = 90, Text = "Add" };
            btnEdit = new Button { Left = 470, Top = 14, Width = 90, Text = "Edit" };
            btnDelete = new Button { Left = 570, Top = 14, Width = 90, Text = "Delete" };
            btnRefresh = new Button { Left = 670, Top = 14, Width = 90, Text = "Refresh" };

            btnAdd.Click += (s, e) => DoAdd();
            btnEdit.Click += (s, e) => DoEdit();
            btnDelete.Click += (s, e) => DoDelete();
            btnRefresh.Click += (s, e) => LoadData();

            grid = new DataGridView
            {
                Left = 20,
                Top = 50,
                Width = 840,
                Height = 480,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false
            };

            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "Title", Width = 250 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Category", HeaderText = "Category", Width = 150 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Content", HeaderText = "Content", Width = 360 });

            Controls.AddRange(new Control[] { lblFilter, cbCategoryFilter, btnAdd, btnEdit, btnDelete, btnRefresh, grid });
        }

        private void LoadCategories()
        {
            var cats = _service.GetCategoriesWithAllOption(ALL_LABEL).ToList();
            cbCategoryFilter.DataSource = cats;
        }

        private void LoadData()
        {
            string cat = cbCategoryFilter.SelectedItem as string ?? ALL_LABEL;
            var list = _service.GetStories(cat, ALL_LABEL).ToList();
            grid.DataSource = list;
        }

        private Story GetSelected()
        {
            if (grid.CurrentRow == null) return null;
            return grid.CurrentRow.DataBoundItem as Story;
        }

        private void DoAdd()
        {
            using (var dlg = new StoryEditDialog())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _service.Add(dlg.Model);
                    LoadCategories();
                    LoadData();
                }
            }
        }

        private void DoEdit()
        {
            var sel = GetSelected();
            if (sel == null) { MessageBox.Show("Επίλεξε μια ιστορία."); return; }
            using (var dlg = new StoryEditDialog(sel))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    // διατηρούμε το Id
                    dlg.Model.Id = sel.Id;
                    _service.Edit(dlg.Model);
                    LoadData();
                }
            }
        }

        private void DoDelete()
        {
            var sel = GetSelected();
            if (sel == null) { MessageBox.Show("Επίλεξε μια ιστορία."); return; }
            if (MessageBox.Show($"Διαγραφή: {sel.Title};", "Επιβεβαίωση", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _service.Remove(sel.Id);
                LoadCategories();
                LoadData();
            }
        }
    }
}
