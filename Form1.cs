using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Simple_Text_Editor
{
    public partial class Form1 : Form
    {
        private float _fontSize;
        private bool _isModified = false;
        private string _currentFilePath = @"C:\Users\george\Desktop\поиск работы.txt";
        private bool _isFirstLaunch = true;
        private string _text;
        private bool _isTextChangedFromLoad;

        public Form1()
        {
#if DEBUG
            string filePath = @"C:\Users\george\Desktop\поиск работы.txt";
            if (File.Exists(filePath))
            {
                try
                {
                    _text = File.ReadAllText(filePath);
                    _currentFilePath = filePath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось открыть файл:\n" + ex.Message);
                }
            }
#endif
            Init();
        }

        private void Init()
        {
            InitializeComponent();

            this.richTextBox1.MouseWheel += RichTextBox1_MouseWheel;
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            UpdateWindowTitle();
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (_text != "")
                {
                    _isTextChangedFromLoad = true;
                    richTextBox1.Text = _text;
                    Refresh();
                }
            }
#if DEBUG
            if (args.Length > 0)
            {
                if (_text != "")
                {
                    _isTextChangedFromLoad = true;
                    richTextBox1.Text = _text;
                    Refresh();
                }
            }
#endif
        }

        public Form1(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    _text = File.ReadAllText(filePath);
                    _currentFilePath = filePath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось открыть файл:\n" + ex.Message);
                }
            }
            Init();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;
                SaveFile();
            }
        }

        private void SaveFile()
        {
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1 || _currentFilePath != null)
            {
                string path = args.Length == 1 ? _currentFilePath : args[1];
                if (File.Exists(path))
                {
                    File.WriteAllText(path, richTextBox1.Text);
                }
                _isModified = false;
                _currentFilePath = path;
                UpdateWindowTitle();
            }
            else
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFile.FileName, richTextBox1.Text);
                    _isModified = false;
                    _currentFilePath = saveFile.FileName;
                    UpdateWindowTitle();
                }
            }
        }

        private void RichTextBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                ((HandledMouseEventArgs)e).Handled = true;

                _fontSize = Math.Max(6f, _fontSize + e.Delta * 0.03f);
                _fontSize = (float)Math.Round(_fontSize);
                ResetFont(_fontSize);
                label1.Text = "Размер шрифта: " + richTextBox1.Font.Size + "   Количество символов: " + richTextBox1.Text.Length;
            }
        }

        private void ResetFont(float fontSize)
        {
            int selStart = richTextBox1.SelectionStart;
            int selLength = richTextBox1.SelectionLength;
            string text = richTextBox1.Rtf;

            richTextBox1.Clear();
            richTextBox1.Font = new Font(richTextBox1.Font, richTextBox1.Font.Style);
            richTextBox1.Rtf = text;

            richTextBox1.SelectionStart = selStart;
            richTextBox1.SelectionLength = selLength;

            richTextBox1.Font = new Font(richTextBox1.Font.FontFamily, fontSize, richTextBox1.Font.Style);
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Text = File.ReadAllText(openFile.FileName);
            }

            richTextBox1.Text = File.ReadAllText(openFile.FileName);
            _currentFilePath = openFile.FileName;
            _isModified = false;
            UpdateWindowTitle();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            float savedFontSize = Properties.Settings.Default.FontSize;
            if (savedFontSize > 0)
            {
                richTextBox1.Font = new Font(richTextBox1.Font.FontFamily, savedFontSize, richTextBox1.Font.Style);
                _fontSize = savedFontSize;
                label1.Text = "Размер шрифта: " + richTextBox1.Font.Size + "   Количество символов: " + richTextBox1.Text.Length;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.FontSize = richTextBox1.Font.SizeInPoints;
            Properties.Settings.Default.Save();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (!_isTextChangedFromLoad)
            {
                _isModified = true;
                UpdateWindowTitle();
            }
            _isTextChangedFromLoad = false;
            label1.Text = "Размер шрифта: " + richTextBox1.Font.Size + "   Количество символов: " + richTextBox1.Text.Length;
        }

        private void UpdateWindowTitle()
        {
            string fileName = _currentFilePath != null
                ? Path.GetFileName(_currentFilePath)
                : "Без имени";

            //if(_isFirstLaunch)
            //{
            //    this.Text = $"{fileName} - Simple Text Editor";
            //    _isFirstLaunch = false;
            //    return;
            //}

            this.Text = _isModified
                ? $"{fileName}* - Simple Text Editor"
                : $"{fileName} - Simple Text Editor";
        }
    }
}
