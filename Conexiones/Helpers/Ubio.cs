using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones.Dto
{
    public class Ubio
    {
        public static byte[] gpImage = new byte[256 * 296];
        public static byte[] gpBin = new byte[256 * 296];
        public static byte[] gpFeature = new byte[256];
        public static byte[] gpFeatureA = new byte[256];//从文件取得的特征值
        public static byte[] gpFeatureB = new byte[256];//从文件取得的特征值
        public static byte[] gpFeatureBuf = new byte[512];//从文件取得的特征值
        public static byte[] gpFeatureLib1 = new byte[10000 * 256];//从文件取得的特征值
        public static byte[] gpFeatureLib2 = new byte[10000 * 256];//从文件取得的特征值

        [DllImport("AvzUbioSdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort AvzFindDevice(byte[] pDeviceName);

        [DllImport("AvzUbioSdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint AvzOpenDevice(short uDeviceID, uint hWnd);

        [DllImport("AvzUbioSdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void AvzCloseDevice(short uDeviceID);

        [DllImport("AvzUbioSdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void AvzGetImage(short uDeviceID, byte[] pImage, ref ushort uStatus);
        [DllImport("AvzUbioSdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void AvzGetImage(short uDeviceID, IntPtr pImage, ref ushort uStatus);

        [DllImport("AvzUbioSdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void AvzGetCard(short uDeviceID, ref uint lCardID);

        [DllImport("AvzUbioSdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AvzProcess(byte[] pimagein, byte[] feature, byte[] pimagebin, byte bthin, byte bdrawfea, ushort uRate);
        [DllImport("AvzUbioSdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AvzProcess(IntPtr ptr, byte[] feature, byte[] pimagebin, byte bthin, byte bdrawfea, ushort uRate);

        [DllImport("AvzUbioSdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort AvzSaveHueBMPFile([MarshalAs(UnmanagedType.LPStr)] string pName, byte[] praw);

        [DllImport("AvzUbioSdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort AvzSaveClrBMPFile([MarshalAs(UnmanagedType.LPStr)] string pName, byte[] praw);

        [DllImport("AvzUbioSdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort AvzShowImage(IntPtr pHND, byte[] praw, uint a, uint b, uint c, uint d, uint e, uint f, uint g, uint h);

        [DllImport("AvzUbioSdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort AvzPackFeature(byte[] pFeature1, byte[] pFeature2, byte[] pFeatureBuf);
        [DllImport("AvzUbioSdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort AvzUnpackFeature(byte[] pFeatureBuf, [Out] byte[] pFeature1, [Out] byte[] pFeature2);

        [DllImport("AvzUbioSdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AvzMatch(byte[] pFeature1, byte[] pFeature2, ushort level, ushort rotate);

        [DllImport("AvzUbioSdk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AvzMatchN(byte[] pFeature, byte[] gpFeatureLib, uint FingerNum, ushort level, ushort rotate);

    }
}
