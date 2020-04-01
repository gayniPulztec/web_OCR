using System;
using System.Runtime.InteropServices;

namespace TessWrapper
{

    public static class TesseractEng
    {
        [DllImport(@"F:\pulz\submitted\web_ocr\web_ocr\DLLs\TessWrapper.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "?LoadSession@@YA_KXZ")]
        public static extern ulong LoadSession();

        [DllImport(@"F:\pulz\submitted\web_ocr\web_ocr\DLLs\TessWrapper.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "?getResult@@YAPEAD_K@Z")]
        public static extern IntPtr GetResultText(ulong address);

        [DllImport(@"F:\pulz\submitted\web_ocr\web_ocr\DLLs\TessWrapper.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "?getResultCode@@YAH_K@Z")]
        public static extern int GetResultCode(ulong address);

        [DllImport(@"F:\pulz\submitted\web_ocr\web_ocr\DLLs\TessWrapper.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "?readImage@@YAX_KPEAD1@Z")]
        public static extern void ReadImage(ulong address, char[] filePath, char[] language);

        [DllImport(@"F:\pulz\submitted\web_ocr\web_ocr\DLLs\TessWrapper.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "?dispose@@YAX_K@Z")]
        public static extern void Dispose(ulong address);


        private static readonly char[] ENGLISH_CODE = { 'e', 'n', 'g', LIMITING_CHARACTER };

        private const char LIMITING_CHARACTER = '\0';

        private const int READING_FAILED = (-1);
        private const int TESSARACT_LOADING_FAILED = (-2);
        private const int SUCCESS = (0);

        private static char[] GetCArrayFromString(string str)
        {
            int l_str = str.Length;
            char[] cstring = new char[l_str + 1];
            for (int i = 0; i < l_str; i++)
            {
                cstring[i] = str[i];
            }
            cstring[l_str] = LIMITING_CHARACTER;
            return cstring;
        }

        public static string ReadImage(string filePath, string languageCode = null)
        {
            char[] languageCode_cstring;
            if (languageCode == null)
            {
                languageCode_cstring = ENGLISH_CODE;
            }
            else
            {
                languageCode_cstring = GetCArrayFromString(languageCode);
            }

            char[] filePath_cstring = GetCArrayFromString(filePath);

            ulong address = LoadSession();

            ReadImage(address, filePath_cstring, languageCode_cstring);

            int resultCode = GetResultCode(address);
            if (resultCode != SUCCESS)
            {
                Dispose(address);
                return null;
            }
            else
            {
                IntPtr intPtr = GetResultText(address);
                string str = Marshal.PtrToStringAnsi(intPtr);
                Dispose(address);

                return str;
            }

        }

        static void Main(string[] args)
        {
           
            //string str = ReadImage(filePath);

            /*if (str != null)
            {
                Console.WriteLine(str);
            }
            else
            {
                Console.WriteLine("Could not read the file or could not load tesseract");
            }

            Console.ReadLine();*/
        }
    }
}
