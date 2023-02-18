using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;

namespace Osu_clone
{
    public partial class Form1 : Form
    {
        public Bitmap HandlerTexture = Resource1.Handler,
                      TargetTexture = Resource1.Target;
        private Point _targetPosition = new Point(400, 300);
        private Point _direction = Point.Empty;
        private int _score = 0;
        private int _scoreLimit = 400;
        private int _timer = 5;
        private int _difficult = 35;

        private void SetDefault()
        {
            _targetPosition = new Point(400, 300);
            _direction = Point.Empty;
            _score = 0;
            _scoreLimit = 400;
            _timer = 40;
            _difficult = 35;
            scoreLabel.Text = "score: " + 0;
            timerLabel.Text = "time: " + _timer;
        }

        public Form1()
        {
            InitializeComponent();

            Size = new Size(800, 600);

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);

            UpdateStyles();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Refresh();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Random rand = new Random();
            timer2.Interval = rand.Next(500, 1001);
            _direction.X = rand.Next(-1, 2);
            _direction.Y = rand.Next(-1, 2);
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (_timer > 0 && _score < _scoreLimit)
            {
                _timer -= 1;
                timerLabel.Text = "time: " + _timer.ToString();
            }
            else
            {
                EndGame();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int objectSize = 100;
            int halfObjectSize = objectSize / 2;

            var localPosition = this.PointToClient(Cursor.Position);

            _targetPosition.X += _direction.X * 7;
            _targetPosition.Y += _direction.Y * 7;

            if (_targetPosition.X < halfObjectSize || _targetPosition.X > this.Width - halfObjectSize)
            {
                RandomTargetPosition();
                _direction.X *= -1;
            }

            if (_targetPosition.Y < halfObjectSize || _targetPosition.Y > this.Height - halfObjectSize)
            {
                RandomTargetPosition();
                _direction.Y *= -1;
            }

            Point between = new Point(localPosition.X - _targetPosition.X, localPosition.Y - _targetPosition.Y);
            float distance = (float)Math.Sqrt((between.X * between.X) + (between.Y * between.Y));

            if (distance < _difficult)
            {
                AddScore(1);
            }

            var handlerRect = new Rectangle(localPosition.X - halfObjectSize, localPosition.Y - halfObjectSize, +
                objectSize, objectSize);
            var targetRect = new Rectangle(_targetPosition.X - halfObjectSize, _targetPosition.Y - halfObjectSize, +
                objectSize, objectSize);

            g.DrawImage(HandlerTexture, handlerRect);
            g.DrawImage(TargetTexture, targetRect);
        }

        private void RandomTargetPosition()
        {
            Random rand = new Random();

            _targetPosition.X = rand.Next(this.Width / 3, this.Width / 3 * 2);
            _targetPosition.Y = rand.Next(this.Height / 3, this.Height / 3 * 2);
        }

        private void AddScore(int score)
        {
            if (_timer > 0 && _score < _scoreLimit)
            {
                _score += score;
                scoreLabel.Text = "score: " + _score.ToString();
            }
        }

        private void retryLabel_MouseClick(object sender, MouseEventArgs e)
        {
            retryLabel.Visible = false;
            endLabel.Visible = false;
            listView1.Visible = false;
            SetDefault();
            timer3.Enabled = true;
        }

        private void EndGame()
        {
            timer3.Enabled = false;

            if (_timer >= 0 && _score >= _scoreLimit)
                endLabel.Text = "You win!";
            else
                endLabel.Text = "You lose(";

            listView1.Visible = true;
            endLabel.Visible = true;
            retryLabel.Visible = true;
        }
    }
}
