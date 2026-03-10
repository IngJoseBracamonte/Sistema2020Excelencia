using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OA99_PLUS_DEMO
{
    public class Win32
    {
        public static byte[] gpImage = new byte[256 * 296];
        public static byte[] gpBin = new byte[256 * 296];
        public static byte[] gpFeature = new byte[256];
        public static byte[] gpFeatureA = new byte[256];//从文件取得的特征值
        public static byte[] gpFeatureB = new byte[256];//从文件取得的特征值
        public static byte[] gpFeatureBuf = new byte[512];//从文件取得的特征值
        public static byte[] gpFeatureLib1 = new byte[10000*256];//从文件取得的特征值
        public static byte[] gpFeatureLib2 = new byte[10000*256];//从文件取得的特征值
        //const string rutadeInicio = @"C:\Users\J_Bra\source\repos\Sistema2020\Anviz\bin\Debug\AvzScanner.dll";
        const string rutadeInicio = @"C:\Sistema2020Vet\SistemaVet2020\AvzScanner.dll";


        [DllImport(rutadeInicio, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 AvzFindDevice(byte[] pDeviceName);

        [DllImport(rutadeInicio, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 AvzOpenDevice(Int16 uDeviceID, UInt32 hWnd);

        [DllImport(rutadeInicio, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AvzCloseDevice(Int16 uDeviceID);

        [DllImport(rutadeInicio, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AvzGetImage(Int16 uDeviceID, byte[] pImage, ref UInt16 uStatus);
        [DllImport(rutadeInicio, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AvzGetImage(Int16 uDeviceID, IntPtr pImage, ref UInt16 uStatus);

        [DllImport(rutadeInicio, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AvzGetCard(Int16 uDeviceID, ref UInt32 lCardID);

        [DllImport(rutadeInicio, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 AvzProcess(byte[] pimagein, byte[] feature, byte[] pimagebin, byte bthin, byte bdrawfea, UInt16 uRate);
        [DllImport(rutadeInicio , CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 AvzProcess(IntPtr ptr, byte[] feature, byte[] pimagebin, byte bthin, byte bdrawfea, UInt16 uRate);
        
        [DllImport(rutadeInicio, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 AvzSaveHueBMPFile([MarshalAs(UnmanagedType.LPStr)]String pName, byte[] praw);
        
        [DllImport(rutadeInicio, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 AvzSaveClrBMPFile([MarshalAs(UnmanagedType.LPStr)]String pName, byte[] praw);
        
        [DllImport(rutadeInicio, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 AvzShowImage(System.IntPtr pHND, byte[] praw, UInt32 a, UInt32 b, UInt32 c, UInt32 d, UInt32 e, UInt32 f, UInt32 g, UInt32 h);

        [DllImport(rutadeInicio, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 AvzPackFeature(byte[] pFeature1, byte[] pFeature2, byte[] pFeatureBuf);
        [DllImport(rutadeInicio, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 AvzUnpackFeature(byte[] pFeatureBuf, [Out] byte[] pFeature1, [Out] byte[] pFeature2);
        
        [DllImport(rutadeInicio, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 AvzMatch(byte[] pFeature1, byte[] pFeature2, UInt16 level, UInt16 rotate);

        [DllImport(rutadeInicio, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 AvzMatchN(byte[] pFeature, byte[] gpFeatureLib, UInt32 FingerNum, UInt16 level, UInt16 rotate);

    }
}