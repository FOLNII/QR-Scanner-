using System;
using System.Drawing;
using System.Windows.Forms;
using ZXing;
using ZXing.Windows.Compatibility;
using System.IO;
using System.Collections.Generic;

namespace QRScanner
{
    public partial class qrUI : Form
    {
        private Button btnLoad;
        private Button btnSave;
        private Button btnLanguage;
        private Label lblTitle;
        private TextBox txtResult;
        private Label lblMessage;

        private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>
        {
            { "ru", new Dictionary<string, string>
                {
                    { "Title", "QR Сканер" },
                    { "LoadButton", "Загрузить изображение" },
                    { "SaveButton", "Сохранить результат" },
                    { "LanguageButton", "English" },
                    { "DefaultText", "Тут будет выведен результат" },
                    { "SuccessMessage", "QR-код успешно распознан!" },
                    { "NotFoundMessage", "QR-код не найден на изображении." },
                    { "SaveSuccessMessage", "Результат успешно сохранен!" },
                    { "SaveErrorMessage", "Ошибка при сохранении: {0}" }
                }
            },
            { "en", new Dictionary<string, string>
                {
                    { "Title", "QR Scanner" },
                    { "LoadButton", "Load Image" },
                    { "SaveButton", "Save Result" },
                    { "LanguageButton", "Русский" },
                    { "DefaultText", "Result will be displayed here" },
                    { "SuccessMessage", "QR code successfully recognized!" },
                    { "NotFoundMessage", "QR code not found in the image." },
                    { "SaveSuccessMessage", "Result successfully saved!" },
                    { "SaveErrorMessage", "Error while saving: {0}" }
                }
            }
        };

        private string currentLanguage = "ru";

        public qrUI()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Окно приложения
            this.Text = "QR Code Scanner";
            this.Size = new Size(650, 600); //начальный размер окна
            this.MinimumSize = new Size(400, 600); //минимальный размер окна
            this.Resize += qrUI_Resize;

            lblTitle = new Label
            {
                //Надпись посередине сверху
                Font = new Font("Arial", 16, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblTitle);

            btnLoad = new Button
            {
                //Кнопка "Загрузить QR"
                Size = new Size(200, 40),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            btnLoad.Click += BtnLoad_Click;
            this.Controls.Add(btnLoad);
            CenterButton(btnLoad, 70);

            // Кнопка сохранения
            btnSave = new Button
            {
                Size = new Size(200, 40),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
            CenterButton(btnSave, 120);

            // Кнопка смены языка
            btnLanguage = new Button
            {
                Size = new Size(100, 30),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            btnLanguage.Click += BtnLanguage_Click;
            this.Controls.Add(btnLanguage);
            CenterButton(btnLanguage, 180);

            // Текстовое с результатом
            txtResult = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Bottom,
                Height = 300,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.LightGray
            };
            this.Controls.Add(txtResult);

            //сообщения о том, что qr прочитан
            lblMessage = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 20,
                ForeColor = Color.Red
            };
            this.Controls.Add(lblMessage);

            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            var lang = translations[currentLanguage];
            lblTitle.Text = lang["Title"];
            btnLoad.Text = lang["LoadButton"];
            btnSave.Text = lang["SaveButton"];
            btnLanguage.Text = lang["LanguageButton"];
            txtResult.Text = lang["DefaultText"];
        }

        private void CenterButton(Button button, int yOffset)
        {
            button.Location = new Point((this.ClientSize.Width - button.Width) / 2, yOffset);
        }

        private void qrUI_Resize(object sender, EventArgs e)
        {
            CenterButton(btnLoad, 70);
            CenterButton(btnSave, 120);
            CenterButton(btnLanguage, 180);
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())//Диалоговое окно, выбор изображения декодинг QR
                {
                    ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        using (var bitmap = new Bitmap(ofd.FileName))
                        {
                            var luminanceSource = new BitmapLuminanceSource(bitmap);
                            var reader = new BarcodeReader();
                            var result = reader.Decode(luminanceSource);

                            if (result != null)
                            {
                                txtResult.Text = result.Text;
                                lblMessage.Text = translations[currentLanguage]["SuccessMessage"];
                            }
                            else
                            {
                                txtResult.Text = translations[currentLanguage]["NotFoundMessage"];
                                lblMessage.Text = string.Empty;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = string.Format(translations[currentLanguage]["SaveErrorMessage"], ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)//Сохранение текста
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Text Files|*.txt";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(sfd.FileName, txtResult.Text);
                        lblMessage.Text = translations[currentLanguage]["SaveSuccessMessage"];
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = string.Format(translations[currentLanguage]["SaveErrorMessage"], ex.Message);
            }
        }

        private void BtnLanguage_Click(object sender, EventArgs e)
        {
            currentLanguage = currentLanguage == "ru" ? "en" : "ru";
            UpdateLanguage();
        }
    }
}
