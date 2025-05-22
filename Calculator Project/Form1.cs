using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Define operations using enum for easy handling
        enum enOperation { enAdd = 1, enSub, enMult, enDiv};
        enOperation CurrentOperation;

        // Various  state variables to control calculator behavior

        bool isFirstOperation = false;
        bool ChangeDefaultText = false;
        bool isDecimalUsed = false;
        bool isEqualPressed = false;
        bool IsDivideByZero = false;
        bool CanRepeatEqual = false;

        // Variables holding numbers used in calculations

        double PrevNumber = 0;
        double CurrentNumber = 0;
        double TotalResult = 0;

        // Disable all buttons and show a message in case of an error
        void DisableAllButtonsAndShowMessageText(string Message)
        {
            lblResult.Text = Message;
            
            foreach (Control control in this.Controls)
            {
                if (control is Button btn)
                {
                    if (btn.Text != "C")
                    {
                        btn.Enabled = false;
                        btn.BackColor = Color.LightGray;
                        btn.ForeColor = Color.Gray;
                    }
                }
            }
        }

        // Handle division by zero case
        void HandleDivissionByZero(string Message)
        {
            DisableAllButtonsAndShowMessageText(Message);
        }

        // Reset the calculator and enable all buttons
        void ResetAllTheForm()
        {
            lblResult.Text = "0";

            PrevNumber = 0;
            CurrentNumber = 0;
            TotalResult = 0;

            isFirstOperation = false;
            ChangeDefaultText = false;
            isEqualPressed = false;
            IsDivideByZero = false;
            isDecimalUsed = false;
            CanRepeatEqual = false;

            foreach (Control control in this.Controls)
            {
                if (control is Button btn)
                {
                    btn.Enabled = true;
                    btn.ForeColor = Color.FromArgb(51, 62, 87);

                    if (btn.Tag == "N")
                    {
                        btn.BackColor = Color.FromArgb(163, 177, 198);
                    }
                    else
                    {
                        btn.BackColor = Color.FromArgb(123, 143, 161);
                    }
                }
            }
        }

        // Change the result label text when number buttons are clicked
        void ChangelblResultText(Button btn)
        {
            if (!ChangeDefaultText)
            {
                lblResult.Text = "";
                isDecimalUsed = false;
            }
            
            if (isDecimalUsed)
            {
                lblResult.Text += btn.Text;
                return;
            }

            lblResult.Text += btn.Text;
        }

        // Handle clicking operation buttons (+, -, *, /)
        void ClickButtons(Button btn)
        {
            isDecimalUsed = false;

            if (!ChangeDefaultText && !isEqualPressed)
            {
                DisableAllButtonsAndShowMessageText("Please Enter a Number After Operation");
                return;
            }

            if (isEqualPressed)
            {
                isEqualPressed = false;
                isFirstOperation = false;
            }

            if (isFirstOperation && !isEqualPressed)
            {
                CurrentNumber = Convert.ToDouble(lblResult.Text);

                TotalResult = Calculator(PrevNumber, CurrentNumber, CurrentOperation);

                PrevNumber = TotalResult;

                lblResult.Text = TotalResult.ToString();

                isFirstOperation = false;
            }
            else
            {
                PrevNumber = Convert.ToDouble(lblResult.Text);   
            }

            ChangeDefaultText = false;

            // Set the current operation based on the button clicked

            switch (btn.Text)
            {
                case "+":
                    CurrentOperation = enOperation.enAdd;
                    break;
                case "-":
                    CurrentOperation = enOperation.enSub;
                    break;
                case "x":
                    CurrentOperation = enOperation.enMult;
                    break;
                case "/":
                    CurrentOperation = enOperation.enDiv;
                    break;
            }

            isFirstOperation = true;
        }

        // Perform basic arithmetic operations
        double Calculator(double PrevNumber, double CurrentNumber, enOperation Op)
        {
            double Total = 0;

            switch (Op)
            {
                case enOperation.enAdd:
                    Total = PrevNumber + CurrentNumber;
                    break;
                case enOperation.enSub:
                    Total = PrevNumber - CurrentNumber;
                    break;
                case enOperation.enMult:
                        Total = PrevNumber * CurrentNumber;
                        break;
                case enOperation.enDiv:
                    {
                        if (CurrentNumber == 0)
                        {
                            IsDivideByZero = true;
                            return 0;
                        }

                        Total = PrevNumber / CurrentNumber;
                        break;
                    }  
            }

            return Total;
        }

        void RemoveLastDigit()
        {
            lblResult.Text = lblResult.Text.Substring(0, lblResult.Text.Length - 1);

            if (lblResult.Text.Length == 0)
            {
                lblResult.Text = "0";
                ChangeDefaultText = false;
            }
        }

        bool IsEqualPressedWithoutNumbers()
        {
            return ChangeDefaultText == false;
        }

        void ToggleSign()
        {
            double Number = Convert.ToDouble(lblResult.Text);

            if (Number == 0)
            {
                return;
            }

            Number *= -1;

            lblResult.Text = Number.ToString();
        }

        void ExecuteEqual()
        {
            isDecimalUsed = false;

            if (IsEqualPressedWithoutNumbers() && !CanRepeatEqual)
            {
                DisableAllButtonsAndShowMessageText("Error");
                return;
            }

            if (!isEqualPressed)
            {
                CurrentNumber = Convert.ToDouble(lblResult.Text);
                TotalResult = Calculator(PrevNumber, CurrentNumber, CurrentOperation);

                if (IsDivideByZero)
                {
                    HandleDivissionByZero("Cannot Divide By Zero");
                    return;
                }

                CanRepeatEqual = true;
            }
            else if (CanRepeatEqual)
            {
                TotalResult = Calculator(TotalResult, CurrentNumber, CurrentOperation);
            }
            else
            {
                TotalResult = Calculator(PrevNumber, CurrentNumber, CurrentOperation);
            }

            lblResult.Text = TotalResult.ToString();
            isEqualPressed = true;
            ChangeDefaultText = false;
        }

        private void Numbersbutton_Click(object sender, EventArgs e)
        {
            ChangelblResultText((Button)sender);
            ChangeDefaultText = true;
        }

        private void Opertaionsbutton_Click(object sender, EventArgs e)
        {
            ClickButtons((Button)sender);
        }

        private void btnResult_Click(object sender, EventArgs e)
        {
            ExecuteEqual();
        }

        private void btnRemoveNumber_Click(object sender, EventArgs e)
        {
            RemoveLastDigit();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetAllTheForm();
        }

        private void btnDecimal_Click(object sender, EventArgs e)
        {
            if (isDecimalUsed)
            {
                return;
            }

            ChangeDefaultText = true;
            isDecimalUsed = true;
            lblResult.Text += ".";
        }

        private void btnChangeSign_Click(object sender, EventArgs e)
        {
            ToggleSign();
        }
    }
}
