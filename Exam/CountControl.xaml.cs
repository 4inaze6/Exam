using System.Windows;
using System.Windows.Controls;

namespace Exam
{
    /// <summary>
    /// Логика взаимодействия для CountControl.xaml
    /// </summary>
    public partial class CountControl : UserControl
    {
        public int MaxValue { get; set; }

        public int Value { get; set; }

        public CountControl()
        {
            InitializeComponent();
            Value = 1;
            countTextBox.TextChanged += CountTextBox_TextChanged;
        }

        public event RoutedEventHandler ValueChanged;

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (Value < MaxValue)
            {
                Value++;
                if (Value == MaxValue)
                    plusButton.IsEnabled = false;
                minusButton.IsEnabled = true;
                countTextBox.Text = Value.ToString();
                ValueChanged?.Invoke(this, new RoutedEventArgs());
            }
        }

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (Value > 0)
            {
                Value--;
                if (Value == 0)
                    minusButton.IsEnabled = false;
                plusButton.IsEnabled = true;
                countTextBox.Text = Value.ToString();
                ValueChanged?.Invoke(this, new RoutedEventArgs());
            }
        }

        private void CountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (countTextBox.Text != "")
                Value = Convert.ToInt32(countTextBox.Text);
            if (Value != MaxValue)
                plusButton.IsEnabled = true;
        }
    }
}
