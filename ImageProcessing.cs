﻿using System;
using System.Windows.Forms;
using CheckedException.Core;
using OpenCvSharp;
using OpenCvSharp.XFeatures2D;

namespace opencv
{
    public enum CopyTypes
    {
        ShallowCopy = 0,
        DeepCopy = 1
    }

    public static class ImageProcessing
    {
        private static readonly CascadeClassifier FaceClassifier =
            new CascadeClassifier(@"..\\..\\..\\Resources\\haarcascade_frontalface_alt2.xml");

        private static readonly CascadeClassifier EyeClassifier =
            new CascadeClassifier("..\\..\\..\\Resources\\haarcascade_eye_tree_eyeglasses.xml");

        private static readonly CascadeClassifier FistClassifier =
            new CascadeClassifier("..\\..\\..\\Resources\\fist.xml");

        private static readonly CascadeClassifier RightPalmClassifier =
            new CascadeClassifier("..\\..\\..\\Resources\\rpalm.xml");

        private static readonly CascadeClassifier PedestrianClassifier =
            new CascadeClassifier("..\\..\\..\\Resources\\pedestrian.xml");

        /// <summary>
        /// 符合窗口大小的Mat
        /// </summary>
        /// <param name="m"></param>
        /// <param name="pb"></param>
        /// <returns></returns>
        public static Mat GetBoxFittedMat(Mat m, PictureBox pb)
        {
            var (row, col) = ComputeSize(m.Rows, m.Cols, pb.Size.Height, pb.Size.Width);
            try
            {
                return m.Resize(new Size(col, row), 0, 0, InterpolationFlags.Cubic);
            }
            catch (OpenCVException)
            {
                MessageBox.Show(@"只能打开图片文件");
                return MainForm.NoneMat;
            }

            static Tuple<int, int> ComputeSize(int imgRow, int imgCol, int boxRow, int boxCol)
            {
                if (imgRow <= boxRow && imgCol <= boxCol)
                {
                    return new Tuple<int, int>(imgRow, imgCol);
                }
                else
                {
                    double ratio = ((double) imgRow) / imgCol;
                    //newRow/newCol = ratio
                    if (imgRow > imgCol)
                    {
                        return new Tuple<int, int>(boxRow, (int) (boxRow / ratio));
                    }
                    else
                    {
                        return new Tuple<int, int>((int) (ratio * boxCol), boxCol);
                    }
                }
            }
        }

        /// <summary>
        /// 人脸检测
        /// </summary>
        /// <param name="img"></param>
        /// <param name="copy"></param>
        /// <returns></returns>
        public static Mat FaceLocate(Mat img, CopyTypes copy = CopyTypes.DeepCopy)
        {
            Mat grayImg = NewGrayMat(img);
            var newImg = copy == CopyTypes.ShallowCopy ? img : img.Clone();
            Rect[] faces = FaceClassifier.DetectMultiScale(grayImg);
            foreach (var rect in faces)
            {
                Cv2.Rectangle(newImg, new Point(rect.X, rect.Y), new Point(rect.X + rect.Width,
                    rect.Y + rect.Height), new Scalar(255, 0, 0), 3);
            }

            return newImg;
        }

        /// <summary>
        /// 人眼检测
        /// </summary>
        /// <param name="img"></param>
        /// <param name="copy"></param>
        /// <returns></returns>
        public static Mat EyeLocate(Mat img, CopyTypes copy = CopyTypes.DeepCopy)
        {
            Mat grayImg = NewGrayMat(img);
            var newImg = copy == CopyTypes.ShallowCopy ? img : img.Clone();
            Rect[] faces = EyeClassifier.DetectMultiScale(grayImg, 1.1, 6);
            foreach (var rect in faces)
            {
                Cv2.Rectangle(newImg, new Point(rect.X, rect.Y), new Point(rect.X + rect.Width,
                    rect.Y + rect.Height), new Scalar(255, 0, 0), 3);
            }

            return newImg;
        }

