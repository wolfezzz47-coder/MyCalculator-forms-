using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleCalcWinForms
{
    public partial class Form1 : Form
    {
        private TextBox txtResult;
        private string currOperation = string.Empty;
        private string currValue = string.Empty;
        private bool isNewNumber = false;

        private readonly Color darkBtn = Color.FromArgb(45, 45, 45);
        private readonly Color numBtn = Color.FromArgb(59, 59, 59);
        private readonly Color bgColor = Color.FromArgb(32, 32, 32);

        public Form1()
        {
            Text = "Calculator";
            ClientSize = new Size(350, 520);
            BackColor = bgColor;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9);

            BuildUI();
        }

        // ---------- ПОБУДОВА ІНТЕРФЕЙСУ ----------
        private void BuildUI()
        {
            txtResult = new TextBox
            {
                Location = new Point(10, 15),
                Size = new Size(330, 75),
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Right,
                ReadOnly = true,
                Text = "0",
                BackColor = bgColor,
                ForeColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.None
            };
            Controls.Add(txtResult);

            // Рядок 0
            CreateButton("%", 0, 0, darkBtn, Button_Percent_Click);
            CreateButton("CE", 1, 0, darkBtn, Button_CE_Click);
            CreateButton("C", 2, 0, darkBtn, Button_Clear_Click);
            CreateButton("⌫", 3, 0, darkBtn, Button_Backspace_Click);

            // Рядок 1
            CreateButton("7", 0, 1, numBtn, Button_Digital_Click);
            CreateButton("8", 1, 1, numBtn, Button_Digital_Click);
            CreateButton("9", 2, 1, numBtn, Button_Digital_Click);
            CreateButton("÷", 3, 1, darkBtn, Button_Operator_Click);

            // Рядок 2
            CreateButton("4", 0, 2, numBtn, Button_Digital_Click);
            CreateButton("5", 1, 2, numBtn, Button_Digital_Click);
            CreateButton("6", 2, 2, numBtn, Button_Digital_Click);
            CreateButton("×", 3, 2, darkBtn, Button_Operator_Click);

            // Рядок 3
            CreateButton("1", 0, 3, numBtn, Button_Digital_Click);
            CreateButton("2", 1, 3, numBtn, Button_Digital_Click);
            CreateButton("3", 2, 3, numBtn, Button_Digital_Click);
            CreateButton("−", 3, 3, darkBtn, Button_Operator_Click);

            // Рядок 4
            CreateButton("0", 0, 4, numBtn, Button_Digital_Click);
            CreateButton(".", 1, 4, numBtn, Button_Dot_Click);
            CreateButton("=", 2, 4, numBtn, Button_Equal_Click);
            CreateButton("+", 3, 4, darkBtn, Button_Operator_Click);

            // Рядок 5
            CreateButton("¹/ₓ", 0, 5, darkBtn, Button_Reciprocal_Click);
            CreateButton("²√x", 1, 5, darkBtn, Button_Sqrt_Click);
            CreateButton("x²", 2, 5, darkBtn, Button_Square_Click);
            CreateButton("+/-", 3, 5, darkBtn, Button_Sign_Click);
        }

        private void CreateButton(string text, int col, int row,
                                  Color color, EventHandler handler)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(10 + col * 83, 105 + row * 66),
                Size = new Size(78, 60),
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Tag = text
            };
            btn.FlatAppearance.BorderSize = 0;
            if (handler != null) btn.Click += handler;
            Controls.Add(btn);
        }

        // ---------- ЦИФРИ ----------
        private void Button_Digital_Click(object sender, EventArgs e)
        {
            string value = ((Button)sender).Text;

            if (txtResult.Text == "0" || isNewNumber)
            {
                txtResult.Text = value;
                isNewNumber = false;
            }
            else if (txtResult.Text.Length < 12)
            {
                txtResult.Text += value;
            }
            else
            {
                MessageBox.Show("Ви досягли максимального розміру числа");
            }
        }

        // ---------- BACKSPACE ----------
        private void Button_Backspace_Click(object sender, EventArgs e)
        {
            if (isNewNumber) return;
            int len = txtResult.Text.Length;
            if (len != 0)
            {
                string str = txtResult.Text.Substring(0, len - 1);
                txtResult.Text = str.Length == 0 ? "0" : str;
            }
        }

        // ---------- C ----------
        private void Button_Clear_Click(object sender, EventArgs e)
        {
            txtResult.Text = "0";
            currOperation = string.Empty;
            currValue = string.Empty;
            isNewNumber = false;
        }

        // ---------- CE ----------
        private void Button_CE_Click(object sender, EventArgs e)
        {
            txtResult.Text = "0";
            isNewNumber = false;
        }

        // ---------- КРАПКА ----------
        private void Button_Dot_Click(object sender, EventArgs e)
        {
            if (isNewNumber)
            {
                txtResult.Text = "0";
                isNewNumber = false;
            }
            if (!txtResult.Text.Contains("."))
                txtResult.Text += ".";
        }

        // ---------- ОПЕРАТОРИ ----------
        private void Button_Operator_Click(object sender, EventArgs e)
        {
            string operation = ((Button)sender).Text;

            if (string.IsNullOrEmpty(currOperation))
            {
                currOperation = operation;
                currValue = txtResult.Text;
                isNewNumber = true;
            }
            else
            {
                if (isNewNumber)
                {
                    currOperation = operation;
                    return;
                }
                if (!Calculate(currOperation)) return;

                currOperation = operation;
                currValue = txtResult.Text;
                isNewNumber = true;
            }
        }

        // ---------- = ----------
        private void Button_Equal_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currOperation)) return;

            Calculate(currOperation);
            currOperation = string.Empty;
            currValue = string.Empty;
            isNewNumber = true;
        }

        // ---------- СПІЛЬНА ЛОГІКА ----------
        private bool Calculate(string op)
        {
            if (!double.TryParse(currValue, out double a)) return false;
            if (!double.TryParse(txtResult.Text, out double b)) return false;

            double result;
            switch (op)
            {
                case "+": result = a + b; break;
                case "−":
                case "-": result = a - b; break;
                case "×":
                case "*": result = a * b; break;
                case "÷":
                case "/":
                    if (b == 0)
                    {
                        MessageBox.Show("Ділення на нуль неможливе");
                        return false;
                    }
                    result = a / b;
                    break;
                default:
                    return false;
            }
            txtResult.Text = result.ToString();
            return true;
        }

        // ---------- % ----------
        private void Button_Percent_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtResult.Text, out double v))
            {
                txtResult.Text = (v / 100).ToString();
                isNewNumber = true;
            }
        }

        // ---------- ¹/ₓ ----------
        private void Button_Reciprocal_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtResult.Text, out double v) && v != 0)
            {
                txtResult.Text = (1 / v).ToString();
                isNewNumber = true;
            }
            else
            {
                MessageBox.Show("Ділення на нуль неможливе");
            }
        }

        // ---------- +/- ----------
        private void Button_Sign_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtResult.Text, out double v))
                txtResult.Text = (-v).ToString();
        }

        // ---------- ²√x ----------
        private void Button_Sqrt_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtResult.Text, out double v) && v >= 0)
            {
                txtResult.Text = Math.Sqrt(v).ToString();
                isNewNumber = true;
            }
            else
            {
                MessageBox.Show("Некоректне значення для кореня");
            }
        }

        // ---------- x² ----------
        private void Button_Square_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtResult.Text, out double v))
            {
                txtResult.Text = (v * v).ToString();
                isNewNumber = true;
            }
        }
    }
}