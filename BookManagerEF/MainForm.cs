using BookManagerEF.Data;
using BookManagerEF.Models;
using System;
using System.Linq;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookManagerEF
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            LoadBooks();
        }

        private void LoadBooks()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var books = context.Books.OrderBy(b => b.Title).ToList();
                    dgvBooks.DataSource = books;

                    // Настройка отображения колонок
                    if (dgvBooks.Columns.Count > 0)
                    {
                        if (dgvBooks.Columns.Contains("Id"))
                            dgvBooks.Columns["Id"].HeaderText = "ID";
                        if (dgvBooks.Columns.Contains("Title"))
                            dgvBooks.Columns["Title"].HeaderText = "Название";
                        if (dgvBooks.Columns.Contains("Author"))
                            dgvBooks.Columns["Author"].HeaderText = "Автор";
                        if (dgvBooks.Columns.Contains("Year"))
                            dgvBooks.Columns["Year"].HeaderText = "Год";

                        if (dgvBooks.Columns.Contains("Id"))
                            dgvBooks.Columns["Id"].Width = 50;
                        if (dgvBooks.Columns.Contains("Title"))
                            dgvBooks.Columns["Title"].Width = 250;
                        if (dgvBooks.Columns.Contains("Author"))
                            dgvBooks.Columns["Author"].Width = 150;
                        if (dgvBooks.Columns.Contains("Year"))
                            dgvBooks.Columns["Year"].Width = 80;
                    }

                    lblCount.Text = $"Всего книг: {books.Count}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки книг: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string author = txtAuthor.Text.Trim();

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Введите название книги!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            try
            {
                var book = new Book
                {
                    Title = title,
                    Author = string.IsNullOrEmpty(author) ? "Не указан" : author,
                    Year = (int)numYear.Value
                };

                using (var context = new AppDbContext())
                {
                    context.Books.Add(book);
                    context.SaveChanges();
                }

                LoadBooks();
                ClearFields();

                MessageBox.Show("Книга успешно добавлена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления книги: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvBooks.CurrentRow == null)
            {
                MessageBox.Show("Выберите книгу для удаления!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int bookId = (int)dgvBooks.CurrentRow.Cells["Id"].Value;
            string bookTitle = dgvBooks.CurrentRow.Cells["Title"].Value.ToString();

            DialogResult result = MessageBox.Show($"Вы уверены, что хотите удалить книгу \"{bookTitle}\"?",
                "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (var context = new AppDbContext())
                    {
                        var book = context.Books.Find(bookId);
                        if (book != null)
                        {
                            context.Books.Remove(book);
                            context.SaveChanges();
                        }
                    }

                    LoadBooks();
                    MessageBox.Show("Книга успешно удалена!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления книги: {ex.Message}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadBooks();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtTitle.Clear();
            txtAuthor.Clear();
            numYear.Value = 2024;
            txtTitle.Focus();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите выйти?",
                "Выход", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
        }

        private void dgvBooks_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBooks.CurrentRow != null && dgvBooks.CurrentRow.Cells.Count > 0)
            {
                if (dgvBooks.CurrentRow.Cells["Title"].Value != null)
                    txtTitle.Text = dgvBooks.CurrentRow.Cells["Title"].Value.ToString();
                if (dgvBooks.CurrentRow.Cells["Author"].Value != null)
                    txtAuthor.Text = dgvBooks.CurrentRow.Cells["Author"].Value.ToString();
                if (dgvBooks.CurrentRow.Cells["Year"].Value != null)
                    numYear.Value = Convert.ToInt32(dgvBooks.CurrentRow.Cells["Year"].Value);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}