        /// <summary>
        /// 拳头
        /// </summary>
        /// <param name="img"></param>
        /// <param name="copy"></param>
        /// <returns></returns>
        public static Mat FistLocate(Mat img, CopyTypes copy = CopyTypes.DeepCopy)
        {
            Mat grayImg = NewGrayMat(img);
            var newImg = copy == CopyTypes.ShallowCopy ? img : img.Clone();
            Rect[] faces = FistClassifier.DetectMultiScale(grayImg);
            foreach (var rect in faces)
            {
                Cv2.Rectangle(newImg, new Point(rect.X, rect.Y), new Point(rect.X + rect.Width,
                    rect.Y + rect.Height), new Scalar(255, 0, 0), 3);
            }

            return newImg;
        }

        /// <summary>
        /// 右掌
        /// </summary>
        /// <param name="img"></param>
        /// <param name="copy"></param>
        /// <returns></returns>
        public static Mat RightPalmLocate(Mat img, CopyTypes copy = CopyTypes.DeepCopy)
        {
            Mat grayImg = NewGrayMat(img);
            var newImg = copy == CopyTypes.ShallowCopy ? img : img.Clone();
            Rect[] faces = RightPalmClassifier.DetectMultiScale(grayImg);
            foreach (var rect in faces)
            {
                Cv2.Rectangle(newImg, new Point(rect.X, rect.Y), new Point(rect.X + rect.Width,
                    rect.Y + rect.Height), new Scalar(0, 255, 0), 3);
            }

            return newImg;
        }

        /// <summary>
        /// 行人
        /// </summary>
        /// <param name="img"></param>
        /// <param name="copy"></param>
        /// <returns></returns>
        public static Mat PedestrianLocate(Mat img, CopyTypes copy = CopyTypes.DeepCopy)
        {
            Mat grayImg = NewGrayMat(img);
            var newImg = copy == CopyTypes.ShallowCopy ? img : img.Clone();
            Rect[] faces = PedestrianClassifier.DetectMultiScale(grayImg,1.3,2);
            foreach (var rect in faces)
            {
                Cv2.Rectangle(newImg, new Point(rect.X, rect.Y), new Point(rect.X + rect.Width,
                    rect.Y + rect.Height), new Scalar(255, 0, 0), 3);
            }

            return newImg;
        }

        /// <summary>
        /// 灰度化
        /// </summary>
        /// <param name="img"></param>
        /// <returns>gray img or null </returns>
        public static Mat NewGrayMat(Mat img)
        {
            Mat grayImg;
            try
            {
                grayImg = img.CvtColor(ColorConversionCodes.BGR2GRAY, 1);
            }
            catch (OpenCVException)
            {
                return img;
            }

            return grayImg;
        }

        /// <summary>
        /// 灰度直方图
        /// </summary>
        /// <param name="src"></param>
        public static Mat Histogram(Mat src)
        {
            // Histogram view
            const int width = 260, height = 200;
            Mat render = new Mat(new Size(width, height), MatType.CV_8UC3, Scalar.All(255));

            // Calculate histogram
            Mat hist = new Mat();
            int[] hDims = {256}; // Histogram size for each dimension
            Rangef[] ranges = {new Rangef(0, 256),}; // min/max 
            Cv2.CalcHist(
                new[] {src},
                new[] {0},
                null,
                hist,
                1,
                hDims,
                ranges);

            // Get the max value of histogram
            Cv2.MinMaxLoc(hist, out _, out double maxVal);

            Scalar color = Scalar.All(100);
            // Scales and draws histogram
            Mat scaleHist = hist * (maxVal != 0 ? height / maxVal : 0.0);
            for (int j = 0; j < hDims[0]; ++j)
            {
                int binW = (int) ((double) width / hDims[0]);
                render.Rectangle(
                    new Point(j * binW, render.Rows - (int) (scaleHist.Get<float>(j))),
                    new Point((j + 1) * binW, render.Rows),
                    color,
                    -1);
            }


            return render;
        }

        /// <summary>
        /// 加高斯噪声
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="ProcessCanceledException"></exception>
        [ThrowsException(typeof(ProcessCanceledException))]
        public static Mat AddGaussianNoise(Mat img)
        {
            var inputWindow = new MeanVariancePopUp();
            var dialogResult = inputWindow.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                Mat gNoise = new Mat(img.Size(), img.Type());
                gNoise.Randn(inputWindow.Mean, inputWindow.Variance);

                return img + gNoise;
            }
            else
            {
                throw new ProcessCanceledException();
            }
        }

