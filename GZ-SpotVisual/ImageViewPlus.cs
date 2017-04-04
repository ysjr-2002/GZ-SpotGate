using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using static Android.Graphics.Shader;

namespace GZ_SpotVisual
{
    public class ImageViewPlus : ImageView
    {
        private Paint mPaintBitmap = new Paint(PaintFlags.AntiAlias);
        private Bitmap mRawBitmap;
        private BitmapShader mShader;
        private Matrix mMatrix = new Matrix();

        public ImageViewPlus(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected override void OnDraw(Canvas canvas)
        {
            Bitmap rawBitmap = getBitmap(this.Drawable);
            if (rawBitmap != null)
            {
                int viewWidth = Width;
                int viewHeight = Height;
                int viewMinSize = Math.Min(viewWidth, viewHeight);
                float dstWidth = viewMinSize;
                float dstHeight = viewMinSize;
                if (mShader == null || !rawBitmap.Equals(mRawBitmap))
                {
                    mRawBitmap = rawBitmap;
                    mShader = new BitmapShader(mRawBitmap, TileMode.Clamp, TileMode.Clamp);
                }
                if (mShader != null)
                {
                    mMatrix.SetScale(dstWidth / rawBitmap.Width, dstHeight / rawBitmap.Height);
                    mShader.SetLocalMatrix(mMatrix);
                }
                mPaintBitmap.SetShader(mShader);
                float radius = viewMinSize / 2.0f;
                canvas.DrawCircle(viewWidth/2, viewHeight/2, radius, mPaintBitmap);
            }
            else
            {
                base.OnDraw(canvas);
            }
        }

        private Bitmap getBitmap(Drawable drawable)
        {
            if (drawable is BitmapDrawable)
            {
                return ((BitmapDrawable)drawable).Bitmap;
            }
            else if (drawable is ColorDrawable)
            {
                Rect rect = ((ColorDrawable)drawable).Bounds;
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;
                var color = ((ColorDrawable)drawable).Color;
                Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas(bitmap);
                canvas.DrawARGB(color.A, color.R, color.G, color.B);
                return bitmap;
            }
            else
            {
                return null;
            }
        }
    }
}