using CoreAudio;
using System;
using System.Windows.Forms;

namespace AudioSwitcher
{
    public class SettingsForm : Form
    {
        private Label _labelDevice1;
        private Label _labelDevice2;
        private ComboBox _comboDevice1;
        private ComboBox _comboDevice2;
        private Button _btnSave;
        private Button _btnCancel;
        private TableLayoutPanel _mainTable;
        private FlowLayoutPanel _buttonPanel;

        private readonly AudioDeviceManager _deviceManager;
        private readonly ConfigManager _configManager;

        public SettingsForm(AudioDeviceManager deviceManager, ConfigManager configManager)
        {
            _deviceManager = deviceManager;
            _configManager = configManager;

            InitializeComponents();
            LoadDevices();
            LoadSettings();
        }

        private void InitializeComponents()
        {
            this.Text = "设置输出设备";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Padding = new Padding(20);
            this.Font = new System.Drawing.Font("Segoe UI", 9f);

            _mainTable = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 3,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnStyles = 
                {
                    new ColumnStyle(SizeType.AutoSize),
                    new ColumnStyle(SizeType.Percent, 100)
                },
                RowStyles = 
                {
                    new RowStyle(SizeType.AutoSize),
                    new RowStyle(SizeType.AutoSize),
                    new RowStyle(SizeType.AutoSize)
                }
            };

            _labelDevice1 = new Label
            {
                Text = "输出设备一:",
                AutoSize = true,
                Margin = new Padding(0, 12, 12, 12),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            _labelDevice2 = new Label
            {
                Text = "输出设备二:",
                AutoSize = true,
                Margin = new Padding(0, 0, 12, 12),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            _comboDevice1 = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 320,
                Margin = new Padding(0, 10, 0, 10),
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            _comboDevice1.DisplayMember = "DeviceFriendlyName";
            _comboDevice2 = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 320,
                Margin = new Padding(0, 0, 0, 10),
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            _comboDevice2.DisplayMember = "DeviceFriendlyName";
            _buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 20, 0, 0)
            };

            _btnCancel = new Button
            {
                Text = "取消",
                AutoSize = true,
                MinimumSize = new System.Drawing.Size(100, 32),
                DialogResult = DialogResult.Cancel,
                Margin = new Padding(6, 0, 0, 0)
            };

            _btnSave = new Button
            {
                Text = "保存",
                AutoSize = true,
                MinimumSize = new System.Drawing.Size(100, 32),
                DialogResult = DialogResult.None,
                Margin = new Padding(0, 0, 6, 0)
            };

            _btnSave.Click += OnBtnSaveClick;

            _buttonPanel.Controls.Add(_btnCancel);
            _buttonPanel.Controls.Add(_btnSave);

            _mainTable.Controls.Add(_labelDevice1, 0, 0);
            _mainTable.Controls.Add(_comboDevice1, 1, 0);
            _mainTable.Controls.Add(_labelDevice2, 0, 1);
            _mainTable.Controls.Add(_comboDevice2, 1, 1);
            _mainTable.Controls.Add(_buttonPanel, 0, 2);
            _mainTable.SetColumnSpan(_buttonPanel, 2);

            this.Controls.Add(_mainTable);
        }

        private void LoadDevices()
        {
            var devices = _deviceManager.GetOutputDevices();
            
            _comboDevice1.Items.Clear();
            _comboDevice2.Items.Clear();

            foreach (var device in devices)
            {
                _comboDevice1.Items.Add(device);
                _comboDevice2.Items.Add(device);
            }
        }

        private void LoadSettings()
        {
            var config = _configManager.Config;

            if (config.Device1 != null)
            {
                for (int i = 0; i < _comboDevice1.Items.Count; i++)
                {
                    if (_comboDevice1.Items[i] is MMDevice device && device.ID == config.Device1.Id)
                    {
                        _comboDevice1.SelectedIndex = i;
                        break;
                    }
                }
            }

            if (config.Device2 != null)
            {
                for (int i = 0; i < _comboDevice2.Items.Count; i++)
                {
                    if (_comboDevice2.Items[i] is MMDevice device && device.ID == config.Device2.Id)
                    {
                        _comboDevice2.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void OnBtnSaveClick(object sender, EventArgs e)
        {
            try
            {
                if (_comboDevice1.SelectedItem is MMDevice device1)
                {
                    _configManager.SetDevice1(device1.ID, device1.DeviceFriendlyName);
                }

                if (_comboDevice2.SelectedItem is MMDevice device2)
                {
                    _configManager.SetDevice2(device2.ID, device2.DeviceFriendlyName);
                }

                MessageBox.Show("设置已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
