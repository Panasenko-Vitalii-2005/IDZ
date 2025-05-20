using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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

    // Інтерфейс для спостерігача
    public interface ITextObserver
    {
        void Update(string message);
    }

    // Конкретний спостерігач для відображення повідомлень
    public class MessageBoxObserver : ITextObserver
    {
        public void Update(string message)
        {
            MessageBox.Show(message, "Text Editor Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    // Клас для відстеження змін у текстовому полі
    public class TextSubject
    {
        private readonly List<ITextObserver> _observers = new List<ITextObserver>();
        private string _previousText = string.Empty;

        public void Attach(ITextObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(ITextObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(string message)
        {
            foreach (var observer in _observers)
            {
                observer.Update(message);
            }
        }

        public void CheckTextChanges(string currentText)
        {
            if (!string.IsNullOrEmpty(_previousText))
            {
                // Перевірка видалення слів
                var previousWords = Regex.Split(_previousText, @"\s+").Length;
                var currentWords = Regex.Split(currentText, @"\s+").Length;
                var deletedWords = previousWords - currentWords;

                if (deletedWords > 1)
                {
                    Notify($"Removed {deletedWords} words.");
                }
            }
            _previousText = currentText;
        }
    }

    // Головна форма (користувацька логіка)
    public partial class FileEditorForm : Form
    {
        private IFileFactory fileFactory;
        private string currentFilePath; // Зберігаємо шлях до поточного файлу
        private TextSubject textSubject;
        private int previousParagraphCount = 0;

        public FileEditorForm()
        {
            InitializeComponent();
            textSubject = new TextSubject();
            textSubject.Attach(new MessageBoxObserver());
            textBox.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            textSubject.CheckTextChanges(textBox.Text);

            // Перевірка кількості абзаців для автозбереження
            int currentParagraphCount = textBox.Text.Split(new[] { "\n" }, StringSplitOptions.None).Length;
            if (currentParagraphCount > previousParagraphCount && fileFactory != null && !string.IsNullOrEmpty(currentFilePath))
            {
                try
                {
                    IFileSaver saver = fileFactory.CreateSaver();
                    saver.Save(currentFilePath, textBox.Text);
                    textSubject.Notify("File updated with new paragraph.");
                }
                catch (Exception ex)
                {
                    textSubject.Notify($"Error during autosave: {ex.Message}");
                }
            }
            previousParagraphCount = currentParagraphCount;
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string extension = Path.GetExtension(openFileDialog.FileName).ToLower();
                currentFilePath = openFileDialog.FileName;

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
                    textBox.Clear();
                    IFileLoader loader = fileFactory.CreateLoader();
                    textBox.Text = loader.Load(currentFilePath);
                    previousParagraphCount = textBox.Text.Split(new[] { "\n" }, StringSplitOptions.None).Length;
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
                currentFilePath = saveFileDialog.FileName;
                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to save to {Path.GetFileName(currentFilePath)}?",
                    "Confirm Save",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        IFileSaver saver = fileFactory.CreateSaver();
                        saver.Save(currentFilePath, textBox.Text);
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