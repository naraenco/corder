using System;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Text;

namespace agentcs
{
    //1. 프로젝트 내의 Resources폴더 (임의로 생성한 폴더)에 폰트파일(ttf) 복사
    //2. Resources.resx 에 복사한 ttf파일을 리소스 추가

    class FontManager
    {
        private static FontManager instance = new FontManager();
        public PrivateFontCollection privateFont = new PrivateFontCollection();
        public static FontFamily[] fontFamilys
        {
            get
            {
                return instance.privateFont.Families;
            }
        }

        public FontManager()
        {
            AddFontFromMemory();
        }
         
        private void AddFontFromMemory()
        {
            List<byte[]> fonts = new List<byte[]>();
            //fonts.Add(Properties.Resources.NanumSquareNeo_cBd);

            foreach (byte[] font in fonts)
            {
                IntPtr fontBuffer = Marshal.AllocCoTaskMem(font.Length);
                Marshal.Copy(font, 0, fontBuffer, font.Length);
                privateFont.AddMemoryFont(fontBuffer, font.Length);

                Marshal.FreeHGlobal(fontBuffer);//메모리 해제
            }
        }
    }
}
