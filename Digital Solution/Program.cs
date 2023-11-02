using Emgu.CV.CvEnum;
using Emgu.CV;
using System.Drawing;

class Program
{
    static void Main(string[] args)
    {
        GetFrames();
        RunAnalysis();
        
    }

    static void GetFrames()
    {
        string videoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../", "videos");
        string outputFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../", "Frames");
        if (!Directory.Exists(videoPath))
        {
            Directory.CreateDirectory(videoPath);
        }
        if (!Directory.Exists(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath);
        }

        DirectoryInfo directory = new DirectoryInfo(videoPath);
        FileInfo[] videoFiles = directory.GetFiles("*.mp4");

        if (videoFiles.Length == 0)
        {
            Console.WriteLine("No se encontraron archivos de video en la carpeta 'videos'.");
            return;
        }

        foreach (FileInfo videoFile in videoFiles)
        {
            using (VideoCapture videoCapture = new VideoCapture(videoFile.FullName))
            {
                if (!videoCapture.IsOpened)
                {
                    Console.WriteLine($"No se pudo abrir el archivo de video '{videoFile.Name}'.");
                    continue;
                }

                int frameCount = (int)videoCapture.Get(CapProp.FrameCount);

                for (int frameNumber = 0; frameNumber < frameCount; frameNumber++)
                {
                    Mat frame = new Mat();
                    videoCapture.Set(CapProp.PosFrames, frameNumber);
                    videoCapture.Read(frame);

                    DetectFaces(frame, videoFile, frameNumber, outputFolderPath);
                }
            }
        }
        Console.WriteLine("se ha obtenido los frames correspondientes.");

    }

    static void DetectFaces(Mat frame, FileInfo videoFile, int frameNumber, string outputFolderPath)
    {
        string faceCascadePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../", "haarcascade_frontalface_default.xml");

        // Realizar la detección facial en el frame
        CascadeClassifier faceCascade = new CascadeClassifier(faceCascadePath);
        var faces = faceCascade.DetectMultiScale(frame);

        foreach (var face in faces)
        {
            // Extraer y guardar la región de la cara
            Mat faceRegion = new Mat(frame, face);
            string outputImagePath = Path.Combine(outputFolderPath, $"{videoFile.Name}_frame_{frameNumber}.jpg");
            faceRegion.Save(outputImagePath);
        }
    }
     static void RunAnalysis()
    {



        string facePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../", "Frames");

        string[] faceFiles = Directory.GetFiles(facePath, "*.jpg");
        List<Mat> images = new List<Mat>();
        List<int> labels = new List<int>();
        foreach (string face in faceFiles)
        {
            Mat image = CvInvoke.Imread(face, ImreadModes.Color);
            // Crop the image to a common size using GetRectSubPix method
            Size imageSize = image.Size;
            Size cropSize = new Size(500, 500);
            PointF center = new PointF(imageSize.Width / 2f, imageSize.Height / 2f);
            Mat croppedImage = new Mat();
            CvInvoke.GetRectSubPix(image, cropSize, center, croppedImage);
            images.Add(croppedImage);
            labels.Add(1);
        }

        //EigenFaceRecognizer recognizer = new EigenFaceRecognizer();
        //recognizer.Train(images.ToArray(), labels.ToArray());

        // Initialize accumulated difference image
        if (images.Count == 0)
        {
            Console.WriteLine("No se encontraron archivos de video en la carpeta 'videos'.");
            return;
        }
        Mat accumulatedDiffImage = images[0].Clone();

        // Convert the accumulated difference image to RGB
        CvInvoke.CvtColor(accumulatedDiffImage, accumulatedDiffImage, ColorConversion.Bgr2Rgb);
        // Iterate through the images starting from the second one
        for (int i = 1; i < images.Count; i++)
        {
            Mat currentImage = images[i];

            // Compute the absolute difference between the accumulated image and the current image
            Mat diffImage = new Mat();
            CvInvoke.AbsDiff(accumulatedDiffImage, currentImage, diffImage);
            CvInvoke.AddWeighted(accumulatedDiffImage, 1.0, diffImage, 1.0, 0, accumulatedDiffImage);

            // Ensure the images are of the same size and channel count
            if (accumulatedDiffImage.Size != diffImage.Size || accumulatedDiffImage.NumberOfChannels != diffImage.NumberOfChannels)
            {
                CvInvoke.Resize(diffImage, diffImage, accumulatedDiffImage.Size);
                CvInvoke.CvtColor(diffImage, diffImage, ColorConversion.Bgr2Gray);
            }

            // Add the difference to the accumulated difference image
            CvInvoke.Add(diffImage, accumulatedDiffImage, accumulatedDiffImage);

            // Update the accumulated image for the next iteration
            accumulatedDiffImage = diffImage;
        }

        CvInvoke.GaussianBlur(accumulatedDiffImage, accumulatedDiffImage, new Size(3, 3), 0);
        // Convert the accumulated difference image to grayscale
        CvInvoke.CvtColor(accumulatedDiffImage, accumulatedDiffImage, ColorConversion.Bgr2Bgra);

        // Threshold the accumulated difference image to highlight differences
        CvInvoke.Threshold(accumulatedDiffImage, accumulatedDiffImage, 30, 255, ThresholdType.Binary);
        string outputFinalFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../", "Differences");

        if (!Directory.Exists(outputFinalFolderPath))
        {
            Directory.CreateDirectory(outputFinalFolderPath);
        }
        // Save the final accumulated differences image
        string finalFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Differences", "facialDifferences.jpg");
        CvInvoke.Imwrite(finalFilePath, accumulatedDiffImage);

        // Display the final accumulated differences image
        CvInvoke.Imshow("Final facial differences image", accumulatedDiffImage);
        Console.WriteLine("Se a finalizado el proceso de analisis y se creo el archivo correspondiente .");
        Console.WriteLine("Se ha finalizado el proceso la imagen se encuentra en la carpeta del proyecto.");
    }

}

