
This sample shows you how to add **swipe actions on any view on iOS and Android using Xamarin Forms**.

I needed to add this feature to one of my app [Podcasts Tracker](https://play.google.com/store/apps/details?id=com.jonathanantoine.Podcasts) and I didn't manage to find a quick and easy set-up so I coded it myself :)


![WORKING DEMO](https://media.giphy.com/media/1oCyjmVrqcvNgNaexK/giphy.gif)](https://www.youtube.com/watch?v=/xEOwv6UjiOI)

Better quality on Youtube : https://youtu.be/xEOwv6UjiOI



## Android configuration
You need to register a specific renderer on Android to let it works nicely on Android.
```csharp
[assembly: ExportRenderer(typeof(SwipeWrapper), typeof(SwipeWrapperRenderer))]
```

In the demo project, this is done in the *[AssemblyInfo.cs](https://github.com/jonathanantoine/XamarinFormSwipeActionWrapper/blob/bfdf1eb047f052d813bfc6cf31505ecc216a8f2c/SwipeActionForms.Android/Properties/AssemblyInfo.cs#L38)* file of the Android project.

## iOS configuration
Nothing special has to be done but  be aware that this specific ios configuration will be enabled :
`PanGestureRecognizerShouldRecognizeSimultaneously`.
