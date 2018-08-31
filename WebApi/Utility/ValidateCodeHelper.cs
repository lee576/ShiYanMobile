using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Text;

namespace WebApi.Utility
{
    #region 验证码生成类

    /// <summary>
    /// 验证码生成类
    /// </summary>
    public class ValidateCodeHelper
    {
        #region 定义和初始化配置字段

        //用户存取验证码字符串

        //生成的验证码字符串
        public char[] chars = null;

        /// <summary>
        /// 获取系统生成的随机验证码
        /// </summary>
        public String ValidationCode { get; set; } = String.Empty;

        /// <summary>
        /// 获取和设置验证码字符串的长度
        /// </summary>
        public Int32 ValidationCodeCount { get; set; } = 4;

        Graphics _dc = null;

        /// <summary>
        /// 验证码的宽度，默认为80
        /// </summary>
        public Int32 Width { get; set; } = 130;

        /// <summary>
        /// 验证码的高度，默认为40
        /// </summary>
        public Int32 Height { get; set; } = 40;

        /// <summary>
        /// 验证码字体的最小值，默认为15,建议不小于15像素
        /// </summary>
        public Int32 FontMinSize { get; set; } = 20;

        /// <summary>
        /// 验证码字体的最大值，默认为20
        /// </summary>
        public Int32 FontMaxSize { get; set; } = 25;

        /// <summary>
        /// 验证码字体的颜色，默认为系统自动生成字体颜色
        /// </summary>
        public Color[] FontColor { get; set; } = { };

        /// <summary>
        /// 验证码的背景色，默认为Color.FromArgb(243, 251, 254)
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.FromArgb(243, 255, 255);

        /// <summary>
        /// 贝塞尔曲线的条数,默认为3条
        /// </summary>
        public Int32 BezierCount { get; set; } = 3;

        /// <summary>
        /// 直线条数，默认为3条
        /// </summary>
        public Int32 LineCount { get; set; } = 3;

        Random random = new Random();

        /// <summary>
        /// 随机字符串列表，请使用英文状态下的逗号分隔。
        /// </summary>
        public String CharCollection { get; set; } = "2,3,4,5,6,7,8,9,a,s,d,f,g,h,z,c,v,b,n,m,k,q,w,e,r,t,y,u,p,A,S,D,F,G,H,Z,C,V,B,N,M,K,Q,W,E,R,T,Y,U,P";

        /// <summary>
        /// 验证码字符串个数，默认为4个字符
        /// </summary>
        public Int32 IntCount { get; set; } = 4;

        /// <summary>
        /// 是否添加噪点，默认添加，噪点颜色为系统随机生成。
        /// </summary>
        public Boolean IsPixel { get; set; } = true;

        /// <summary>
        /// 是否添加随机噪点字符串，默认添加
        /// </summary>
        public Boolean IsRandString { get; set; } = true;

        /// <summary>
        /// 随机背景字符串的个数
        /// </summary>
        public Int32 RandomStringCount { get; set; }

        /// <summary>
        /// 随机背景字符串的大小
        /// </summary>
        public Int32 RandomStringFontSize { get; set; } = 9;

        /// <summary>
        /// 是否对图片进行扭曲
        /// </summary>
        public Boolean IsTwist { get; set; }

        /// <summary>
        /// 边框样式
        /// </summary>
        public enum BorderStyle
        {
            /// <summary>
            /// 无边框
            /// </summary>
            None,

            /// <summary>
            /// 矩形边框
            /// </summary>
            Rectangle,

            /// <summary>
            /// 圆角边框
            /// </summary>
            RoundRectangle
        }

        /// <summary>
        /// 验证码字符串随机转动的角度的最大值
        /// </summary>
        public Int32 RotationAngle { get; set; } = 40;

        /// <summary>
        /// 设置或获取边框样式
        /// </summary>
        public BorderStyle Border { get; set; }

        private Point[] strPoint = null;


        /// <summary>
        /// 对验证码图片进行高斯模糊的阀值，如果设置为0，则不对图片进行高斯模糊，该设置可能会对图片处理的性能有较大影响
        /// </summary>
        public Double GaussianDeviation { get; set; } = 0;

        /// <summary>
        /// 对图片进行暗度和亮度的调整，如果该值为0，则不调整。该设置会对图片处理性能有较大影响
        /// </summary>
        public Int32 BrightnessValue { get; set; } = 0;

        #endregion

