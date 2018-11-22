using System;
using System.Runtime.InteropServices;

namespace SharpTox.Av
{
    public sealed class ToxAvVideoFrame
    {
        public byte[] Y { get; }

        public int YStride { get; }

        public byte[] U { get; }

        public int UStride { get; }

        public byte[] V { get; }

        public int VStride { get; }

        public ushort Width { get; }

        public ushort Height { get; }

        //this relies on the caller to call vpx_img_free (which is currently the case in toxav)
        internal ToxAvVideoFrame(ushort width, ushort height, IntPtr y, IntPtr u, IntPtr v, int yStride, int uStride, int vStride)
        {
            this.Width = width;
            this.Height = height;

            this.YStride = yStride;
            this.UStride = uStride;
            this.VStride = vStride;

            this.Y = new byte[Math.Max(width, Math.Abs(yStride)) * height];
            this.U = new byte[Math.Max(width / 2, Math.Abs(uStride)) * (height / 2)];
            this.V = new byte[Math.Max(width / 2, Math.Abs(vStride)) * (height / 2)];

            //TODO (?): use unsafe code to access the data directly instead of copying it over
            Marshal.Copy(y, Y, 0, Y.Length);
            Marshal.Copy(u, U, 0, U.Length);
            Marshal.Copy(v, V, 0, V.Length);
        }

        public ToxAvVideoFrame(ushort width, ushort height, byte[] y, byte[] u, byte[] v)
        {
            this.Width = width;
            this.Height = height;

            this.Y = y;
            this.U = u;
            this.V = v;

            this.YStride = width;
            this.UStride = width / 2;
            this.VStride = width / 2;
        }
    }
}
