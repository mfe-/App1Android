﻿using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.IO;
using SixLabors.ImageSharp;

namespace App1Android
{
    [Activity(Label = "App1Android", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Getting an image stream from the resources and loading it with ImageSharp works
            //https://github.com/SixLabors/ImageSharp/issues/223
            Stream src = this.Resources.OpenRawResource(Resource.Drawable.tree);
            Image<Rgba32> image = Image.Load(src);

            ButtonOnClick();
        }
        public static readonly int PickImageId = 1000;
        private void ButtonOnClick()
        {
            Intent Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                Android.Net.Uri uri = data.Data;
                //get the selected file
                using (Android.Runtime.InputStreamInvoker stream = ContentResolver.OpenInputStream(uri) as Android.Runtime.InputStreamInvoker)
                {
                    //InputStreamInvoker.Position throws a System.NotSupportedException: Specified method is not supported.
                    //stream.Position = 0;
                    //therefore create a new memoryStream
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                        using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(memoryStream)) //causes the app to crash
                        {
                            var format = SixLabors.ImageSharp.Image.DetectFormat(stream);
                            //var output = new MemoryStream();
                            //image.Mutate(x => x.Resize(50, 50));
                            //image.Save(output, format);
                        }
                    }

                }
            }
        }
    }
}