        /// <summary>
        /// 构造函数，用于初始化常用变量
        /// </summary>
        public void DrawValidationCode()
        {
            random = new Random(Guid.NewGuid().GetHashCode());
            strPoint = new Point[ValidationCodeCount + 1];
            if (GaussianDeviation < 0) GaussianDeviation = 0;
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="target">用于存储图片的一般字节序列</param>
        public string CreateImage(string code)
        {
            MemoryStream target = new MemoryStream();
            Bitmap bit = new Bitmap(Width + 1, Height + 1);
            //写字符串
            _dc = Graphics.FromImage(bit);
            _dc.SmoothingMode = SmoothingMode.HighQuality;
            _dc.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            ;
            _dc.InterpolationMode = InterpolationMode.HighQualityBilinear;
            _dc.CompositingQuality = CompositingQuality.HighQuality;

            try
            {
                _dc.Clear(Color.White);
                DrawValidationCode();
                _dc.DrawImageUnscaled(DrawBackground(), 0, 0);
                _dc.DrawImageUnscaled(DrawRandomString(code), 0, 0);

                //对图片进行高斯模糊
                if (GaussianDeviation > 0)
                {
                    Gaussian gau = new Gaussian();
                    bit = gau.FilterProcessImage(GaussianDeviation, bit);
                }

                //进行暗度和亮度处理
                if (BrightnessValue != 0)
                {
                    //对图片进行调暗处理
                    bit = AdjustBrightness(bit, BrightnessValue);
                }

                bit.Save(target, ImageFormat.Jpeg);
                //输出图片流
                return Convert.ToBase64String(target.ToArray());
            }
            finally
            {
                bit.Dispose();
                _dc.Dispose();
            }
        }

        #region 画验证码背景，例如，增加早点，添加曲线和直线等

        /// <summary>
        /// 画验证码背景，例如，增加早点，添加曲线和直线等
        /// </summary>
        /// <returns></returns>
        private Bitmap DrawBackground()
        {
            Bitmap bit = new Bitmap(Width + 1, Height + 1);
            Graphics g = Graphics.FromImage(bit);
            g.SmoothingMode = SmoothingMode.HighQuality;

            g.Clear(Color.White);
            Rectangle rectangle = new Rectangle(0, 0, Width, Height);
            Brush brush = new SolidBrush(BackgroundColor);
            g.FillRectangle(brush, rectangle);

            //画噪点
            if (IsPixel)
            {
                g.DrawImageUnscaled(DrawRandomPixel(30), 0, 0);
            }

            g.DrawImageUnscaled(DrawRandBgString(), 0, 0);


            //画曲线
            //g.DrawImageUnscaled(DrawRandomBezier(bezierCount), 0, 0);
            ////画直线
            //g.DrawImageUnscaled(DrawRandomLine(lineCount), 0, 0);

            //dc.DrawImageUnscaled(DrawStringline(), 0, 0);
            if (Border == BorderStyle.Rectangle)
            {
                //绘制边框
                g.DrawRectangle(new Pen(Color.FromArgb(90, 87, 46)), 0, 0, Width, Height);
            }
            else if (Border == BorderStyle.RoundRectangle)
            {
                //画圆角
                DrawRoundRectangle(g, rectangle, Color.FromArgb(90, 87, 46), 1, 3);
            }

            return bit;

        }

        #endregion

        #region 画正弦曲线

        private Bitmap DrawTwist(Bitmap bmp, Int32 tWidth, Int32 tHeight, float angle, Color color)
        {
            //为了方便查看效果，在这里我定义了一个常量。
            //它在定义数组的长度和for循环中都要用到。
            int size = Width;

            double[] x = new double[size];
            Bitmap b = new Bitmap(bmp.Width, bmp.Height);
            b.MakeTransparent();
            Graphics graphics = Graphics.FromImage(b);
            Pen pen = new Pen(color);

            //画正弦曲线的横轴间距参数。建议所用的值应该是 正数且是2的倍数。
            //在这里采用2。
            int val = 2;

            float temp = 0.0f;

            //把画布下移100。为什么要这样做，只要你把这一句给注释掉，运行一下代码，
            //你就会明白是为什么？
            graphics.TranslateTransform(0, 100);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            for (int i = 0; i < size; i++)
            {
                //改变tWidth，实现正弦曲线宽度的变化。
                //改tHeight，实现正弦曲线高度的变化。
                x[i] = Math.Sin(2 * Math.PI * i / tWidth) * tHeight;

                graphics.DrawLine(pen, i * val, temp, i * val + val / 2, (float)x[i]);
                temp = (float)x[i];
            }

            graphics.RotateTransform(60, MatrixOrder.Prepend);

            //旋转图片
            // b = KiRotate(b, angle, Color.Transparent);
            return b;
        }

        #endregion

        #region 正弦曲线Wave扭曲图片

        /// <summary>
        /// 正弦曲线Wave扭曲图片
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="dMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>
        /// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param>
        /// <returns></returns>
        public Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            System.Drawing.Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
            double PI2 = 6.283185307179586476925286766559;
            // 将位图背景填充为白色
            System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(System.Drawing.Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();

            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;

            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (PI2 * (double)j) / dBaseAxisLen : (PI2 * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);

                    // 取得当前点的颜色
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                    nOldY = bXDir ? j : j + (int)(dy * dMultValue);

                    System.Drawing.Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width
                                   && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }

            return destBmp;
        }

