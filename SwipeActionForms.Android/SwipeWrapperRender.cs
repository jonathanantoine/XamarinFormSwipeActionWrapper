using Android.Content;
using Android.Views;
using SwipeActionForms;
using Xamarin.Forms.Platform.Android;

namespace Podcasts.Droid.Renderers
{
    public class SwipeWrapperRenderer : ViewRenderer<SwipeWrapper, View>
    {
        public SwipeWrapperRenderer(Context context) : base(context)
        {
        }

        private bool _disallowed;
        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Move:
                case MotionEventActions.Down:
                    if (Element.IsSwipping && !_disallowed)
                    {
                        RequestDisallowInterceptTouchEvent(true);
                        _disallowed = true;
                        return true;
                    }
                    break;

                case MotionEventActions.Cancel:
                case MotionEventActions.Up:
                    RequestDisallowInterceptTouchEvent(false);
                    _disallowed = false;
                    break;
            }

            return base.OnTouchEvent(e);
        }
    }
}