        /// <summary>
        /// 加均匀噪声
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="ProcessCanceledException"></exception>
        [ThrowsException(typeof(ProcessCanceledException))]
        public static Mat AddUniformNoise(Mat img)
        {
            var inputWindow = new UpperLowerLimitPopUp();
            var dRest = inputWindow.ShowDialog();
            if (dRest == DialogResult.OK)
            {
                Mat uNoise = new Mat(img.Size(), img.Type());
                uNoise.Randu(inputWindow.Low, inputWindow.High);
                return img + uNoise;
            }
            else
            {
                throw new ProcessCanceledException();
            }
        }

        /// <summary>
        /// 加椒盐噪声
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="ProcessCanceledException"></exception>
        [ThrowsException(typeof(ProcessCanceledException))]
        public static Mat AddSaltAndPepperNoise(Mat img)
        {
            var inputWindow = new UpperLowerLimitPopUp();
            var dialogRes = inputWindow.ShowDialog();
            if (dialogRes == DialogResult.OK)
            {
                var noiseImg = img.Clone();

                Mat rand = Mat.Zeros(new Size(noiseImg.Width, noiseImg.Height), noiseImg.Type());
                rand.Randu(0, 255);
                Mat white = rand.LessThanOrEqual(inputWindow.Low);
                Mat black = rand.GreaterThanOrEqual(inputWindow.High);

                noiseImg -= black;
                noiseImg += white;
                return noiseImg;
            }
            else
            {
                throw new ProcessCanceledException();
            }
        }

        /// <summary>
        /// 中值滤波
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="NotGrayImageException"></exception>
        /// <exception cref="ProcessCanceledException"></exception>
        [ThrowsException(typeof(ProcessCanceledException))]
        [ThrowsException(typeof(NotGrayImageException))]
        public static Mat MedianBlur(Mat img)
        {
            var inputWindow = new RectangleBoxSizePopUp();
            var dRes = inputWindow.ShowDialog();
            if (dRes == DialogResult.OK)
            {
                if (inputWindow.WindowSize % 2 == 0 || inputWindow.WindowSize < 3)
                {
                    MessageBox.Show(@"输入应为奇数并且>=3");
                    throw new NotGrayImageException();
                }

                return img.MedianBlur(inputWindow.WindowSize);
            }
            else
            {
                throw new ProcessCanceledException();
            }
        }

        /// <summary>
        /// 均值滤波
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="ProcessCanceledException">Thrown.</exception>
        [ThrowsException(typeof(ProcessCanceledException))]
        public static Mat AverageBlur(Mat img)
        {
            var inputWindow = new BoxHeightWidthPopUp();
            var dRes = inputWindow.ShowDialog();
            if (dRes == DialogResult.OK)
            {
                return img.Blur(new Size(inputWindow.W, inputWindow.H));
            }
            else
            {
                throw new ProcessCanceledException();
            }
        }

        /// <summary>
        /// 高斯滤波
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="ProcessCanceledException"></exception>
        [ThrowsException(typeof(ProcessCanceledException))]
        public static Mat GaussianBlur(Mat img)
        {
            var inputWindow = new BoxHeightWidthPopUp();
            var dRes = inputWindow.ShowDialog();
            if (dRes == DialogResult.OK)
            {
                return img.GaussianBlur(new Size(inputWindow.W, inputWindow.H), 0);
            }
            else
            {
                throw new ProcessCanceledException();
            }
        }

        /// <summary>
        /// Laplacian锐化
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="ProcessCanceledException">Thrown.</exception>
        [ThrowsException(typeof(ProcessCanceledException))]
        public static Mat LaplacianSharpen(Mat img)
        {
            var inputWindow = new CoefficientAlphaPopUp();
            var dialogRes = inputWindow.ShowDialog();
            if (dialogRes == DialogResult.OK)
            {
                Mat newImg = new Mat(img.Size(), MatType.CV_16S);
                img.ConvertTo(newImg, MatType.CV_16S);
                // why minus works?
                Mat sharpenImg = newImg - img.Laplacian(MatType.CV_16S) * inputWindow.Alpha;
                Mat resSharpenImg = new Mat(sharpenImg.Size(), MatType.CV_8U);
                sharpenImg.ConvertTo(resSharpenImg, MatType.CV_8U);
                return resSharpenImg;
            }
            else
            {
                throw new ProcessCanceledException();
            }
        }

