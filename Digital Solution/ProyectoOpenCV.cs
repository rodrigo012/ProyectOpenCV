using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Face;
using System;
using System.Collections.Generic;
using static Emgu.CV.Face.FaceRecognizer;
using System.Drawing;

namespace ProyectoOpenCV
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string videoPath = "WIN_20231031_11_15_44_Pro.mp4";
                string faceCascadePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_default.xml");
                string modelPath = "eigenfaces.xml";

                if (!File.Exists(videoPath))
                {
                    Console.WriteLine("Error: video file not found.");
                    return;
                }

                if (!File.Exists(faceCascadePath))
                {
                    faceCascadePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "haarcascade_frontalface_default.xml");
                    if (!File.Exists(faceCascadePath))
                    {
                        Console.WriteLine("Error: face detection classifier file not found.");
                        return;
                    }
                }

                if (!File.Exists(modelPath))
                {
                    Console.WriteLine("Error: face recognition model file not found.");
                    return;
                }

                VideoCapture capture = new VideoCapture(videoPath);


                // Read video file

                // Create folder to store frames
                string folderPath = "Frames";
                Directory.CreateDirectory(folderPath);

                // Read frames from video and save them as images
                int frameCount = (int)capture.Get(CapProp.FrameCount);
                for (int i = 0; i < frameCount; i++)
                {
                    Mat frame = new Mat();
                    capture.Read(frame);
                    if (frame.IsEmpty)
                        break;
                    string fileName = Path.Combine(folderPath, $"frame_{i}.jpg");
                    CvInvoke.Imwrite(fileName, frame);
                }

                // Load face detection classifier
                CascadeClassifier faceDetector = new CascadeClassifier(faceCascadePath);

                // Load face recognition model
                EigenFaceRecognizer recognizer = new EigenFaceRecognizer();
                recognizer.Read(modelPath);

                // Process each image in the folder
                foreach (string imagePath in Directory.GetFiles(folderPath))
                {
                    // Load image
                    Mat image = CvInvoke.Imread(imagePath);

                    // Detect faces in image
                    Rectangle[] faces = faceDetector.DetectMultiScale(image);

                    // Process each face in image
                    foreach (Rectangle face in faces)
                    {
                        // Crop face from image
                        Mat faceImage = new Mat(image, face);

                        // Resize face image to match training data size
                        Size faceSize = new Size(100, 100);
                        CvInvoke.Resize(faceImage, faceImage, faceSize);

                        // Recognize face
                        PredictionResult prediction = recognizer.Predict(faceImage);
                        int label = prediction.Label;

                        // Draw rectangle around face
                        CvInvoke.Rectangle(image, face, new MCvScalar(0, 0, 255), 2);

                        // Save image with differences facial features
                        string fileName = Path.Combine(folderPath, $"face_{label}.jpg");
                        Mat grayImage = new Mat();
                        CvInvoke.CvtColor(image, grayImage, ColorConversion.Bgr2Gray);
                        Mat cannyImage = new Mat();
                        CvInvoke.Canny(grayImage, cannyImage, 100, 60);
                        Mat diffImage = new Mat();
                        CvInvoke.AbsDiff(grayImage, cannyImage, diffImage);
                        CvInvoke.Imwrite(fileName, diffImage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
