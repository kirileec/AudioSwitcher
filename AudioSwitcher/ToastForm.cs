using System;
using System.Drawing;
using System.Windows.Forms;

namespace AudioSwitcher
{
    public class ToastForm : Form
    {
        private Label _messageLabel;
        private System.Windows.Forms.Timer _hideTimer;

        public ToastForm(string title, string message)
        {
            InitializeComponents();
            SetText(title, message);
        }

        private void InitializeComponents()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.FromArgb(40, 40, 40);
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size(800, 160);
            this.Opacity = 0.9;

            _messageLabel = new Label
            {
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 0, 20, 0)
            };

            this.Controls.Add(_messageLabel);

            _hideTimer = new System.Windows.Forms.Timer
            {
                Interval = 2000
            };
            _hideTimer.Tick += OnHideTimerTick;

            this.Load += OnLoad;
        }

        private void SetText(string title, string message)
        {
            _messageLabel.Text = $"{title}\n{message}";
        }

        private void OnLoad(object sender, EventArgs e)
        {
            PositionForm();
            AnimateIn();
            _hideTimer.Start();
        }

        private void PositionForm()
        {
            var workingArea = Screen.PrimaryScreen.WorkingArea;
            var x = (workingArea.Width - this.Width) / 2;
            var y = workingArea.Bottom - this.Height - 50;
            this.Location = new Point(x, y);
        }

        private void AnimateIn()
        {
            this.Opacity = 0;
            var fadeInTimer = new System.Windows.Forms.Timer { Interval = 20 };
            var opacity = 0.0;
            fadeInTimer.Tick += (s, args) =>
            {
                opacity += 0.1;
                if (opacity >= 0.9)
                {
                    this.Opacity = 0.9;
                    fadeInTimer.Stop();
                    fadeInTimer.Dispose();
                }
                else
                {
                    this.Opacity = opacity;
                }
            };
            fadeInTimer.Start();
        }

        private void AnimateOut()
        {
            var fadeOutTimer = new System.Windows.Forms.Timer { Interval = 20 };
            var opacity = this.Opacity;
            fadeOutTimer.Tick += (s, args) =>
            {
                opacity -= 0.1;
                if (opacity <= 0)
                {
                    this.Opacity = 0;
                    fadeOutTimer.Stop();
                    fadeOutTimer.Dispose();
                    this.Close();
                }
                else
                {
                    this.Opacity = opacity;
                }
            };
            fadeOutTimer.Start();
        }

        private void OnHideTimerTick(object sender, EventArgs e)
        {
            _hideTimer.Stop();
            AnimateOut();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var pen = new Pen(Color.FromArgb(100, 100, 100), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _hideTimer?.Stop();
                _hideTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}