        /// <summary>
        /// Sobel锐化
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="ProcessCanceledException">Thrown.</exception>
        [ThrowsException(typeof(ProcessCanceledException))]
        public static Mat SobelSharpen(Mat img)
        {
            var w = new SobelCoefficientPopUp();
            var dialogRes = w.ShowDialog();
            if (dialogRes == DialogResult.OK)
            {
                Mat newImg = new Mat(img.Size(), MatType.CV_16S);
                img.ConvertTo(newImg, MatType.CV_16S);
                // there plus works ?!
                Mat sharpenImg = newImg + img.Sobel(MatType.CV_16S, w.XOrder, w.YOrder, w.WindowSize) * w.Alpha;
                Mat resSharpenImg = new Mat(sharpenImg.Size(), MatType.CV_8U);
                sharpenImg.ConvertTo(resSharpenImg, MatType.CV_8U);
                return resSharpenImg;
            }
            else
            {
                throw new ProcessCanceledException();
            }
        }

        /// <summary>
        /// 伪彩色增强
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="ProcessCanceledException">Thrown</exception>
        [ThrowsException(typeof(ProcessCanceledException))]
        public static Mat PseudoColorFortify(Mat img)
        {
            var w = new ColorMapComboPopUp();
            var dialogRes = w.ShowDialog();
            if (dialogRes == DialogResult.OK)
            {
                Mat cImg = new Mat(img.Size(), img.Type());
                Cv2.ApplyColorMap(img, cImg, w.SelectedTypes);
                return cImg;
            }
            else
            {
                throw new ProcessCanceledException();
            }
        }

        /// <summary>
        /// Laplacian边缘检测
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Mat LaplacianEdgeDetect(Mat img)
        {
            Mat dst = new Mat();
            Mat absDst = new Mat();
            Cv2.Laplacian(img, dst, MatType.CV_16S, 3);
            Cv2.ConvertScaleAbs(dst, absDst);
            return absDst;
        }

        /// <summary>
        /// Sobel边缘检测
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="ProcessCanceledException">Thrown.</exception>
        [ThrowsException(typeof(ProcessCanceledException))]
        public static Mat SobelEdgeDetect(Mat img)
        {
            Mat gradX = new Mat();
            Mat gradY = new Mat();
            Mat absGradX = new Mat();
            Mat absGradY = new Mat();
            Mat dst = new Mat();

            Cv2.Sobel(img, gradX, MatType.CV_16S, 1, 0, 3, 1, 1);
            Cv2.ConvertScaleAbs(gradX, absGradX);

            Cv2.Sobel(img, gradY, MatType.CV_16S, 0, 1, 3, 1, 1);
            Cv2.ConvertScaleAbs(gradY, absGradY);

            Cv2.AddWeighted(absGradX, 0.5, absGradY, 0.5, 0, dst);
            return dst;
        }

        /// <summary>
        /// Canny边缘检测
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="ProcessCanceledException"></exception>
        [ThrowsException(typeof(ProcessCanceledException))]
        public static Mat CannyEdgeDetect(Mat img)
        {
            var w = new UpperLowerLimitPopUp(true);
            var dialogRes = w.ShowDialog();
            if (dialogRes == DialogResult.OK)
            {
                Mat edgeImg = img.Canny(w.Low, w.High, w.ApertureSize);
                return edgeImg;
            }
            else
            {
                throw new ProcessCanceledException();
            }
        }

        /// <summary>
        /// 均值门限分割
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="NotGrayImageException">Thrown</exception>
        /// <exception cref="ProcessCanceledException">Thrown.</exception>
        [ThrowsException(typeof(ProcessCanceledException))]
        [ThrowsException(typeof(NotGrayImageException))]
        public static Mat MeanThresholdSeg(Mat img)
        {
            var w = new AdaptiveSegmentationComboPopUp();
            var dialogRes = w.ShowDialog();
            if (dialogRes == DialogResult.OK)
            {
                Mat seg;
                try
                {
                    seg = img.AdaptiveThreshold(255, AdaptiveThresholdTypes.MeanC, w.SelectedTypes, w.WindowSize,
                        w.Constant);
                }
                catch (OpenCVException)
                {
                    throw new NotGrayImageException();
                }

                return seg;
            }
            else
            {
                throw new ProcessCanceledException();
            }
        }