        #endregion

        #region 图片任意角度旋转

        /// <summary>
        /// 图片任意角度旋转
        /// </summary>
        /// <param name="bmp">原始图Bitmap</param>
        /// <param name="angle">旋转角度</param>
        /// <param name="bkColor">背景色</param>
        /// <returns>输出Bitmap</returns>
        public static Bitmap KiRotate(Bitmap bmp, float angle, Color bkColor)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            PixelFormat pf;

            if (bkColor == Color.Transparent)
            {
                pf = PixelFormat.Format32bppArgb;
            }
            else
            {
                pf = bmp.PixelFormat;
            }

            Bitmap tmp = new Bitmap(w, h, pf);
            Graphics g = Graphics.FromImage(tmp);
            g.Clear(bkColor);
            g.DrawImageUnscaled(bmp, 1, 1);
            g.Dispose();

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, w, h));
            Matrix mtrx = new Matrix();
            mtrx.Rotate(angle);
            RectangleF rct = path.GetBounds(mtrx);

            Bitmap dst = new Bitmap((int)rct.Width, (int)rct.Height, pf);
            g = Graphics.FromImage(dst);
            g.Clear(bkColor);
            g.TranslateTransform(-rct.X, -rct.Y);
            g.RotateTransform(angle);
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(tmp, 0, 0);
            g.Dispose();
            tmp.Dispose();

            return dst;
        }

        #endregion

        #region 随机生成贝塞尔曲线

        /// <summary>
        /// 随机生成贝塞尔曲线
        /// </summary>
        /// <param name="bmp">一个图片的实例</param>
        /// <param name="lineNum">线条数量</param>
        /// <returns></returns>
        public Bitmap DrawRandomBezier(Int32 lineNum)
        {
            Bitmap b = new Bitmap(Width, Height);
            b.MakeTransparent();
            Graphics g = Graphics.FromImage(b);
            g.Clear(Color.Transparent);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            GraphicsPath gPath1 = new GraphicsPath();
            Int32 lineRandNum = random.Next(lineNum);

            for (int i = 0; i < (lineNum - lineRandNum); i++)
            {
                Pen p = new Pen(GetRandomDeepColor());
                Point[] point =
                {
                    new Point(random.Next(1, (b.Width / 10)), random.Next(1, (b.Height))),
                    new Point(random.Next((b.Width / 10) * 2, (b.Width / 10) * 4), random.Next(1, (b.Height))),
                    new Point(random.Next((b.Width / 10) * 4, (b.Width / 10) * 6), random.Next(1, (b.Height))),
                    new Point(random.Next((b.Width / 10) * 8, b.Width), random.Next(1, (b.Height)))
                };

                gPath1.AddBeziers(point);
                g.DrawPath(p, gPath1);
                p.Dispose();
            }

            for (int i = 0; i < lineRandNum; i++)
            {
                Pen p = new Pen(GetRandomDeepColor());
                Point[] point =
                {
                    new Point(random.Next(1, b.Width), random.Next(1, b.Height)),
                    new Point(random.Next((b.Width / 10) * 2, b.Width), random.Next(1, b.Height)),
                    new Point(random.Next((b.Width / 10) * 4, b.Width), random.Next(1, b.Height)),
                    new Point(random.Next(1, b.Width), random.Next(1, b.Height))
                };
                gPath1.AddBeziers(point);
                g.DrawPath(p, gPath1);
                p.Dispose();
            }

            return b;
        }

        #endregion

        #region 画直线

        /// <summary>
        /// 画直线
        /// </summary>
        /// <param name="bmp">一个bmp实例</param>
        /// <param name="lineNum">线条个数</param>
        /// <returns></returns>
        public Bitmap DrawRandomLine(Int32 lineNum)
        {
            if (lineNum < 0) throw new ArgumentNullException("参数bmp为空！");
            Bitmap b = new Bitmap(Width, Height);
            b.MakeTransparent();
            Graphics g = Graphics.FromImage(b);
            g.Clear(Color.Transparent);
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            for (int i = 0; i < lineNum; i++)
            {
                Pen p = new Pen(GetRandomDeepColor());
                Point pt1 = new Point(random.Next(1, (b.Width / 5) * 2), random.Next(b.Height));
                Point pt2 = new Point(random.Next((b.Width / 5) * 3, b.Width), random.Next(b.Height));
                g.DrawLine(p, pt1, pt2);
                p.Dispose();
            }

            return b;
        }

        #endregion

        #region 画随机噪点

        /// <summary>
        /// 画随机噪点
        /// </summary>
        /// <param name="pixNum">噪点的百分比</param>
        /// <returns></returns>
        public Bitmap DrawRandomPixel(Int32 pixNum)
        {
            Bitmap b = new Bitmap(Width, Height);
            b.MakeTransparent();
            Graphics graph = Graphics.FromImage(b);
            graph.SmoothingMode = SmoothingMode.HighQuality;
            graph.InterpolationMode = InterpolationMode.HighQualityBilinear;

            //画噪点 
            for (int i = 0; i < (Height * Width) / pixNum; i++)
            {
                int x = random.Next(b.Width);
                int y = random.Next(b.Height);
                b.SetPixel(x, y, GetRandomDeepColor());
                //下移坐标重新画点
                if ((x + 1) < b.Width && (y + 1) < b.Height)
                {
                    //画图片的前景噪音点
                    graph.DrawRectangle(new Pen(Color.Silver), random.Next(b.Width), random.Next(b.Height), 1, 1);
                }

            }

            return b;
        }

        #endregion

        #region 画随机字符串中间连线

        /// <summary>
        /// 画随机字符串中间连线
        /// </summary>
        /// <returns></returns>
        private Bitmap DrawStringline()
        {
            Bitmap b = new Bitmap(Width, Height);
            b.MakeTransparent();
            Graphics g = Graphics.FromImage(b);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Point[] p = new Point[ValidationCodeCount];
            for (int i = 0; i < ValidationCodeCount; i++)
            {
                p[i] = strPoint[i];
                //throw new Exception(strPoint.Length.ToString());
            }

            // g.DrawBezier(new Pen(GetRandomDeepColor()), strPoint);
            //g.DrawClosedCurve(new Pen(GetRandomDeepColor()), strPoint);
            g.DrawCurve(new Pen(GetRandomDeepColor(), 1), strPoint);

            return b;
        }

        #endregion

        #region 写入验证码的字符串

        /// <summary>
        /// 写入验证码的字符串
        /// </summary>
        private Bitmap DrawRandomString(string Code)
        {
            if (FontMaxSize >= (Height / 5) * 4)
                throw new ArgumentException("字体最大值参数FontMaxSize与验证码高度相近，这会导致描绘验证码字符串时出错，请重新设置参数！");
            Bitmap b = new Bitmap(Width, Height);
            b.MakeTransparent();
            Graphics g = Graphics.FromImage(b);

            g.Clear(Color.Transparent);
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;

            chars = Code.ToCharArray(); //拆散字符串成单字符数组
            ValidationCode = chars.ToString();

            //设置字体显示格式
            StringFormat format = new StringFormat(StringFormatFlags.NoClip)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            FontFamily f = new FontFamily(GenericFontFamilies.Monospace);


            Int32 charNum = chars.Length;

            Point sPoint = new Point();
            Int32 fontSize = 12;
            for (int i = 0; i < ValidationCodeCount; i++)
            {
                int findex = random.Next(5);
                //定义字体
                Font textFont = new Font(f, random.Next(FontMinSize, FontMaxSize), FontStyle.Bold);
                //定义画刷，用于写字符串
                //Brush brush = new SolidBrush(GetRandomDeepColor());
                Int32 textFontSize = Convert.ToInt32(textFont.Size);
                fontSize = textFontSize;
                Point point = new Point(random.Next((Width / charNum) * i + 5, (Width / charNum) * (i + 1)),
                    random.Next(Height / 5 + textFontSize / 2, Height - textFontSize / 2));



                //如果当前字符X坐标小于字体的二分之一大小
                if (point.X < textFontSize / 2)
                {
                    point.X = point.X + textFontSize / 2;
                }

                //防止文字叠加
                if (i > 0 && (point.X - sPoint.X < (textFontSize / 2 + textFontSize / 2)))
                {
                    point.X = point.X + textFontSize;
                }

                //如果当前字符X坐标大于图片宽度，就减去字体的宽度
                if (point.X > (Width - textFontSize / 2))
                {
                    point.X = Width - textFontSize / 2;
                }

                sPoint = point;

                float angle = random.Next(-RotationAngle, RotationAngle); //转动的度数
                g.TranslateTransform(point.X, point.Y); //移动光标到指定位置
                g.RotateTransform(angle);

                //设置渐变画刷  
                Rectangle myretang =
                    new Rectangle(0, 1, Convert.ToInt32(textFont.Size), Convert.ToInt32(textFont.Size));
                Color c = GetRandomDeepColor();
                LinearGradientBrush mybrush2 =
                    new LinearGradientBrush(myretang, c, GetLightColor(c, 120), random.Next(180));

                g.DrawString(chars[i].ToString(), textFont, mybrush2, 1, 1, format);

                g.RotateTransform(-angle); //转回去
                g.TranslateTransform(-point.X, -point.Y); //移动光标到指定位置，每个字符紧凑显示，避免被软件识别

                strPoint[i] = point;

                textFont.Dispose();
                mybrush2.Dispose();
            }

            return b;
        }

        #endregion

        #region 画干扰背景文字

        /// <summary>
        /// 画背景干扰文字
        /// </summary>
        /// <returns></returns>
        private Bitmap DrawRandBgString()
        {
            Bitmap b = new Bitmap(Width, Height);
            String[] randStr =
            {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u",
                "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P",
                "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
            };
            b.MakeTransparent();
            Graphics g = Graphics.FromImage(b);

            g.Clear(Color.Transparent);
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;

            //设置字体显示格式
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            FontFamily f = new FontFamily(GenericFontFamilies.Serif);
            Font textFont = new Font(f, RandomStringFontSize, FontStyle.Underline);

            int randAngle = 60; //随机转动角度

            for (int i = 0; i < RandomStringCount; i++)
            {

                Brush brush = new System.Drawing.SolidBrush(GetRandomLightColor());
                Point pot = new Point(random.Next(5, Width - 5), random.Next(5, Height - 5));
                //随机转动的度数
                float angle = random.Next(-randAngle, randAngle);

                //转动画布
                g.RotateTransform(angle);
                g.DrawString(randStr[random.Next(randStr.Length)], textFont, brush, pot, format);
                //转回去，为下一个字符做准备
                g.RotateTransform(-angle);
                //释放资源
                brush.Dispose();
            }

            textFont.Dispose();
            format.Dispose();
            f.Dispose();

            return b;
        }

        #endregion

        #region 生成随机字符串

        /// <summary>
        /// 生成随机字符串    
        /// </summary>
        /// <returns></returns>
        public string GetRandomString(Int32 textLength)
        {
            string[] randomArray = CharCollection.Split(','); //将字符串生成数组     
            int arrayLength = randomArray.Length;
            string randomString = "";
            for (int i = 0; i < textLength; i++)
            {
                randomString += randomArray[random.Next(0, arrayLength)];
            }

            return randomString; //长度是textLength +1
        }

        #endregion

        #region 内部方法：绘制验证码背景

        private void DrawBackground(HatchStyle hatchStyle)
        {
            //设置填充背景时用的笔刷
            HatchBrush hBrush = new HatchBrush(hatchStyle, BackgroundColor);

            //填充背景图片
            _dc.FillRectangle(hBrush, 0, 0, this.Width, this.Height);
        }

        #endregion

        #region 根据指定长度，返回随机验证码

        /// <summary>
        /// 根据指定长度，返回随机验证码
        /// </summary>
        /// <param >制定长度</param>
        /// <returns>随即验证码</returns>
        public string Next(int length)
        {
            this.ValidationCode = GetRandomCode(length);
            return this.ValidationCode;
        }

        #endregion

        #region 内部方法：返回指定长度的随机验证码字符串

        /// <summary>
        /// 根据指定大小返回随机验证码
        /// </summary>
        /// <param >字符串长度</param>
        /// <returns>随机字符串</returns>
        private string GetRandomCode(int length)
        {
            StringBuilder sb = new StringBuilder(6);

            for (int i = 0; i < length; i++)
            {
                sb.Append(Char.ConvertFromUtf32(RandomAZ09()));
            }

            return sb.ToString();
        }

        #endregion

        #region 内部方法：产生随机数和随机点

        /// <summary>
        /// 产生0-9A-Z的随机字符代码
        /// </summary>
        /// <returns>字符代码</returns>
        private int RandomAZ09()
        {
            int result = 48;
            Random ram = new Random();
            int i = ram.Next(2);

            switch (i)
            {
                case 0:
                    result = ram.Next(48, 58);
                    break;
                case 1:
                    result = ram.Next(65, 91);
                    break;
            }

            return result;
        }

        /// <summary>
        /// 返回一个随机点，该随机点范围在验证码背景大小范围内
        /// </summary>
        /// <returns>Point对象</returns>
        private Point RandomPoint()
        {
            Random ram = new Random();
            Point point = new Point(ram.Next(this.Width), ram.Next(this.Height));
            return point;
        }

        #endregion

        #region 随机生成颜色值

        /// <summary>
        /// 生成随机深颜色
        /// </summary>
        /// <returns></returns>
        public Color GetRandomDeepColor()
        {
            int nRed, nGreen, nBlue; // nBlue,nRed  nGreen 相差大一点 nGreen 小一些
            //int high = 255;       
            int redLow = 160;
            int greenLow = 100;
            int blueLow = 160;
            nRed = random.Next(redLow);
            nGreen = random.Next(greenLow);
            nBlue = random.Next(blueLow);
            Color color = Color.FromArgb(nRed, nGreen, nBlue);
            return color;
        }

        /// <summary>
        /// 生成随机浅颜色
        /// </summary>
        /// <returns>randomColor</returns>
        public Color GetRandomLightColor()
        {
            int nRed, nGreen, nBlue; //越大颜色越浅
            int low = 180; //色彩的下限
            int high = 255; //色彩的上限      
            nRed = random.Next(high) % (high - low) + low;
            nGreen = random.Next(high) % (high - low) + low;
            nBlue = random.Next(high) % (high - low) + low;
            Color color = Color.FromArgb(nRed, nGreen, nBlue);
            return color;
        }

        /// <summary>
        /// 生成随机颜色值
        /// </summary>
        /// <returns></returns>
        public Color GetRandomColor()
        {
            int nRed, nGreen, nBlue; //越大颜色越浅
            int low = 10; //色彩的下限
            int high = 255; //色彩的上限    
            nRed = random.Next(high) % (high - low) + low;
            nGreen = random.Next(high) % (high - low) + low;
            nBlue = random.Next(high) % (high - low) + low;
            Color color = Color.FromArgb(nRed, nGreen, nBlue);
            return color;
        }

        /// <summary>
        /// 获取与当前颜色值相加后的颜色
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public Color GetLightColor(Color c, Int32 value)
        {
            int nRed = c.R, nGreen = c.G, nBlue = c.B; //越大颜色越浅
            if (nRed + value < 255 && nRed + value > 0)
            {
                nRed = c.R + 40;
            }

            if (nGreen + value < 255 && nGreen + value > 0)
            {
                nGreen = c.G + 40;
            }

            if (nBlue + value < 255 && nBlue + value > 0)
            {
                nBlue = c.B + 40;
            }

            Color color = Color.FromArgb(nRed, nGreen, nBlue);
            return color;
        }

        #endregion

        #region 合并图片

        /// <summary>       
        /// 合并图片        
        /// </summary>        
        /// <param name="maps"></param>        
        /// <returns></returns>        
        private Bitmap MergerImg(params Bitmap[] maps)
        {
            int i = maps.Length;
            if (i == 0)
                throw new Exception("图片数不能够为0");
            //创建要显示的图片对象,根据参数的个数设置宽度            
            Bitmap backgroudImg = new Bitmap(i * 12, 16);
            Graphics g = Graphics.FromImage(backgroudImg);
            //清除画布,背景设置为白色            
            g.Clear(System.Drawing.Color.White);
            for (int j = 0; j < i; j++)
            {
                //g.DrawImage(maps[j], j * 11, 0, maps[j].Width, maps[j].Height);
                g.DrawImageUnscaled(maps[j], 0, 0);
            }

            g.Dispose();
            return backgroudImg;
        }

        #endregion

        #region 生成不重复的随机数，该函数会消耗大量系统资源

        /// <summary>
        /// 生成不重复的随机数，该函数会消耗大量系统资源
        /// </summary>
        /// <returns></returns>
        private static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng =
                new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        #endregion

        #region 缩放图片

        /// <summary>
        /// 缩放图片
        /// </summary>
        /// <param name="bmp">原始Bitmap</param>
        /// <param name="newW">新的宽度</param>
        /// <param name="newH">新的高度</param>
        /// <param name="Mode">缩放质量</param>
        /// <returns>处理以后的图片</returns>
        public static Bitmap KiResizeImage(Bitmap bmp, int newW, int newH, InterpolationMode Mode)
        {
            try
            {
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量
                g.InterpolationMode = Mode;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height),
                    GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region 绘制圆角矩形

        /// <summary>
        /// C# GDI+ 绘制圆角矩形
        /// </summary>
        /// <param name="g">Graphics 对象</param>
        /// <param name="rectangle">Rectangle 对象，圆角矩形区域</param>
        /// <param name="borderColor">边框颜色</param>
        /// <param name="borderWidth">边框宽度</param>
        /// <param name="r">圆角半径</param>
        private static void DrawRoundRectangle(Graphics g, Rectangle rectangle, Color borderColor, float borderWidth,
            int r)
        {
            // 如要使边缘平滑，请取消下行的注释
            g.SmoothingMode = SmoothingMode.HighQuality;

            // 由于边框也需要一定宽度，需要对矩形进行修正
            //rectangle = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            Pen p = new Pen(borderColor, borderWidth);
            // 调用 getRoundRectangle 得到圆角矩形的路径，然后再进行绘制
            g.DrawPath(p, getRoundRectangle(rectangle, r));
        }

        #endregion

        #region 根据普通矩形得到圆角矩形的路径

        /// <summary>
        /// 根据普通矩形得到圆角矩形的路径
        /// </summary>
        /// <param name="rectangle">原始矩形</param>
        /// <param name="r">半径</param>
        /// <returns>图形路径</returns>
        private static GraphicsPath getRoundRectangle(Rectangle rectangle, int r)
        {
            int l = 2 * r;
            // 把圆角矩形分成八段直线、弧的组合，依次加到路径中
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(new Point(rectangle.X + r, rectangle.Y), new Point(rectangle.Right - r, rectangle.Y));
            gp.AddArc(new Rectangle(rectangle.Right - l, rectangle.Y, l, l), 270F, 90F);

            gp.AddLine(new Point(rectangle.Right, rectangle.Y + r), new Point(rectangle.Right, rectangle.Bottom - r));
            gp.AddArc(new Rectangle(rectangle.Right - l, rectangle.Bottom - l, l, l), 0F, 90F);

            gp.AddLine(new Point(rectangle.Right - r, rectangle.Bottom), new Point(rectangle.X + r, rectangle.Bottom));
            gp.AddArc(new Rectangle(rectangle.X, rectangle.Bottom - l, l, l), 90F, 90F);

            gp.AddLine(new Point(rectangle.X, rectangle.Bottom - r), new Point(rectangle.X, rectangle.Y + r));
            gp.AddArc(new Rectangle(rectangle.X, rectangle.Y, l, l), 180F, 90F);
            return gp;
        }

        #endregion

        /// <summary>
        /// 增加或減少亮度
        /// </summary>
        /// <param name="img">System.Drawing.Image Source </param>
        /// <param name="valBrightness">0~255</param>
        /// <returns></returns>
        public Bitmap AdjustBrightness(Image img, int valBrightness)
        {
            // 讀入欲轉換的圖片並轉成為 Bitmap
            System.Drawing.Bitmap bitmap = new Bitmap(img);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    // 取得每一個 pixel
                    var pixel = bitmap.GetPixel(x, y);

                    // 判斷 如果處理過後 255 就設定為 255 如果小於則設定為 0
                    var pR = ((pixel.R + valBrightness > 255) ? 255 : pixel.R + valBrightness) < 0
                        ? 0
                        : ((pixel.R + valBrightness > 255) ? 255 : pixel.R + valBrightness);
                    var pG = ((pixel.G + valBrightness > 255) ? 255 : pixel.G + valBrightness) < 0
                        ? 0
                        : ((pixel.G + valBrightness > 255) ? 255 : pixel.G + valBrightness);
                    var pB = ((pixel.B + valBrightness > 255) ? 255 : pixel.B + valBrightness) < 0
                        ? 0
                        : ((pixel.B + valBrightness > 255) ? 255 : pixel.B + valBrightness);

                    // 將改過的 RGB 寫回
                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(pixel.A, pR, pG, pB);

                    bitmap.SetPixel(x, y, newColor);

                }
            }

            // 回傳結果
            return bitmap;
        }

        #endregion


        #region 水波纹效果



    } //END Class DrawValidationCode

    #endregion

    #region 高斯模糊算法

    /// <summary>
    /// 高斯模糊算法
    /// </summary>
    public class Gaussian
    {
        public static double[,] Calculate1DSampleKernel(double deviation, int size)
        {
            double[,] ret = new double[size, 1];
            double sum = 0;
            int half = size / 2;
            for (int i = 0; i < size; i++)
            {
                ret[i, 0] = 1 / (Math.Sqrt(2 * Math.PI) * deviation) *
                            Math.Exp(-(i - half) * (i - half) / (2 * deviation * deviation));
                sum += ret[i, 0];
            }

            return ret;
        }

        public static double[,] Calculate1DSampleKernel(double deviation)
        {
            int size = (int)Math.Ceiling(deviation * 3) * 2 + 1;
            return Calculate1DSampleKernel(deviation, size);
        }

        public static double[,] CalculateNormalized1DSampleKernel(double deviation)
        {
            return NormalizeMatrix(Calculate1DSampleKernel(deviation));
        }

        public static double[,] NormalizeMatrix(double[,] matrix)
        {
            double[,] ret = new double[matrix.GetLength(0), matrix.GetLength(1)];
            double sum = 0;
            for (int i = 0; i < ret.GetLength(0); i++)
            {
                for (int j = 0; j < ret.GetLength(1); j++)
                    sum += matrix[i, j];
            }

            if (sum != 0)
            {
                for (int i = 0; i < ret.GetLength(0); i++)
                {
                    for (int j = 0; j < ret.GetLength(1); j++)
                        ret[i, j] = matrix[i, j] / sum;
                }
            }

            return ret;
        }

        public static double[,] GaussianConvolution(double[,] matrix, double deviation)
        {
            double[,] kernel = CalculateNormalized1DSampleKernel(deviation);
            double[,] res1 = new double[matrix.GetLength(0), matrix.GetLength(1)];
            double[,] res2 = new double[matrix.GetLength(0), matrix.GetLength(1)];
            //x-direction
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                    res1[i, j] = processPoint(matrix, i, j, kernel, 0);
            }

            //y-direction
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                    res2[i, j] = processPoint(res1, i, j, kernel, 1);
            }

            return res2;
        }

        private static double processPoint(double[,] matrix, int x, int y, double[,] kernel, int direction)
        {
            double res = 0;
            int half = kernel.GetLength(0) / 2;
            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                int cox = direction == 0 ? x + i - half : x;
                int coy = direction == 1 ? y + i - half : y;
                if (cox >= 0 && cox < matrix.GetLength(0) && coy >= 0 && coy < matrix.GetLength(1))
                {
                    res += matrix[cox, coy] * kernel[i, 0];
                }
            }

            return res;
        }

        /// <summary>
        /// 对颜色值进行灰色处理
        /// </summary>
        /// <param name="cr"></param>
        /// <returns></returns>
        private Color grayscale(Color cr)
        {
            return Color.FromArgb(cr.A, (int)(cr.R * .3 + cr.G * .59 + cr.B * 0.11),
                (int)(cr.R * .3 + cr.G * .59 + cr.B * 0.11),
                (int)(cr.R * .3 + cr.G * .59 + cr.B * 0.11));
        }

        /// <summary>
        /// 对图片进行高斯模糊
        /// </summary>
        /// <param name="d">模糊数值，数值越大模糊越很</param>
        /// <param name="image">一个需要处理的图片</param>
        /// <returns></returns>
        public Bitmap FilterProcessImage(double d, Bitmap image)
        {
            Bitmap ret = new Bitmap(image.Width, image.Height);
            Double[,] matrixR = new Double[image.Width, image.Height];
            Double[,] matrixG = new Double[image.Width, image.Height];
            Double[,] matrixB = new Double[image.Width, image.Height];
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    //matrix[i, j] = grayscale(image.GetPixel(i, j)).R;
                    matrixR[i, j] = image.GetPixel(i, j).R;
                    matrixG[i, j] = image.GetPixel(i, j).G;
                    matrixB[i, j] = image.GetPixel(i, j).B;
                }
            }

            matrixR = Gaussian.GaussianConvolution(matrixR, d);
            matrixG = Gaussian.GaussianConvolution(matrixG, d);
            matrixB = Gaussian.GaussianConvolution(matrixB, d);
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Int32 R = (int)Math.Min(255, matrixR[i, j]);
                    Int32 G = (int)Math.Min(255, matrixG[i, j]);
                    Int32 B = (int)Math.Min(255, matrixB[i, j]);
                    ret.SetPixel(i, j, Color.FromArgb(R, G, B));
                }
            }

            return ret;
        }

    }
}
#endregion

