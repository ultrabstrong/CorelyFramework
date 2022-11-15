using System.Windows;
using System.Windows.Controls;

namespace UsefulUtilities.UI
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        #region Constructor / Initialization

        /// <summary>
        /// Initialize numericupdown component
        /// </summary>
        public NumericUpDown()
        {
            Max = decimal.MaxValue;
            Min = decimal.MinValue;
            InitializeComponent();

        }

        /// <summary>
        /// Run value validation on control loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ConstrainValue();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Max dependency property
        /// </summary>
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register(nameof(Max), typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(default(decimal), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Maximum value allowed
        /// </summary>
        public decimal Max
        {
            get => (decimal)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        /// <summary>
        /// Min dependency property
        /// </summary>
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register(nameof(Min), typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(default(decimal), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Minimum value allowed
        /// </summary>
        public decimal Min
        {
            get => (decimal)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        /// <summary>
        /// Value dependency property
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(default(decimal), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Current value
        /// </summary>
        public decimal Value
        {
            get => (decimal)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// Is Integer dependency property
        /// </summary>
        public static readonly DependencyProperty IsIntegerProperty =
            DependencyProperty.Register(nameof(IsInteger), typeof(bool), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Is Integer
        /// </summary>
        public bool IsInteger
        {
            get => (bool)GetValue(IsIntegerProperty);
            set => SetValue(IsIntegerProperty, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Make sure entry is numeric only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
            {
                ConstrainValue();
            }
        }

        /// <summary>
        /// Increase the number in the numeric box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            decimal newval = Value + 1.0M;
            if (newval > Max)
            {
                Value = Max;
            }
            else
            {
                Value = newval;
            }
        }

        /// <summary>
        /// Decrease the number in the numeric box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            decimal newval = Value - 1.0M;
            if (newval < Min)
            {
                Value = Min;
            }
            else
            {
                Value = newval;
            }
        }

        /// <summary>
        /// Restrain value to min/max
        /// </summary>
        private void ConstrainValue()
        {
            if (Value > Max)
            {
                Value = Max;
            }
            if (Value < Min)
            {
                Value = Min;
            }
        }

        #endregion

    }
}