        /// <summary>
        /// 高斯门限分割
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="NotGrayImageException"></exception>
        /// <exception cref="ProcessCanceledException"></exception>
        [ThrowsException(typeof(ProcessCanceledException))]
        [ThrowsException(typeof(NotGrayImageException))]
        public static Mat GaussianThresholdSeg(Mat img)
        {
            var w = new AdaptiveSegmentationComboPopUp();
            var dialogRes = w.ShowDialog();
            if (dialogRes == DialogResult.OK)
            {
                Mat seg;
                try
                {
                    seg = img.AdaptiveThreshold(255, AdaptiveThresholdTypes.GaussianC, w.SelectedTypes, w.WindowSize,
                        w.Constant);
                }
                catch (OpenCVException)
                {
                    throw new NotGrayImageException();
                }

                return seg;
            }
            else
            {
                throw new ProcessCanceledException();
            }
        }

        /// <summary>
        /// Otsu门限分割
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="NotGrayImageException"></exception>
        [ThrowsException(typeof(NotGrayImageException))]
        public static Mat OtsuSeg(Mat img)
        {
            try
            {
                var otsuImg = img.Threshold(0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
                return otsuImg;
            }
            catch (OpenCVException)
            {
                throw new NotGrayImageException();
            }
        }

        /// <summary>
        /// DFT
        /// </summary>
        /// <param name="I"></param>
        /// <returns></returns>
        /// <exception cref="NotGrayImageException"></exception>
        [ThrowsException(typeof(NotGrayImageException))]
        public static Mat DftTransform(Mat I)
        {
            Mat padded = new Mat(); //expand input image to optimal size
            int m = Cv2.GetOptimalDFTSize(I.Rows), n = Cv2.GetOptimalDFTSize(I.Cols); // on the border add zero values
            Cv2.CopyMakeBorder(I, padded, 0, m - I.Rows, 0, n - I.Cols, BorderTypes.Constant, Scalar.All(0));

            padded.ConvertTo(padded, MatType.CV_32F);
            var t = Mat.Zeros(padded.Size(), MatType.CV_32F);
            Mat[] planes = {padded, t};
            Mat complexI = new Mat();
            Cv2.Merge(planes, complexI); // Add to the expanded another plane with zeros

            try
            {
                Cv2.Dft(complexI, complexI);
            }
            catch (OpenCVException)
            {
                throw new NotGrayImageException();
            }

            // this way the result may fit in the source matrix
            // compute the magnitude and switch to logarithmic scale
            // => log(1 + sqrt(Re(DFT(I))^2 + Im(DFT(I))^2))
            Cv2.Split(complexI, out planes); // planes.get(0) = Re(DFT(I)// planes.get(1) = Im(DFT(I))
            Cv2.Magnitude(planes[0], planes[1], planes[0]); // planes.get(0) = magnitude
            Mat magI = planes[0];

            Mat matOfOnes = Mat.Ones(magI.Size(), magI.Type());
            Cv2.Add(matOfOnes, magI, magI); // switch to logarithmic scale
            Cv2.Log(magI, magI);

            // crop the spectrum, if it has an odd number of rows or columns
            magI = new Mat(magI, new Rect(0, 0, magI.Cols & -2, magI.Rows & -2));
            int cx = magI.Cols / 2, cy = magI.Rows / 2;
            Mat q0 = new Mat(magI, new Rect(0, 0, cx, cy)); // Top-Left - Create a ROI per quadrant
            Mat q1 = new Mat(magI, new Rect(cx, 0, cx, cy)); // Top-Right
            Mat q2 = new Mat(magI, new Rect(0, cy, cx, cy)); // Bottom-Left
            Mat q3 = new Mat(magI, new Rect(cx, cy, cx, cy)); // Bottom-Right

            Mat tmp = new Mat(); // swap quadrants (Top-Left with Bottom-Right)
            q0.CopyTo(tmp);
            q3.CopyTo(q0);
            tmp.CopyTo(q3);

            q1.CopyTo(tmp); // swap quadrant (Top-Right with Bottom-Left)
            q2.CopyTo(q1);
            tmp.CopyTo(q2);

            magI.ConvertTo(magI, MatType.CV_8UC1);
            Cv2.Normalize(magI, magI, 0, 255, NormTypes.MinMax, MatType.CV_8UC1);
            // Transform the matrix with float values
            // into a viewable image form (float between
            // values 0 and 255).

            return magI;
        }

        /// <summary>
        /// Star特征提取
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Mat StarFeatureDetect(Mat img)
        {
            var newImg = img.Clone();
            var gray = NewGrayMat(img);
            var detector = StarDetector.Create(); //maxsize:45

            KeyPoint[] keyPoints = detector.Detect(gray);

            foreach (KeyPoint kpt in keyPoints)
            {
                var color = new Scalar(0, 255, 0);
                float r = kpt.Size / 2;
                Cv2.Circle(newImg, (Point) kpt.Pt, (int) r, color);
                Cv2.Line(newImg,
                    (Point) new Point2f(kpt.Pt.X + r, kpt.Pt.Y + r),
                    (Point) new Point2f(kpt.Pt.X - r, kpt.Pt.Y - r),
                    color);
                Cv2.Line(newImg,
                    (Point) new Point2f(kpt.Pt.X - r, kpt.Pt.Y + r),
                    (Point) new Point2f(kpt.Pt.X + r, kpt.Pt.Y - r),
                    color);
            }

            return newImg;
        }

        /// <summary>
        /// ORB and FREAK特征提取
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Mat ORBandFREAKFeatureDetect(Mat img)
        {
            //ORB
            Mat newImg = img.Clone();
            Mat gray = NewGrayMat(img);
            ORB orb = ORB.Create(1000);
            KeyPoint[] keyPoints = orb.Detect(gray);

            // FREAK
            FREAK freak = FREAK.Create();
            Mat freakDescriptors = new Mat();
            freak.Compute(gray, ref keyPoints, freakDescriptors);

            var color = new Scalar(0, 255, 0);
            foreach (KeyPoint kpt in keyPoints)
            {
                float r = kpt.Size / 2;
                Cv2.Circle(newImg, (Point) kpt.Pt, (int) r, color);
                Cv2.Line(newImg,
                    (Point) new Point2f(kpt.Pt.X + r, kpt.Pt.Y + r),
                    (Point) new Point2f(kpt.Pt.X - r, kpt.Pt.Y - r),
                    color);
                Cv2.Line(newImg,
                    (Point) new Point2f(kpt.Pt.X - r, kpt.Pt.Y + r),
                    (Point) new Point2f(kpt.Pt.X + r, kpt.Pt.Y - r),
                    color);
            }

            return newImg;
        }

        /// <summary>
        /// BRISK特征提取
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Mat BRISKFeatureDetect(Mat img)
        {
            var newImg = img.Clone();
            var gray = NewGrayMat(img);
            var brisk = BRISK.Create();

            KeyPoint[] keyPoints = brisk.Detect(gray);

            foreach (KeyPoint kpt in keyPoints)
            {
                var color = new Scalar(0, 255, 0);
                float r = kpt.Size / 2;
                Cv2.Circle(newImg, (Point) kpt.Pt, (int) r, color);
                Cv2.Line(newImg,
                    (Point) new Point2f(kpt.Pt.X + r, kpt.Pt.Y + r),
                    (Point) new Point2f(kpt.Pt.X - r, kpt.Pt.Y - r),
                    color);
                Cv2.Line(newImg,
                    (Point) new Point2f(kpt.Pt.X - r, kpt.Pt.Y + r),
                    (Point) new Point2f(kpt.Pt.X + r, kpt.Pt.Y - r),
                    color);
            }

            return newImg;
        }

        /// <summary>
        /// MSER特征提取
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Mat MSERFeatureDetect(Mat img)
        {
            Mat newImg = img.Clone();
            var gray = NewGrayMat(img);

            MSER mser = MSER.Create();
            KeyPoint[] contour = mser.Detect(gray);

            Scalar color = Scalar.Green;
            foreach (KeyPoint p in contour)
            {
                newImg.Circle((Point) p.Pt, 3, color);
            }

            return newImg;
        }

        /// <summary>
        /// 同态滤波
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <exception cref="NotGrayImageException"></exception>
        [ThrowsException(typeof(NotGrayImageException))]
        public static Mat HomoFilter(Mat img)
        {
            Mat newImg = new Mat();
            img.ConvertTo(newImg, MatType.CV_64FC1);
            Size nImgSize = newImg.Size();
            //dct函数只能处理偶数大小图片
            if (nImgSize.Height % 2 != 0)
            {
                newImg = newImg.RowRange(0, nImgSize.Height - 1);
                if (nImgSize.Width % 2 != 0)
                {
                    newImg = newImg.ColRange(0, nImgSize.Width - 1);
                }
            }
            else if (nImgSize.Width % 2 != 0)
            {
                newImg = newImg.ColRange(0, nImgSize.Width - 1);
            }

            newImg = (newImg + 0.0001);
            newImg = newImg.Log();

            Mat dst = new Mat();
            dst.ConvertTo(dst, MatType.CV_64FC1);

            Mat matDct = new Mat(newImg.Size(), MatType.CV_64FC1);
            try
            {
                Cv2.Dct(newImg, matDct);
            }
            catch (OpenCVException)
            {
                throw new NotGrayImageException();
            }

            const double gammaH = 1.5;
            const double gammaL = 0.5;
            const double c = 1;
            double d0 = (newImg.Rows / 2) * (newImg.Rows / 2) + (newImg.Cols / 2) * (newImg.Cols / 2);
            Mat hUv = Mat.Zeros(newImg.Size(), MatType.CV_64FC1);

            var indexer = hUv.GetGenericIndexer<Vec3d>();
            for (int h = 0; h < newImg.Rows; h++)
            {
                for (int w = 0; w < newImg.Cols; w++)
                {
                    var d2 = Math.Pow(h, 2) + Math.Pow(w, 2);
                    indexer[h, w] = new Vec3d((gammaH - gammaL) * (1 - Math.Exp(-c * d2 / d0)) + gammaL,
                        indexer[h, w].Item1, indexer[h, w].Item2);
                }
            }

            indexer[0, 0] = new Vec3d(1.1, indexer[0, 0].Item1, indexer[0, 0].Item2);

            matDct = matDct.Mul(matDct);
            Cv2.Idct(matDct, dst);

            var tmp = newImg.GaussianBlur(new Size(9, 9), 1.5, 1.5);
            const double alpha = 0.5;
            dst = (1 + alpha) * newImg - alpha * tmp;

            dst = dst.Exp();

            dst.ConvertTo(dst, MatType.CV_8UC1);
            matDct.Dispose();
            newImg.Dispose();
            return dst;
        }

        /// <summary>
        /// 小波变换
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Mat WaveletTransform(Mat img)
        {
            int height = img.Cols;
            int width = img.Rows;
            int depth = 3; //分解深度
            int depthCount = 1;
            Mat tmp = Mat.Ones(new Size(width, height), MatType.CV_32FC1);
            Mat wavelet = Mat.Ones(new Size(width, height), MatType.CV_32FC1);
            Mat imgTmp = img.Clone();
            imgTmp.ConvertTo(imgTmp, MatType.CV_32FC1);
            while (depthCount <= depth)
            {
                width = img.Rows / depthCount;
                height = img.Cols / depthCount;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height / 2; j++)
                    {
                        tmp.At<float>(i, j) = (imgTmp.At<float>(i, 2 * j) + imgTmp.At<float>(i, 2 * j + 1)) / 2;
                        tmp.At<float>(i, j + height / 2) =
                            (imgTmp.At<float>(i, 2 * j) - imgTmp.At<float>(i, 2 * j + 1)) / 2;
                    }
                }

                for (int i = 0; i < width / 2; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        wavelet.At<float>(i, j) = (tmp.At<float>(2 * i, j) + tmp.At<float>(2 * i + 1, j)) / 2;
                        wavelet.At<float>(i + width / 2, j) =
                            (tmp.At<float>(2 * i, j) - tmp.At<float>(2 * i + 1, j)) / 2;
                    }
                }

                imgTmp = wavelet;
                depthCount++;
            }

            imgTmp.Dispose();

            return wavelet;
        }
    }
}