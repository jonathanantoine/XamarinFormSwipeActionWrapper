using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SwipeActionForms
{
    public class SwipeWrapper : ContentView
    {
        private static bool _specificiOsConfigurationSet;

        private readonly Frame _rightContainer;
        private readonly Frame _leftContainer;
        private readonly Grid _innerGrid;
        bool _translatedLeftOrRight;
        private double _previousXTranslation;
        private bool _ParentListView_IsPullToRefreshEnabled;
        private bool _disablePan;

        public bool IsSwipping { get; private set; }

        #region NotActionnableColor property
        public static readonly BindableProperty NotActionnableColorProperty =
            BindableProperty.Create(
                nameof(NotActionnableColor),
                typeof(Color),
                typeof(SwipeWrapper),
                defaultValue: Color.DarkGray,
                propertyChanged: OnNotActionnableColorPropertyChanged);

        public Color NotActionnableColor
        {
            get => (Color)GetValue(NotActionnableColorProperty);
            set => SetValue(NotActionnableColorProperty, value);
        }

        private static void OnNotActionnableColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((SwipeWrapper)bindable).GenerateView();
        }
        #endregion

        #region LeftActionColor property
        public static readonly BindableProperty LeftActionColorProperty =
            BindableProperty.Create(
                nameof(LeftActionColor),
                typeof(Color),
                typeof(SwipeWrapper),
                defaultValue: Color.Green,
                propertyChanged: OnLeftActionColorPropertyChanged);

        public Color LeftActionColor
        {
            get => (Color)GetValue(LeftActionColorProperty);
            set => SetValue(LeftActionColorProperty, value);
        }

        private static void OnLeftActionColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((SwipeWrapper)bindable).GenerateView();
        }
        #endregion

        #region RightActionColor property
        public static readonly BindableProperty RightActionColorProperty =
            BindableProperty.Create(
                nameof(RightActionColor),
                typeof(Color),
                typeof(SwipeWrapper),
                defaultValue: Color.Red,
                propertyChanged: OnRightActionColorPropertyChanged);

        public Color RightActionColor
        {
            get => (Color)GetValue(RightActionColorProperty);
            set => SetValue(RightActionColorProperty, value);
        }

        private static void OnRightActionColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((SwipeWrapper)bindable).GenerateView();
        }
        #endregion

        #region RightActionTriggered event
        public event EventHandler<object> RightActionTriggered;
        protected virtual void RaiseRightActionTriggered(global::System.Object e)
        {
            RightActionTriggered?.Invoke(this, e);
        }
        #endregion

        #region LeftActionTriggered event
        public event EventHandler<object> LeftActionTriggered;
        protected virtual void RaiseLeftActionTriggered(global::System.Object e)
        {
            LeftActionTriggered?.Invoke(this, e);
        }
        #endregion

        #region ParentListView property
        public static readonly BindableProperty ParentListViewProperty =
            BindableProperty.Create(
                nameof(ParentListView),
                typeof(Xamarin.Forms.ListView),
                typeof(SwipeWrapper));

        public Xamarin.Forms.ListView ParentListView
        {
            get => (Xamarin.Forms.ListView)GetValue(ParentListViewProperty);
            set => SetValue(ParentListViewProperty, value);
        }
        #endregion

        #region CenterView property
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
        #endregion

        #region RightView property
        public static readonly BindableProperty RightViewProperty =
            BindableProperty.Create(
                nameof(RightView),
                typeof(View),
                typeof(SwipeWrapper),
                propertyChanged: OnRightViewPropertyChanged);

        public View RightView
        {
            get => (View)GetValue(RightViewProperty);
            set => SetValue(RightViewProperty, value);
        }

        private static void OnRightViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((SwipeWrapper)bindable).GenerateView();
        }
        #endregion

        #region LeftView property
        public static readonly BindableProperty LeftViewProperty =
            BindableProperty.Create(
                nameof(LeftView),
                typeof(View),
                typeof(SwipeWrapper),
                propertyChanged: OnLeftViewPropertyChanged);

        public View LeftView
        {
            get => (View)GetValue(LeftViewProperty);
            set => SetValue(LeftViewProperty, value);
        }

        private static void OnLeftViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((SwipeWrapper)bindable).GenerateView();
        }
        #endregion

        public SwipeWrapper()
        {
            _rightContainer = new Frame
            {
                BackgroundColor = RightActionColor,
                BorderColor = RightActionColor,
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

            _leftContainer = new Frame
            {
                BackgroundColor = LeftActionColor,
                BorderColor = LeftActionColor,
                HasShadow = false,
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                CornerRadius = 0,
                HorizontalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Start
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
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);

            LayoutChanged += OnLayoutChanged;

            // only set once. Set it here to let the control performs
            // the most of it's configuration itself
            if (!_specificiOsConfigurationSet)
            {
                _specificiOsConfigurationSet = true;
                Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPanGestureRecognizerShouldRecognizeSimultaneously(true);
            }

        }

        private void OnLayoutChanged(object sender, EventArgs e)
        {
            _rightContainer.WidthRequest = Width;
            _rightContainer.Margin = new Thickness(0, 0, -Width, 0);

            _leftContainer.WidthRequest = Width;
            _leftContainer.Margin = new Thickness(-Width, 0, 0, 0);
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (_disablePan)
            {
                return;
            }
            //https://forums.xamarin.com/discussion/102756/pangesture-on-item-in-listview-getting-canceled-but-only-on-android
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    if (ParentListView != null)
                    {
                        IsSwipping = true;
                        _ParentListView_IsPullToRefreshEnabled = ParentListView.IsPullToRefreshEnabled;
                        ParentListView.IsPullToRefreshEnabled = false;
                        ParentListView.Unfocus();
                    }
                    break;
                case GestureStatus.Running:
                    HandleTouch(e.TotalX);
                    break;
                case GestureStatus.Completed:
                    IsSwipping = false;

                    if (ParentListView != null)
                    {
                        ParentListView.IsPullToRefreshEnabled = _ParentListView_IsPullToRefreshEnabled;
                    }
                    if (RightView != null && CanTriggerRightAction(_previousXTranslation))
                    {
                        AnimRightActionAsync();
                    }
                    else if (LeftView != null && CanTriggerLeftAction(_previousXTranslation))
                    {
                        AnimLeftActionAsync();
                    }
                    else
                    {
                        HandleTouch(0, animated: false);
                    }

                    break;
                case GestureStatus.Canceled:
                    IsSwipping = false;

                    if (ParentListView != null)
                    {
                        ParentListView.IsPullToRefreshEnabled = _ParentListView_IsPullToRefreshEnabled;
                    }
                    HandleTouch(0, animated: false);
                    break;
            }
        }

        private async Task AnimRightActionAsync()
        {
            _disablePan = true;
            await _innerGrid.TranslateTo(-Width, 0, length: 200, easing: Easing.CubicOut);
            await Task.Delay(200);
            _disablePan = false;
            RaiseRightActionTriggered(BindingContext);
            HandleTouch(0, animated: false);
        }

        private async Task AnimLeftActionAsync()
        {
            _disablePan = true;
            await _innerGrid.TranslateTo(Width, 0, length: 200, easing: Easing.CubicOut);
            await Task.Delay(200);
            _disablePan = false;
            RaiseLeftActionTriggered(BindingContext);
            HandleTouch(0, animated: false);
        }

        private void HandleTouch(double xTranslation, bool animated = true)
        {
            _previousXTranslation = xTranslation;

            // scrolling back to "normal-centered" view
            if (Math.Abs(xTranslation) < 0.002)
            {
                if (_translatedLeftOrRight)
                {
                    if (animated)
                    {
                        _innerGrid.TranslateTo(0, 0, length: 200, easing: Easing.CubicOut);
                    }
                    else
                    {
                        _innerGrid.TranslationX = 0;
                    }
                    _translatedLeftOrRight = false;
                }
            }
            // scrolling to left if the RightView exists
            else if (xTranslation < 0 && RightView != null)
            {
                double totalX = Math.Max(xTranslation, -Width / 2);
                _innerGrid.TranslationX = totalX;
                _rightContainer.BorderColor = _rightContainer.BackgroundColor = CanTriggerRightAction(totalX) ? RightActionColor : NotActionnableColor;
                _translatedLeftOrRight = true;
            }
            // scrolling to right if the LeftView exists
            else if (xTranslation > 0 && LeftView != null)
            {
                double totalX = Math.Min(xTranslation, Width / 2);
                _innerGrid.TranslationX = totalX;
                _leftContainer.BorderColor = _leftContainer.BackgroundColor = CanTriggerLeftAction(totalX) ? LeftActionColor : NotActionnableColor;
                _translatedLeftOrRight = true;
            }
        }

        private bool CanTriggerRightAction(double totalX)
        {
            return -1 * totalX > (Width / 3);
        }

        private bool CanTriggerLeftAction(double totalX)
        {
            return totalX > (Width / 3);
        }

        private void GenerateView()
        {
            if (CenterView == null)
            {
                return;
            }

            _disablePan = false;
            _innerGrid.TranslationX = 0;
            _innerGrid.Children.Clear();

            if (RightView != null)
            {
                _innerGrid.Children.Add(_rightContainer);
                _rightContainer.Content = RightView;
            }

            if (LeftView != null)
            {
                _innerGrid.Children.Add(_leftContainer);
                _leftContainer.Content = LeftView;
            }

            _innerGrid.Children.Add(CenterView);
        }
    }
}

