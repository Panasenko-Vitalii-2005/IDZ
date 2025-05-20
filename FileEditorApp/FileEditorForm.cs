using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace FileEditorApp
{
    // Абстрактні інтерфейси для завантажувачів і зберігачів
    public interface IFileLoader
    {
        string Load(string filePath);
    }

    public interface IFileSaver
    {
        void Save(string filePath, string content);
    }

    // Абстрактна фабрика
    public interface IFileFactory
    {
        IFileLoader CreateLoader();
        IFileSaver CreateSaver();
    }

    // Конкретні завантажувачі
    public class HtmlLoader : IFileLoader
    {
        public string Load(string filePath)
        {
            string content = File.ReadAllText(filePath);
            // Видаляємо HTML-теги, зберігаємо абзаци
            content = Regex.Replace(content, "<[^>]+>", match =>
            {
                if (match.Value.ToLower().Contains("<p") || match.Value.ToLower().Contains("<br"))
                    return "\n";
                return "";
            });
            return content.Trim();
        }
    }

    public class TxtLoader : IFileLoader
    {
        public string Load(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }

    public class BinLoader : IFileLoader
    {
        public string Load(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(bytes);
        }
    }

    // Конкретні зберігачі
    public class HtmlSaver : IFileSaver
    {
        public void Save(string filePath, string content)
        {
            // Додаємо базову HTML-структуру
            string htmlContent = $"<!DOCTYPE html>\n<html>\n<body>\n<p>{content.Replace("\n", "</p>\n<p>")}</p>\n</body>\n</html>";
            File.WriteAllText(filePath, htmlContent);
        }
    }

    public class TxtSaver : IFileSaver
    {
        public void Save(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }
    }

    public class BinSaver : IFileSaver
    {
        public void Save(string filePath, string content)
        {
            byte[] bytes = Convert.FromBase64String(content);
            File.WriteAllBytes(filePath, bytes);
        }
    }

    // Конкретні фабрики
    public class HtmlFactory : IFileFactory
    {
        public IFileLoader CreateLoader() => new HtmlLoader();
        public IFileSaver CreateSaver() => new HtmlSaver();
    }

    public class TxtFactory : IFileFactory
    {
        public IFileLoader CreateLoader() => new TxtLoader();
        public IFileSaver CreateSaver() => new TxtSaver();
    }

    public class BinFactory : IFileFactory
    {
        public IFileLoader CreateLoader() => new BinLoader();
        public IFileSaver CreateSaver() => new BinSaver();
    }

    // Головна форма (користувацька логіка)
    public partial class FileEditorForm : Form
    {
        private IFileFactory fileFactory;

        public FileEditorForm()
        {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string extension = Path.GetExtension(openFileDialog.FileName).ToLower();

                // Визначаємо фабрику залежно від розширення
                switch (extension)
                {
                    case ".html":
                        fileFactory = new HtmlFactory();
                        break;
                    case ".txt":
                        fileFactory = new TxtFactory();
                        break;
                    case ".bin":
                        fileFactory = new BinFactory();
                        break;
                    default:
                        MessageBox.Show("Unsupported file format");
                        return;
                }

                try
                {
                    // Очищаємо текстове поле перед завантаженням нового файлу
                    textBox.Clear();
                    IFileLoader loader = fileFactory.CreateLoader();
                    textBox.Text = loader.Load(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading file: {ex.Message}");
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (fileFactory == null)
            {
                MessageBox.Show("Please open a file first");
                return;
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Запитуємо підтвердження перед збереженням
                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to save to {Path.GetFileName(saveFileDialog.FileName)}?",
                    "Confirm Save",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        IFileSaver saver = fileFactory.CreateSaver();
                        saver.Save(saveFileDialog.FileName, textBox.Text);
                        MessageBox.Show("File saved successfully");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file: {ex.Message}");
                    }
                }
            }
        }
    }
}