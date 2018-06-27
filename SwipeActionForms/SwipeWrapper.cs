using System;

using Xamarin.Forms;

namespace SwipeActionForms
{
    public class SwipeWrapper : ContentView
    {
        private static Color _notActionnableColor = Color.FromHex("0x555");
        private readonly Frame _delete;
        private readonly Grid _innerGrid;

        public static readonly BindableProperty CenterViewProperty =
      BindableProperty.Create(
                nameof(CenterView),
                typeof(View),
                typeof(SwipeWrapper),
                propertyChanged: OnCenterViewPropertyChanged);

        public View CenterView
        {
            get => (View)GetValue(CenterViewProperty);
            set => SetValue(CenterViewProperty, value);
        }
        private static void OnCenterViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((SwipeWrapper)bindable).GenerateView();
        }

        public static readonly BindableProperty DeleteViewProperty =
            BindableProperty.Create(
                nameof(DeleteView),
                typeof(View),
                typeof(SwipeWrapper),
                propertyChanged: OnDeleteViewPropertyChanged);

        public View DeleteView
        {
            get => (View)GetValue(DeleteViewProperty);
            set => SetValue(DeleteViewProperty, value);
        }
        private static void OnDeleteViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((SwipeWrapper)bindable).GenerateView();
        }

        private void GenerateView()
        {
            if (CenterView == null || DeleteView == null)
            {
                return;
            }
            _innerGrid.Children.Clear();
            _innerGrid.Children.Add(_delete);
            _delete.Content = DeleteView;
            _innerGrid.Children.Add(CenterView);
    
        
        }

        public SwipeWrapper()
        {
            _delete = new Frame
            {
                BackgroundColor = Color.Red,
                BorderColor = Color.Red,
                HasShadow = false,
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                CornerRadius = 0,
                HorizontalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.End
                },
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Fill
                }
            };

            _innerGrid = new Grid
            {
                ColumnSpacing = 0,
                Padding = new Thickness(0),
                Margin = new Thickness(0)
            };

            Content = _innerGrid;

            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += this.OnPanUpdated;
            this.GestureRecognizers.Add(panGesture);

            LayoutChanged += OnLayoutChanged;
        }

        private void OnLayoutChanged(object sender, EventArgs e)
        {
            _delete.WidthRequest = Width;
            _delete.Margin = new Thickness(0, 0, -Width, 0);
        }

        bool _translatedLeft = false;
        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (Math.Abs(e.TotalX) < 0.002)
            {
                if (_translatedLeft)
                {
                    _innerGrid.TranslateTo(0, 0, length: 200, easing: Easing.CubicOut);
                    _translatedLeft = false;
                }
            }
            else if (e.TotalX < 0)
            {
                double totalX = Math.Max(e.TotalX, -Width / 2);
                _innerGrid.TranslationX = totalX;
                _delete.BorderColor = _delete.BackgroundColor = -1 * totalX > (Width / 3) ? Color.Red : Color.DarkGray;
                _translatedLeft = true;
            }
        }

        private void HandleTouch(float totalX)
        {

        }
    }
}

